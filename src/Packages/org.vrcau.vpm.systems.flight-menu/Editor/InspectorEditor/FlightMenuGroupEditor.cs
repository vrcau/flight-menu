using System.Collections.Generic;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VAU.FlightMenuSystem.Runtime.MenuData;
using VAU.FlightMenuSystem.Runtime.MenuData.Item;

namespace VAU.FlightMenuSystem.Editor.InspectorEditor
{
    [CustomEditor(typeof(FlightMenuGroup))]
    public sealed class FlightMenuGroupEditor : UnityEditor.Editor
    {
        private FlightMenuPreviewGUI _previewGUI;
        private FlightMenuGroup _targetMenuGroup;

        private void OnEnable()
        {
            _targetMenuGroup = (FlightMenuGroup)target;
            _previewGUI = new FlightMenuPreviewGUI(_targetMenuGroup);
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            EditorGUILayout.HelpBox(
                "Child FlightMenuGroup only got update when there is a SubMenuItem in same object",
                MessageType.Info
            );

            if (GUILayout.Button("Scan child menu item update"))
            {
                _targetMenuGroup.menuItems = ScanChildMenuItem(_targetMenuGroup);
                EditorUtility.SetDirty(_targetMenuGroup);
            }

            _previewGUI?.OnGui();
        }

        internal static FlightMenuItemBase[] ScanChildMenuItem(FlightMenuGroup targetMenuGroup)
        {
            var newItemList = new List<FlightMenuItemBase>();
            foreach (Transform child in targetMenuGroup.transform)
            {
                var item = child.GetComponent<FlightMenuItemBase>();
                if (!item) continue;

                if (item is FlightMenuReferenceItem referenceItem)
                {
                    AddReferenceTarget(newItemList, referenceItem, new HashSet<FlightMenuItemBase>());
                    continue;
                }

                if (item is FlightMenuSubMenuItem subMenuItem)
                {
                    var menuGroupInSameObject = subMenuItem.GetComponent<FlightMenuGroup>();
                    if (menuGroupInSameObject)
                    {
                        menuGroupInSameObject.menuItems = ScanChildMenuItem(menuGroupInSameObject);

                        if (!subMenuItem.subMenu) subMenuItem.subMenu = menuGroupInSameObject;
                    }
                }

                AddIfMissing(newItemList, item);
            }

            return newItemList.ToArray();
        }

        private static void AddReferenceTarget(
            List<FlightMenuItemBase> itemList,
            FlightMenuReferenceItem referenceItem,
            HashSet<FlightMenuItemBase> visiting)
        {
            var target = referenceItem.targetMenuItem;
            if (!target)
            {
                Debug.LogWarning(
                    $"[FlightMenu] Reference menu item '{referenceItem.gameObject.name}' has no target menu item, it is skipped during scan.",
                    referenceItem);
                return;
            }

            if (!visiting.Add(target))
            {
                Debug.LogWarning(
                    $"[FlightMenu] Reference loop detected at '{referenceItem.gameObject.name}' -> '{target.gameObject.name}', it is skipped during scan.",
                    referenceItem);
                return;
            }

            if (target is FlightMenuReferenceItem nestedReferenceItem)
            {
                AddReferenceTarget(itemList, nestedReferenceItem, visiting);
                return;
            }

            AddIfMissing(itemList, target);
        }

        private static void AddIfMissing(List<FlightMenuItemBase> itemList, FlightMenuItemBase item)
        {
            if (itemList.Contains(item)) return;

            itemList.Add(item);
        }
    }
}