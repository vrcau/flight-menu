using System;
using System.Collections.Generic;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VAU.FlightMenuSystem.Runtime.EditorOnly;
using VAU.FlightMenuSystem.Runtime.MenuData;
using VAU.FlightMenuSystem.Runtime.MenuData.Item;

namespace VAU.FlightMenuSystem.Editor.InspectorEditor
{
    public class FlightMenuPreviewGUI
    {
        private static readonly Color ReferenceSeparatorColor = new(0.5f, 0.5f, 0.5f, 0.4f);

        private readonly FlightMenuGroup _flightMenuGroup;
        private readonly SerializedObject _serializedObject;

        private readonly SerializedProperty _menuItemsProperty;
        private readonly SerializedProperty _menuGroupNameProperty;
        private readonly SerializedProperty _menuGroupDescriptionProperty;
        private readonly SerializedProperty _keepUpdateTitleProperty;

        private readonly Dictionary<FlightMenuItemBase, FlightMenuItemGUI> _itemGuis = new();
        private readonly Dictionary<FlightMenuReferenceItem, SerializedObject> _referenceSerializedObjects = new();

        private bool _referenceTargetChanged;

        public FlightMenuPreviewGUI(FlightMenuGroup flightMenuGroup)
        {
            _flightMenuGroup = flightMenuGroup;
            _serializedObject = new SerializedObject(flightMenuGroup);

            _menuItemsProperty = _serializedObject
                .FindProperty(nameof(FlightMenuGroup.menuItems));
            _menuGroupNameProperty = _serializedObject.FindProperty(nameof(FlightMenuGroup.groupName));
            _menuGroupDescriptionProperty = _serializedObject.FindProperty(nameof(FlightMenuGroup.description));
            _keepUpdateTitleProperty = _serializedObject.FindProperty(nameof(FlightMenuGroup.keepUpdateGroupTitle));
        }

        public void OnGui()
        {
            if (_flightMenuGroup.menuItems == null)
            {
                _flightMenuGroup.menuItems = Array.Empty<FlightMenuItemBase>();
                EditorUtility.SetDirty(_flightMenuGroup);
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_menuGroupNameProperty);
            EditorGUILayout.PropertyField(_menuGroupDescriptionProperty);
            EditorGUILayout.PropertyField(_keepUpdateTitleProperty);
            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }

            var referenceItems = CollectReferenceItems();
            var drawnReferenceItems = new List<FlightMenuReferenceItem>();
            _referenceTargetChanged = false;

            for (var index = 0; index < _flightMenuGroup.menuItems.Length; index++)
            {
                var menuItem = _flightMenuGroup.menuItems[index];
                if (!menuItem)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.HelpBox(
                        "Menu item reference missing, please re-assign or re-scan child menu items",
                        MessageType.Error
                    );

                    EditorGUI.BeginChangeCheck();

                    var objectProperty = _menuItemsProperty.GetArrayElementAtIndex(index);
                    EditorGUILayout.PropertyField(objectProperty);

                    GUILayout.EndVertical();
                    if (EditorGUI.EndChangeCheck())
                    {
                        _serializedObject.ApplyModifiedProperties();
                    }

                    continue;
                }

                if (!_itemGuis.TryGetValue(menuItem, out var itemGui))
                {
                    itemGui = new FlightMenuItemGUI(menuItem);
                    _itemGuis[menuItem] = itemGui;
                }

                // Reference menu items pointing to this menu item are drawn inside this item's own
                // box, so it is always clear which menu item a reference belongs to.
                List<FlightMenuReferenceItem> referencesForItem = null;
                foreach (var referenceItem in referenceItems)
                {
                    if (referenceItem.targetMenuItem != menuItem) continue;

                    referencesForItem ??= new List<FlightMenuReferenceItem>();
                    referencesForItem.Add(referenceItem);
                    drawnReferenceItems.Add(referenceItem);
                }

                if (referencesForItem == null)
                {
                    itemGui.SetFooterGui(null);
                }
                else
                {
                    var references = referencesForItem;
                    itemGui.SetFooterGui(() => DrawReferenceItemsForItem(references));
                }

                itemGui.OnGui();
            }

            DrawUnresolvedReferenceItems(referenceItems, drawnReferenceItems);

