using UnityEngine;
//============================================================
namespace FKLib
{
    public class RemotePlayer : IPlayer
    {
        public INetworkConnection NetworkConnection { get; set; }

        private bool _isPlayerLeft;

        public override void Initialize(ICellGrid cellGrid)
        {
            base.Initialize(cellGrid);
            NetworkConnection.PlayerLeftRoom += (sender, networkUser) =>
            {
                if (networkUser.CustomProperties.TryGetValue("playerID", out string leavingPlayerNumber) 
                && PlayerID.Equals(int.Parse(leavingPlayerNumber)))
                {
                    Debug.Log("Remote player left");
                    _isPlayerLeft = true;

                    if (NetworkConnection.IsHost && PlayerID.Equals(cellGrid.CurrentPlayerID))
                    {
                        cellGrid.EndTurn();
                    }
                }
            };
        }

        public override void Play(ICellGrid cellGrid)
        {
            cellGrid.cellGridState = new CellGridStateRemotePlayerTurn(cellGrid);
            if (NetworkConnection.IsHost && _isPlayerLeft)
            {
                cellGrid.EndTurn();
            }
        }
    }
}
