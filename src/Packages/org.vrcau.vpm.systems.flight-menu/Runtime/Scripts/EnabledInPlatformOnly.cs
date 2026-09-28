using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VAU.FlightMenuSystem.Runtime
{
    /// <summary>
    /// Enable the target GameObject only on the platform it is made for, so that the menu system can switch between
    /// the desktop overlay canvas and the VR hand canvases.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class EnabledInPlatformOnly : UdonSharpBehaviour
    {
        public GameObject targetGameObject;
        public bool enableInVR;
        public bool enableInDesktop;

        private void Start()
        {
            if (!targetGameObject) targetGameObject = gameObject;
            var userInVR = Networking.LocalPlayer.IsUserInVR();
            var shouldActive =
                (userInVR && enableInVR) ||
                (!userInVR && enableInDesktop);

            targetGameObject.SetActive(shouldActive);
        }
    }
}