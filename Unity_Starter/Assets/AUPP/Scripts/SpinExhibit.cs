using UnityEngine;
namespace AUPP.Metaverse
{
    public class SpinExhibit : MonoBehaviour
    {
        [Range(0, 60)] public float degreesPerSecond = 16f;
        void Update() => transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.World);
    }
}
