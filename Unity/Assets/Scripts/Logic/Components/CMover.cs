using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Logic {
    public partial class CMover : PlayerComponent {
        static LFloat _sqrStopDist = new LFloat(true, 40);
        public LFloat speed => entity.speed;
        public bool hasReachTarget = false;

        public bool needMove = true;

        public override void DoUpdate(LFloat deltaTime){
            //if (InputManager.hasHitFloor) {
            //    var dir = (InputManager.mousePos - GameManager.player.transform2D.pos);
            //    transform2D.deg = CTransform2D.ToDeg(dir);
            //}
            if (!entity.rigidbody.isOnFloor) {
                return;
            }
#if true
            var needAc = input.inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needAc) {
                var dir = input.inputUV.normalized;
                transform.pos = transform.pos + dir * speed * deltaTime;
                var targetDeg = dir.ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, 360 * deltaTime, out var hasReachDeg);
            }

            hasReachTarget = !needAc;
#else
            if (InputManager.hasHitFloor) {
                needMove = true;
            }

            if (!needMove) {
                return;
            }
            var targetPos = InputManager.mousePos;
            var movement = targetPos - transform2D.pos;
            var hasReachPos = movement.sqrMagnitude < _sqrStopDist;
            if (!hasReachPos) {
                movement = movement.normalized * speed * deltaTime;
                transform2D.pos = transform2D.pos + movement;
            }

            var deg = CTransform2D.TurnToward(targetPos, transform2D.pos,
                transform2D.deg, 150 * deltaTime,
                out var hasReachDeg);

            if (!hasReachDeg) {
                transform2D.deg = deg;
            }

            hasReachTarget = hasReachPos & hasReachDeg;
            if (hasReachTarget) {
                needMove = false;
            }
#endif
        }
    }
}