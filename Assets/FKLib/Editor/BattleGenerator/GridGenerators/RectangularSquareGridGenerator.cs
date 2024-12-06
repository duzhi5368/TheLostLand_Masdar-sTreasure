using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    [ExecuteInEditMode()]
    public class RectangularSquareGridGenerator : ICellGridGenerator
    {
        #region params will be shown in editor
        public GameObject BaseSquarePrefab;
        public int Width = 16;
        public int Height = 9;
        public int ReplaceRate = 60;
        public List<GameObject> RandomSquarePrefab;
        #endregion

        public override IGridInfo GenerateGrid()
        {
            var cells = new List<ICell>();
            if(BaseSquarePrefab == null || BaseSquarePrefab.GetComponent<ISquareCell>() == null)
            {
                Debug.LogError("Invalid square cell prefab provided");
                return null;
            }

            Vector3 squareSize = BaseSquarePrefab.GetComponent<ICell>().GetCellDimensions();
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++)
                {
                    var squareObject = PrefabUtility.InstantiatePrefab(BaseSquarePrefab) as GameObject;
                    var position = new Vector3(i * squareSize.x, j * squareSize.y, 0);
                    squareObject.transform.position = position;
                    squareObject.GetComponent<ICell>().OffsetCoord = new Vector2(i, j);
                    squareObject.GetComponent<ICell>().MovementCost.Add(ENUM_MoveType.eMT_Normal, 1);
                    cells.Add(squareObject.GetComponent<ICell>());
                    squareObject.transform.parent = CellsParent;
                }
            }
            return GetGridInfo(cells);
        }

        public override IGridInfo ShuffleGridInfo(List<ICell> cells)
        {
            if(RandomSquarePrefab == null || RandomSquarePrefab.Count == 0)
            {
                Debug.LogError("Please provide random noise square cell prefab.");
                return null;
            }
            if(ReplaceRate <= 0)
            {
                Debug.LogWarning("Replace rate is 0. Should not need to noise replace.");
                return null;
            }
            foreach (var cell in RandomSquarePrefab) { 
                if(cell.GetComponent<ISquareCell>() == null)
                {
                    Debug.LogError("Invalid noise square cell prefab provided");
                    return null;
                }
            }

            System.Random random = new System.Random();
            System.Random shouldReplaceRandom = new System.Random();
            for (int i = 0; i < cells.Count; i++)
            {
                // 随机是否需要替换
                int randomReplace = shouldReplaceRandom.Next(0, 100);
                if(randomReplace >= ReplaceRate) { continue; }   // 无需替换

                // 随机选择一个新单元
                int randomIndex = random.Next(RandomSquarePrefab.Count);
                ICell replacementCell = null;
                if (!RandomSquarePrefab[randomIndex].TryGetComponent<ICell>(out replacementCell))
                {
                    continue;
                }

                // 获取原单元的相关信息
                Vector3 position = cells[i].gameObject.transform.position;
                Vector2 offsetCoord = cells[i].OffsetCoord;

                // 销毁原单元对象
                GameObject.DestroyImmediate(cells[i].gameObject);

                // 创建新单元对象
                GameObject replacementObject = PrefabUtility.InstantiatePrefab(replacementCell.gameObject) as GameObject;

                // 设置新单元的位置和父级
                replacementObject.transform.position = position;
                replacementObject.transform.parent = CellsParent;

                // 更新新单元的坐标等信息
                var newCellComponent = replacementObject.GetComponent<ICell>();
                newCellComponent.OffsetCoord = offsetCoord;

                // 替换 `cells` 列表中的引用
                cells[i] = newCellComponent;
            }

            return GetGridInfo(cells);
        }

        public override void SaveData(string configPath) {
            var gridData = new RectangularSquareGridGeneratorData
            {
                BaseSquarePrefabPath = AssetDatabase.GetAssetPath(BaseSquarePrefab),
                Width = Width,
                Height = Height,
                ReplaceRate = ReplaceRate,
                RandomSquarePrefabPaths = RandomSquarePrefab.ConvertAll(AssetDatabase.GetAssetPath)
            };

            string json = JsonUtility.ToJson(gridData, true);
            File.WriteAllText(configPath, json);
            Debug.Log($"Rectangular Square Grid Generator's data saved to {configPath}");
        }

        public override void LoadData(string configPath) 
        {
            if (!File.Exists(configPath))
            {
                Debug.LogError("Rectangular Square Grid Generator's data file not found!");
                return;
            }

            string json = File.ReadAllText(configPath);
            var gridData = JsonUtility.FromJson<RectangularSquareGridGeneratorData>(json);

            // 应用加载的数据
            BaseSquarePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(gridData.BaseSquarePrefabPath);
            Width = gridData.Width;
            Height = gridData.Height;
            ReplaceRate = gridData.ReplaceRate;
            RandomSquarePrefab = gridData.RandomSquarePrefabPaths.ConvertAll(AssetDatabase.LoadAssetAtPath<GameObject>);

            Debug.Log("Rectangular Square Grid Generator's data loaded successfully.");
        }
    }
}
