using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    // 编辑器主面板
    [Serializable]
    public abstract class TCollectionEditor<T> : ICollectionEditor
    {
        private const float     _list_min_width = 200f;
        private const float     _list_max_width = 400f;
        private const float     _list_resize_width = 10f;
        private bool            _startDrag;
        private bool            _drag;
        private Rect            _dragRect = Rect.zero;

        protected Rect          _sidebarRect = new Rect(0, 30, 200, 1000);
        protected Vector2       _scrollPosition;
        protected Vector2       _sidebarScrollPosition;
        protected string        _searchString = string.Empty;

        protected abstract List<T> Items { get; }
        protected virtual bool CanAdd => true;
        protected virtual bool CanRemove => true;
        protected virtual bool CanDuplicate => true;
        protected virtual bool IsUseInspectorDefaultMargins => true;

        protected int _selectedItemIndex;   // 当前所选择的Item
        protected T SelectedItem
        {
            get
            {
                if (_selectedItemIndex > -1 && _selectedItemIndex < Items.Count)
                {
                    return Items[_selectedItemIndex];
                }
                return default;
            }
        }

        private string _toolbarName;        // 本面板的名称
        public virtual string ToolbarName => !string.IsNullOrEmpty(_toolbarName) ? _toolbarName : (GetType().IsGenericType ?
            ObjectNames.NicifyVariableName(GetType().GetGenericArguments()[0].Name) :
            ObjectNames.NicifyVariableName(GetType().Name.Replace("Editor", "")));

        public TCollectionEditor() { }
        public TCollectionEditor(string toolbarName) { 
            _toolbarName = toolbarName; 
        }

        public virtual void OnEnable()
        {
            string prefix = "CollectionEditor." + ToolbarName + ".";
            _selectedItemIndex = EditorPrefs.GetInt(prefix + "_selectedItemIndex");
            _sidebarRect.width = EditorPrefs.GetFloat(prefix + "_sidebarRect.width", _list_min_width);
            _scrollPosition.y = EditorPrefs.GetFloat(prefix + "_scrollposition.y");
            _sidebarScrollPosition.y = EditorPrefs.GetFloat(prefix + "_sidebarScrollPosition.y");

            if (_selectedItemIndex > -1 && this._selectedItemIndex < Items.Count)
                Select(Items[this._selectedItemIndex]);
        }

        public virtual void OnDisable()
        {
            string prefix = "CollectionEditor." + ToolbarName + ".";
            EditorPrefs.SetInt(prefix + "_selectedItemIndex", _selectedItemIndex);
            EditorPrefs.SetFloat(prefix + "_sidebarRect.width", _sidebarRect.width);
            EditorPrefs.SetFloat(prefix + "_scrollposition.y", _scrollPosition.y);
            EditorPrefs.SetFloat(prefix + "_sidebarScrollPosition.y", _sidebarScrollPosition.y);
        }

        public virtual void OnDestroy()
        {
            Debug.Log("OnDestroy " + ToolbarName);
        }

        public virtual void OnGUI(Rect position)
        {
            DrawSidebar(new Rect(position.x, position.y, _sidebarRect.width, position.height));
            DrawContent(new Rect(_sidebarRect.width, _sidebarRect.y, position.width - _sidebarRect.width, position.height));
            ResizeSidebar();
        }

        protected void DrawSidebar(Rect position)
        {
            _sidebarRect = position;
            GUILayout.BeginArea(_sidebarRect, "", CollectionEditorStyles.LeftPane);
            GUILayout.BeginHorizontal();
            DoSearchGUI();

            if (CanAdd)
            {
                GUIStyle style = new GUIStyle("ToolbarCreateAddNewDropDown");
                GUIContent content = EditorGUIUtility.IconContent("CreateAddNew");

                if (GUILayout.Button(content, style, GUILayout.Width(35f)))
                {
                    GUI.FocusControl("");
                    Create();
                    if (Items.Count > 0)
                    {
                        Select(Items[Items.Count - 1]);
                    }
                }
            }
            GUILayout.Space(1f);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            _sidebarScrollPosition = GUILayout.BeginScrollView(_sidebarScrollPosition);

            List<Rect> rects = new List<Rect>();
            for (int i = 0; i < Items.Count; i++)
            {
                T currentItem = Items[i];
                if (!MatchesSearch(currentItem, _searchString) && Event.current.type == EventType.Repaint)
                    continue;

                using (var h = new EditorGUILayout.HorizontalScope(CollectionEditorStyles.SelectButton, GUILayout.Height(25f)))
                {
                    Color backgroundColor = GUI.backgroundColor;
                    Color textColor = CollectionEditorStyles.SelectButtonText.normal.textColor;
                    GUI.backgroundColor = CollectionEditorStyles.NormalColor;

                    if (SelectedItem != null && SelectedItem.Equals(currentItem))
                    {
                        GUI.backgroundColor = CollectionEditorStyles.ActiveColor;
                        CollectionEditorStyles.SelectButtonText.normal.textColor = Color.white;
                        CollectionEditorStyles.SelectButtonText.fontStyle = FontStyle.Bold;
                    }
                    else if (h.rect.Contains(Event.current.mousePosition))
                    {
                        GUI.backgroundColor = CollectionEditorStyles.HoverColor;
                        CollectionEditorStyles.SelectButtonText.normal.textColor = textColor;
                        CollectionEditorStyles.SelectButtonText.fontStyle = FontStyle.Normal;
                    }

                    GUI.Label(h.rect, GUIContent.none, CollectionEditorStyles.SelectButton);
                    Rect rect = h.rect;
                    rect.width -= _list_resize_width * 0.5f;
                    if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        GUI.FocusControl("");
                        Select(currentItem);
                        _startDrag = true;
                        Event.current.Use();
                    }
                    DrawItemLabel(i, currentItem);

                    string error = HasConfigurationErrors(currentItem);
                    if (!string.IsNullOrEmpty(error))
                    {
                        GUI.backgroundColor = CollectionEditorStyles.WarningColor;
                        Rect errorRect = new Rect(h.rect.width - 20f, h.rect.y + 4.5f, 16f, 16f);
                        GUI.Label(errorRect, new GUIContent("", error), (GUIStyle)"CN EntryWarnIconSmall");
                    }
                    GUI.backgroundColor = backgroundColor;
                    CollectionEditorStyles.SelectButtonText.normal.textColor = textColor;
                    CollectionEditorStyles.SelectButtonText.fontStyle = FontStyle.Normal;
                    rects.Add(rect);
                }
            }

            switch (Event.current.rawType)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 1)
                        for (int j = 0; j < rects.Count; j++)
                        {
                            if (rects[j].Contains(Event.current.mousePosition))
                            {
                                ShowContextMenu(Items[j]);
                                break;
                            }
                        }
                    break;
                case EventType.MouseUp:
                    if (_drag)
                    {
                        _drag = false;
                        _startDrag = false;
                        for (int j = 0; j < rects.Count; j++)
                        {
                            Rect rect = rects[j];

                            Rect rect1 = new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f);
                            Rect rect2 = new Rect(rect.x, rect.y + rect.height * 0.5f, rect.width, rect.height * 0.5f);
                            int index = j;
                            if (index < _selectedItemIndex)
                                index += 1;
                            if (rect1.Contains(Event.current.mousePosition) && (index - 1) > -1)
                            {
                                MoveItem(_selectedItemIndex, index - 1);
                                Select(Items[index - 1]);
                                break;
                            }
                            else if (rect2.Contains(Event.current.mousePosition))
                            {
                                MoveItem(_selectedItemIndex, index);
                                Select(Items[index]);
                                break;
                            }
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (_startDrag)
                    {
                        for (int j = 0; j < rects.Count; j++)
                        {
                            if (rects[j].Contains(Event.current.mousePosition))
                            {
                                _drag = true;
                                break;
                            }
                        }
                    }
                    break;
            }

            for (int j = 0; j < rects.Count; j++)
            {
                Rect rect = rects[j];
                Rect rect1 = new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f);
                Rect rect2 = new Rect(rect.x, rect.y + rect.height * 0.5f, rect.width, rect.height * 0.5f);

                if (rect1.Contains(Event.current.mousePosition))
                {
                    _dragRect = rect;
                    _dragRect.y = rect.y + 10f - 25f;
                    _dragRect.x = rect.x + 5f;
                    break;
                }
                else if (rect2.Contains(Event.current.mousePosition))
                {
                    _dragRect = rect;
                    _dragRect.y = rect.y + 10f;
                    _dragRect.x = rect.x + 5f;
                    break;
                }
                else
                {
                    _dragRect = Rect.zero;
                }
            }

            if (_drag)
            {
                GUI.Label(_dragRect, GUIContent.none, CollectionEditorStyles.DragInsertion);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        protected void ShowContextMenu(T currentItem)
        {
            GenericMenu contextMenu = new GenericMenu();
            if (CanRemove)
                contextMenu.AddItem(new GUIContent("Delete"), false, delegate { Remove(currentItem); });
            if (CanDuplicate)
                contextMenu.AddItem(new GUIContent("Duplicate"), false, delegate { Duplicate(currentItem); });
            int oldIndex = Items.IndexOf(currentItem);
            if (CanMove(currentItem, oldIndex - 1))
            {
                contextMenu.AddItem(new GUIContent("Move Up"), false, delegate { MoveUp(currentItem); });
            }
            else
            {
                contextMenu.AddDisabledItem(new GUIContent("Move Up"));
            }
            if (CanMove(currentItem, oldIndex + 1))
            {
                contextMenu.AddItem(new GUIContent("Move Down"), false, delegate { MoveDown(currentItem); });
            }
            else
            {
                contextMenu.AddDisabledItem(new GUIContent("Move Down"));
            }

            AddContextItem(contextMenu);
            contextMenu.ShowAsContext();
        }

        protected virtual void AddContextItem(GenericMenu menu) { }

        protected virtual void DrawContent(Rect position)
        {
            GUILayout.BeginArea(position, "", CollectionEditorStyles.CenterPane);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, 
                IsUseInspectorDefaultMargins ? EditorStyles.inspectorDefaultMargins : GUIStyle.none);
            if (SelectedItem != null)
            {
                DrawItem(SelectedItem);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        // Select an item.
        protected virtual void Select(T item)
        {
            int index = Items.IndexOf(item);
            if (_selectedItemIndex != index)
            {
                _selectedItemIndex = index;
                _scrollPosition.y = 0f;
            }
        }

        // Create an item.
        protected virtual void Create(){}

        // Does the specified item has configuration errors.
        protected virtual string HasConfigurationErrors(T item)
        {
            return string.Empty;
        }
        // Remove the specified item from collection.
        protected virtual void Remove(T item) { }

        // Duplicates the specified item in collection
        protected virtual void Duplicate(T item) { }

        // Moves the item in database up.
        protected virtual void MoveUp(T item)
        {
            int oldIndex = Items.IndexOf(item);
            MoveItem(oldIndex, oldIndex - 1);
            Select(Items[oldIndex - 1]);
        }

        // Moves the item in database down.
        protected virtual void MoveDown(T item)
        {
            int oldIndex = Items.IndexOf(item);
            MoveItem(oldIndex, oldIndex + 1);
            Select(Items[oldIndex + 1]);
        }

        protected virtual bool CanMove(T item, int newIndex)
        {
            int oldIndex = Items.IndexOf(item);
            if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= Items.Count) 
                || (0 > newIndex) || (newIndex >= Items.Count))
                return false;
            return true;
        }

        protected void MoveItem(int oldIndex, int newIndex)
        {
            if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= Items.Count) 
                || (0 > newIndex) || (newIndex >= Items.Count)) 
                return;

            T tmp = Items[oldIndex];
            if (oldIndex < newIndex)
            {
                for (int i = oldIndex; i < newIndex; i++)
                {
                    Items[i] = Items[i + 1];
                }
            }
            else
            {
                for (int i = oldIndex; i > newIndex; i--)
                {
                    Items[i] = Items[i - 1];
                }
            }
            Items[newIndex] = tmp;
        }

        // Draws the item properties.
        protected virtual void DrawItem(T item) { }

        protected virtual void DrawItemLabel(int index, T item)
        {
            GUILayout.Label(ButtonLabel(index, item), CollectionEditorStyles.SelectButtonText);
        }

        // Gets the sidebar label displayed in sidebar.
        protected abstract string GetSidebarLabel(T item);

        protected virtual string ButtonLabel(int index, T item)
        {
            return index + ":  " + GetSidebarLabel(item);
        }

        // Checks for search.
        protected abstract bool MatchesSearch(T item, string search);

        protected virtual void DoSearchGUI()
        {
            _searchString = EditorTools.SearchField(_searchString);
        }

        protected void ResizeSidebar()
        {
            Rect rect = new Rect(_sidebarRect.width - _list_resize_width * 0.5f, _sidebarRect.y, _list_resize_width, _sidebarRect.height);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event ev = Event.current;
            switch (ev.rawType)
            {
                case EventType.MouseDown:
                    if (rect.Contains(ev.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        ev.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        ev.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        _drag = false;
                        _sidebarRect.width = ev.mousePosition.x;
                        _sidebarRect.width = Mathf.Clamp(_sidebarRect.width, _list_min_width, _list_max_width);
                        EditorPrefs.SetFloat("CollectionEditorSidebarWidth" + ToolbarName, _sidebarRect.width);
                        ev.Use();
                    }
                    break;
            }
        }
    }
}
