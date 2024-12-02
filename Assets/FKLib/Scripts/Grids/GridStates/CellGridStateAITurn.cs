using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class CellGridStateAITurn : ICellGridState
    {
        private Dictionary<ICell, DebugInfo> _cellDebugInfo;
        private AIPlayer _AIPlayer;

        public Dictionary<IUnit, string> UnitDebugInfo { private get; set; }

        public Dictionary<ICell, DebugInfo> CellDebugInfo
        {
            get { return _cellDebugInfo; }
            set
            {
                _cellDebugInfo = value;
                if (value != null && _AIPlayer.IsDebugMode)
                {
                    foreach (ICell cell in _cellDebugInfo.Keys)
                    {
                        cell.SetColor(_cellDebugInfo[cell].Color);
                    }
                }
            }
        }

        public CellGridStateAITurn(ICellGrid cellGrid, AIPlayer AIPlayer)
            : base(cellGrid)
        {
            this._AIPlayer = AIPlayer;
        }

        public override void OnCellUnSelected(ICell cell)
        {
            base.OnCellUnSelected(cell);
            if (_AIPlayer.IsDebugMode && CellDebugInfo != null && CellDebugInfo.ContainsKey(cell))
                cell.SetColor(CellDebugInfo[cell].Color);
        }

        public override void OnCellSelected(ICell cell)
        {
            base.OnCellSelected(cell);
        }

        public override void OnCellClicked(ICell cell)
        {
            if (_AIPlayer.IsDebugMode && CellDebugInfo != null && CellDebugInfo.ContainsKey(cell))
                UnityEngine.Debug.Log(CellDebugInfo[cell].Metadata);
        }

        public override void OnUnitClicked(IUnit unit)
        {
            if (_AIPlayer.IsDebugMode && UnitDebugInfo != null && UnitDebugInfo.ContainsKey(unit))
                UnityEngine.Debug.Log(UnitDebugInfo[unit]);
        }

        public override void OnStateEnter()
        {
            // base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            // base.OnStateExit();
        }
    }
}
