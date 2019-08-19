using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    public class CAnimatorView {
        private Transform unityTransform;
        private Animator anim;

        public CAnimatorView Init(BaseEntity entity){
            unityTransform = entity.GetUnityTransform();
            anim = unityTransform.GetComponent<Animator>();
            return this;
        }

        public void SetInteger(string name, int val){
            anim.SetInteger(name, val);
        }

        public void SetTrigger(string name){
            anim.SetTrigger(name);
        }

        public LFloat speed;
        //{
        //    get => anim.speed.ToLFloat();
        //    set => anim.speed = value.ToFloat();
        //}
    }
}