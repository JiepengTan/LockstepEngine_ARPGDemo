using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {
    public interface IUpdate {
        void DoUpdate(LFloat deltaTime);
    }

    public interface IAwake {
        void DoAwake();
    }

    public interface IStart {
        void DoStart();
    }

    public interface IDestroy {
        void DoDestroy();
    }

    public interface ILifeCycle : IUpdate, IAwake, IStart, IDestroy { }

    [Serializable]
    public class BaseLifeCycle : ILifeCycle {
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }

    public interface IEntity : ILifeCycle { }

    [Serializable]
    public class BaseEntity : BaseLifeCycle, IEntity, ILPTriggerEventHandler {
        [Header("BaseComponents")] public EntityAttri entityAttri = new EntityAttri();
        public CRigidbody rigidbody = new CRigidbody();
        public CTransform2D transform = new CTransform2D();
        public CAnimation animator = new CAnimation();
        public int EntityId;

        public int PrefabId;
        [Header("Other Attri")] public object engineTransform;
        public bool isDead;
        public int startingHealth = 100;
        public int currentHealth;
        public bool isInvincible;
        public bool isFire;
        public LFloat speed = new LFloat(5);
        public Action<int, LVector3> OnBeAtked;


        protected List<BaseComponent> allComponents = new List<BaseComponent>();

        protected List<CSkill> allSkills = new List<CSkill>();

        public static int _IdCounter;

        public BaseEntity(){
            Debug.Trace("BaseEntity  " + _IdCounter.ToString(), true);
            EntityId = _IdCounter++;
            rigidbody.transform2D = transform;
        }

        protected void RegisterComponent(BaseComponent comp){
            allComponents.Add(comp);
            comp.BindEntity(this);
        }

        public override void DoAwake(){
            allSkills = ((Transform) (UnityEngine.Object) (engineTransform)).GetComponents<CSkill>().ToList();
            currentHealth = startingHealth;
            foreach (var comp in allComponents) {
                comp.DoAwake();
            }
        }

        public override void DoStart(){
            rigidbody.DoStart();
            foreach (var comp in allComponents) {
                comp.DoStart();
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            foreach (var skill in allSkills) {
                skill.DoUpdate(deltaTime);
            }

            rigidbody.DoUpdate(deltaTime);
            foreach (var comp in allComponents) {
                comp.DoUpdate(deltaTime);
            }
        }

        public override void DoDestroy(){
            foreach (var comp in allComponents) {
                comp.DoDestroy();
            }
        }

        public virtual void TakeDamage(int amount, LVector3 hitPoint){
            if (isInvincible || isDead) return;
            OnTakeDamage(amount, hitPoint);
        }

        protected virtual void OnTakeDamage(int amount, LVector3 hitPoint){ }
        public virtual void OnLPTriggerEnter(ColliderProxy other){ }

        public virtual void OnLPTriggerStay(ColliderProxy other){ }

        public virtual void OnLPTriggerExit(ColliderProxy other){ }
    }

    public interface IManager : ILifeCycle { }

    [Serializable]
    public class BaseManager : MonoBehaviour, IManager {
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }
}