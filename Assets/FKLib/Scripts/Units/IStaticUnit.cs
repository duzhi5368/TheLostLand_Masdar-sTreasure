using UnityEngine;
//============================================================
namespace FKLib
{
    public class IStaticUnit : IUnit
    {
        public Vector3 Offset;

        public override void Initialize()
        {
            base.Initialize();
            transform.localScale += Offset;
        }
        protected override void OnMoveFinished()
        {
            GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)(Cell.OffsetCoord.x * Cell.OffsetCoord.y);
        }
        public override bool IsUnitAttackable(IUnit other, ICell otherCell, ICell sourceCell)
        {
            return otherCell != null && base.IsUnitAttackable(other, otherCell, sourceCell);
        }
        public override void OnMouseDown()
        {
            base.OnMouseDown();
        }
    }
}
