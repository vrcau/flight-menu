using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VAU.FlightMenuSystem.Runtime
{
    /// <summary>
    /// Follow a player tracking data point (for example the left or the right hand) with the target transform, so that
    /// the VR hand menu canvases stay on the hands of the local player.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTrackingDataFollower : UdonSharpBehaviour
    {
        public Transform moveTarget;

        public VRCPlayerApi.TrackingDataType trackingTarget;
        public Vector3 positionOffset;
        public bool trackRotation;

        private VRCPlayerApi _localPlayer;

        private void Start()
        {
            if (!moveTarget) moveTarget = transform;
            _localPlayer = Networking.LocalPlayer;
        }

        public override void PostLateUpdate()
        {
            var trackingData = _localPlayer.GetTrackingData(trackingTarget);
            if (trackRotation)
            {
                moveTarget.SetPositionAndRotation(trackingData.position + positionOffset, trackingData.rotation);
            }
            else
            {
                moveTarget.position = trackingData.position + positionOffset;
            }
        }
    }
}