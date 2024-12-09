using UnityEngine;
//============================================================
namespace FKLib
{
    public class TUISlot<T> : MonoBehaviour where T : class
    {
        private TUIContainer<T> _container; // The item container that holds the slots.
        public TUIContainer<T> Container
        {
            get { return _container; }
            set { _container = value; }
        }

        private int _index = -1;            // Index of the item container
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private T _item;                    // the item that this slot is holding
        public virtual T ObservedItem
        {
            get { return _item; }
            set { _item = value; Repaint(); }
        }

        public bool IsEmpty                 // Is this slot is empty
        {
            get { return ObservedItem == null; }
        }

        public virtual void Repaint()
        {

        }

        public virtual bool CanAddItem(T item)
        {
            return true;
        }
    }
}
