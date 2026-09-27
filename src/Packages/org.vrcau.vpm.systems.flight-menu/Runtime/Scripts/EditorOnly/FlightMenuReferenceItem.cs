using UnityEngine;
using VRC.SDKBase;
using VAU.FlightMenuSystem.Runtime.MenuData.Item;

namespace VAU.FlightMenuSystem.Runtime.EditorOnly
{
    [DisallowMultipleComponent]
    public sealed class FlightMenuReferenceItem : MonoBehaviour, IEditorOnly
    {
        public FlightMenuItemBase targetMenuItem;
    }
}
