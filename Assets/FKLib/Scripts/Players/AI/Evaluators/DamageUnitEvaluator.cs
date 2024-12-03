using System.Linq;
//============================================================
namespace FKLib
{
    public class DamageUnitEvaluator : IUnitEvaluator
    {
        private float _topDamage;


        public override void Precalculate(IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid) 
        {
            var enemyUnits = cellGrid.GetEnemyUnits(currentPlayer);
            var enemiesInRange = enemyUnits.Where(u => evaluatingUnit.Cell.GetDistance(u.Cell) <= evaluatingUnit.AttackRange);
            _topDamage = enemiesInRange.Select(u => evaluatingUnit.DryAttack(u)).DefaultIfEmpty().Max();
        }
        public override float Evaluate(IUnit unitToEvaluate, IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid)
        {
            var score = evaluatingUnit.DryAttack(unitToEvaluate) / _topDamage;
            return score;
        }
    }
}
