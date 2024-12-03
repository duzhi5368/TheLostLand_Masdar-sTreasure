using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    public class DamageCellEvaluator : ICellEvaluator
    {
        private float _maxPossibleDamage;
        private List<IUnit> _enemyUnits;
        private Dictionary<IUnit, float> _damage;

        public override void Precalculate(IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid) 
        {
            _damage = new Dictionary<IUnit, float>();
            _maxPossibleDamage = 0f;
            _enemyUnits = cellGrid.GetEnemyUnits(currentPlayer);
            foreach (var enemy in _enemyUnits)
            {
                var realDamage = evaluatingUnit.DryAttack(enemy);
                _damage.Add(enemy, realDamage);
                if (realDamage > _maxPossibleDamage) {
                    _maxPossibleDamage = realDamage;
                }
            }
        }
        public override float Evaluate(ICell cellToEvaluate, IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid)
        {
            if (_maxPossibleDamage.Equals(0f))
                return 0f;

            var scores = _enemyUnits.Select(u =>
            {
                var isAttackableVal = evaluatingUnit.IsUnitAttackable(u, cellToEvaluate) ? 1.0f : 0.0f;
                var localScore = (isAttackableVal * _damage[u]) / _maxPossibleDamage;
                return localScore;
            });

            var maxScore = scores.DefaultIfEmpty().Max();
            return maxScore;
        }
    }
}
