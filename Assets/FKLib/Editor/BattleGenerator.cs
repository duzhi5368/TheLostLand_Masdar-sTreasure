using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
//============================================================
namespace FKLib
{
    public class BattleGenerator : EditorWindow
    {
        public static List<Type> sGenerators;                // ��ǰ֧�ֵĵ�ͼ�������б�
        public static string[] sGeneratorName;             // ��ǰ֧�ֵĵ�ͼ�����������б�

        public bool IsKeepMainCamera = false;
        public bool IsUseMovableCamera = false;
        public float CameraScrollSpeed = 15.0f;
        public float CameraScrollEdge = 0.01f;

        public int PlayerNumbers = 1;
        public int PlayerFriendlyNumbers = 0;
        public int AIFriendlyNumbers = 0;
        public int AINeutrallyNumbers = 1;
        public int PlayerEnemyNumbers = 0;
        public int AIEnemyNumbers = 1;

        private GameObject _cellGrid;
        private GameObject _units;
        private GameObject _players;
        private GameObject _guiController;
        private GameObject _directionalLight;
        private ICellGridGenerator _cellGenerator;
        private Dictionary<string, object> _cellParameterValues;
        private StaticUnitGeneratorData _staticUnitGeneratorData;
        private List<GameObject> _randomNeutrallyUnitPrefabs;
        private int _aiNeutrallyID = -1;

        private BoolWrapper _isTileEditModeOn = new BoolWrapper(false);
        [SerializeField]
        private ICell _tilePrefab;
        private GameObject _tilePrefabObject;
        private int _tilePaintingRadius = 1;
        private int _lastPaintedHash = -1;

        private BoolWrapper _isUnitEditModeOn = new BoolWrapper(false);
        [SerializeField]
        private IUnit _unitPrefab;
        private GameObject _unitPrefabObject;
        private int _playerID;

        private bool _isGridGameObjectPresent;
        private bool _isUnitsGameObjectPresent;
        private GameObject _gridGameObject;
        private GameObject _unitsGameObject;

        private BoolWrapper _isToToggle = null;
        private bool _isShouldDisplayCollider2DWarning;
        private bool _isShouldDisplayNoColliderOnCellWarning;
        private Vector2 _scrollPosition = Vector2.zero;

        #region unity editor functions
        [MenuItem("Windows/FKս��������")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(BattleGenerator));
            window.titleContent.text = "FKս��������";
        }

        public void OnEnable()
        {
            Initialize();

            var gridGameObject = GameObject.Find("CellGrid");
            var unitsGameObject = GameObject.Find("Units");
            _isGridGameObjectPresent = gridGameObject != null;
            _isUnitsGameObjectPresent = unitsGameObject != null;

            _isTileEditModeOn = new BoolWrapper(false);
            _isUnitEditModeOn = new BoolWrapper(false);
            EnableSceneViewInteraction();
            Selection.selectionChanged += OnSelectionChanged;
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        public void OnDestroy()
        {
            _isTileEditModeOn = new BoolWrapper(false);
            _isUnitEditModeOn = new BoolWrapper(false);
            DisableSceneViewInteraction();
            Selection.selectionChanged -= OnSelectionChanged;
            Undo.undoRedoPerformed -= OnUndoPerformed;
        }

        public void OnHierarchyChange()
        {
            var gridGameObject = GameObject.Find("CellGrid");
            var unitsGameObject = GameObject.Find("Units");
            _isGridGameObjectPresent = gridGameObject != null;
            _isUnitsGameObjectPresent = unitsGameObject != null;

            if (_unitsGameObject != null)
                this._unitsGameObject = null;
            if (_gridGameObject != null)
                this._gridGameObject = null;

            Repaint();
        }

        public void OnGUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUIStyle.none);

