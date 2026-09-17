using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace VAU.FlightMenuSystem.Runtime.MenuData.Item
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class FlightMenuItemBase : UdonSharpBehaviour
    {
        public string title;
        public Sprite icon;
        public bool isActivated;
        public bool isDisabled;

        public bool requestClosePopupWhenTrigger;

        public bool isHide;

        public UdonSharpBehaviour eventTarget;
        public string triggerEventName;
        public string holdStartEventName;
        public string holdEndEventName;
        public bool updateHoldStateToEventTarget;
        public string holdStateVariableName;

        public bool updateIsActivatedFromEventTarget;
        public string isActivatedVariableName;
        public bool invertIsActivatedVariable;

        public bool updateTitleFromEventTarget;
        public string titleVariableName;
        public string titleTemplate = "{0}";

        public bool updateIsEnabledFromEventTarget;
        public string isDisabledVariableName;
        public bool invertIsDisabledVariable;

        public virtual FlightMenuTriggerResult Trigger()
        {
            if (eventTarget && !string.IsNullOrWhiteSpace(triggerEventName))
            {
                eventTarget.SendCustomEvent(triggerEventName);
            }

            if (requestClosePopupWhenTrigger)
            {
                return FlightMenuTriggerResult.RequestClosePopup;
            }

            return FlightMenuTriggerResult.Noop;
        }

        public virtual void OnHoldStart()
        {
            if (!eventTarget) return;

            if (updateHoldStateToEventTarget && !string.IsNullOrWhiteSpace(holdStateVariableName))
            {
                eventTarget.SetProgramVariable(holdStateVariableName, true);
            }

            if (!string.IsNullOrWhiteSpace(holdStartEventName))
            {
                eventTarget.SendCustomEvent(holdStartEventName);
            }
        }

        public virtual void OnHoldEnd()
        {
            if (!eventTarget) return;

            if (updateHoldStateToEventTarget && !string.IsNullOrWhiteSpace(holdStateVariableName))
            {
                eventTarget.SetProgramVariable(holdStateVariableName, false);
            }

            if (!string.IsNullOrWhiteSpace(holdEndEventName))
            {
                eventTarget.SendCustomEvent(holdEndEventName);
            }
        }

        [CanBeNull]
        public virtual FlightMenuGroup GetNewMenu()
        {
            return null;
        }
    }

    public enum FlightMenuTriggerResult
    {
        Noop = 0,
        OpenNewMenu = 1,
        OpenPopupMenu = 2,
        InternalBackMenu = 3,
        RequestClosePopup = 4,
        OpenSliderMenu = 5
    }
}