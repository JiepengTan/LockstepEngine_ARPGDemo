using Lockstep.Collision2D;
using Lockstep.Math;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {
    public class InputManager : UnityEngine.MonoBehaviour {
        [HideInInspector] public Player player => GameManager.player;
        [HideInInspector] public int floorMask;
        public float camRayLength = 100;

        public static bool IsReplay = false;

        void Start(){
            floorMask = LayerMask.GetMask("Floor");
        }

        public void Update(){
            if (!IsReplay) {
                float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
                inputUV = new LVector2(h.ToLFloat(), v.ToLFloat());

                isInputFire = Input.GetButton("Fire1");
                hasHitFloor = Input.GetMouseButtonDown(1);
                if (hasHitFloor) {
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit floorHit;
                    if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
                        mousePos = floorHit.point.ToLVector2XZ();
                    }
                }

                skillId = -1;
                for (int i = 0; i < 6; i++) {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                        skillId = i;
                    }
                }

                isSpeedUp = Input.GetKeyDown(KeyCode.Space);
            }
        }

        public static bool hasHitFloor;
        public static LVector2 mousePos;
        public static LVector2 inputUV;
        public static bool isInputFire;
        public static int skillId;
        public static bool isSpeedUp;
    }


}