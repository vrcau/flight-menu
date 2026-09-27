using UnityEditor;
using UnityEngine;
using VAU.FlightMenuSystem.Runtime.EditorOnly;
using VAU.FlightMenuSystem.Runtime.MenuData;

namespace VAU.FlightMenuSystem.Editor.InspectorEditor
{
    [CustomEditor(typeof(FlightMenuReferenceItem))]
    public sealed class FlightMenuReferenceItemEditor : UnityEditor.Editor
    {
        private SerializedProperty _targetMenuItemProperty;

        private void OnEnable()
        {
            _targetMenuItemProperty = serializedObject.FindProperty(nameof(FlightMenuReferenceItem.targetMenuItem));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "This component is editor only and is removed from the built world. When the parent menu group is scanned, the target menu item is added to the menu instead of this component.",
                MessageType.Info);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_targetMenuItemProperty);
            var targetChanged = EditorGUI.EndChangeCheck();

            if (!_targetMenuItemProperty.objectReferenceValue)
            {
                EditorGUILayout.HelpBox(
                    "No target menu item assigned, this component is skipped during scan.",
                    MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();

            if (targetChanged)
            {
                RefreshParentMenuGroup();
            }
        }

        private void RefreshParentMenuGroup()
        {
            var referenceItem = (FlightMenuReferenceItem)target;
            var parent = referenceItem.transform.parent;
            if (!parent) return;

            var menuGroup = parent.GetComponent<FlightMenuGroup>();
            if (!menuGroup) return;

            FlightMenuGroupEditor.RefreshMenuItems(menuGroup);
        }
    }
}