            if (EditorGUILayout.DropdownButton(new GUIContent("Create new menu item"), FocusType.Keyboard))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Button"), false,
                    () =>
                    {
                        var newGameObject = new GameObject("[Empty]");
                        newGameObject.transform.SetParent(_flightMenuGroup.transform);

                        newGameObject.AddUdonSharpComponent<FlightMenuButtonItem>();

                        _flightMenuGroup.menuItems = FlightMenuGroupEditor.ScanChildMenuItem(_flightMenuGroup);
                        EditorUtility.SetDirty(_flightMenuGroup);
                    });

                menu.AddItem(new GUIContent("SubMenu"), false, () =>
                {
                    var newGameObject = new GameObject("[Empty]");
                    newGameObject.transform.SetParent(_flightMenuGroup.transform);

                    var subMenuItem = newGameObject.AddUdonSharpComponent<FlightMenuSubMenuItem>();
                    subMenuItem.isPopupMenu = false;

                    _flightMenuGroup.menuItems = FlightMenuGroupEditor.ScanChildMenuItem(_flightMenuGroup);
                    EditorUtility.SetDirty(_flightMenuGroup);
                });

                menu.AddItem(new GUIContent("Popup Menu"), false, () =>
                {
                    var newGameObject = new GameObject("[Empty]");
                    newGameObject.transform.SetParent(_flightMenuGroup.transform);

                    var popupMenuItem = newGameObject.AddUdonSharpComponent<FlightMenuSubMenuItem>();
                    popupMenuItem.isPopupMenu = true;

                    _flightMenuGroup.menuItems = FlightMenuGroupEditor.ScanChildMenuItem(_flightMenuGroup);
                    EditorUtility.SetDirty(_flightMenuGroup);
                });

                menu.AddItem(new GUIContent("Reference"), false, () =>
                {
                    var newGameObject = new GameObject("[Reference]");
                    newGameObject.transform.SetParent(_flightMenuGroup.transform);

                    newGameObject.AddComponent<FlightMenuReferenceItem>();

                    // The referenced menu item is unknown yet, so keep the menu items untouched
                    // until the target is assigned in the reference menu item list above.
                });

                menu.ShowAsContext();
            }

            GUILayout.EndVertical();
            GUILayout.Space(8);

            if (!_referenceTargetChanged) return;

            // The referenced menu item changed, rebuild this menu group's menu items immediately.
            FlightMenuGroupEditor.RefreshMenuItems(_flightMenuGroup);
            _serializedObject.Update();
        }

        private List<FlightMenuReferenceItem> CollectReferenceItems()
        {
            var referenceItems = new List<FlightMenuReferenceItem>();
            foreach (Transform child in _flightMenuGroup.transform)
            {
                var referenceItem = child.GetComponent<FlightMenuReferenceItem>();
                if (referenceItem) referenceItems.Add(referenceItem);
            }

            return referenceItems;
        }

        private void DrawReferenceItemsForItem(List<FlightMenuReferenceItem> referenceItems)
        {
            DrawReferenceSeparator();

            EditorGUILayout.LabelField("Referenced by", EditorStyles.miniBoldLabel);

            foreach (var referenceItem in referenceItems)
            {
                DrawReferenceItem(referenceItem);
            }
        }

        private static void DrawReferenceSeparator()
        {
            var separatorRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(separatorRect, ReferenceSeparatorColor);
        }

        private void DrawReferenceItem(FlightMenuReferenceItem referenceItem)
        {
            if (!_referenceSerializedObjects.TryGetValue(referenceItem, out var referenceObject) ||
                !referenceObject.targetObject)
            {
                referenceObject = new SerializedObject(referenceItem);
                _referenceSerializedObjects[referenceItem] = referenceObject;
            }

            referenceObject.Update();
            var targetMenuItemProperty =
                referenceObject.FindProperty(nameof(FlightMenuReferenceItem.targetMenuItem));

            GUILayout.BeginHorizontal();

            // Read only reference to the reference item component this row belongs to.
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(referenceItem, typeof(FlightMenuReferenceItem), false);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(targetMenuItemProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                referenceObject.ApplyModifiedProperties();
                _referenceTargetChanged = true;
            }

            GUILayout.EndHorizontal();

            if (!referenceItem.targetMenuItem)
            {
                EditorGUILayout.HelpBox(
                    "No target menu item assigned, this reference is skipped during scan.",
                    MessageType.Warning);
            }
        }

        private void DrawUnresolvedReferenceItems(
            List<FlightMenuReferenceItem> referenceItems,
            List<FlightMenuReferenceItem> drawnReferenceItems)
        {
            var unresolvedReferenceItems = new List<FlightMenuReferenceItem>();
            foreach (var referenceItem in referenceItems)
            {
                if (drawnReferenceItems.Contains(referenceItem)) continue;

                unresolvedReferenceItems.Add(referenceItem);
            }

            if (unresolvedReferenceItems.Count == 0) return;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Unlinked Reference Menu Items", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "These references are not in the menu items above. Assign a target menu item or scan the child menu item update.",
                MessageType.Warning);

            foreach (var referenceItem in unresolvedReferenceItems)
            {
                DrawReferenceItem(referenceItem);
            }

            GUILayout.EndVertical();
        }
    }
}