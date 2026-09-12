namespace VAU.FlightMenuSystem.Runtime.MenuData.Item
{
    public sealed class FlightMenuBackItem : FlightMenuItemBase
    {
        public override FlightMenuTriggerResult Trigger()
        {
            base.Trigger();
            return FlightMenuTriggerResult.InternalBackMenu;
        }
    }
}