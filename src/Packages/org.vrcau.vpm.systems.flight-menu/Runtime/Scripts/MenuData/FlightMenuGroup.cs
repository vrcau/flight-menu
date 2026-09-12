using System;
using UdonSharp;
using VAU.FlightMenuSystem.Runtime.MenuData.Item;

namespace VAU.FlightMenuSystem.Runtime.MenuData
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class FlightMenuGroup : UdonSharpBehaviour
    {
        public string groupName;
        public string description;
        public FlightMenuItemBase[] menuItems = Array.Empty<FlightMenuItemBase>();

        public bool keepUpdateGroupTitle;
    }
}