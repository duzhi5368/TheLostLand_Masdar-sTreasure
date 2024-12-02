using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TbsFramework.Players.AI.Evaluators;
using UnityEngine;
//============================================================
namespace FKLib
{
    /// <summary>
    /// Base class for all units in the game.
    /// </summary>
    [ExecuteInEditMode]
    public class IUnit : MonoBehaviour
    {
        #region params
        [SerializeField]
        [HideInInspector]
        private ICell _cell;            // The cell that this unit currently occupying.
        [SerializeField]
        private float _movementPoints;  // The unit's mobility 
        [SerializeField]
        private float _actionPoints;    // How many attack times can be use in one turn.

        private Dictionary<ICell, IList<ICell>> _cachedPaths = null;
        private int _abilityID = 0;
        private List<(IBuffer buffer, int timeleft)> _buffers;
        private static IPathfinding _pathfinder = new DijkstraPathfinding();

        public event EventHandler UnitClicked;                          // Event when this unit be clicked
        public event EventHandler UnitSelected;                         // Event when user clicks on unit which belongs to him
        public event EventHandler UnitUnSelected;                       // Event when user clicks outside of currently selected unit
        public event EventHandler UnitHighLighted;                      // Event when user moves cursor over this unit
        public event EventHandler UnitUnHighLighted;                    // Event when user moves cursor exit this unit
        public event EventHandler<AttackEventArgs> UnitAttacked;        // Event when this unit is attacked
        public event EventHandler<AttackEventArgs> UnitDestroyed;       // Event when this unit hp below 0
        public event EventHandler<MovementEventArgs> UnitMoved;         // Event when this unit move from one cell to another
        public event EventHandler<AbilityAddedEventArgs> AbilityAdded;  // Event when this unit added a ability

        public UnitHighLighterAggregator UnitHighLighterAggregator;
        public List<IAbility> Abilities { get; private set; } = new List<IAbility>();
        public int UnitID {  get; set; }
        public bool IsObstructable = true;                              // Is this unit block a cell
        public IUnitState UnitState { get; set; }
        public float MovementAnimationSpeed;                            // Animation play speed
        public int OwnerPlayerID;                                       // This unit's owner's player id

        public int TotalHitPoints {  get; set; }
        public float TotalMovementPoints { get; set; }
        public float TotalActionPoints { get; set; }
        public int HitPoints;
        public int AttackRange;
        public int AttackFactor;
        public int DefenceFactor;

        public ICell Cell { get { return _cell; } set { _cell = value; } }
        public float MovementPoints { get { return _movementPoints; } set { _movementPoints = value; } }
        public float ActionPoints {  get { return _actionPoints; } set {_actionPoints = value; } }
        #endregion

        public void RegisterAbility(IAbility ability)
        {
            ability.AbilityID = _abilityID++;
            ability.UnitReference = this;
            Abilities.Add(ability);
            if(AbilityAdded != null)
                AbilityAdded.Invoke(this, new AbilityAddedEventArgs(ability));
        }

        public void SetState(IUnitState state)
        {
            UnitState.MakeTransition(state);
        }

        public void AddBuffer(IBuffer buffer)
        {
            buffer.Apply(this);
            _buffers.Add((buffer, buffer.Duration));
        }

        public virtual void SetColor(Color color) { }

        #region unity framework method
        public virtual void OnMouseDown()
        {
            if(UnitClicked != null)
                UnitClicked.Invoke(this, EventArgs.Empty);
        }
        public virtual void OnMouseEnter()
        {
            if(UnitHighLighted != null)
                UnitHighLighted.Invoke(this, EventArgs.Empty);
        }
        public virtual void OnMouseExit()
        {
            if(UnitUnHighLighted != null)
                UnitUnHighLighted.Invoke(this,EventArgs.Empty);
        }

