using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
//============================================================
namespace FKLib
{
    public abstract class ICellGridGenerator
    {
        [HideInInspector]
        public Transform CellsParent;

        private Dictionary<string, object> generatorParameterValues = new Dictionary<string, object>();

        // core function should be inherited
        public abstract IGridInfo GenerateGrid();
        // need to be override
        virtual public IGridInfo ShuffleGridInfo(List<ICell> cells) { return GetGridInfo(cells); }
        virtual public void SaveData(string configPath) { }
        virtual public void LoadData(string configPath) { }

        protected IGridInfo GetGridInfo(List<ICell> cells)
        {
            var minX = cells.Find(c => c.transform.position.x.Equals(cells.Min(c2 => c2.transform.position.x))).transform.position.x;
            var maxX = cells.Find(c => c.transform.position.x.Equals(cells.Max(c2 => c2.transform.position.x))).transform.position.x;

            var minY = float.MinValue;
            var maxY = float.MaxValue;

            minY = cells.Find(c => c.transform.position.y.Equals(cells.Min(c2 => c2.transform.position.y))).transform.position.y;
            maxY = cells.Find(c => c.transform.position.y.Equals(cells.Max(c2 => c2.transform.position.y))).transform.position.y;

            IGridInfo gridInfo = new IGridInfo();
            gridInfo.Cells = cells;
            gridInfo.Dimensions = new Vector3(maxX - minX, maxY - minY, 0);
            gridInfo.Center = gridInfo.Dimensions / 2 + new Vector3(minX, minY, 0);
            return gridInfo;
        }

        public virtual Dictionary<string, object> ReadGeneratorParams()
        {
            Dictionary<Type, Func<string, object, object>> parameterHandlers = new Dictionary<Type, Func<string, object, object>>();
            parameterHandlers.Add(typeof(int), (string x, object y) => EditorGUILayout.IntField(new GUIContent(x), (int)y));
            parameterHandlers.Add(typeof(double), (string x, object y) => EditorGUILayout.DoubleField(new GUIContent(x), (double)y));
            parameterHandlers.Add(typeof(float), (string x, object y) => EditorGUILayout.FloatField(new GUIContent(x), (float)y));
            parameterHandlers.Add(typeof(string), (string x, object y) => EditorGUILayout.TextField(new GUIContent(x), (string)y));
            parameterHandlers.Add(typeof(bool), (string x, object y) => EditorGUILayout.Toggle(new GUIContent(x), (bool)y));
            parameterHandlers.Add(typeof(GameObject), (string x, object y) => EditorGUILayout.ObjectField(x, (GameObject)y, typeof(GameObject), false, new GUILayoutOption[0]));
            
            parameterHandlers.Add(typeof(List<GameObject>), (string x, object y) =>
            {
                List<GameObject> gameObjects = y as List<GameObject>;
                if (gameObjects == null)
                {
                    gameObjects = new List<GameObject>();
                }

                EditorGUILayout.BeginHorizontal();              // 开始水平布局
                EditorGUILayout.LabelField(new GUIContent(x));  // 显示列表名称
                GUILayout.FlexibleSpace();                      // 左侧添加空白空间
                if (GUILayout.Button("添加新噪音地图块对象"))
                {
                    gameObjects.Add(null);                      // 添加一个空元素
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;                        // 缩进以表示层级结构
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    // 绘制每个 GameObject 的 ObjectField
                    gameObjects[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", gameObjects[i], typeof(GameObject), false);
                    // 删除按钮
                    if (GUILayout.Button("移除", GUILayout.Width(60)))
                    {
                        gameObjects.RemoveAt(i);
                        i--; // 修正索引
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--; // 恢复缩进
                return gameObjects; // 返回更新后的列表
            });

            foreach (var field in GetType().GetFields().Where(f => f.IsPublic))
            {
                if (!parameterHandlers.ContainsKey(field.FieldType) || Attribute.GetCustomAttribute(field, typeof(HideInInspector)) != null)
                {
                    continue;
                }

                var parameterHandler = parameterHandlers[field.FieldType];
                object value = field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null;

                if (generatorParameterValues.ContainsKey(field.Name))
                {
                    value = generatorParameterValues[field.Name];
                }
                value = parameterHandler(field.Name, value);
                generatorParameterValues[field.Name] = value;
            }

            return generatorParameterValues;
        }
    }
}
