#define DEBUG_SKILL
using System;
using System.Collections.Generic;
using AIToolkitDemo;
using Lockstep.BehaviourTree;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {

    public class CSkill : MonoBehaviour {


        [HideInInspector] public BaseEntity owner => view?.owner;

        public LFloat CD;
        public LFloat cdTimer;
        public EAnimDefine animName;
        public KeyCode fireKeyCode;
        public LFloat doneDelay;
        public int targetLayer;
        public List<SkillPart> parts = new List<SkillPart>();

        public enum ESkillState {
            Idle,
            Firing,
        }

        private ESkillState state;
        [Header("__Debug")] [SerializeField] public LFloat maxPartTime;
        [SerializeField] private LFloat skillTimer;
        [SerializeField] private bool __DebugFireOnce = false;
        private PlayerView view;

        void Start(){
            ResortSkill();
            view = GetComponent<PlayerView>();
            skillTimer = maxPartTime;
        }

        void ResortSkill(){
            parts.Sort((a, b) => LMath.Sign(a.startTimer - b.startTimer));
            var time = LFloat.MinValue;
            foreach (var part in parts) {
                part.startTimer = part.startTimer * SkillPart.AnimFrameScale;
                var partDeadTime = part.DeadTimer;
                if (partDeadTime > time) {
                    time = partDeadTime;
                }
            }

            maxPartTime = time + doneDelay;
        }

        public void Fire(){
            if (cdTimer <= 0 && state == ESkillState.Idle) {
                cdTimer = CD;
                skillTimer = LFloat.zero;
                foreach (var part in parts) {
                    part.counter = 0;
                }

                state = ESkillState.Firing;
                owner.animator?.Play(animName.ToString());
                ((Player) owner).CMover.needMove = false;
                OnFire();
            }
        }

        public void OnFire(){
            owner.isInvincible = true;
            owner.isFire = true;
        }

        public void Done(){
            owner.isFire = false;
            owner.isInvincible = false;
            state = ESkillState.Idle;
            owner.animator?.Play(AnimDefine.Idle);
        }

        public void _Update(){
            if (__DebugFireOnce) {
                if (Input.GetKey(fireKeyCode)) {
                    Fire();
                }
            }


            DoUpdate(Time.deltaTime.ToLFloat());
        }

        public void DoUpdate(LFloat deltaTime){
            cdTimer -= deltaTime;
            skillTimer += deltaTime;
            if (skillTimer < maxPartTime) {
                foreach (var part in parts) {
                    CheckSkillPart(part);
                }
            }
            else {
                _curPart = null;
                if (state == ESkillState.Firing) {
                    Done();
                }
            }

#if DEBUG_SKILL
            if (_showTimer < Time.realtimeSinceStartup) {
                _curPart = null;
            }
#endif
        }

        void CheckSkillPart(SkillPart part){
            if (part.counter > part.otherCount) return;
            if (skillTimer > part.NextTriggerTimer()) {
                TriggerPart(part);
                part.counter++;
            }
        }
        public SkillPart _curPart;
#if DEBUG_SKILL
        public float _showTimer;
#endif
        void TriggerPart(SkillPart part){
            _curPart = part;
#if DEBUG_SKILL
            _showTimer = Time.realtimeSinceStartup + 0.1f;
#endif


            var col = part.collider;
            if (col.radius > 0) {
                //circle
                CollisionManager.QueryRegion(targetLayer, owner.transform.TransformPoint(col.pos), col.radius,
                    _OnTriggerEnter);
            }
            else {
                //aabb
                CollisionManager.QueryRegion(targetLayer, owner.transform.TransformPoint(col.pos), col.size,
                    owner.transform.forward,
                    _OnTriggerEnter);
            }

            foreach (var other in _tempTargets) {
                other.Entity.TakeDamage(_curPart.damage, other.Entity.transform.pos.ToLVector3());
            }

            //add force
            if (part.needForce ) {
                var force = part.impulseForce;
                var forward = owner.transform.forward;
                var right = forward.RightVec();
                var z = forward * force.z + right * force.x;
                force.x = z.x;
                force.z = z.y;
                foreach (var other in _tempTargets) {
                    other.Entity.rigidbody.AddImpulse(force);
                }
                
            }

            if (part.isResetForce) {
                foreach (var other in _tempTargets) {
                    other.Entity.rigidbody.ResetSpeed(new LFloat(3));
                }
            }

            _tempTargets.Clear();
        }

        static readonly HashSet<ColliderProxy> _tempTargets = new HashSet<ColliderProxy>();

        private void _OnTriggerEnter(ColliderProxy other){
            if (_curPart.collider.IsCircle && _curPart.collider.deg > 0) {
                var deg = (other.Transform2D.pos - owner.transform.pos).ToDeg();
                var degDiff = owner.transform.deg.Abs() - deg;
                if (LMath.Abs(degDiff) <= _curPart.collider.deg) {
                    _tempTargets.Add(other);
                }
            }
            else {
                _tempTargets.Add(other);
            }
        }

        public void Interrupt(){ }

        private void OnDrawGizmos(){
#if DEBUG_SKILL
            float tintVal = 0.3f;
            Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);
            if (Application.isPlaying) {
                if (owner == null) return;
                if (_curPart == null) return;
                ShowPartGizmons(_curPart);
            }
            else {
                foreach (var part in parts) {
                    if (part._DebugShow) {
                        ShowPartGizmons(part);
                    }
                }
            }

            Gizmos.color = Color.white;
#endif
        }

        private void ShowPartGizmons(SkillPart part){
            var col = part.collider;
            if (col.radius > 0) {
                //circle
                var pos = owner?.transform.TransformPoint(col.pos) ?? col.pos;
                Gizmos.DrawSphere(pos.ToVector3XZ(LFloat.one), col.radius.ToFloat());
            }
            else {
                //aabb
                var pos = owner?.transform.TransformPoint(col.pos) ?? col.pos;
                Gizmos.DrawCube(pos.ToVector3XZ(LFloat.one), col.size.ToVector3XZ(LFloat.one));
                DebugExtension.DebugLocalCube(Matrix4x4.TRS(
                        pos.ToVector3XZ(LFloat.one),
                        Quaternion.Euler(0, owner.transform.deg.ToFloat(), 0),
                        Vector3.one),
                    col.size.ToVector3XZ(LFloat.one), Gizmos.color);
            }
        }
    }
}