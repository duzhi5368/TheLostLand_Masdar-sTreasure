using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;
//============================================================
namespace FKLib
{
    public class StatsHandler : MonoBehaviour, IJsonSerializable
    {
        [InspectorLabel("Ãû³Æ")]
        [SerializeField]
        private string _handlerName = string.Empty;
        public string HandlerName { get { return _handlerName; } }

        [StatPicker]
        [SerializeField]
        public List<IStat> Stats = new List<IStat>();

        [HideInInspector]
        [SerializeField]
        private List<StatOverride> _statOverrides = new List<StatOverride>();

        [SerializeField]
        protected List<IStatEffect> _effects = new List<IStatEffect>();

        public bool IsSaveable = false;
        public System.Action OnUpdate;
        private AudioSource _audioSource;

        private void Start()
        {
            for (int i = 0; i < Stats.Count; i++)
                Stats[i] = Instantiate(Stats[i]);

            if(_statOverrides.Count < Stats.Count)
            {
                for (int i = _statOverrides.Count; i < Stats.Count; i++)
                    _statOverrides.Insert(i, new StatOverride());
            }

            for(int i = 0; i < Stats.Count; i++)
                Stats[i].Initialize(this, _statOverrides[i]);
            for (int i = 0; i < Stats.Count; i++)
                Stats[i].ApplyStartValues();
            for(int i = 0; i < _effects.Count; i++)
            {
                _effects[i] = Instantiate(_effects[i]);
                _effects[i].Initialize(this);
            }

            if (!string.IsNullOrEmpty(_handlerName))
                StatsManager.RegisterStatsHandler(this);

            IEventHandler.Register<GameObject, UnityEngine.Object>(gameObject, "SendDamage", SendDamage);
        }

        private void Update()
        {
            OnUpdate?.Invoke();
            for (int i = 0; i < _effects.Count; i++) 
            {
                _effects[i].Execute();
            }
        }

        public void ApplyDamage(object[] data)
        {
            string name = (string)data[0];
            float damage = (float)data[1];
            ApplyDamage(name, damage);
        }

        public void ApplyDamage(string name, float damage)
        {
            StatAttribute stat = GetStat(name) as StatAttribute;
            if (stat == null)
                return;

            float currentValue = stat.CurrentValue;
            currentValue = Mathf.Clamp(currentValue - damage, 0f, stat.Value);
            stat.CurrentValue = currentValue;
        }

        protected void TriggerAnimationEvent(AnimationEvent ev)
        {
            if (ev.animatorClipInfo.weight > 0.5f)
                SendMessage(ev.stringParameter, ev.objectReferenceParameter, SendMessageOptions.DontRequireReceiver);
        }

