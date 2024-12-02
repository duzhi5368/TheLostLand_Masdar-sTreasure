//============================================================
namespace FKLib
{
    public class UnitStateNormal : IUnitState
    {
        public UnitStateNormal(IUnit unit) : base(unit) { }

        public override void Apply()
        {
            _unit.UnMark();
        }

        public override void MakeTransition(IUnitState state)
        {
            state.Apply();
            _unit.UnitState = state;
        }
    }
}
