using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Logic {
    public partial class SkillShoot : PlayerComponent {
        public CTransform2D shootTrans = new CTransform2D() {y = LFloat.half};
        public IPlayerView eventHandler=> ((Player) entity).eventHandler;
        public LFloat effectsDisplayTime = new LFloat(true, 100);
        public int PrefabId;
        public int damagePerShot = 20;
        public LFloat timeBetweenBullets;
        public LFloat range;
        public int shootableMask;
        public LFloat timer;

        public bool isInputFire => input.isInputFire;

        public override void DoUpdate(LFloat deltaTime){
            timer += deltaTime;
            if (isInputFire) {
                if (timer >= timeBetweenBullets) {
                    timer = LFloat.zero;
                    Shoot();
                }
            }

            if (timer >= timeBetweenBullets * effectsDisplayTime) {
                eventHandler.DisableEffects();
            }
        }


        void Shoot(){
            timer = LFloat.zero;
            eventHandler.Shoot();
            var shootRay = new Ray2D {origin = transform.pos, direction = transform.forward};
            if (CollisionManager.Raycast((int) EColliderLayer.Enemy, shootRay, out var shootHit)) {
                var collider = CollisionManager.GetCollider(shootHit.colliderId);
                var point = shootHit.point.ToLVector3XZ(LFloat.one);
                point.y = shootTrans.y;
                collider?.Entity?.TakeDamage(damagePerShot, point);
                eventHandler.SetLinePosition(1, point);
            }
            else {
                var point = (shootRay.origin + shootRay.direction * range).ToLVector3XZ(LFloat.one);
                point.y = shootTrans.y;
                eventHandler.SetLinePosition(1, point);
            }
        }

    }
}