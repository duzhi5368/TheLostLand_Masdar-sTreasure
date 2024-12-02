//============================================================
namespace FKLib
{
    public abstract class IUnitState
    {
        protected IUnit _unit;

        public IUnitState(IUnit unit)
        {
            _unit = unit;
        }

        public abstract void Apply();
        public abstract void MakeTransition(IUnitState state);
    }
}
