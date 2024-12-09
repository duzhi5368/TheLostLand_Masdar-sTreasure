using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class TUIContainer<T> : IUIWidget where T : class
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks);
                callbacks.Add("OnAddItem");
                callbacks.Add("OnRemoveItem");
                return callbacks.ToArray();
            }
        }

        [Header("Behaviour")]
        [Tooltip("Sets the container as dynamic. Slots are instantiated at runtime.")]
        [SerializeField]
        protected bool _isDynamicContainer = false;

        [Tooltip("The parent transform of slots.")]
        [SerializeField]
        protected Transform _slotParent;

        [Tooltip("The slot prefab. This game object should contain the Slot component or a child class of Slot.")]
        [SerializeField]
        protected GameObject _slotPrefab;

        protected List<T> _collection;
        protected List<TUISlot<T>> _slots = new List<TUISlot<T>>();
        public ReadOnlyCollection<TUISlot<T>> Slots
        {
            get
            {
                return _slots.AsReadOnly();
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _collection = new List<T>();
            RefreshSlots();
        }

        // Adds a new item to a free or dynamicly created slot in this container.
        public virtual bool AddItem(T item)
        {
            TUISlot<T> slot = null;
            if (CanAddItem(item, out slot, true))
            {
                ReplaceItem(slot.Index, item);
                return true;
            }
            return false;
        }

        // Removes the item at index. Sometimes an item requires more then one slot(two-handed weapon),
        // this will remove the item with the extra slots.
        public virtual bool RemoveItem(int index)
        {
            if (index < _slots.Count)
            {
                TUISlot<T> slot = _slots[index];
                T item = slot.ObservedItem;
                if (item != null)
                {
                    _collection.Remove(item);
                    slot.ObservedItem = null;
                    return true;
                }
            }
            return false;
        }

        // Replaces the items at index and returns the previous item.
        public virtual T ReplaceItem(int index, T item)
        {
            if (index < _slots.Count)
            {
                TUISlot<T> slot = _slots[index];
                if (!slot.CanAddItem(item))
                {
                    return item;
                }
                if (item != null)
                {
                    _collection.Add(item);
                    T current = slot.ObservedItem;
                    if (current != null)
                    {
                        RemoveItem(slot.Index);
                    }
                    slot.ObservedItem = item;
                    return current;
                }
            }
            return item;
        }

        // Checks if the item can be added to this container.
        public virtual bool CanAddItem(T item, out TUISlot<T> slot, bool createSlot = false)
        {
            slot = null;
            if (item == null)
                return true;

            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].IsEmpty && _slots[i].CanAddItem(item))
                {
                    slot = _slots[i];
                    return true;
                }
            }
            if (_isDynamicContainer)
            {
                if (createSlot)
                {
                    slot = CreateSlot();
                }
                return true;
            }
            return false;
        }

        // Refreshs the slot list and reorganize indices
        public void RefreshSlots()
        {
            if (_isDynamicContainer && _slotParent != null)
            {
                _slots = _slotParent.GetComponentsInChildren<TUISlot<T>>(true).ToList();
                _slots.Remove(_slotPrefab.GetComponent<TUISlot<T>>());
            }
            else
            {
                _slots = GetComponentsInChildren<TUISlot<T>>(true).ToList();
            }

            for (int i = 0; i < _slots.Count; i++)
            {
                TUISlot<T> slot = _slots[i];
                slot.Index = i;
                slot.Container = this;
            }
        }

        // Creates a new slot
        protected virtual TUISlot<T> CreateSlot()
        {
            if (_slotPrefab != null && _slotParent != null)
            {
                GameObject go = (GameObject)Instantiate(_slotPrefab);
                go.SetActive(true);
                go.transform.SetParent(_slotParent, false);
                TUISlot<T> slot = go.GetComponent<TUISlot<T>>();
                _slots.Add(slot);
                slot.Index = Slots.Count - 1;
                slot.Container = this;
                return slot;
            }
            Debug.LogWarning("Please ensure that the slot prefab and slot parent is set in the inspector.");
            return null;
        }

        // Destroy the slot and reorganize indices.
        protected virtual void DestroySlot(int index)
        {
            if (index < _slots.Count)
            {
                DestroyImmediate(_slots[index].gameObject);
                RefreshSlots();
            }
        }
    }
}
