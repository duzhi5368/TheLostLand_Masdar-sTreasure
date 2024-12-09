using System;
//============================================================
namespace FKLib
{
    [Serializable]
    public class CallbackHandlerEntry
    {
        public string EventID;
        public CallbackEvent CallbackEvent;
        public CallbackHandlerEntry() { }
    }
}
