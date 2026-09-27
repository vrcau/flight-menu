using UdonSharp;

namespace VAU.FlightMenuSystem.Runtime.MenuData.Item
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class FlightMenuReferenceItem : FlightMenuItemBase
    {
        public FlightMenuItemBase targetMenuItem;
    }
}
