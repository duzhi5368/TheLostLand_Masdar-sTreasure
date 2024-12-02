//============================================================
namespace FKLib
{
    public class UnitStateMarkedAsFinished : IUnitState
    {
        public UnitStateMarkedAsFinished(IUnit unit) : base(unit) { }

        public override void Apply()
        {
            _unit.MarkAsFinished();
        }

        public override void MakeTransition(IUnitState state)
        {
            if (state is UnitStateNormal) { 
                state.Apply();
                _unit.UnitState = state;
            }
        }
    }
}