        private void SendDamage(UnityEngine.Object data)
        {
            DamageData damageData = data as DamageData;
            if (damageData == null)
                return;
            damageData.Sender = gameObject;

            Collider[] colliders = Physics.OverlapSphere(transform.position, damageData.MaxDistance);

            for(int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i].transform != transform)
                {
                    Vector3 direction = colliders[i].transform.position - transform.position;
                    float angle = Vector3.Angle(direction, transform.forward);
                    if (Mathf.Abs(angle) < damageData.MaxAngle)
                        SendDamage(colliders[i].gameObject, damageData);
                }
            }
        }

        private void SendDamage(GameObject receiver, UnityEngine.Object data)
        {
            StatsHandler receiverHandler = receiver.GetComponent<StatsHandler>();
            DamageData damageData = data as DamageData;

            if (gameObject.tag == receiver.tag)
                return;

            if (receiverHandler != null && receiverHandler.enabled && damageData != null)
            {
                IStat sendingStat = Stats.FirstOrDefault(x => x.Name == damageData.SendingStat);
                if (sendingStat == null)
                    return;

                IStat criticalStrikeStat = Stats.FirstOrDefault(x => x.Name == damageData.CriticalStrikeStat);
                bool isCriticalStrike = criticalStrikeStat != null && criticalStrikeStat.Value > UnityEngine.Random.Range(0f, 100f);
                sendingStat.CalculateValue();

                float damage = sendingStat.Value;
                if (isCriticalStrike)
                    damage *= 2f;

                receiverHandler.ApplyDamage(damageData.ReceivingStat, damage);
                IEventHandler.Execute(receiver, "OnGetHit", gameObject, damageData.ReceivingStat, damage);

                SendMessage("UseItem", SendMessageOptions.DontRequireReceiver);

                if(damageData.ParticleEffect != null)
                {
                    Vector3 pos = receiver.GetComponent<Collider>().ClosestPoint(transform.position + damageData.Offset);
                    Vector3 right = UnityEngine.Random.Range(-damageData.Randomize.x, damageData.Randomize.x) * transform.right;
                    Vector3 up = UnityEngine.Random.Range(-damageData.Randomize.y, damageData.Randomize.y) * transform.up;
                    Vector3 forward = UnityEngine.Random.Range(-damageData.Randomize.z, damageData.Randomize.z) * transform.forward;

                    Vector3 relativePos = (transform.position + damageData.Offset + right + up + forward) - pos;
                    GameObject effect = Instantiate(damageData.ParticleEffect, pos, Quaternion.LookRotation(relativePos, Vector3.up));
                    Destroy(effect, damageData.LifeTime);
                }

                if (damageData.IsEnableShake)
                    CameraEffects.Shake(damageData.Duration, damageData.Speed, damageData.Amount);

                if(damageData.HitSounds.Length > 0)
                    receiverHandler.PlaySound(damageData.HitSounds[UnityEngine.Random.Range(9, damageData.HitSounds.Length)],
                        damageData.AudioMixerGroup, damageData.VolumeScale);

                if (damageData.IsDisplayDamage)
                    receiverHandler.DisplayDamage(damageData.DamagePrefab, damage, 
                        isCriticalStrike ? damageData.CriticalDamageColor : damageData.DamageColor, damageData.Intensity);

                if (damageData.IsEnableKnockback && damageData.KnockbackChance > UnityEngine.Random.Range(0f, 1f))
                    StartCoroutine(Knockback(receiver, damageData));

                Animator animator = receiver.GetComponent<Animator>();
                if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") && !animator.IsInTransition(0))
                    animator.SetTrigger("Hit");
            }
        }

        private IEnumerator Knockback(GameObject receiver, DamageData damageData)
        {
            NavMeshAgent agent = receiver.GetComponent<NavMeshAgent>();
            if (agent == null) 
                yield break;

            Vector3 direction = receiver.transform.position - transform.position;
            float speed = agent.speed;
            float angularSpeed = agent.angularSpeed;
            float acceleration = agent.acceleration;
            agent.speed = damageData.KnockbackStrength;
            agent.angularSpeed = 0f;
            agent.acceleration = damageData.KnockbackAcceleration;
            agent.SetDestination(receiver.transform.position + direction.normalized);
            yield return new WaitForSeconds(damageData.KnockbackAcceleration);
            if (agent == null) 
                yield break;

            agent.speed = speed;
            agent.angularSpeed = angularSpeed;
            agent.acceleration = acceleration;
        }

        private void DisplayDamage(GameObject prefab, float damage, Color color, Vector3 intensity)
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                GameObject go = Instantiate(prefab, canvas.transform);
                go.transform.localPosition += new Vector3(UnityEngine.Random.Range(-intensity.x, intensity.x), 
                    UnityEngine.Random.Range(-intensity.y, intensity.y), UnityEngine.Random.Range(-intensity.z, intensity.z));
                Text text = go.GetComponentInChildren<Text>();
                text.color = color;
                text.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage).ToString();

                go.SetActive(true);
                Destroy(go, 4f);
            }
        }

        private void PlaySound(AudioClip clip, AudioMixerGroup audioMixerGroup, float volumeScale)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.outputAudioMixerGroup = audioMixerGroup;
            _audioSource.spatialBlend = 1f;
            _audioSource.PlayOneShot(clip, volumeScale);
        }

        public IStat GetStat(IStat stat)
        {
            return GetStat(stat.Name);
        }

        public IStat GetStat(string name)
        {
            return Stats.Find(x => x.Name == name);
        }

        public bool CanApplyDamage(string name, float damage)
        {
            StatAttribute stat = GetStat(name) as StatAttribute;
            if (stat != null)
            {
                if ((damage > 0 && stat.CurrentValue >= damage) || (damage < 0 && stat.CurrentValue < stat.Value))
                    return true;
            }
            return false;
        }

        public void AddEffect(IStatEffect effect) 
        {
            effect = Instantiate(effect);
            _effects.Add(effect);
            effect.Initialize(this);
        }

        public void RemoveEffect(IStatEffect effect)
        {
            IStatEffect instance = _effects.Find(x => x.Name == effect.Name);
            _effects.Remove(instance);
        }

        public void AddModifier(object[] data)
        {
            string name = (string)data[0];
            float value = (float)data[1];
            int modType = (int)data[2];
            object source = data[3];
            AddModifier(name, value, (ENUM_StatModType)modType, source);
        }

        public void AddModifier(string statName, float value, ENUM_StatModType type, object source)
        {
            IStat stat = GetStat(statName);
            if (stat != null) {
                stat.AddModifier(new StatModifier(value, type, source));
            }
        }

        public bool RemoveModifiersFromSource(object[] data)
        {
            string name = (string)(data[0]);
            object source = data[1];
            return RemoveModifiersFromSource(name, source);
        }

        public bool RemoveModifiersFromSource(string statName, object source)
        {
            IStat stat = GetStat(statName);
            if(stat != null)
            {
                return stat.RemoveModifiersFromSource(source);
            }
            return false;
        }

        public float GetStatValue(string name)
        {
            IStat stat = GetStat(name);
            if (stat != null)
                return stat.Value;
            return 0f;
        }

        public float GetStatCurrentValue(string name)
        {
            IStat stat = GetStat(name);
            if (stat != null && stat is StatAttribute attribute)
                return attribute.CurrentValue;
            return 0f;
        }

        public void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", _handlerName);
            List<object> statsList = new List<object>();
            for (int i = 0; i < Stats.Count; i++) {
                IStat stat = Stats[i];
                if (stat != null) { 
                    Dictionary<string, object> statData = new Dictionary<string, object>();
                    stat.GetObjectData(statData);
                    statsList.Add(statData);
                }
            }
            data.Add("Stats", statsList);
        }

        public void SetObjectData(Dictionary<string, object> data)
        {
            if (data.ContainsKey("Stats"))
            {
                List<object> statList = data["Stats"] as List<object>;
                for (int i = 0; i < Stats.Count; i++)
                {
                    Dictionary<string, object> statData = statList[i] as Dictionary<string, object>;
                    if (statData != null)
                    {
                        IStat stat = GetStat((string)statData["Name"]);
                        if (stat != null)
                        {
                            stat.SetObjectData(statData);
                        }
                    }
                }
            }
        }
    }
}
