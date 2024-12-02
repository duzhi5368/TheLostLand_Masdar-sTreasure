//============================================================
namespace FKLib
{
    public class UnitStateMarkedAsSelected : IUnitState
    {
        public UnitStateMarkedAsSelected(IUnit unit) : base(unit) { }

        public override void Apply()
        {
            _unit.MarkAsSelected();
        }

        public override void MakeTransition(IUnitState state)
        {
            state.Apply();
            _unit.UnitState = state;
        }
    }
}
