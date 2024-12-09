using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//============================================================
namespace FKLib
{
    public abstract class ICallbackHandler : MonoBehaviour
    {
        [HideInInspector]
        public List<CallbackHandlerEntry> Delegates;
        public abstract string[] Callbacks {  get; }

        protected void Execute(string eventID, CallbackEventData eventData)
        {
            if (Delegates != null)
            {
                int num = 0;
                int count = Delegates.Count;
                while (num < count)
                {
                    CallbackHandlerEntry entry = Delegates[num];
                    if(entry.EventID == eventID && entry.CallbackEvent != null)
                    {
                        entry.CallbackEvent.Invoke(eventData);
                    }
                    num++;
                }
            }
        }

        public void RegisterListener(string eventID, UnityAction<CallbackEventData> call)
        {
            if (Delegates == null)
            {
                Delegates = new List<CallbackHandlerEntry>();
            }
            CallbackHandlerEntry entry = null;
            for (int i = 0; i < Delegates.Count; i++)
            {
                CallbackHandlerEntry tempEntry = Delegates[i];
                if (tempEntry.EventID == eventID)
                {
                    entry = tempEntry;
                    break;
                }
            }
            if (entry == null)
            {
                entry = new CallbackHandlerEntry();
                entry.EventID = eventID;
                entry.CallbackEvent = new CallbackEvent();
                Delegates.Add(entry);
            }
            entry.CallbackEvent.AddListener(call);
        }

        public void RemoveListener(string eventID, UnityAction<CallbackEventData> call)
        {
            if (Delegates == null)
            {
                return;
            }
            for (int i = 0; i < Delegates.Count; i++)
            {
                CallbackHandlerEntry entry = Delegates[i];
                if (entry.EventID == eventID)
                {
                    entry.CallbackEvent.RemoveListener(call);
                }
            }
        }
    }
}
