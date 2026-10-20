using UnityEngine;

namespace AUPP.Metaverse
{
    /// <summary>Shared tour destinations. Coordinates are in meters. Assign an XR Origin or desktop camera as traveller.</summary>
    public class TourManager : MonoBehaviour
    {
        public Transform[] stops;
        public string[] stopNames = { "Welcome Hall", "Showcase Corridor", "Robotics Lab", "AI & XR Lab", "Project Space" };
        [Range(0, 4)] public int activeStop = 0;

        public string CurrentName => stopNames != null && activeStop < stopNames.Length ? stopNames[activeStop] : "Tour";
        public void GoTo(int index, Transform traveller)
        {
            if (stops == null || stops.Length == 0 || traveller == null) return;
            activeStop = (index % stops.Length + stops.Length) % stops.Length;
            var cc = traveller.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            traveller.position = stops[activeStop].position + (traveller.GetComponent<DesktopTourController>() ? Vector3.up * 1.65f : Vector3.zero);
            traveller.rotation = stops[activeStop].rotation;
            if (cc) cc.enabled = true;
        }
        public void Next(Transform traveller) => GoTo(activeStop + 1, traveller);
        public void Previous(Transform traveller) => GoTo(activeStop - 1, traveller);
    }
}
