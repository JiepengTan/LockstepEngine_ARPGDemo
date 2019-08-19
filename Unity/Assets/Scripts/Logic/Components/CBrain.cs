using AIToolkitDemo;
using Lockstep.BehaviourTree;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    public class CBrain {
        //-----------------------------------------------
        public const string BBKEY_NEXTMOVINGPOSITION = "NextMovingPosition";
        public BaseEntity owner;
        private BTAction _behaviorTree;
        public BTInfo BTInfo { get; protected set; }
        public AIEntityWorkingData BehaviorWorkingData { get; protected set; }

        private GameObject _targetDummyObject;

        private string _lastTriggeredAnimation;
        private BlackBoard _blackboard;

        public bool isDead => owner.isDead;
        public void Init(BaseEntity owner, int id){
            this.owner = owner;

            BTInfo = AIEntityBehaviorTreeFactory.GetBTInfo(id);
            _behaviorTree = BTInfo.RootNode;
            BehaviorWorkingData = new AIEntityWorkingData();
            BehaviorWorkingData.entity = this;
            BehaviorWorkingData.Init(BTInfo.Offsets, BTInfo.MemSize);
            BehaviorWorkingData.entityTF = owner.transform;
            BehaviorWorkingData.EntityAnimatorView = new CAnimatorView().Init(owner);
            _blackboard = new BlackBoard();
            _lastTriggeredAnimation = string.Empty;
        }

        public T GetBBValue<T>(string key, T defaultValue){
            return _blackboard.GetValue<T>(key, defaultValue);
        }

        public void PlayAnimation(string name){
            if (_lastTriggeredAnimation == name) {
                return;
            }

            _lastTriggeredAnimation = name;
            BehaviorWorkingData.EntityAnimatorView.SetTrigger(name);
        }

        public void DoUpdate(LFloat deltaTime){
            //update working data
            BehaviorWorkingData.EntityAnimatorView.speed = (LFloat) 1;
            BehaviorWorkingData.deltaTime = deltaTime;
            BehaviorWorkingData.ClearRunTimeInfo();
            _blackboard.SetValue(BBKEY_NEXTMOVINGPOSITION, GameManager.player.transform.pos);
            if (_behaviorTree.Evaluate(BehaviorWorkingData)) {
                _behaviorTree.Update(BehaviorWorkingData);
            }
            else {
                _behaviorTree.Transition(BehaviorWorkingData);
            }

        }
    }
}