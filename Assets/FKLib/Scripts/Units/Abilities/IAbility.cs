using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IAbility : MonoBehaviour
    {
        public int AbilityID { get; set; }
        public IUnit UnitReference { get; internal set; }

        public event EventHandler<(bool isNetworkInvoked, IDictionary<string, string> actionParams)> AbilityUsed;

        public IEnumerator Execute(ICellGrid cellGrid, Action<ICellGrid> preAction, Action<ICellGrid> postAction, bool isNetworkInvoked = false)
        {
            if (AbilityUsed != null)
            {
                var actionParams = Encapsulate();
                actionParams.Add("unit_id", UnitReference.UnitID.ToString());
                actionParams.Add("ability_id", AbilityID.ToString());
                AbilityUsed.Invoke(this, (isNetworkInvoked, actionParams));
            }
            yield return StartCoroutine(Act(cellGrid, preAction, postAction));
        }

        public IEnumerator HumanExecute(ICellGrid cellGrid)
        {
            yield return Execute(cellGrid,
                _ => cellGrid.cellGridState = new CellGridStateBlockInput(cellGrid),
                _ => cellGrid.cellGridState = new CellGridStateAbilitySelected(cellGrid, UnitReference, UnitReference.GetComponents<IAbility>().ToList()));
        }

        public IEnumerator AIExecute(ICellGrid cellGrid)
        {
            yield return Execute(cellGrid,
                _ => { },
                _ => OnAbilityUnSelected(cellGrid));
        }

        public virtual IEnumerator Act(ICellGrid cellGrid)
        {
            yield return null;
        }

        private IEnumerator Act(ICellGrid cellGrid, Action<ICellGrid> preAction, Action<ICellGrid> postAction)
        {
            preAction(cellGrid);
            yield return StartCoroutine(Act(cellGrid));
            postAction(cellGrid);

            yield return null;
        }

        public virtual void Initialize() { }

        public virtual void OnUnitClicked(IUnit unit, ICellGrid cellGrid) { }
        public virtual void OnUnitHighLighted(IUnit unit, ICellGrid cellGrid) { }
        public virtual void OnUnitUnHighLighted(IUnit unit, ICellGrid cellGrid) { }
        public virtual void OnUnitDestroyed(ICellGrid cellGrid) { }
        public virtual void OnCellClicked(ICell cell, ICellGrid cellGrid) { }
        public virtual void OnCellSelected(ICell cell, ICellGrid cellGrid) { }
        public virtual void OnCellUnSelected(ICell cell, ICellGrid cellGrid) { }
        public virtual void Display(ICellGrid cellGrid) { }
        public virtual void CleanUp(ICellGrid cellGrid) { }

        public virtual void OnAbilitySelected(ICellGrid cellGrid) { }
        public virtual void OnAbilityUnSelected(ICellGrid cellGrid) { }
        public virtual void OnTurnStart(ICellGrid cellGrid) { }
        public virtual void OnTurnEnd(ICellGrid cellGrid) { }

        public virtual bool CanPerform(ICellGrid cellGrid) { return false; }

        // �����ܲ�����װ��Dictionary�У��Ա����л�
        public virtual IDictionary<string, string> Encapsulate() { throw new NotImplementedException(); }
        // �����ܵ��ļ��ܲ���Dictionary��Ӧ�ü���Ч������д�÷���ʱӦ�������л�Dictionary��
        // Params: cellGrid - Ӧ�ü��ܵĵ�Ԫ��
        // Params: actionParams - ���ܵĸ��������װ������
        public virtual IEnumerator Apply(ICellGrid cellGrid, IDictionary<string, string> actionParams) { throw new NotImplementedException(); }
    }
}
