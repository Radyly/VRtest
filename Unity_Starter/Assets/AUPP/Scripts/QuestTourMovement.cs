using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace AUPP.Metaverse
{
    /// <summary>
    /// Basic XR device locomotion for an XR Origin root. Set head to the XR Main Camera.
    /// Use INSTEAD of XRI locomotion providers, not simultaneously. Head tracking comes
    /// from the XR Origin's Tracked Pose Driver/Input Actions, not from this component.
    /// Left joystick: walk, right joystick: snap turn, A: next zone, B: previous zone.
    /// </summary>
    public class QuestTourMovement : MonoBehaviour
    {
        public Transform head;
        public TourManager tour;
        public float speed = 1.6f;
        public float snapDegrees = 30f;
        [Range(.1f, .9f)] public float deadZone = .23f;
        private readonly List<InputDevice> devices = new List<InputDevice>();
        private InputDevice left, right;
        private bool turnReady = true, previousA, previousB;

        void Start()
        {
            if (!tour) tour = FindFirstObjectByType<TourManager>();
            if (!head && Camera.main) head = Camera.main.transform;
        }
        void Refresh()
        {
            if (!left.isValid)
            {
                devices.Clear(); InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, devices);
                if (devices.Count > 0) left = devices[0];
            }
            if (!right.isValid)
            {
                devices.Clear(); InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);
                if (devices.Count > 0) right = devices[0];
            }
        }
        void Update()
        {
            if (!head) return;
            Refresh();
            Vector2 walk;
            if (left.isValid && left.TryGetFeatureValue(CommonUsages.primary2DAxis, out walk))
            {
                if (walk.magnitude > deadZone)
                {
                    Vector3 forward = head.forward; forward.y = 0; forward.Normalize();
                    Vector3 rightward = head.right; rightward.y = 0; rightward.Normalize();
                    Vector3 amount = (forward * walk.y + rightward * walk.x) * (speed * Time.deltaTime);
                    var cc = GetComponent<CharacterController>();
                    if (cc && cc.enabled) cc.Move(amount); else transform.position += amount;
                }
            }
            Vector2 turn;
            if (right.isValid && right.TryGetFeatureValue(CommonUsages.primary2DAxis, out turn))
            {
                if (Mathf.Abs(turn.x) > .72f && turnReady)
                {
                    transform.RotateAround(new Vector3(head.position.x, transform.position.y, head.position.z), Vector3.up,
                        Mathf.Sign(turn.x) * snapDegrees);
                    turnReady = false;
                }
                if (Mathf.Abs(turn.x) < .25f) turnReady = true;
            }
            bool a = false, b = false;
            if (right.isValid)
            {
                right.TryGetFeatureValue(CommonUsages.primaryButton, out a);
                right.TryGetFeatureValue(CommonUsages.secondaryButton, out b);
            }
            if (a && !previousA && tour) tour.Next(transform);
            if (b && !previousB && tour) tour.Previous(transform);
            previousA = a; previousB = b;
        }
    }
}
