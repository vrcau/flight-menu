using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VAU.FlightMenuSystem.Runtime.MenuData;

namespace VAU.FlightMenuSystem.Runtime
{
    /// <summary>
    /// Replace the root menu group of the menu views in the target <see cref="FlightMenuSetup"/> when the local
    /// player enters the station on this GameObject.
    /// </summary>
    /// <remarks>
    /// VRCStation only sends OnStationEntered to the UdonBehaviour on the same GameObject as the station, so this
    /// component has to be placed on the station GameObject, and not on a parent or a child of it.
    /// A menu view without menu group assigned to it simply keeps its root menu group unchanged.
    /// </remarks>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class FlightMenuStationMenuSwitcher : UdonSharpBehaviour
    {
        [Header("Core")]
        [Tooltip("Menu system which menu views root menu group will be replaced when the local player enters the station")]
        public FlightMenuSetup menuSetup;

        [Header("Menu Groups")]
        [Tooltip("Menu group to show on the desktop menu view when the local player enters the station")]
        public FlightMenuGroup menuGroupOnDesktop;

        [Tooltip("Menu group to show on the left hand menu view when the local player enters the station")]
        public FlightMenuGroup menuGroupOnVrLeft;

        [Tooltip("Menu group to show on the right hand menu view when the local player enters the station")]
        public FlightMenuGroup menuGroupOnVrRight;

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            if (!menuSetup) return;

            menuSetup.SetRootMenuGroups(menuGroupOnDesktop, menuGroupOnVrLeft, menuGroupOnVrRight);
        }
    }
}