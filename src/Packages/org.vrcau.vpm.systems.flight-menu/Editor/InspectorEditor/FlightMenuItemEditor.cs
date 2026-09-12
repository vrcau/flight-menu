using UdonSharpEditor;
using UnityEditor;
using VAU.FlightMenuSystem.Runtime.MenuData.Item;

namespace VAU.FlightMenuSystem.Editor.InspectorEditor
{
    [CustomEditor(typeof(FlightMenuItemBase), true)]
    public sealed class FlightMenuItemEditor : UnityEditor.Editor
    {
        private FlightMenuItemGUI _previewGUI;
        private FlightMenuItemBase _targetMenuItem;

        private void OnEnable()
        {
            _targetMenuItem = (FlightMenuItemBase)target;
            _previewGUI = new FlightMenuItemGUI(_targetMenuItem);
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            _previewGUI?.OnGui();
        }
    }
}