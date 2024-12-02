using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    /// <summary>
    /// This is the core class, include scene create, unit create and so on.
    /// </summary>
    public partial class ICellGrid : MonoBehaviour
    {
        #region private event params
        // This event is invoked before Initialize method runs.
        public event EventHandler LevelLoading;
        // This event is invoked after Initialize method has finished.
        public event EventHandler LevelLoadingDone;
        // This event is invoked at the beginning of StartGame method.
        public event EventHandler GameStarted;
        // This event is invoked at the end of a game.
        public event EventHandler<GameEndedArgs> GameEnded;
        // This event is invoked at the end of each turn.
        public event EventHandler<bool> TurnEnded;
        // This event is invoked when AddUnit method is called.
        public event EventHandler<UnitCreatedEventArgs> UnitAdded;
        #endregion

        #region params
        private ICellGridState _cellGridState;              // Current cell grid state
        private int _unitID = 0;
        private Func<List<IUnit>> _playableUnits = () => new List<IUnit>();

        public List<IPlayer> Players { get; private set; }  // all the players who play this game
        public List<ICell> Cells { get; private set; }      // all the cells in this game
        public List<IUnit> Units { get; private set; }      // all the units in this game
        public Transform PlayersParent;                     // The unity GameObject that holds all player objects
        public int CurrentPlayerID { get; private set; }    // current player id

        public bool ShouldStartGameImmediately = true;      // Should start game automatically
        public bool IsGameFinished { get; private set; }    // Is game finished

        #endregion

        #region private params methods
        public ICellGridState cellGridState
        {
            get { return _cellGridState; }
            set
            {
                ICellGridState nextState;
                if (_cellGridState != null)
                {
                    _cellGridState.OnStateExit();
                    nextState = _cellGridState.MakeTransition(value);
                }
                else
                {
                    nextState = value;
                }
                _cellGridState = nextState;
                _cellGridState.OnStateEnter();
            }
        }

        public IPlayer CurrentPlayer
        {
            get
            {
                return Players.Find(p => p.PlayerID.Equals(CurrentPlayerID));
            }
        }

        public int NumberOfPlayers
        {
            get
            {
                return Players.Count;
            }
        }
        #endregion

        #region unity methods
        private void Start()
        {
            if (ShouldStartGameImmediately)
            {
                Initialize();
            }
        }
        #endregion

        #region private event handle methods
        private void OnCellUnHighLighted(object sender, EventArgs e)
        {
            if (_cellGridState != null)
                _cellGridState.OnCellUnSelected(sender as ICell);
        }
        private void OnCellHighLighted(object sender, EventArgs e)
        {
            if (_cellGridState != null)
                _cellGridState.OnCellSelected(sender as ICell);
        }
        private void OnCellClicked(object sender, EventArgs e)
        {
            if (_cellGridState != null)
                _cellGridState.OnCellClicked(sender as ICell);
        }
        private void OnUnitClicked(object sender, EventArgs e)
        {
            if (_cellGridState != null)
                _cellGridState.OnUnitClicked(sender as IUnit);
        }
        private void OnUnitHighLighted(object sender, EventArgs e)
        {
            if (_cellGridState != null)
                _cellGridState.OnUnitHighLighted(sender as IUnit);
        }
        private void OnUnitUnHighLighted(object sender, EventArgs e)
        {
            if (_cellGridState != null)
                _cellGridState.OnUnitUnHighLighted(sender as IUnit);
        }
        private void OnUnitMoved(object sender, EventArgs e)
        {
            CheckIsGameFinished();
        }
        private void OnUnitDestroyed(object sender, AttackEventArgs e)
        {
            Units.Remove(e.Defender);
            e.Defender.GetComponents<IAbility>().ToList().ForEach(a => a.OnUnitDestroyed(this));
            e.Defender.UnitClicked -= OnUnitClicked;
            e.Defender.UnitHighLighted -= OnUnitHighLighted;
            e.Defender.UnitUnHighLighted -= OnUnitUnHighLighted;
            e.Defender.UnitDestroyed -= OnUnitDestroyed;
            e.Defender.UnitMoved -= OnUnitMoved;
            CheckIsGameFinished();
        }
        #endregion

        #region core functions
        // Method will be called once, at the beginning of the game.
        public void Initialize()
        {
            IsGameFinished = false;

            // dispatch event
            if (LevelLoading != null)
            {
                LevelLoading.Invoke(this, EventArgs.Empty);
            }

            // initialize players
            Players = new List<IPlayer>();
            for (int i = 0; i < PlayersParent.childCount; i++)
            {
                var player = PlayersParent.GetChild(i).GetComponent<IPlayer>();
                if (player != null && player.gameObject.activeInHierarchy)
                {
                    player.Initialize(this);
                    Players.Add(player);
                }
            }

            // initialize cells
            Cells = new List<ICell>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var cell = transform.GetChild(i).gameObject.GetComponent<ICell>();
                if (cell != null)
                {
                    if (cell.gameObject.activeInHierarchy)
                    {
                        Cells.Add(cell);
                        cell.Initialize(this);
                    }
                }
                else
                {
                    Debug.LogError("Invalid object in cells parent game object");
                }
            }

            foreach (var cell in Cells)
            {
                cell.CellClicked += OnCellClicked;
                cell.CellHighLighted += OnCellHighLighted;
                cell.CellUnHighLighted += OnCellUnHighLighted;
                cell.GetComponent<ICell>().GetNeighbors(Cells);
            }

            // initialize units
            Units = new List<IUnit>();
            var unitGenerator = GetComponent<IUnitGenerator>();
            if (unitGenerator != null)
            {
                var units = unitGenerator.SpawnUnits(Cells);
                foreach (var unit in units)
                {
                    AddUnit(unit.GetComponent<Transform>());
                }
            }
            else
            {
                Debug.LogError("No IUnitGenerator script attached to cell grid");
            }

            // dispatch event
            if (LevelLoadingDone != null)
            {
                LevelLoadingDone.Invoke(this, EventArgs.Empty);
            }

            // start turn
            ITransitionResult transitionResult = GetComponent<ITurnResolver>().ResolveStart(this);
            _playableUnits = transitionResult.PlayableUnits;
            CurrentPlayerID = transitionResult.NextPlayer.PlayerID;

            GameStarted?.Invoke(this, EventArgs.Empty);

            _playableUnits().ForEach(unit =>
            {
                unit.GetComponents<IAbility>().ToList<IAbility>().ForEach(ability => ability.OnTurnStart(this));
                unit.OnTurnStart();
            });
            CurrentPlayer.Play(this);
            Debug.Log("Game started.");
        }

        // add unit to the game
        public void AddUnit(Transform unit, ICell targetCell = null, IPlayer ownerPlayer = null)
        {
            unit.GetComponent<IUnit>().UnitID = _unitID++;
            Units.Add(unit.GetComponent<IUnit>());

            if (targetCell != null) {
                targetCell.IsTaken = unit.GetComponent<IUnit>().IsObstructable;
                unit.GetComponent<IUnit>().Cell = targetCell;
                unit.GetComponent<IUnit>().transform.localPosition = targetCell.transform.localPosition;
            }

            if (ownerPlayer != null)
                unit.GetComponent<IUnit>().OwnerPlayerID = ownerPlayer.PlayerID;

            if (unit.GetComponent<IUnit>().Cell != null)
                unit.GetComponent<IUnit>().Cell.CurrentUnits.Add(unit.GetComponent<IUnit>());

            unit.GetComponent<IUnit>().transform.localRotation = Quaternion.Euler(0, 0, 0);
            unit.GetComponent<IUnit>().Initialize();

            unit.GetComponent<IUnit>().UnitClicked += OnUnitClicked;
            unit.GetComponent<IUnit>().UnitHighLighted += OnUnitHighLighted;
            unit.GetComponent<IUnit>().UnitUnHighLighted += OnUnitUnHighLighted;
            unit.GetComponent<IUnit>().UnitDestroyed += OnUnitDestroyed;
            unit.GetComponent<IUnit>().UnitMoved += OnUnitMoved;

            if (UnitAdded != null)
                UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit));
        }

        // Method will be called when end a turn.
        public void EndTurn(bool isNetworkInvoked = false)
        {
            _cellGridState.EndTurn(isNetworkInvoked);
        }

        // Actual turn transitions.
        public void EndTurnExecute(bool isNetworkInvoked = false)
        {
            _cellGridState = new CellGridStateBlockInput(this);

            bool isGameFinished = CheckIsGameFinished();
            if (isGameFinished)
                return;

            var playableUnits = _playableUnits();
            for(int i = 0; i < playableUnits.Count; i++)
            {
                var unit = playableUnits[i];
                if(unit == null)
                    continue;
                unit.OnTurnEnd();
                var abilities = unit.GetComponents<IAbility>();
                for(int j = 0; j < abilities.Length; j++)
                {
                    var ability = abilities[j];
                    ability.OnTurnEnd(this);
                }
            }

            ITransitionResult transitionResult = GetComponent<ITurnResolver>().ResolveTurn(this);
            _playableUnits = transitionResult.PlayableUnits;
            CurrentPlayerID = transitionResult.NextPlayer.PlayerID;

            if (TurnEnded != null)
                TurnEnded.Invoke(this, isNetworkInvoked);

            Debug.Log(string.Format("Current turn is Player {0}'s turn", CurrentPlayerID));

            playableUnits = _playableUnits();
            for (int i = 0; i < playableUnits.Count; i++)
            {
                var unit = playableUnits[i];
                if (unit == null)
                    continue;

                var abilities = unit.GetComponents<IAbility>();
                for(int j = 0;j < abilities.Length; j++)
                {
                    var ability = abilities[j];
                    ability.OnTurnStart(this);
                }
                unit.OnTurnStart();
            }
            CurrentPlayer.Play(this);
        }

        // Check is the game finished.
        public bool CheckIsGameFinished()
        {
            List<GameResult> gameResults = GetComponents<IGameEndCondition>().Select(c => c.CheckCondition(this)).ToList();
            foreach (var gameResult in gameResults)
            {
                _cellGridState = new CellGridStateGameOver(this);
                IsGameFinished = true;
                if(GameEnded != null)
                    GameEnded.Invoke(this, new GameEndedArgs(gameResult));
                break;
            }
            return IsGameFinished;
        }
        #endregion

        #region simple convenient functions
        public List<IUnit> GetCurrentPlayerUnits()
        {
            return _playableUnits();
        }

        public List<IUnit> GetEnemyUnits(IPlayer player)
        {
            return Units.FindAll(unit => unit.OwnerPlayerID != player.PlayerID);
        }

        public List<IUnit> GetPlayerUnits(IPlayer player)
        {
            return Units.FindAll(unit => unit.OwnerPlayerID == player.PlayerID);
        }
        #endregion
    }
}