        [ExecuteInEditMode]
        public void OnDestroy()
        {
#if UNITY_EDITOR
            if(Cell != null && !Application.isPlaying)
            {
                Cell.IsTaken = false;
                UnityEditor.EditorUtility.SetDirty(Cell);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
#endif
        }

        private void Reset()
        {
            if (GetComponent<AttackAbility>() == null)
                gameObject.AddComponent<AttackAbility>();
            if (GetComponent<MoveAbility>() == null)
                gameObject.AddComponent<MoveAbility>();
            if (GetComponent<AttackRangeHighLightAbility>() == null)
                gameObject.AddComponent<AttackRangeHighLightAbility>();

            GameObject brain = new GameObject("Brain");
            brain.transform.parent = transform;
            brain.AddComponent<MoveToPositionAIAction>();
            brain.AddComponent<AttackAIAction>();
            brain.AddComponent<DamageCellEvaluator>();
            brain.AddComponent<DamageUnitEvaluator>();
        }
        #endregion

        #region core function
        // Method will be called when this unit be added to the game.
        public virtual void Initialize()
        {
            _buffers = new List<(IBuffer, int)>();
            UnitState = new UnitStateNormal(this);

            TotalHitPoints = HitPoints;
            TotalMovementPoints = MovementPoints;
            TotalActionPoints = ActionPoints;

            foreach (var ability in GetComponentsInChildren<IAbility>())
            {
                RegisterAbility(ability);
                ability.Initialize();
            }
        }

        // Method will be called at the start of each turn.
        public virtual void OnTurnStart()
        {
            _cachedPaths = null;

            _buffers.FindAll(b => b.timeleft == 0).ForEach(b => { b.buffer.Undo(this); });
            _buffers.RemoveAll(b => b.timeleft == 0);
            var name = this.name;
            var state = UnitState;
            SetState(new UnitStateMarkedAsFriendly(this));
        }

        // Method will be called at the end of each turn.
        public virtual void OnTurnEnd()
        {
            for (int i = 0; i < _buffers.Count; i++)
            {
                (IBuffer buff, int timeLeft) = _buffers[i];
                _buffers[i] = (buff, timeLeft - 1);
            }
            MovementPoints = TotalMovementPoints;
            ActionPoints = TotalActionPoints;

            SetState(new UnitStateNormal(this));
        }

        // Method will be called when units HP drops below 0.
        protected virtual void OnDestoryed()
        {
            Cell.IsTaken = true;
            Cell.CurrentUnits.Remove(this);
            MarkAsDestoryed();
            Destroy(gameObject);
        }

        // Method will be called when unit is selected.
        public virtual void OnUnitSelected()
        {
            if(FindAnyObjectByType<ICellGrid>().GetCurrentPlayerUnits().Contains(this))
                SetState(new UnitStateMarkedAsSelected(this));
            if (UnitSelected != null)
                UnitSelected.Invoke(this, EventArgs.Empty);
        }

        // Method will be called when unit is unselected.
        public virtual void OnUnitUnSelected()
        {
            if (FindAnyObjectByType<ICellGrid>().GetCurrentPlayerUnits().Contains(this))
                SetState(new UnitStateMarkedAsFriendly(this));
            if (UnitUnSelected != null)
                UnitUnSelected.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region attack and defend functions
        // Check if it is possible to attack a unit from a given cell.
        public virtual bool IsUnitAttackable(IUnit other, ICell sourceCell)
        {
            return IsUnitAttackable(other, other.Cell, sourceCell);
        }
        public virtual bool IsUnitAttackable(IUnit other, ICell otherCell, ICell sourceCell)
        {
            return sourceCell.GetDistance(otherCell) <= AttackRange
                && other.OwnerPlayerID != OwnerPlayerID
                && ActionPoints >= 1;
        }

        // Method for calculating damage.
        protected virtual AttackAction DealDamage(IUnit unitToAttack)
        {
            return new AttackAction(AttackFactor, 1.0f);
        }

        // Method will be called after unit performed an attack.
        protected virtual void AttackActionPerformed(float actionCost)
        {
            ActionPoints -= actionCost;
        }

        // Method performs an attack on given unit.
        public void AttackHandler(IUnit unitToAttack)
        {
            AttackAction attackAction = DealDamage(unitToAttack);
            MarkAsAttacking(unitToAttack);
            unitToAttack.DefendHandler(this, attackAction.Damage);
            AttackActionPerformed(attackAction.ActionCost);
        }

        // Method for defending against an attack.
        public void DefendHandler(IUnit aggressor, int damage)
        {
            MarkAsDefending(aggressor);
            int damageTaken = Defend(aggressor, damage);
            HitPoints -= damageTaken;
            DefenceActionPerformed();

            if (UnitAttacked != null)
                UnitAttacked.Invoke(this, new AttackEventArgs(aggressor, this, damage));
            if (HitPoints <= 0)
            {
                if (UnitDestroyed != null)
                    UnitDestroyed.Invoke(this, new AttackEventArgs(aggressor, this, damage));
                OnDestoryed();
            }
        }

        // Method for calculating actual damage taken by the unit.
        protected virtual int Defend(IUnit aggressor, int damage)
        {
            return Mathf.Clamp(damage - DefenceFactor, 1, damage);
        }

        // Method will be called after unit performed defence.
        protected virtual void DefenceActionPerformed() { }
        #endregion

        #region move functions
        // Method for moving the unit
        public virtual IEnumerator Move(ICell destinationCell, IList<ICell> path)
        {
            var totalMovementCost = path.Sum(c => c.MovementCost[ENUM_MoveType.eMT_Normal]);
            MovementPoints -= totalMovementCost;

            Cell.IsTaken = false;
            Cell.CurrentUnits.Remove(this);
            Cell = destinationCell;
            destinationCell.IsTaken = true;
            destinationCell.CurrentUnits.Add(this);

            if(MovementAnimationSpeed > 0)
            {
                yield return DoMovementAnimation(path);
            }
            else
            {
                transform.position = Cell.transform.position;
                OnMoveFinished();
            }

            if (UnitMoved != null)
                UnitMoved.Invoke(this, new MovementEventArgs(Cell, destinationCell, path, this));
        }

        // Method for play movement animation.
        protected virtual IEnumerator DoMovementAnimation(IList<ICell> path)
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                var currentCell = path[i];
                Vector3 destinationPos = new Vector3(currentCell.transform.localPosition.x,
                    currentCell.transform.localPosition.y, transform.localPosition.z);
                while(transform.localPosition != destinationPos)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, destinationPos, Time.deltaTime * MovementAnimationSpeed);
                    yield return null;
                }
            }
            OnMoveFinished();
        }

        // Method will be called after movement animation has finished.
        protected virtual void OnMoveFinished() { }

        // Check if unit is able to moving to the cell which is given.
        public virtual bool IsCellMoveableTo(ICell cell)
        {
            return !cell.IsTaken;
        }

        // Check if unit is able to walk though the cell which is given.
        public virtual bool IsCellTraversable(ICell cell)
        {
            return !cell.IsTaken;
        }

        // Calculate all cells that the unit is able to move to.
        public HashSet<ICell> GetAvailableDestinations(List<ICell> cells)
        {
            if(_cachedPaths == null)
                CachePaths(cells);

            var availableDestinations = new HashSet<ICell>();
            foreach (var cell in cells.Where(c => IsCellMoveableTo(c)))
            {
                if(_cachedPaths.TryGetValue(cell, out var path))
                {
                    var pathCost = path.Sum(c => c.MovementCost[ENUM_MoveType.eMT_Normal]);
                    if (pathCost <= MovementPoints)
                        availableDestinations.Add(cell);
                }
            }
            return availableDestinations;
        }

        // Cache path cells
        public void CachePaths(List<ICell> cells)
        {
            var edges = GetGraphEdges(cells);
            _cachedPaths = _pathfinder.FindAllPaths(edges, Cell);
        }

        // Find path
        public IList<ICell> FindPath(List<ICell> cells, ICell destination)
        {
            if (_cachedPaths.TryGetValue(destination, out var path))
                return path;
            return new List<ICell>();
        }

        // Calculate graph representation of cell grid for pathfinding.
        protected virtual Dictionary<ICell, Dictionary<ICell, float>> GetGraphEdges(List<ICell> cells)
        {
            Dictionary<ICell, Dictionary<ICell, float>> ret = new Dictionary<ICell, Dictionary<ICell, float>>();
            foreach (var cell in cells)
            {
                if(IsCellTraversable(cell) || cell == Cell)
                {
                    ret[cell] = new Dictionary<ICell, float>();
                    foreach(var neighbor in cell.GetNeighbors(cells))
                    {
                        if(IsCellTraversable(neighbor) || IsCellMoveableTo(neighbor))
                        {
                            ret[cell][neighbor] = neighbor.MovementCost[ENUM_MoveType.eMT_Normal];
                        }
                    }
                }
            }
            return ret;
        }
        #endregion

        #region unit's visual state functions
        // Add visual sign that this unit is under attack.
        public virtual void MarkAsDefending(IUnit attacker)
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsDefendingFn?.ForEach(
                    o => o.Apply(this, attacker)
                );
        }

        // Add visual sign that this unit is attacking.
        public virtual void MarkAsAttacking(IUnit attackTarget)
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsAttackingFn?.ForEach(
                    o => o.Apply(this, attackTarget)
                );
        }

        // Add visual sign that this unit will be destoryed.
        // This method will be called before this unit be destoryed.
        public virtual void MarkAsDestoryed()
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsDestroyedFn?.ForEach(
                    o => o.Apply(this, null)
                );
        }

        // Add visual sign mark this unit is current player's unit.
        public virtual void MarkAsFriendly()
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsFriendlyFn?.ForEach(
                    o => o.Apply(this, null)
                );
        }

        // Add visual sign mark the units in range and can be attacked.
        public virtual void MarkAsReachableEnemy()
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsReachableEnemyFn?.ForEach(
                    o => o.Apply(this, null)
                );
        }

        // Add visual sign mark the unit that is currently selected.
        public virtual void MarkAsSelected()
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsSelectedFn?.ForEach(
                    o => o.Apply(this, null)
                );
        }

        // Add visual sign mark the unit that can't do anything more.
        public virtual void MarkAsFinished()
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.MarkAsFinishedFn?.ForEach(
                    o => o.Apply(this, null)
                );
        }

        // Recover the unit's default visual.
        public virtual void UnMark()
        {
            if (UnitHighLighterAggregator != null)
                UnitHighLighterAggregator.UnMarkFn?.ForEach(
                    o => o.Apply(this, null)
                );
        }
        #endregion
    }
}