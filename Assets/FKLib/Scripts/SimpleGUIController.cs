using System;
using TbsFramework.Players;
using UnityEngine;
using UnityEngine.UI;
//============================================================
namespace FKLib
{
    public class SimpleGUIController : MonoBehaviour
    {
        public ICellGrid CellGrid;
        public Button EndTurnButton;

        private void Awake()
        {
            CellGrid.LevelLoading += OnLevelLoading;
            CellGrid.LevelLoadingDone += OnLevelLoadingDone;
            CellGrid.GameEnded += OnGameEnded;
            CellGrid.TurnEnded += OnTurnEnded;
            CellGrid.GameStarted += OnGameStarted;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M) && !(CellGrid.cellGridState is CellGridStateAITurn))
                CellGrid.EndTurn();
        }

        private void OnGameStarted(object sender, EventArgs e)
        {
            if (EndTurnButton != null)
                EndTurnButton.interactable = CellGrid.CurrentPlayer is HumanPlayer;
        }
        private void OnTurnEnded(object sender, bool isNetworkInvoked)
        {
            if (EndTurnButton != null)
                EndTurnButton.interactable = CellGrid.CurrentPlayer is HumanPlayer;
        }
        private void OnGameEnded(object sender, GameEndedArgs e)
        {
            Debug.Log(string.Format("Player{0} wins!", e.gameResult.WinningPlayers[0]));
            if (EndTurnButton != null)
                EndTurnButton.interactable = false;
        }
        private void OnLevelLoading(object sender, EventArgs e)
        {
            Debug.Log("Level is loading");
        }
        private void OnLevelLoadingDone(object sender, EventArgs e)
        {
            Debug.Log("Level loading done");
            Debug.Log("Press 'm' to end turn");
        }
    }
}
