using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class Blackboard : MonoBehaviour
    {
        [SerializeReference]
        protected List<IVariable> _variables = new List<IVariable>();

        public T GetValue<T>(string name)
        {
            return GetValue<T>(GetVariable(name));
        }

        public T GetValue<T>(IVariable variable)
        {
            if(variable == null)
                return default(T);

            if (!variable.IsShared)
                return (T)variable.RawValue;

            IVariable p = GetVariable(variable.Name);
            if(p != null)
                return (T)p.RawValue;

            return default(T);
        }

        public void SetValue<T>(string name, object value)
        {
            IVariable v = GetVariable(name);
            if (v != null) 
            {
                v.RawValue = value;
            }
            else
            {
                AddVariable<T>(name, value);
            }
        }

        public void SetValue(string name, object value, Type type)
        {
            IVariable v = GetVariable(name);
            if (v != null)
            {
                v.RawValue = value;
            }
            else
            {
                AddVariable(name, value, type);
            }
        }

        public bool DeleteVariable(string name) 
        {
            return this._variables.RemoveAll(x => x.Name == name) > 0;
        }

        public IVariable GetVariable(string name)
        {
            return this._variables.FirstOrDefault(x => x.Name == name);
        }

        public void AddVariable(IVariable variable)
        {
            if(GetVariable(variable.Name) != null)
            {
                Debug.LogWarning("Variable with the same name (" + name + ") already exists!");
                return;
            }
            _variables.Add(variable);
        }

        public IVariable AddVariable<T>(string name, object value)
        {
            return AddVariable(name, value, typeof(T));
        }

        public IVariable AddVariable(string name, object value, Type type)
        {
            if (GetVariable(name) != null)
            {
                Debug.LogWarning("Variable with the same name (" + name + ") already exists!");
                return null;
            }
            IVariable variable = null;
            if (typeof(bool).IsAssignableFrom(type)) 
            {
                variable = new BoolVariable(name);
            }
            else if (typeof(float).IsAssignableFrom(type))
            {
                variable = new FloatVariable(name);
            }
            else if (typeof(Color).IsAssignableFrom(type))
            {
                variable = new ColorVariable(name);
            }
            else if (typeof(GameObject).IsAssignableFrom(type))
            {
                variable = new GameObjectVariable(name);
            }
            else if (typeof(int).IsAssignableFrom(type))
            {
                variable = new IntVariable(name);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                variable = new ObjectVariable(name);
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                variable = new StringVariable(name);
            }
            else if (typeof(Vector2).IsAssignableFrom(type))
            {
                variable = new Vector2Variable(name);
            }
            else if (typeof(Vector3).IsAssignableFrom(type))
            {
                variable = new Vector3Variable(name);
            }
            else if (typeof(ArrayList).IsAssignableFrom(type))
            {
                variable = new ArrayListVariable(name);
            }

            if (variable != null) 
            {
                variable.RawValue = value;
                _variables.Add(variable);
            }
            else
            {
                Debug.LogWarning("Variable type (" + type + ") is not supported.");
            }
            return variable;
        }
    }
}
