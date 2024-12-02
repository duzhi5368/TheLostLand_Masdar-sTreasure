using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class AttackAIAction : IAIAction
    {
        private IUnit _targetUnit;
        private Dictionary<IUnit, string> _unitDebugInfo;
        private List<(IUnit unit, float value)> _unitScores;
        private Dictionary<string, Dictionary<string, float>> _executionTime;
        private Stopwatch _stopwatch = new Stopwatch();

        public override void InitializeAction(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            unit.GetComponent<AttackAbility>().OnAbilitySelected(cellGrid);

            _executionTime = new Dictionary<string, Dictionary<string, float>>();
            _executionTime.Add("precalculate", new Dictionary<string, float>());
            _executionTime.Add("evaluate", new Dictionary<string, float>());
        }

        public override bool ShouldExecute(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            if (unit.GetComponent<AttackAbility>() == null)
                return false;

            var enemyUnits = cellGrid.GetEnemyUnits(player);
            var isEnemyInRange = enemyUnits.Select(u => unit.IsUnitAttackable(u, unit.Cell))
                .Aggregate((result, next) => result || next);
            return isEnemyInRange && unit.ActionPoints > 0;
        }

        public override void Precalculate(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            var enemyUnits = cellGrid.GetEnemyUnits(player);
            var enemiesInRange = enemyUnits.Where(e => unit.IsUnitAttackable(e, unit.Cell)).ToList();

            _unitDebugInfo = new Dictionary<IUnit, string>();
            enemyUnits.ForEach(u => _unitDebugInfo[u] = "");

            if (enemiesInRange.Count == 0)
                return;

            var evaluators = GetComponents<IUnitEvaluator>();
            foreach( var e in evaluators)
            {
                _stopwatch.Start();
                e.Precalculate(unit, player, cellGrid);
                _stopwatch.Stop();

                _executionTime["precalculate"].Add(e.GetType().Name, _stopwatch.ElapsedMilliseconds);
                _executionTime["evaluate"].Add(e.GetType().Name, 0);

                _stopwatch.Reset();
            }

            _unitScores = enemiesInRange.Select(u => (unit: u, value: evaluators.Select(e =>
            {
                _stopwatch.Start();
                var score = e.Evaluate(u, unit, player, cellGrid);
                _stopwatch.Stop();
                _executionTime["evaluate"][e.GetType().Name] += _stopwatch.ElapsedMilliseconds;
                _stopwatch.Reset();

                var weightedScore = score * e.Weight;
                _unitDebugInfo[u] += string.Format("{0:+0.00;-0.00} * {1:+0.00;-0.00} = {2:+0.00;-0.00} : {3}\n", 
                    e.Weight, score, weightedScore, e.GetType().ToString());

                return weightedScore;
            }).DefaultIfEmpty(0.0f).Aggregate((result,next) => result + next))).ToList();

            _unitScores.ToList().ForEach(s => _unitDebugInfo[s.unit] += string.Format("Total: {0:0.00}", s.value));

            var (topUnit, maxValue) = _unitScores.OrderByDescending(o => o.value).First();
            _targetUnit = topUnit;
        }

        public override System.Collections.IEnumerator Execute(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            unit.GetComponent<AttackAbility>().UnitToAttack = _targetUnit;
            unit.GetComponent<AttackAbility>().UnitToAttackID = _targetUnit.UnitID;
            yield return StartCoroutine(unit.GetComponent<AttackAbility>().AIExecute(cellGrid));
            yield return new WaitForSeconds(0.5f);
        }

        public override void CleanUp(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            foreach(var enemy in cellGrid.GetEnemyUnits(player))
                enemy.UnMark();
            _targetUnit = null;
            _unitScores = null;
        }

        public override void ShowDebugInfo(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            (cellGrid.cellGridState as CellGridStateAITurn).UnitDebugInfo = _unitDebugInfo;
            if (_unitScores == null)
                return;

            var minScore = _unitScores.DefaultIfEmpty().Min(e => e.value);
            var maxScore = _unitScores.DefaultIfEmpty().Max(e => e.value);
            foreach(var (u, value) in _unitScores)
            {
                var color = Color.Lerp(Color.red, Color.green, value >= 0 ? value / maxScore : value / minScore * (-1));
                u.SetColor(color);
            }
            if(_targetUnit != null)
                _targetUnit.SetColor(Color.blue);

            var evaluators = GetComponents<IUnitEvaluator>();
            var sb = new StringBuilder();
            var sum = 0.0f;
            sb.AppendFormat("{0} evaluators execution time summary:\n", GetType().Name);
            foreach (var e in evaluators)
            {
                var precalculateTime = _executionTime["precalculate"][e.GetType().Name];
                var evaluateTime = _executionTime["evaluate"][e.GetType().Name];
                sum += precalculateTime + evaluateTime;

                sb.AppendFormat("total: {0}ms\tprecalculate: {1}ms\tevaluate: {2}ms\t:{3}\n",
                                (precalculateTime + evaluateTime).ToString().PadLeft(4),
                                precalculateTime.ToString().PadLeft(4),
                                evaluateTime.ToString().PadLeft(4),
                                e.GetType().Name);
            }
            sb.AppendFormat("sum: {0}ms", sum.ToString().PadLeft(4));
            UnityEngine.Debug.Log(sb.ToString());
        }
    }
}
