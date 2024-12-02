using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IBuffer : ScriptableObject
    {
        public int Duration;    // How many turns this buffer will last, -1 means forever

        // Upgrade the unit
        public abstract void Apply(IUnit unit);
        // Update unit to normal state
        public abstract void Undo(IUnit unit);
    }
}
