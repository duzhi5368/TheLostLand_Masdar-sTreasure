using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
//============================================================
namespace FKLib
{
    public static class WindowsRuntimeExtension
    {
        public static Type GetBaseType(this Type type)
        {
            return type.BaseType;
        }
    }

    public static class Utility
    {
        private static Assembly[]                                   _sAssembliesLookup;
        private static Dictionary<string, Type>                     _sTypeLookup;
        private static Dictionary<Type, FieldInfo[]>                _sSerializedFieldInfoLookup;
        private readonly static Dictionary<Type, MethodInfo[]>      _sMethodInfoLookup;
        private readonly static Dictionary<MemberInfo, object[]>    _sMemberAttributeLookup;

        static Utility()
        {
            _sAssembliesLookup = GetLoadedAssemblies();
            _sTypeLookup = new Dictionary<string, Type>();
            _sSerializedFieldInfoLookup = new Dictionary<Type, FieldInfo[]>();
            _sMethodInfoLookup = new Dictionary<Type, MethodInfo[]>();
            _sMemberAttributeLookup = new Dictionary<MemberInfo, object[]>();
        }

        // 获取指定名称的Type，如无，则返回null
        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogWarning("Type name should not be null or empty!");
                return null;
            }
            Type type;
            if (_sTypeLookup.TryGetValue(typeName, out type))
            {
                return type;
            }
            type = Type.GetType(typeName);
            if (type == null)
            {
                int num = 0;
                while (num < _sAssembliesLookup.Length)
                {
                    type = Type.GetType(string.Concat(typeName, ",", _sAssembliesLookup[num].FullName));
                    if (type == null)
                    {
                        num++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (type == null)
            {
                foreach (Assembly a in _sAssembliesLookup)
                {
                    Type[] assemblyTypes = a.GetTypes();
                    for (int j = 0; j < assemblyTypes.Length; j++)
                    {
                        if (assemblyTypes[j].Name == typeName)
                        {
                            type = assemblyTypes[j];
                            break;
                        }
                    }
                }
            }

            if (type != null)
            {
                _sTypeLookup.Add(typeName, type);
            }
            return type;
        }

        public static Type GetElementType(Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            return (from i in interfaces
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select i.GetGenericArguments()[0]).FirstOrDefault();
        }

        public static MethodInfo[] GetAllMethods(this Type type)
        {
            MethodInfo[] methods = new MethodInfo[0];
            if (type != null && !Utility._sMethodInfoLookup.TryGetValue(type, out methods))
            {
                methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Concat(GetAllMethods(type.GetBaseType())).ToArray();
                Utility._sMethodInfoLookup.Add(type, methods);
            }
            return methods;
        }

        public static FieldInfo GetSerializedField(this Type type, string name)
        {
            return type.GetAllSerializedFields().Where(x => x.Name == name).FirstOrDefault();
        }

        public static FieldInfo[] GetAllSerializedFields(this Type type)
        {
            if (type == null)
            {
                return new FieldInfo[0];
            }
            FieldInfo[] fields = GetSerializedFields(type).Concat(GetAllSerializedFields(type.BaseType)).ToArray();
            fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
            return fields;
        }

        public static FieldInfo[] GetSerializedFields(this Type type)
        {
            FieldInfo[] fields;
            if (!Utility._sSerializedFieldInfoLookup.TryGetValue(type, out fields))
            {
                fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.IsPublic && !x.HasAttribute(typeof(NonSerializedAttribute)) || x.HasAttribute(typeof(SerializeField)) || x.HasAttribute(typeof(SerializeReference))).ToArray();
                fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
                Utility._sSerializedFieldInfoLookup.Add(type, fields);
            }
            return fields;
        }

        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        public static IEnumerable<Type> BaseTypes(this Type type)
        {
            while (type != null)
            {
                type = type.BaseType;
                yield return type;
            }
        }

        public static object[] GetCustomAttributes(MemberInfo memberInfo, bool inherit)
        {
            object[] customAttributes;
            if (!Utility._sMemberAttributeLookup.TryGetValue(memberInfo, out customAttributes))
            {
                customAttributes = memberInfo.GetCustomAttributes(inherit);
                Utility._sMemberAttributeLookup.Add(memberInfo, customAttributes);
            }
            return customAttributes;
        }

        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo)
        {
            object[] objArray = Utility.GetCustomAttributes(memberInfo, true);
            List<T> list = new List<T>();
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(T) || objArray[i].GetType().IsSubclassOf(typeof(T)))
                {
                    list.Add((T)objArray[i]);
                }
            }
            return list.ToArray();
        }

        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
        {
            object[] objArray = Utility.GetCustomAttributes(memberInfo, true);
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(T) || objArray[i].GetType().IsSubclassOf(typeof(T)))
                {
                    return (T)objArray[i];
                }
            }
            return default(T);
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo)
        {
            return memberInfo.HasAttribute(typeof(T));
        }

        public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            object[] objArray = Utility.GetCustomAttributes(memberInfo, true);
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                if (objArray[i].GetType() == attributeType || objArray[i].GetType().IsSubclassOf(attributeType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        private static Assembly[] GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
