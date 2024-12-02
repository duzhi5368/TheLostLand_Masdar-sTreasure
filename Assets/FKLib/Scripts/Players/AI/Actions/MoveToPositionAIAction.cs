using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class MoveToPositionAIAction : IAIAction
    {
        public bool IsShouldMoveAllTheWay = true;

        private ICell                                           _topDestination = null;
        private Dictionary<ICell, string>                       _cellMetadata;
        private IEnumerable<(ICell cell, float value)>          _cellScores;
        private Dictionary<ICell, float>                        _cellScoresDict;
        private Dictionary<string, Dictionary<string, float>>   _executionTime;
        private Gradient                                        _debugGradient;
        private Stopwatch                                       _stopwatch = new Stopwatch();


        private void Awake()
        {
            var colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0.2f);
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.green, 0.8f);
            _debugGradient = new Gradient();
            _debugGradient.SetKeys(colorKeys, new GradientAlphaKey[0]);
        }

        public override void InitializeAction(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            unit.GetComponent<MoveAbility>().OnAbilitySelected(cellGrid);

            _cellMetadata = new Dictionary<ICell, string>();
            _cellScoresDict = new Dictionary<ICell, float>();
            cellGrid.Cells.ForEach(c =>
            {
                _cellMetadata[c] = "";
                _cellScoresDict[c] = 0.0f;
            });

            _executionTime = new Dictionary<string, Dictionary<string, float>>();
            _executionTime.Add("precalculate", new Dictionary<string, float>());
            _executionTime.Add("evaluate", new Dictionary<string, float>());
        }

        public override bool ShouldExecute(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            if (unit.GetComponent<MoveAbility>() == null)
                return false;

            var evaluators = GetComponents<ICellEvaluator>();
            foreach (var evaluator in evaluators)
            {
                _stopwatch.Start();
                evaluator.Precalculate(unit, player, cellGrid);
                _stopwatch.Stop();

                _executionTime["precalculate"].Add(evaluator.GetType().Name, _stopwatch.ElapsedMilliseconds);
                _executionTime["evaluate"].Add(evaluator.GetType().Name, 0);

                _stopwatch.Reset();
            }

            _cellScores = cellGrid.Cells.Select(c => (cell: c, value: evaluators.Select(e =>
            {
                _stopwatch.Start();
                var score = e.Evaluate(c, unit, player, cellGrid);
                _stopwatch.Stop();

                _executionTime["evaluate"][e.GetType().Name] += _stopwatch.ElapsedMilliseconds;

                _stopwatch.Reset();

                var weightedScore = score * e.Weight;
                if ((player as AIPlayer).IsDebugMode)
                {
                    _cellMetadata[c] += string.Format("{0} * {1} = {2} : {3}\n", 
                        e.Weight.ToString("+0.00;-0.00"), 
                        score.ToString("+0.00;-0.00"), 
                        weightedScore.ToString("+0.00;-0.00"), 
                        e.GetType().ToString());
                }

                _cellScoresDict[c] += weightedScore;
                return weightedScore;

            }).DefaultIfEmpty(0.0f).Aggregate((result, next) => result + next))).OrderByDescending(x => x.value);

            var (topCell, maxValue) = _cellScores.Where(o => unit.IsCellMoveableTo(o.cell)).First();
            var currentCellVal = _cellScoresDict[unit.Cell];

            if(maxValue > currentCellVal)
            {
                _topDestination = topCell;
                return true;
            }
            _topDestination = unit.Cell;
            return false;
        }

        public override void Precalculate(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            var path = unit.FindPath(cellGrid.Cells, _topDestination);
            List<ICell> selectedPath = new List<ICell>();
            float cost = 0.0f;
            for (int i = path.Count - 1; i >= 0; i--) { 
                var cell = path[i];
                cost += cell.MovementCost[ENUM_MoveType.eMT_Normal];
                if (cost <= unit.MovementPoints)
                {
                    selectedPath.Add(cell);
                }
                else
                {
                    for (int j = selectedPath.Count - 1; j >= 0; j--)
                    {
                        if (!unit.IsCellMoveableTo(selectedPath[j]))
                        {
                            selectedPath.RemoveAt(j);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                }
            }
            selectedPath.Reverse();
            if (selectedPath.Count != 0) { 
                _topDestination = IsShouldMoveAllTheWay ? selectedPath[0] : selectedPath.OrderByDescending(c => _cellScoresDict[c]).First();
            }
        }

        public override IEnumerator Execute(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            unit.GetComponent<MoveAbility>().Destination = _topDestination;
            yield return unit.GetComponent<MoveAbility>().AIExecute(cellGrid);
        }

        public override void CleanUp(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            foreach(var cell in cellGrid.Cells)
                cell.UnMark();
            _topDestination = null;
            (cellGrid.cellGridState as CellGridStateAITurn).CellDebugInfo = null;
        }

        public override void ShowDebugInfo(IPlayer player, IUnit unit, ICellGrid cellGrid)
        {
            Dictionary<ICell, DebugInfo> cellDebugInfo = new Dictionary<ICell, DebugInfo>();

            var maxScore = _cellScores.Max(x => x.value);
            var minScore = _cellScores.Min(x => x.value);
            maxScore = maxScore.Equals(float.NaN) ? 0 : maxScore;
            minScore = minScore.Equals(float.NaN) ? 0 : minScore;

            var cellScoresEnumerator = _cellScores.GetEnumerator();
            cellScoresEnumerator.MoveNext();
            var (topCell, _) = cellScoresEnumerator.Current;
            cellDebugInfo[topCell] = new DebugInfo(_cellMetadata[topCell], Color.blue);

            while (cellScoresEnumerator.MoveNext())
            {
                var (cell, value) = cellScoresEnumerator.Current;
                var color = _debugGradient.Evaluate((value - minScore) / (Mathf.Abs(maxScore - minScore) + float.Epsilon));
                _cellMetadata[cell] += string.Format("Total: {0}", _cellScoresDict[cell].ToString("0.00"));
                cellDebugInfo[cell] = new DebugInfo(_cellMetadata[cell], color);
            }

            cellScoresEnumerator.Dispose();

            cellDebugInfo[_topDestination].Color = Color.magenta;
            (cellGrid.cellGridState as CellGridStateAITurn).CellDebugInfo = cellDebugInfo;

            var evaluators = GetComponents<ICellEvaluator>();
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