            MapGenerationGUI();
            TilePaintingGUI();
            UnitPaintingGUI();
            PrefabHelperGUI();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.R)
                ToggleEditMode();

            GUILayout.EndScrollView();
        }
        #endregion

        #region utility function
        private void Initialize()
        {
            if (_cellParameterValues == null)
                _cellParameterValues = new Dictionary<string, object>();
            if (_staticUnitGeneratorData == null)
                _staticUnitGeneratorData = new StaticUnitGeneratorData();
            if (_randomNeutrallyUnitPrefabs == null)
                _randomNeutrallyUnitPrefabs = new List<GameObject>();

            if (sGenerators == null)
            {
                Type generatorInterface = typeof(ICellGridGenerator);
                var assembly = generatorInterface.Assembly;

                sGenerators = new List<Type>();
                foreach (var t in assembly.GetTypes())
                {
                    if (generatorInterface.IsAssignableFrom(t) && t != generatorInterface)
                        sGenerators.Add(t);
                }
            }

            if (sGeneratorName == null)
                sGeneratorName = sGenerators.Select(t => t.Name).ToArray();
        }

        private void ToggleEditMode()
        {
            if (_isToToggle == null)
                return;

            _isToToggle.value = !_isToToggle.value;
            if (_isToToggle.value)
                EnableSceneViewInteraction();

            Repaint();
        }

        private ICell GetSelectedCell()
        {
            var raycastHit2D = Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
            if (raycastHit2D.transform != null)
            {
                if (raycastHit2D.transform.GetComponent<ICell>() != null)
                {
                    return raycastHit2D.transform.GetComponent<ICell>();
                }
            }
            return null;
        }
        #endregion

        #region draw GUI
        private void MapGenerationGUI()
        {
            GUILayout.Label("��ս��������Ϣ��", EditorStyles.boldLabel);

            IsUseMovableCamera = EditorGUILayout.Toggle(new GUIContent("ʹ�ÿ��ƶ������", "�Ƿ�Ϊ��������һ�����׿�����"), IsUseMovableCamera, new GUILayoutOption[0]);
            if (IsUseMovableCamera)
            {
                CameraScrollSpeed = EditorGUILayout.FloatField(new GUIContent("�ƶ��ٶ�"), CameraScrollSpeed);
                CameraScrollEdge = EditorGUILayout.Slider("�ƶ��߽�", CameraScrollEdge, 0.05f, 0.25f, new GUILayoutOption[0]);
            }

            IsKeepMainCamera = EditorGUILayout.Toggle(new GUIContent("���浱ǰ���������", "�Ƿ���ʹ�õ�ǰ��������������Զ����´���һ��"), IsKeepMainCamera, new GUILayoutOption[0]);

            PlayerNumbers = EditorGUILayout.IntField(new GUIContent("��� ��Ӫ����"), PlayerNumbers);
            PlayerFriendlyNumbers = EditorGUILayout.IntField(new GUIContent("�ѷ���� ��Ӫ����"), PlayerFriendlyNumbers);
            AIFriendlyNumbers = EditorGUILayout.IntField(new GUIContent("�ѷ�AI ��Ӫ����"), AIFriendlyNumbers);
            AINeutrallyNumbers = EditorGUILayout.IntField(new GUIContent("����AI ��Ӫ����"), AINeutrallyNumbers);
            PlayerEnemyNumbers = EditorGUILayout.IntField(new GUIContent("�з���� ��Ӫ����"), PlayerEnemyNumbers);
            AIEnemyNumbers = EditorGUILayout.IntField(new GUIContent("�з�AI ��Ӫ����"), AIEnemyNumbers);

            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck() || _cellGenerator == null)
            {
                _cellParameterValues = new Dictionary<string, object>();
                _cellGenerator = (ICellGridGenerator)Activator.CreateInstance(sGenerators[0]);  // create default one
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("����ͼ���������ԡ�", EditorStyles.boldLabel);
            if (GUILayout.Button("������������"))
            {
                SaveMapGeneratorConfigData();
            }
            if (GUILayout.Button("������������"))
            {
                LoadMapGeneratorConfigData();
            }
            EditorGUILayout.EndHorizontal();

            _cellParameterValues = _cellGenerator.ReadGeneratorParams();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("��ս���������������ԡ�", EditorStyles.boldLabel);
            if (GUILayout.Button("������������"))
            {
                SaveUnitGeneratorConfigData();
            }
            if (GUILayout.Button("������������"))
            {
                LoadUnitGeneratorConfigData();
            }
            EditorGUILayout.EndHorizontal();
            _staticUnitGeneratorData.NeutrallyUnitNumber = EditorGUILayout.IntField("����������ָ���", _staticUnitGeneratorData.NeutrallyUnitNumber);
            _staticUnitGeneratorData.ProtectedColumns = EditorGUILayout.IntField("ǰ�󱣻�����", _staticUnitGeneratorData.ProtectedColumns);
            {
                EditorGUILayout.BeginHorizontal();              // ��ʼˮƽ����
                EditorGUILayout.LabelField(new GUIContent("RandomNeutrallyUnitPrefab"));  // ��ʾ�б�����
                GUILayout.FlexibleSpace();                      // �����ӿհ׿ռ�
                if (GUILayout.Button("�������������Ԥ��"))
                {
                    _randomNeutrallyUnitPrefabs.Add(null);                      // ���һ����Ԫ��
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;                        // �����Ա�ʾ�㼶�ṹ
                for (int i = 0; i < _randomNeutrallyUnitPrefabs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    // ����ÿ�� GameObject �� ObjectField
                    _randomNeutrallyUnitPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", _randomNeutrallyUnitPrefabs[i], typeof(GameObject), false);
                    // ɾ����ť
                    if (GUILayout.Button("�Ƴ�", GUILayout.Width(60)))
                    {
                        _randomNeutrallyUnitPrefabs.RemoveAt(i);
                        i--; // ��������
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--; // �ָ�����
            }


            if (GUILayout.Button("���ɳ�����ͼ"))
            {
                Undo.ClearAll();
                GenerateBaseStructure();
            }
            if (GUILayout.Button("�����������������ͼ"))
            {
                Undo.ClearAll();
                GenerateBaseStructure();
                ShuffleMapCellGrid();
            }
            if (GUILayout.Button("�����������ս����λ"))
            {
                Undo.ClearAll();
                GenerateBaseStructure();
                ShuffleMapCellGrid();
                GenerateNeutrallyUnits();
            }
            if (GUILayout.Button("���������ͼ"))
            {
                string dialogTitle = "ȷ��ɾ��?";
                string dialogMessage = "����ɾ����ǰ������ȫ��GameObject�����Ƿ�ȷ��?";
                string dialogOK = "Ok";
                string dialogCancel = "Cancel";

                bool shouldDelete = EditorUtility.DisplayDialog(dialogTitle, dialogMessage, dialogOK, dialogCancel);
                if (shouldDelete)
                {
                    Undo.ClearAll();
                    BattleGeneratorUtils.ClearScene(false);
                }
            }
        }

        private void TilePaintingGUI()
        {
            GUILayout.Label("����ͼ��༭��", EditorStyles.boldLabel);
            if (!_isGridGameObjectPresent)
            {
                if (_gridGameObject == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("CellGrid GameObject missing", style);
                }
                _gridGameObject = (GameObject)EditorGUILayout.ObjectField("CellGrid", _gridGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }

            _tilePaintingRadius = EditorGUILayout.IntSlider(new GUIContent("��ˢ��С"), _tilePaintingRadius, 1, 4);
            EditorGUI.BeginChangeCheck();


            _tilePrefabObject = (GameObject)EditorGUILayout.ObjectField("��ͼ�� prefab", _tilePrefabObject, typeof(GameObject), false, new GUILayoutOption[0]);
            if (_tilePrefabObject != null)
            {
                ICell cellComponent = _tilePrefabObject.GetComponent<ICell>();
                if (cellComponent != null)
                    _tilePrefab = cellComponent;
                else
                    _tilePrefab = null;
            }

            //_tilePrefab = (ICell)EditorGUILayout.ObjectField("��ͼ�� prefab", _tilePrefab, typeof(ICell), false, new GUILayoutOption[0]);

            if (_tilePrefab != null
                //&& _tilePrefab.GetComponent<Collider>() == null
                //&& _tilePrefab.GetComponentInChildren<Collider>() == null
                && _tilePrefab.GetComponent<Collider2D>() == null
                && _tilePrefab.GetComponentInChildren<Collider2D>() == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                GUILayout.Label("Please add a collider to your cell prefab. Without the collider the scene will not be playable", style);
            }

            if (EditorGUI.EndChangeCheck())
                _lastPaintedHash = -1;

            GUILayout.Label(string.Format("��ͼ��༭ģʽ��{0}", _isTileEditModeOn.value ? "����" : "�ر�"));

            if (_isToToggle != null && _isToToggle == _isTileEditModeOn)
                GUILayout.Label("���� Ctrl+R ����/�ر� ��ͼ��༭��ģʽ");

            if (_isTileEditModeOn.value)
                GUI.enabled = false;

            if (GUILayout.Button("������ͼ��༭ģʽ"))
            {
                _isTileEditModeOn = new BoolWrapper(true);
                _isUnitEditModeOn = new BoolWrapper(false);
                _isToToggle = _isTileEditModeOn;
                EnableSceneViewInteraction();

                GameObject cellGrid = _isGridGameObjectPresent ? GameObject.Find("CellGrid") : _gridGameObject;
                if (cellGrid == null) {
                    Debug.LogError("CellGrid gameobject is missing, assign it in FK battle generator");
                }
            }

            GUI.enabled = true;
            if (!_isTileEditModeOn.value)
                GUI.enabled = false;

            if (GUILayout.Button("�رյ�ͼ��༭ģʽ"))
            {
                _isTileEditModeOn = new BoolWrapper(false);
                DisableSceneViewInteraction();
            }
            GUI.enabled = true;
        }

        private void UnitPaintingGUI()
        {
            GUILayout.Label("��ս����Ԫ�༭��", EditorStyles.boldLabel);
            if (!_isUnitsGameObjectPresent)
            {
                if (_unitsGameObject == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Unit parent GameObject missing", style);
                }
                _unitsGameObject = (GameObject)EditorGUILayout.ObjectField("Units parent", _unitsGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }

            _unitPrefabObject = (GameObject)EditorGUILayout.ObjectField("ս����Ԫ prefab", _unitPrefabObject, typeof(GameObject), false, new GUILayoutOption[0]);
            if (_unitPrefabObject != null)
            {
                IUnit unitComponent = _unitPrefabObject.GetComponent<IUnit>();
                if (unitComponent != null)
                    _unitPrefab = unitComponent;
                else
                    _unitPrefab = null;
            }
            //_unitPrefab = (IUnit)EditorGUILayout.ObjectField("ս����Ԫ prefab", _unitPrefab, typeof(IUnit), false, new GUILayoutOption[0]);

            if (_unitPrefab != null
                // && _unitPrefab.GetComponent<Collider>() == null
                // && _unitPrefab.GetComponentInChildren<Collider>() == null
                && _unitPrefab.GetComponent<Collider2D>() == null
                && _unitPrefab.GetComponentInChildren<Collider2D>() == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                GUILayout.Label("Please add a collider to your unit prefab. Without the collider the scene will not be playable", style);
            }

            _playerID = EditorGUILayout.IntField(new GUIContent("��������ӪID"), _playerID);
            GUILayout.Label(string.Format("ս����Ԫ�༭ģʽ��{0}", _isUnitEditModeOn.value ? "����" : "�ر�"));

            if (_isToToggle != null && _isToToggle == _isUnitEditModeOn)
                GUILayout.Label("���� Ctrl+R ����/�ر� ս����Ԫ�༭��ģʽ");

            if (_isUnitEditModeOn.value)
                GUI.enabled = false;

            if (GUILayout.Button("����ս����Ԫ�༭ģʽ"))
            {
                _isUnitEditModeOn = new BoolWrapper(true);
                _isTileEditModeOn = new BoolWrapper(false);
                _isToToggle = _isUnitEditModeOn;

                GameObject unitsParent = _isUnitsGameObjectPresent ? GameObject.Find("Units") : _unitsGameObject;
                if (unitsParent == null)
                    Debug.LogError("Units parent game object is missing, assign it in FK battle generator");
            }

            GUI.enabled = true;
            if (!_isUnitEditModeOn.value)
                GUI.enabled = false;

            if (GUILayout.Button("�ر�ս����Ԫ�༭ģʽ"))
            {
                _isUnitEditModeOn = new BoolWrapper(false);
                DisableSceneViewInteraction();
            }
            GUI.enabled = true;
        }

        private void PrefabHelperGUI()
        {
            return;
            /*
            GUILayout.Label("������ prefab �������ߡ�", EditorStyles.boldLabel);
            GUILayout.Label("�ڳ���������ѡȡ����ĵ�Ԫ�����·���ť������������ prefab��ע�⣺���ܻỨ��һЩʱ�䣬�����ĵȴ���", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Selection to prefabs"))
            {
                string path = EditorUtility.SaveFolderPanel("Save prefabs", "", "");
                if (path.Length != 0)
                {
                    path = path.Replace(Application.dataPath, "Assets");

                    GameObject[] objectArray = Selection.gameObjects;
                    for (int i = 0; i < objectArray.Length; i++)
                    {
                        GameObject gameobj = objectArray[i];
                        string localPath = path + "/" + gameobj.name + ".prefab";
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(gameobj, localPath, InteractionMode.UserAction);
                    }
                    Debug.Log(string.Format("{0} prefabs saved to {1}", objectArray.Length, path));
                }
            }

            if (GUILayout.Button("Selection to prefabs(variants)"))
            {
                string path = EditorUtility.SaveFolderPanel("Save prefabs", "", "");
                if (path.Length != 0)
                {
                    path = path.Replace(Application.dataPath, "Assets");

                    Transform[] objectArray = Selection.transforms;
                    GameObject root = objectArray[0].gameObject;

                    string localPath = path + "/" + root.name + ".prefab";
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                    var rootPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(root, localPath, InteractionMode.UserAction);

                    for (int i = 0; i < objectArray.Length; i++)
                    {
                        GameObject gameobj = objectArray[i].gameObject;
                        var rootInstance = PrefabUtility.InstantiatePrefab(rootPrefab) as GameObject;
                        foreach (var component in gameobj.GetComponents<Component>())
                        {
                            var destComponent = rootInstance.GetComponent(component.GetType());
                            if (destComponent)
                                EditorUtility.CopySerialized(component, rootInstance.GetComponent(component.GetType()));
                        }
                        localPath = path + "/" + gameobj.name + ".prefab";
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(rootInstance, localPath, InteractionMode.UserAction);

                        DestroyImmediate(rootInstance);
                    }
                    Debug.Log(string.Format("{0} prefabs saved to {1}", objectArray.Length, path));
                }
            }
            */
        }
        #endregion

        #region draw scene tree
        private void GenerateBaseStructure()
        {
            if (BattleGeneratorUtils.CheckMissingParameters(_cellParameterValues))
                return;

            BattleGeneratorUtils.ClearScene(IsKeepMainCamera);

            _cellGrid = new GameObject("CellGrid");
            _players = new GameObject("Players");
            _units = new GameObject("Units");
            _guiController = new GameObject("GUIController");
            _directionalLight = new GameObject("DirectionalLight");

            var light = _directionalLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.Rotate(45f, 0, 0);

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            for (int i = 0; i < PlayerNumbers; i++)
            {
                var player = new GameObject(string.Format("Player_{0}", _players.transform.childCount));
                player.AddComponent<HumanPlayer>();
                player.GetComponent<IPlayer>().PlayerID = _players.transform.childCount;
                player.transform.parent = _players.transform;
            }
            for (int i = 0; i < PlayerFriendlyNumbers; i++)
            {
                var player = new GameObject(string.Format("RemoteFriendlyPlayer_{0}", _players.transform.childCount));
                player.AddComponent<RemotePlayer>();
                player.GetComponent<IPlayer>().PlayerID = _players.transform.childCount;
                player.transform.parent = _players.transform;
            }
            for (int i = 0; i < AIFriendlyNumbers; i++)
            {
                var aiPlayer = new GameObject(string.Format("AIFriendlyPlayer_{0}", _players.transform.childCount));
                aiPlayer.AddComponent<AIPlayer>();
                aiPlayer.GetComponent<IPlayer>().PlayerID = _players.transform.childCount;
                aiPlayer.transform.parent = _players.transform;
            }
            for (int i = 0; i < AINeutrallyNumbers; i++)
            {
                var aiPlayer = new GameObject(string.Format("AINeutrallyPlayer_{0}", _players.transform.childCount));
                _aiNeutrallyID = _players.transform.childCount;
                aiPlayer.AddComponent<AIPlayer>();
                aiPlayer.GetComponent<IPlayer>().PlayerID = _players.transform.childCount;
                aiPlayer.transform.parent = _players.transform;
            }
            for (int i = 0; i < PlayerEnemyNumbers; i++)
            {
                var player = new GameObject(string.Format("RemoteEnemyPlayer_{0}", _players.transform.childCount));
                player.AddComponent<RemotePlayer>();
                player.GetComponent<IPlayer>().PlayerID = _players.transform.childCount;
                player.transform.parent = _players.transform;
            }
            for (int i = 0; i < AIEnemyNumbers; i++)
            {
                var aiPlayer = new GameObject(string.Format("AIEnemyPlayer_{0}", _players.transform.childCount));
                aiPlayer.AddComponent<AIPlayer>();
                aiPlayer.GetComponent<IPlayer>().PlayerID = _players.transform.childCount;
                aiPlayer.transform.parent = _players.transform;
            }

            var cellGridScript = _cellGrid.AddComponent<ICellGrid>();
            _cellGenerator.CellsParent = _cellGrid.transform;

            _cellGrid.GetComponent<ICellGrid>().PlayersParent = _players.transform;

            var unitGenerator = _cellGrid.AddComponent<CustomUnitGenerator>();
            unitGenerator.UnitsParent = _units.transform;
            unitGenerator.CellsParent = _cellGrid.transform;

            var guiControllerScript = _guiController.AddComponent<SimpleGUIController>();
            guiControllerScript.CellGrid = cellGridScript;

            _cellGrid.AddComponent<SubsequentTurnResolver>();
            _cellGrid.AddComponent<DominationCondition>();

            foreach (var fieldName in _cellParameterValues.Keys)
            {
                FieldInfo prop = _cellGenerator.GetType().GetField(fieldName);
                if (prop != null)
                    prop.SetValue(_cellGenerator, _cellParameterValues[fieldName]);
            }

            IGridInfo gridInfo = _cellGenerator.GenerateGrid();
            var camera = Camera.main;
            if (camera == null || !IsKeepMainCamera)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                cameraObject.AddComponent<Camera>();
                camera = cameraObject.GetComponent<Camera>();

                if (IsUseMovableCamera)
                {
                    camera.gameObject.AddComponent<SimpleCameraController>();
                    camera.gameObject.GetComponent<SimpleCameraController>().ScrollSpeed = CameraScrollSpeed;
                    camera.gameObject.GetComponent<SimpleCameraController>().ScrollEdge = CameraScrollEdge;
                }

                camera.transform.position = gridInfo.Center;
                camera.transform.position += new Vector3(0, 0, -1 * (gridInfo.Dimensions.x > gridInfo.Dimensions.y ? gridInfo.Dimensions.x : gridInfo.Dimensions.y) * (Mathf.Sqrt(3) / 2));
                camera.transform.Rotate(new Vector3(0, 0, 0));
                camera.transform.SetAsFirstSibling();
            }
        }

        // shuffle map cell grid, to make it randomly
        private void ShuffleMapCellGrid()
        {
            foreach (var fieldName in _cellParameterValues.Keys)
            {
                FieldInfo prop = _cellGenerator.GetType().GetField(fieldName);
                if (prop != null)
                    prop.SetValue(_cellGenerator, _cellParameterValues[fieldName]);
            }
            GameObject cellGrid = _isGridGameObjectPresent ? GameObject.Find("CellGrid") : _gridGameObject;
            if (cellGrid == null || _cellGenerator == null)
                return;

            var cells = cellGrid.GetComponentsInChildren<ICell>().ToList();
            _cellGenerator.ShuffleGridInfo(cells);
        }

        // random generate neutrally units
        private void GenerateNeutrallyUnits()
        {
            GameObject unitsParent = _isUnitsGameObjectPresent ? GameObject.Find("Units") : _unitsGameObject;
            if (unitsParent == null)
                return;

            if(_randomNeutrallyUnitPrefabs == null || _randomNeutrallyUnitPrefabs.Count == 0)
            {
                Debug.LogError("Please provide random neutrally unit prefab.");
                return;
            }
            if(_staticUnitGeneratorData.NeutrallyUnitNumber <= 0)
            {
                Debug.LogWarning("Neutrally unit number is 0. Nothing will be generate.");
                return;
            }
            foreach (var cell in _randomNeutrallyUnitPrefabs)
            {
                if (cell.GetComponent<IUnit>() == null)
                {
                    Debug.LogError("Invalid neutrally unit prefab provided");
                    return;
                }
            }
            if(_aiNeutrallyID < 0)
            {
                Debug.LogError("AI neutrally player ID is not exit.");
                return;
            }
            GameObject cellGrid = _isGridGameObjectPresent ? GameObject.Find("CellGrid") : _gridGameObject;
            if (cellGrid == null || _cellGenerator == null)
                return;

            var cells = cellGrid.GetComponentsInChildren<ICell>().ToList();
            // �� cells ���� x, y ��������
            cells = cells.OrderBy(cell => cell.OffsetCoord.x)
                         .ThenBy(cell => cell.OffsetCoord.y)
                         .ToList();
            // ȥ��ǰ�������кͺ󷽱�����
            if (_staticUnitGeneratorData.ProtectedColumns > 0)
            {
                int minX = cells.Min(cell => (int)cell.OffsetCoord.x); // �ҵ���С x
                int maxX = cells.Max(cell => (int)cell.OffsetCoord.x); // �ҵ���� x
                cells = cells.Where(cell => cell.OffsetCoord.x >= minX + _staticUnitGeneratorData.ProtectedColumns 
                                    && cell.OffsetCoord.x <= maxX - _staticUnitGeneratorData.ProtectedColumns).ToList();
            }
            if (cells.Count <= _staticUnitGeneratorData.NeutrallyUnitNumber)
            {
                Debug.LogError("Not enough cell can be fill with units.");
                return;
            }

            var randomCell = new System.Random();
            var selectedCells = cells.OrderBy(_ => randomCell.Next()).Take(_staticUnitGeneratorData.NeutrallyUnitNumber).ToList();

            System.Random randomUnit = new System.Random();
            // ��ѡȡ�� ICell ���в���
            foreach (var cell in selectedCells)
            {
                // ���ѡ��һ���µ�Ԫ
                int randomIndex = randomUnit.Next(_randomNeutrallyUnitPrefabs.Count);
                var newUnit = (PrefabUtility.InstantiatePrefab(_randomNeutrallyUnitPrefabs[randomIndex].gameObject) as GameObject).GetComponent<IUnit>();
                newUnit.OwnerPlayerID = _aiNeutrallyID;
                newUnit.Cell = cell;
                cell.IsTaken = newUnit.IsObstructable;
                newUnit.transform.position = cell.transform.position;
                newUnit.transform.parent = unitsParent.transform;
                newUnit.transform.rotation = cell.transform.rotation;
            }
        }
        #endregion

        #region draw scene
        private void OnSceneGUI(SceneView senceView) 
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.R)
                ToggleEditMode();
            if (_isTileEditModeOn.value || _isUnitEditModeOn.value)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            if (_isTileEditModeOn.value && _tilePrefab != null)
                PaintTiles();
            if (_isUnitEditModeOn.value && _unitPrefab != null)
                PaintUnits();
        }

        private void PaintUnits() 
        {
            GameObject unitsParent = _isUnitsGameObjectPresent ? GameObject.Find("Units") : _unitsGameObject;
            if (unitsParent == null)
                return;
            var selectedCell = GetSelectedCell();
            if (selectedCell == null) 
                return;

            Handles.color = Color.red;
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.up, selectedCell.GetCellDimensions().y / 2);
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.forward, selectedCell.GetCellDimensions().y / 2);
            HandleUtility.Repaint();

            if(Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
            {
                if (_isUnitEditModeOn.value && selectedCell.IsTaken)
                    return;

                Undo.SetCurrentGroupName("Unit painting");
                int group = Undo.GetCurrentGroup();

                Undo.RecordObject(selectedCell, "Unit painting");
                var newUnit = (PrefabUtility.InstantiatePrefab(_unitPrefab.gameObject) as GameObject).GetComponent<IUnit>();
                newUnit.OwnerPlayerID = _playerID;
                newUnit.Cell = selectedCell;

                selectedCell.IsTaken = newUnit.IsObstructable;

                newUnit.transform.position = selectedCell.transform.position;
                newUnit.transform.parent = unitsParent.transform;
                newUnit.transform.rotation = selectedCell.transform.rotation;

                Undo.RegisterCreatedObjectUndo(newUnit.gameObject, "Unit painting");
            }
        }

        private void PaintTiles() 
        {
            GameObject cellGrid = _isGridGameObjectPresent ? GameObject.Find("CellGrid") : _gridGameObject;
            if (cellGrid == null)
                return;
            ICell selectedCell = GetSelectedCell();
            if (selectedCell == null) 
                return;

            Handles.color = Color.red;
            Handles.DrawWireDisc(selectedCell.transform.position + new Vector3(selectedCell.GetCellDimensions().x / 2, - selectedCell.GetCellDimensions().y / 2, 0), Vector3.up, selectedCell.GetCellDimensions().y * (_tilePaintingRadius - 0.5f));
            Handles.DrawWireDisc(selectedCell.transform.position + new Vector3(selectedCell.GetCellDimensions().x / 2, - selectedCell.GetCellDimensions().y / 2, 0), Vector3.forward, selectedCell.GetCellDimensions().y * (_tilePaintingRadius - 0.5f));
            HandleUtility.Repaint();

            int selectedCellHash = selectedCell.GetHashCode();
            if (_lastPaintedHash != selectedCellHash)
            {
                if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
                {
                    _lastPaintedHash = selectedCellHash;
                    Undo.SetCurrentGroupName("Tile painting");
                    int group = Undo.GetCurrentGroup();
                    var cells = cellGrid.GetComponentsInChildren<ICell>();
                    var cellsInRange = cells.Where(c => c.GetDistance(selectedCell) <= _tilePaintingRadius - 1).ToList();
                    foreach (var cell in cellsInRange) 
                    {
                        if (_tilePrefab == PrefabUtility.GetCorrespondingObjectFromSource(cell))
                            continue;
                        var newCell = (PrefabUtility.InstantiatePrefab(_tilePrefab.gameObject, cell.transform.parent) as GameObject).GetComponent<ICell>();
                        newCell.transform.position = cell.transform.position;

                        try
                        {
                            cell.CopyFields(newCell);
                            Undo.RegisterCreatedObjectUndo(newCell.gameObject, "Tile painting");
                            Undo.DestroyObjectImmediate(cell.gameObject);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("{0} - Probably you are using wrong tile prefab", e.Message));
                            DestroyImmediate(newCell.gameObject);
                        }
                    }
                    Undo.CollapseUndoOperations(group);
                    Undo.IncrementCurrentGroup();
                }
            }
        }
        #endregion

        #region gui event
        private void EnableSceneViewInteraction() 
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        private void DisableSceneViewInteraction() 
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeGameObject == null)
                return;

            if (PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) == PrefabAssetType.NotAPrefab)
                return;

            if (_isTileEditModeOn.value || _isToToggle == _isTileEditModeOn)
            {
                if (Selection.activeGameObject.GetComponent<ICell>() != null)
                {
                    _lastPaintedHash = -1;
                    if (PrefabUtility.GetPrefabInstanceStatus(Selection.activeGameObject) == PrefabInstanceStatus.Connected)
                    {
                        _tilePrefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<ICell>();
                    }
                    else
                    {
                        _tilePrefab = Selection.activeGameObject.GetComponent<ICell>();
                    }
                    Repaint();
                }
            }
            else if (_isUnitEditModeOn.value || _isToToggle == _isUnitEditModeOn)
            {
                if (Selection.activeGameObject.GetComponent<IUnit>() != null)
                {
                    if (PrefabUtility.GetPrefabInstanceStatus(Selection.activeGameObject) == PrefabInstanceStatus.Connected)
                    {
                        _unitPrefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<IUnit>();
                    }
                    else
                    {
                        _unitPrefab = Selection.activeGameObject.GetComponent<IUnit>();
                    }
                    Repaint();
                }
            }
        }

        private void OnUndoPerformed() 
        {
            _lastPaintedHash = -1;
        }
        #endregion

        #region
        private void SaveMapGeneratorConfigData()
        {
            string savePath = EditorUtility.SaveFilePanel(
                "�����ͼ���������ļ�",           // ����
                Application.dataPath + Path.MAP_GENERATOR_CONFIG_DIR,           // Ĭ��·��
                "MapGenerator.json",            // Ĭ���ļ���
                "json"                          // �ļ���չ��
            );

            if (!string.IsNullOrEmpty(savePath))
            {
                _cellGenerator.SaveData(savePath);
            }
        }

        private void LoadMapGeneratorConfigData()
        {
            string loadPath = EditorUtility.OpenFilePanel(
                "���ص�ͼ���������ļ�",          // ����
                Application.dataPath + Path.MAP_GENERATOR_CONFIG_DIR,           // Ĭ��·��
                "json"                          // �ļ���չ��
            );

            if (!string.IsNullOrEmpty(loadPath))
            {
                _cellGenerator.LoadData(loadPath);
            }

            // ͬ����Editor
            var keys = new List<string>(_cellParameterValues.Keys); // �����޸�ʱ��������
            foreach (var fieldName in keys)
            {
                FieldInfo field = _cellGenerator.GetType().GetField(fieldName);
                if (field != null)
                {
                    _cellParameterValues[fieldName] = field.GetValue(_cellGenerator);
                }
            }
        }

        private void SaveUnitGeneratorConfigData()
        {
            string savePath = EditorUtility.SaveFilePanel(
                "����ս����Ԫ���������ļ�",     // ����
                Application.dataPath + Path.MAP_GENERATOR_CONFIG_DIR,           // Ĭ��·��
                "UnitGenerator.json",           // Ĭ���ļ���
                "json"                          // �ļ���չ��
            );

            if (!string.IsNullOrEmpty(savePath))
            {
                _staticUnitGeneratorData.RandomNeutrallyUnitPrefabPaths.Clear();
                foreach(var prefab in _randomNeutrallyUnitPrefabs)
                {
                    if (prefab != null) { 
                        string assetPath = AssetDatabase.GetAssetPath(prefab);
                        _staticUnitGeneratorData.RandomNeutrallyUnitPrefabPaths.Add(assetPath);
                    }
                }

                string json = JsonUtility.ToJson(_staticUnitGeneratorData, true);
                File.WriteAllText(savePath, json);
                Debug.Log($"Rectangular Square Grid Generator's data saved to {savePath}");
            }
        }

        private void LoadUnitGeneratorConfigData()
        {
            string loadPath = EditorUtility.OpenFilePanel(
                "����ս����Ԫ���������ļ�",     // ����
                Application.dataPath + Path.MAP_GENERATOR_CONFIG_DIR,           // Ĭ��·��
                "json"                          // �ļ���չ��
            );

            if (!string.IsNullOrEmpty(loadPath))
            {
                string json = File.ReadAllText(loadPath);
                _staticUnitGeneratorData = JsonUtility.FromJson<StaticUnitGeneratorData>(json);

                _randomNeutrallyUnitPrefabs.Clear();
                foreach (var path in _staticUnitGeneratorData.RandomNeutrallyUnitPrefabPaths)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    _randomNeutrallyUnitPrefabs.Add(prefab);
                }
            }
        }
        #endregion
    }
}
