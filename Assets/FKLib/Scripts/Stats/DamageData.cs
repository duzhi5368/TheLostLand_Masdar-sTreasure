using System;
using UnityEngine;
using UnityEngine.Audio;
//============================================================
namespace FKLib
{
    [CreateAssetMenu(fileName = "DamageData", menuName = "FKLib/Damage Data")]
    [Serializable]
    public class DamageData : ScriptableObject
    {
        [HeaderLine("Default")]
        public string SendingStat = "Damage";
        public string CriticalStrikeStat = "Critical Strike";
        public string ReceivingStat = "Health";
        public float MaxDistance = 2f;
        [Range(0f, 360f)]
        public float MaxAngle = 60f;
        public bool IsDisplayDamage = false;
        [Compound("IsDisplayDamage")]
        public GameObject DamagePrefab;
        [Compound("IsDisplayDamage")]
        public Color DamageColor = Color.yellow;
        [Compound("IsDisplayDamage")]
        public Color CriticalDamageColor = Color.red;
        [Compound("IsDisplayDamage")]
        public Vector3 Intensity = new Vector3(3f, 2f, 0f);

        [HeaderLine("Particles")]
        public GameObject ParticleEffect;
        public Vector3 Offset = Vector3.up;
        public Vector3 Randomize = new Vector3(0.2f, 0.1f);
        public float LifeTime = 3f;

        [HeaderLine("Sounds")]
        public AudioMixerGroup AudioMixerGroup;
        [Range(0f, 1f)]
        public float VolumeScale = 0.7f;
        public AudioClip[] HitSounds;

        [HeaderLine("Camera Shake")]
        [InspectorLabel("Enabled")]
        public bool IsEnableShake;
        [Compound("IsEnableShake")]
        public float Duration = 0.4f;
        [Compound("IsEnableShake")]
        public float Speed = 5f;
        [Compound("IsEnableShake")]
        public Vector3 Amount = new Vector3(0.4f, 0.4f);

        [HeaderLine("Knockback")]
        [InspectorLabel("Enabled")]
        public bool IsEnableKnockback;
        [Compound("IsEnableKnockback")]
        public float KnockbackChance = 0.7f;
        [Compound("IsEnableKnockback")]
        public float KnockbackStrength = 30f;
        [Compound("IsEnableKnockback")]
        public float KnockbackAcceleration = 50f;
        [Compound("IsEnableKnockback")]
        public float KnockbackDuration = 1f;

        [NonSerialized]
        public GameObject Sender;
    }
}
