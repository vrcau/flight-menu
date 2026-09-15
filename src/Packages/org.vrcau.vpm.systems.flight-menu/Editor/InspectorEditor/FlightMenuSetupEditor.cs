using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VAU.FlightMenuSystem.Runtime;
using VAU.FlightMenuSystem.Runtime.EditorOnly;

namespace VAU.FlightMenuSystem.Editor.InspectorEditor
{
    [CustomEditor(typeof(FlightMenuSetup))]
    public sealed class FlightMenuSetupEditor : UnityEditor.Editor
    {
        private FlightMenuSetup _setup;

        private Dictionary<FlightMenuView, FlightMenuSetupGui> _views;

        private SerializedProperty _viewMenuGroupProperty;

        private void OnEnable()
        {
            _setup = (FlightMenuSetup)target;

            _views =
                _setup.GetComponentsInChildren<FlightMenuView>(true)
                    .Where(x => !x.isPopupMenu)
                    .ToDictionary(x => x, x => new FlightMenuSetupGui(x));
        }

        public override void OnInspectorGUI()
        {
            foreach (var (view, gui) in _views)
            {
                if (!view)
                {
                    continue;
                }

                gui.OnGui();
                GUILayout.Space(10);
            }
        }

        private class FlightMenuSetupGui
        {
            private readonly FlightMenuView _view;

            private readonly SerializedProperty _remarkProperty;
            private readonly SerializedProperty _menuGroupProperty;

            private bool _expandSubMenuPreview;
            private FlightMenuPreviewGUI _subMenuPreviewGui;

            public FlightMenuSetupGui(FlightMenuView view)
            {
                _view = view;
                var o = new SerializedObject(view);

                _remarkProperty = o.FindProperty(nameof(FlightMenuView.remark));
                _menuGroupProperty = o.FindProperty(nameof(FlightMenuView.rootMenuGroup));
            }

            public void OnGui()
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                // [GameObject] | [ViewCore Object]
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(_remarkProperty, new GUIContent());
                EditorGUILayout.ObjectField(_view, typeof(FlightMenuView), true);

                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(_menuGroupProperty);

                if (_view.rootMenuGroup)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(12);
                    GUILayout.BeginVertical();

                    _expandSubMenuPreview = EditorGUILayout.Foldout(_expandSubMenuPreview, "Preview Menu");
                    if (_expandSubMenuPreview)
                    {
                        _subMenuPreviewGui ??= new FlightMenuPreviewGUI(_view.rootMenuGroup);
                        _subMenuPreviewGui.OnGui();
                    }
                    else
                    {
                        _subMenuPreviewGui = null;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    _subMenuPreviewGui = null;
                }

                GUILayout.EndVertical();
            }
        }
    }
}