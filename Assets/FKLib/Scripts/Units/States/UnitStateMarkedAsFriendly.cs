//============================================================
namespace FKLib
{
    public class UnitStateMarkedAsFriendly : IUnitState
    {
        public UnitStateMarkedAsFriendly(IUnit unit) : base(unit) { }

        public override void Apply()
        {
            _unit.MarkAsFriendly();
        }

        public override void MakeTransition(IUnitState state)
        {
            state.Apply();
            _unit.UnitState = state;
        }
    }
}
