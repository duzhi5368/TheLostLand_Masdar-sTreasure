using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class AddObjectWindow : EditorWindow
    {
        private static AddObjectWindowStyles _sStyles;

        private string  _isSearchString = string.Empty;
        private Vector2 _scrollPosition;
        private Type    _type;
        private Element _rootElement;
        private Element _selectedElement;
        private string  _newScriptName;

        private bool isSearching
        {
            get
            {
                return !string.IsNullOrEmpty(_isSearchString);
            }
        }

        public delegate void AddCallbackDelegate(Type type);
        public AddCallbackDelegate OnAddCallback;
        public delegate void CreateCallbackDelegate(string scriptName);
        public CreateCallbackDelegate OnCreateCallback;

        public static void ShowWindow<T>(Rect buttonRect, AddObjectWindow.AddCallbackDelegate addCallback, AddObjectWindow.CreateCallbackDelegate createCallback)
        {
            ShowWindow(buttonRect, typeof(T), addCallback, createCallback);
        }

        public static void ShowWindow(Rect buttonRect, Type type, AddObjectWindow.AddCallbackDelegate addCallback, AddObjectWindow.CreateCallbackDelegate createCallback)
        {
            AddObjectWindow window = ScriptableObject.CreateInstance<AddObjectWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window._type = type;
            window.OnAddCallback = addCallback;
            window.OnCreateCallback = createCallback;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 280f));
        }

        private void OnEnable()
        {
            this._isSearchString = EditorPrefs.GetString("AddAssetSearch", this._isSearchString);
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (_sStyles == null)
            {
                _sStyles = new AddObjectWindowStyles();
            }
            if (this._rootElement == null)
            {
                this._rootElement = BuildElements();
                this._selectedElement = this._rootElement;
            }
            GUILayout.Space(5f);
            this._isSearchString = SearchField(_isSearchString);
            Header();

            if (isSearching)
            {
                Element[] elements = GetAllElements(this._rootElement);
                DrawElements(elements);
            }
            else
            {
                DrawElements(this._selectedElement.Children.ToArray());
            }
            if (Event.current.type == EventType.Repaint)
            {
                _sStyles.Background.Draw(new Rect(0, 0, position.width, position.height), false, false, false, false);
            }
        }

        private void Header()
        {
            GUIContent content = this._selectedElement.Label;
            Rect headerRect = GUILayoutUtility.GetRect(content, _sStyles.Header);
            if (GUI.Button(headerRect, content, _sStyles.Header))
            {
                if (this._selectedElement.Parent != null && !isSearching)
                {
                    this._selectedElement = this._selectedElement.Parent;
                }
            }
            if (Event.current.type == EventType.Repaint && this._selectedElement.Parent != null)
            {
                _sStyles.LeftArrow.Draw(new Rect(headerRect.x, headerRect.y + 4f, 16f, 16f), false, false, false, false);
            }
        }

        private void DrawElements(Element[] elements)
        {
            this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition);
            foreach (Element element in elements)
            {
                if (element.OnGUI != null && !isSearching)
                {
                    element.OnGUI();
                    continue;
                }
                if (!SearchMatch(element))
                {
                    continue;
                }

                Color backgroundColor = GUI.backgroundColor;
                Color textColor = _sStyles.ElementButton.normal.textColor;
                int padding = _sStyles.ElementButton.padding.left;
                Rect rect = GUILayoutUtility.GetRect(element.Label, _sStyles.ElementButton, GUILayout.Height(20f));
                GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));
                _sStyles.ElementButton.normal.textColor = (rect.Contains(Event.current.mousePosition) ? Color.white : textColor);
                Texture2D icon = null;//(Texture2D)EditorGUIUtility.ObjectContent(null, element.type).image;

                if (element.Type != null)
                {
                    icon = EditorGUIUtility.FindTexture("cs Script Icon");
                    IconAttribute iconAttribute = element.Type.GetCustomAttribute<IconAttribute>();
                    if (iconAttribute != null)
                    {
                        if (iconAttribute.Type != null)
                        {
                            icon = AssetPreview.GetMiniTypeThumbnail(iconAttribute.Type);
                        }
                        else
                        {
                            icon = Resources.Load<Texture2D>(iconAttribute.Path);
                        }
                    }
                }
                _sStyles.ElementButton.padding.left = (icon != null ? 22 : padding);
                if (GUI.Button(rect, element.Label, _sStyles.ElementButton))
                {
                    if (element.Children.Count == 0)
                    {
                        if (OnAddCallback != null)
                        {
                            OnAddCallback(element.Type);
                        }
                        Close();
                    }
                    else
                    {
                        this._selectedElement = element;
                    }
                }
                GUI.backgroundColor = backgroundColor;
                _sStyles.ElementButton.normal.textColor = textColor;
                _sStyles.ElementButton.padding.left = padding;

                if (icon != null)
                {
                    GUI.Label(new Rect(rect.x, rect.y, 20f, 20f), icon);
                }
                if (element.Children.Count > 0)
                {
                    GUI.Label(new Rect(rect.x + rect.width - 16f, rect.y + 2f, 16f, 16f), "", _sStyles.RightArrow);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private bool SearchMatch(Element element)
        {
            if (isSearching && (element.Type == null || element.Type.IsAbstract || !_isSearchString.ToLower().Split(' ').All(element.Type.Name.ToLower().Contains)))
            {
                return false;
            }
            return true;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();
            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) 
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        private Element BuildElements()
        {
            Element root = new Element(ObjectNames.NicifyVariableName(this._type.Name), "");
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => (IsAssignableToGenericType(type, this._type) 
            || this._type.IsAssignableFrom(type)) && !type.IsAbstract && !type.HasAttribute(typeof(ExcludeFromCreationAttribute))).ToArray();
            types = types.OrderBy(x => x.BaseType.Name).ToArray();
            foreach (Type type in types)
            {
                ComponentMenuAttribute attribute = type.GetCustomAttribute<ComponentMenuAttribute>();
                string menu = attribute != null ? attribute.componentMenu : string.Empty;
                if (string.IsNullOrEmpty(menu))
                {
                    Element element = new Element(ObjectNames.NicifyVariableName(type.Name), menu);
                    element.Type = type;
                    element.Parent = root;
                    root.Children.Add(element);
                }
                menu = menu.Replace("/", ".");
                string[] s = menu.Split('.');

                Element prev = null;
                string cur = string.Empty;
                for (int i = 0; i < s.Length - 1; i++)
                {
                    cur += (string.IsNullOrEmpty(cur) ? "" : ".") + s[i];
                    Element parent = root.Find(cur);
                    if (parent == null)
                    {
                        parent = new Element(s[i], cur);
                        if (prev != null)
                        {
                            parent.Parent = prev;
                            prev.Children.Add(parent);
                        }
                        else
                        {
                            parent.Parent = root;
                            root.Children.Add(parent);
                        }
                    }
                    prev = parent;
                }
                if (prev != null)
                {
                    Element element = new Element(ObjectNames.NicifyVariableName(type.Name), menu);
                    element.Type = type;
                    element.Parent = prev;
                    prev.Children.Add(element);
                }
            }
            root.Children = root.Children.OrderByDescending(x => x.Children.Count).ToList();

            Element newScript = new Element("New script", "");
            newScript.Parent = root;
            Element script = new Element(ObjectNames.NicifyVariableName(this._type.Name), "New script." + ObjectNames.NicifyVariableName(this._type.Name));
            script.Parent = newScript;
            script.Type = this._type;
            script.OnGUI = delegate ()
            {
                GUILayout.Label("Name");
                GUI.SetNextControlName("AddAssetNewScript");
                this._newScriptName = GUILayout.TextField(this._newScriptName);
                GUI.FocusControl("AddAssetNewScript");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginDisabledGroup(OnCreateCallback == null || string.IsNullOrEmpty(this._newScriptName));
                if (GUILayout.Button("Create and add") || Event.current.keyCode == KeyCode.Return)
                {
                    OnCreateCallback(this._newScriptName);
                    Close();
                }
                EditorGUI.EndDisabledGroup();
            };
            newScript.Children.Add(script);
            root.Children.Add(newScript);

            return root;
        }

        private Element[] GetAllElements(Element root)
        {
            List<Element> elements = new List<Element>();
            GetElements(root, ref elements);
            return elements.ToArray();
        }

        private void GetElements(Element current, ref List<Element> list)
        {
            list.Add(current);
            for (int i = 0; i < current.Children.Count; i++)
            {
                GetElements(current.Children[i], ref list);
            }
        }

        private string SearchField(string search, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            string before = search;

            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, "ToolbarSeachTextField", options);
            rect.x += 2f;
            rect.width -= 2f;
            Rect buttonRect = rect;
            buttonRect.x = rect.width - 14;
            buttonRect.width = 14;

            if (!String.IsNullOrEmpty(before))
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);

            if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition) || before == "Search..." && GUI.GetNameOfFocusedControl() == "SearchTextFieldFocus")
            {
                before = "";
                GUI.changed = true;
                GUI.FocusControl(null);
            }
            GUI.SetNextControlName("SearchTextFieldFocus");
            GUIStyle style = new GUIStyle("ToolbarSeachTextField");
            if (before == "Search...")
            {
                style.normal.textColor = Color.gray;
                style.hover.textColor = Color.gray;
            }
            string after = EditorGUI.TextField(rect, "", before, style);
            EditorGUI.FocusTextInControl("SearchTextFieldFocus");

            GUI.Button(buttonRect, GUIContent.none, (after != "" && after != "Search...") ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty");
            EditorGUILayout.EndHorizontal();
            return after;
        }

        private static Rect GUIToScreenRect(Rect guiRect)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
            guiRect.x = vector.x;
            guiRect.y = vector.y;
            return guiRect;
        }

        public class Element
        {
            public Type             Type;
            public Element          Parent;
            public System.Action    OnGUI;

            private string          _path;
            private GUIContent      _label;
            private List<Element>   _children;

            public string Path { get { return this._path; } }
            public GUIContent Label { get { return this._label; } set { this.Label = value; } }

            public Element(string label, string path)
            {
                this.Label = new GUIContent(label);
                this._path = path;
            }

            public List<Element> Children
            {
                get
                {
                    if (this._children == null)
                    {
                        this._children = new List<Element>();
                    }
                    return _children;
                }
                set
                {
                    this._children = value;
                }
            }

            public bool Contains(Element item)
            {
                if (item.Label.text == Label.text)
                {
                    return true;
                }
                for (int i = 0; i < Children.Count; i++)
                {
                    bool contains = Children[i].Contains(item);
                    if (contains)
                    {
                        return true;
                    }
                }
                return false;
            }

            public Element Find(string path)
            {
                if (this.Path == path)
                {
                    return this;
                }
                for (int i = 0; i < Children.Count; i++)
                {
                    Element tree = Children[i].Find(path);
                    if (tree != null)
                    {
                        return tree;
                    }
                }
                return null;
            }
        }
    }
}
