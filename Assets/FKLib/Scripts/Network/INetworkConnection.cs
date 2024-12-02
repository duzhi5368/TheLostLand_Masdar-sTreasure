using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class INetworkConnection : MonoBehaviour
    {
        [SerializeField]
        private ICellGrid _cellGrid;

        public event EventHandler ServerConnected;                  // when the server is connect
        public event EventHandler<IRoomData> RoomJoined;            // when a room is successfully joined
        public event EventHandler RoomExited;                       // local player exit a room
        public event EventHandler<INetworkUser> PlayerEnteredRoom;  // when a new player enters the room
        public event EventHandler<INetworkUser> PlayerLeftRoom;     // when a player left the room
        public event EventHandler<string> JoinRoomFailed;           // join a room failed
        public event EventHandler<string> CreateRoomFailed;         // create a room failed

        public virtual bool IsHost { get; protected set; }          // is local player hosting this room

        protected Dictionary<long, Action<Dictionary<string, string>>> Handlers = new Dictionary<long, Action<Dictionary<string, string>>>();
        protected Queue<(Action preAction, Func<IEnumerator> routine)> EventQueue = new Queue<(Action preAction, Func<IEnumerator> routine)>();
        protected bool isProcessingEvents;


        public abstract void ConnectToServer(string userName, Dictionary<string, string> customParams);
        public abstract void JoinQuickMatch(int maxPlayers, Dictionary<string, string> customParams);
        public abstract void CreateRoom(string roomName, int maxPlayers, bool isPrivate, Dictionary<string, string> customParams);
        public abstract void JoinRoomByName(string roomName);
        public abstract void JoinRoomByID(string roomID);
        public abstract void LeaveRoom();
        public abstract Task<IEnumerable<IRoomData>> GetRoomList();
        public abstract void SendMatchState(long opCode, IDictionary<string, string> actionParams);

        public virtual void AddHandler(Action<Dictionary<string, string>> handler, long OpCode)
        {
            Handlers.Add(OpCode, handler);
        }
        public virtual void InitializeRng(int seed)
        {
            UnityEngine.Random.InitState(seed);
        }
        protected virtual void Awake()
        {
            Handlers.Add((long)ENUM_OpCode.eOC_TurnEnded, HandleRemoteTurnEnding);
            Handlers.Add((long)ENUM_OpCode.eOC_AbilityUsed, HandleRemoteAbilityUsed);
        }
        protected virtual void Start()
        {
            _cellGrid.UnitAdded += OnUnitAdded;
            _cellGrid.TurnEnded += OnTurnEndedLocal;
        }
        protected void InvokeServerConnected()
        {
            Debug.Log("Server connected");
            ServerConnected?.Invoke(this, EventArgs.Empty);
        }
        protected void InvokeRoomJoined(IRoomData roomData)
        {
            var players = roomData.Users.ToList();
            Debug.Log($"Joined room: {roomData.RoomID}; players inside: {players.Count}");
            RoomJoined?.Invoke(this, roomData);
        }
        protected void InvokeRoomExited()
        {
            Debug.Log("Exited room");
            RoomExited?.Invoke(this, EventArgs.Empty);
        }
        protected void InvokePlayerEnteredRoom(INetworkUser networkUser)
        {
            Debug.Log($"Player {networkUser.UserID} entered room");
            PlayerEnteredRoom?.Invoke(this, networkUser);
        }
        protected void InvokePlayerLeftRoom(INetworkUser networkUser)
        {
            Debug.Log($"Player {networkUser.UserID} left room");
            PlayerLeftRoom?.Invoke(this, networkUser);
        }
        protected void InvokeCreateRoomFailed(string message)
        {
            CreateRoomFailed?.Invoke(this, message);
        }
        protected void InvokeJoinRoomFailed(string message)
        {
            JoinRoomFailed?.Invoke(this, message);
        }
        private void OnUnitAdded(object sender, UnitCreatedEventArgs e)
        {
            foreach (var ability in e.Unit.GetComponent<IUnit>().Abilities)
            {
                ability.AbilityUsed += OnAbilityUsedLocal;
            }
            e.Unit.GetComponent<IUnit>().AbilityAdded += OnAbilityAdded;
        }
        private void OnAbilityAdded(object sender, AbilityAddedEventArgs e)
        {
            e.ability.AbilityUsed += OnAbilityUsedLocal;
        }
        private void OnAbilityUsedLocal(object sender, (bool isNetworkInvoked, IDictionary<string, string> actionParams) e)
        {
            if (e.isNetworkInvoked)
                return;
            SendMatchState((int)ENUM_OpCode.eOC_AbilityUsed, e.actionParams);
        }
        private void OnTurnEndedLocal(object sender, bool isNetworkInvoked)
        {
            if (isNetworkInvoked)
                return;
            SendMatchState((int)ENUM_OpCode.eOC_TurnEnded, new Dictionary<string, string>());
        }

        private void HandleRemoteAbilityUsed(Dictionary<string, string> actionParams)
        {
            var unit = _cellGrid.Units.Find(u => u.UnitID == int.Parse(actionParams["unit_id"]));
            var ability = unit.Abilities.Find(a => a.AbilityID == int.Parse(actionParams["ability_id"]));

            EventQueue.Enqueue((() => ability.OnAbilitySelected(_cellGrid), () => ability.Apply(_cellGrid, actionParams)));
            if (!isProcessingEvents)
            {
                StartCoroutine(ProcessEvents());
            }
        }
        private void HandleRemoteTurnEnding(Dictionary<string, string> actionParams)
        {
            EventQueue.Enqueue((new Action(() => { }), () => EndTurn(true)));
            if (!isProcessingEvents)
            {
                StartCoroutine(ProcessEvents());
            }
        }
        protected IEnumerator EndTurn(bool isNetworkInvoked)
        {
            _cellGrid.EndTurn(isNetworkInvoked);
            yield return null;
        }
        protected virtual IEnumerator ProcessEvents()
        {
            isProcessingEvents = true;
            while (EventQueue.Count > 0)
            {
                var (preAction, routine) = EventQueue.Dequeue();
                preAction.Invoke();
                yield return StartCoroutine(routine.Invoke());
            }
            isProcessingEvents = false;
        }
    }
}
