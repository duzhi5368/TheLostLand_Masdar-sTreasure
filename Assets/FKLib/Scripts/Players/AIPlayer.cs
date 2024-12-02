using System;
using System.Collections;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class AIPlayer : IPlayer
    {
        public bool IsDebugMode;

        public override void Initialize(ICellGrid cellGrid)
        {
            cellGrid.GameEnded += OnGameEnded;
        }

        public override void Play(ICellGrid cellGrid)
        {
            cellGrid.cellGridState = new CellGridStateAITurn(cellGrid, this);
            StartCoroutine(PlayCoroutine(cellGrid));
        }


        private void Reset()
        {
            if (GetComponent<IUnitSelection>() == null)
                gameObject.AddComponent<MovementFreedomUnitSelection>();
        }
        private void OnGameEnded(object sender, EventArgs e)
        {
            StopAllCoroutines();
        }
        private IEnumerator PlayCoroutine(ICellGrid cellGrid)
        {
            var unitsOrdered = GetComponent<IUnitSelection>().SelectNext(() => cellGrid.GetCurrentPlayerUnits(), cellGrid);
            foreach (var unit in unitsOrdered) 
            {
                if (IsDebugMode)
                {
                    unit.MarkAsSelected();
                    Debug.Log(string.Format("Current unit: {0}, press N to continue", unit.name));
                    while(!Input.GetKeyDown(KeyCode.N))
                        yield return null;
                }

                var AIActions = unit.GetComponentsInChildren<IAIAction>();
                foreach (var aiAction in AIActions)
                {
                    yield return null;

                    aiAction.InitializeAction(this, unit, cellGrid);
                    var shouldExecuteAction = aiAction.ShouldExecute(this, unit, cellGrid);
                    if (IsDebugMode)
                    {
                        aiAction.Precalculate(this, unit, cellGrid);
                        aiAction.ShowDebugInfo(this, unit, cellGrid);
                        Debug.Log(string.Format("Current action: {0}, press A to execute", aiAction.GetType().ToString()));
                        while (!Input.GetKeyDown(KeyCode.A))
                        {
                            yield return null;
                        }
                        yield return null;
                    }
                    if (shouldExecuteAction) 
                    {
                        if (!IsDebugMode)
                        {
                            yield return null;
                            aiAction.Precalculate(this, unit, cellGrid);
                        }
                        yield return (aiAction.Execute(this, unit, cellGrid));
                    }
                    aiAction.CleanUp(this, unit, cellGrid);
                }

                unit.MarkAsFriendly();
            }

            cellGrid.EndTurn();
            yield return null;
        }
    }
}
