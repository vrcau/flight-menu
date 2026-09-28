using UdonSharp;
using UnityEngine;
using VAU.FlightMenuSystem.Runtime.MenuData;

namespace VAU.FlightMenuSystem.Runtime
{
    /// <summary>
    /// Menu system root component. In editor it previews and configures the root menu group of every child non popup
    /// <see cref="FlightMenuView"/>, at runtime it is the menu view set which station menu switcher reference.
    /// </summary>
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class FlightMenuSetup : UdonSharpBehaviour
    {
        [Header("Menu Views")]
        [Tooltip("Menu view of the desktop overlay canvas")]
        public FlightMenuView desktopMenuView;

        [Tooltip("Menu view of the left hand canvas, shown in VR")]
        public FlightMenuView vrLeftMenuView;

        [Tooltip("Menu view of the right hand canvas, shown in VR")]
        public FlightMenuView vrRightMenuView;

        private void Start()
        {
            if (!desktopMenuView && !vrLeftMenuView && !vrRightMenuView)
            {
                Debug.LogWarning(
                    $"[{nameof(FlightMenuSetup)}] no menu view is assigned, " +
                    "no menu view will be switched when a station menu switcher request it");
            }
        }

        /// <summary>
        /// Replace the root menu group of every assigned menu view. Unassigned views and null menu groups are skipped.
        /// </summary>
        public void SetRootMenuGroups(FlightMenuGroup desktop, FlightMenuGroup vrLeft, FlightMenuGroup vrRight)
        {
            if (desktopMenuView && desktop) desktopMenuView.SetRootMenuGroup(desktop);
            if (vrLeftMenuView && vrLeft) vrLeftMenuView.SetRootMenuGroup(vrLeft);
            if (vrRightMenuView && vrRight) vrRightMenuView.SetRootMenuGroup(vrRight);
        }
    }
}