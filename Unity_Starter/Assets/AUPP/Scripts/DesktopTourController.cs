using UnityEngine;

namespace AUPP.Metaverse
{
    /// <summary>Desktop preview only. Disable Desktop Camera when you add an XR Origin.</summary>
    [RequireComponent(typeof(CharacterController))]
    public class DesktopTourController : MonoBehaviour
    {
        public float moveSpeed = 3.2f;
        public float mouseSensitivity = 1.8f;
        public TourManager tour;
        private float pitch;
        private CharacterController cc;
        private bool mouseLocked = false;

        void Start() { cc = GetComponent<CharacterController>(); Cursor.lockState = CursorLockMode.None; }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SetMouse(false);
            if (Input.GetMouseButtonDown(0) && !mouseLocked) SetMouse(true);
            if (mouseLocked)
            {
                transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseSensitivity, Space.World);
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity, -85, 85);
                var e = transform.eulerAngles; transform.eulerAngles = new Vector3(pitch, e.y, 0);
            }
            var forward = transform.forward; forward.y = 0; forward.Normalize();
            var right = transform.right; right.y = 0; right.Normalize();
            var horizontal = Input.GetAxisRaw("Horizontal"); var vertical = Input.GetAxisRaw("Vertical");
            var direction = (forward * vertical + right * horizontal).normalized;
            cc.SimpleMove(direction * moveSpeed);
            if (!tour) return;
            for (int i = 0; i < 5; i++)
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) tour.GoTo(i, transform);
        }
        void SetMouse(bool state) { mouseLocked = state; Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None; Cursor.visible = !state; }
        void OnGUI()
        {
            if (!tour) return;
            GUI.Box(new Rect(12, 12, 255, 126), "AUPP ETL  |  " + tour.CurrentName);
            GUI.Label(new Rect(24, 36, 239, 38), "WASD move | Click to look | Esc unlock");
            GUI.Label(new Rect(24, 62, 239, 26), "Keys 1–5: jump between five zones");
            if (GUI.Button(new Rect(23, 91, 106, 32), "Previous")) tour.Previous(transform);
            if (GUI.Button(new Rect(140, 91, 105, 32), "Next")) tour.Next(transform);
        }
    }
}
