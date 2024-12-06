using UnityEngine;
//============================================================
namespace FKLib
{
    public enum ENUM_ActionStatus
    {
        eAS_Inactive,
        eAS_Failure,
        eAS_Success,
        eAS_Running
    }

    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    public interface IAction
    {
        void Initialize(GameObject gameObject, MainPlayerInfo mainPlayerInfo, Blackboard blackboard);

        bool IsActiveAndEnabled { get; }

        ENUM_ActionStatus OnUpdate();
        void Update();
        void OnStart();
        void OnEnd();
        void OnSequenceStart();
        void OnSequenceEnd();
        void OnInterrupt();
    }
}
