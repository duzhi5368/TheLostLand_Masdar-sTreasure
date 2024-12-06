using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class IEventHandler : MonoBehaviour
    {
        private static Dictionary<string, Delegate> _sGlobalEvents;
        private static Dictionary<object, Dictionary<string, Delegate>> _sEvents;

        static IEventHandler()
        {
            _sGlobalEvents = new Dictionary<string, Delegate>();
            _sEvents = new Dictionary<object, Dictionary<string, Delegate>>();
        }

        public static void Execute(string eventName)
        {
            System.Action mDelegate = IEventHandler.GetDelegate(eventName) as System.Action;
            if (mDelegate != null)
            {
                mDelegate();
            }
        }

        public static void Execute(object obj, string eventName)
        {
            System.Action mDelegate = IEventHandler.GetDelegate(obj, eventName) as System.Action;
            if (mDelegate != null)
            {
                mDelegate();
            }
        }

        public static void Execute<T1>(string eventName, T1 arg1)
        {
            Action<T1> mDelegate = IEventHandler.GetDelegate(eventName) as Action<T1>;
            if (mDelegate != null)
            {
                mDelegate(arg1);
            }
        }

        public static void Execute<T1>(object obj, string eventName, T1 arg1)
        {
            Action<T1> mDelegate = IEventHandler.GetDelegate(obj, eventName) as Action<T1>;
            if (mDelegate != null)
            {
                mDelegate(arg1);
            }
        }

        public static void Execute<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            Action<T1, T2> mDelegate = IEventHandler.GetDelegate(eventName) as Action<T1, T2>;
            if (mDelegate != null)
            {
                mDelegate(arg1, arg2);
            }
        }

        public static void Execute<T1, T2>(object obj, string eventName, T1 arg1, T2 arg2)
        {
            Action<T1, T2> mDelegate = IEventHandler.GetDelegate(obj, eventName) as Action<T1, T2>;
            if (mDelegate != null)
            {
                mDelegate(arg1, arg2);
            }
        }

        public static void Execute<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            Action<T1, T2, T3> mDelegate = IEventHandler.GetDelegate(eventName) as Action<T1, T2, T3>;
            if (mDelegate != null)
            {
                mDelegate(arg1, arg2, arg3);
            }
        }

        public static void Execute<T1, T2, T3>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            Action<T1, T2, T3> mDelegate = IEventHandler.GetDelegate(obj, eventName) as Action<T1, T2, T3>;
            if (mDelegate != null)
            {
                mDelegate(arg1, arg2, arg3);
            }
        }

        public static void Register(string eventName, System.Action handler)
        {
            IEventHandler.Register(eventName, (Delegate)handler);
        }

        public static void Register(object obj, string eventName, System.Action handler)
        {
            IEventHandler.Register(obj, eventName, (Delegate)handler);
        }

        public static void Register<T1>(string eventName, Action<T1> handler)
        {
            IEventHandler.Register(eventName, (Delegate)handler);
        }

        public static void Register<T1>(object obj, string eventName, Action<T1> handler)
        {
            IEventHandler.Register(obj, eventName, (Delegate)handler);
        }

        public static void Register<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            IEventHandler.Register(eventName, (Delegate)handler);
        }

        public static void Register<T1, T2>(object obj, string eventName, Action<T1, T2> handler)
        {
            IEventHandler.Register(obj, eventName, (Delegate)handler);
        }

        public static void Register<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            IEventHandler.Register(eventName, (Delegate)handler);
        }

        public static void Register<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> handler)
        {
            IEventHandler.Register(obj, eventName, (Delegate)handler);
        }

        public static void Unregister(string eventName, System.Action handler)
        {
            IEventHandler.Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister(object obj, string eventName, System.Action handler)
        {
            IEventHandler.Unregister(obj, eventName, (Delegate)handler);
        }

        public static void Unregister<T1>(string eventName, Action<T1> handler)
        {
            IEventHandler.Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister<T1>(object obj, string eventName, Action<T1> handler)
        {
            IEventHandler.Unregister(obj, eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            IEventHandler.Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2>(object obj, string eventName, Action<T1, T2> handler)
        {
            IEventHandler.Unregister(obj, eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            IEventHandler.Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> handler)
        {
            IEventHandler.Unregister(obj, eventName, (Delegate)handler);
        }

        private static void Register(string eventName, Delegate handler)
        {
            Delegate mDelegate;
            if (!IEventHandler._sGlobalEvents.TryGetValue(eventName, out mDelegate))
            {
                IEventHandler._sGlobalEvents.Add(eventName, handler);
            }
            else
            {
                IEventHandler._sGlobalEvents[eventName] = Delegate.Combine(mDelegate, handler);
            }
        }

        private static void Register(object obj, string eventName, Delegate handler)
        {
            if (obj == null) return;
            Dictionary<string, Delegate> mEvents;
            Delegate mDelegate;
            if (!IEventHandler._sEvents.TryGetValue(obj, out mEvents))
            {
                mEvents = new Dictionary<string, Delegate>();
                IEventHandler._sEvents.Add(obj, mEvents);
            }
            if (!mEvents.TryGetValue(eventName, out mDelegate))
            {
                mEvents.Add(eventName, handler);
            }
            else
            {
                mEvents[eventName] = Delegate.Combine(mDelegate, handler);
            }
        }


        private static void Unregister(string eventName, Delegate handler)
        {
            Delegate mDelegate;
            if (IEventHandler._sGlobalEvents.TryGetValue(eventName, out mDelegate))
            {
                IEventHandler._sGlobalEvents[eventName] = Delegate.Remove(mDelegate, handler);
            }
        }

        private static void Unregister(object obj, string eventName, Delegate handler)
        {
            if (obj == null) return;
            Dictionary<string, Delegate> mEvents;
            Delegate mDelegate;
            if (IEventHandler._sEvents.TryGetValue(obj, out mEvents) && mEvents.TryGetValue(eventName, out mDelegate))
            {
                mEvents[eventName] = Delegate.Remove(mDelegate, handler);
            }
        }

        private static Delegate GetDelegate(string eventName)
        {
            Delegate mDelegate;
            if (IEventHandler._sGlobalEvents.TryGetValue(eventName, out mDelegate))
            {
                return mDelegate;
            }
            return null;
        }

        private static Delegate GetDelegate(object obj, string eventName)
        {
            Dictionary<string, Delegate> mEvents;
            Delegate mDelegate;
            if (IEventHandler._sEvents.TryGetValue(obj, out mEvents) && mEvents.TryGetValue(eventName, out mDelegate))
            {
                return mDelegate;
            }
            return null;
        }
    }
}
