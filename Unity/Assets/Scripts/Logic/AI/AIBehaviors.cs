using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Lockstep.Logic;
using Lockstep.BehaviourTree;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AIToolkitDemo {
    public class AIEntityWorkingData : BTWorkingData {
        public CBrain entity { get; set; }
        public CTransform2D entityTF { get; set; }
        public CAnimatorView EntityAnimatorView { get; set; }
        public LFloat gameTime { get; set; }
        public LFloat deltaTime { get; set; }
    }

    public class AIEntityBehaviorTreeFactory {
        private static Dictionary<int, BTInfo> _id2BtInfo = new Dictionary<int, BTInfo>();
        private static string relPath = "Resources/AI/";
        private static string relResPath = "AI/";

        public static BTInfo GetBTInfo(int id){
            if (_id2BtInfo.TryGetValue(id, out var val)) {
                return val;
            }

            var info = CreateBtInfo(id);
            _id2BtInfo[id] = info;
            return info;
        }

        static BTInfo CreateBtInfo(int id){
            //return LoadBehaviorTreeDemo();
            var textAsset = Resources.Load<TextAsset>(relResPath + id);
            var bytes = textAsset.bytes;
            //var path = PathUtil.GetUnityPath(relPath + id + BTFactory.FileExt);
            //var bytes = File.ReadAllBytes(path);
            var info = BTInfo.Deserialize(bytes, BTFactoryAI.CreateNode);
            return info;
        }

        static BTInfo LoadBehaviorTreeDemo(){
            var info = new BTInfo();
            info.Id = 0;
            info.Description = "asdfasf";
            BTFactory.BeforeCreateNode();
            var bt = Create<BTActionPrioritizedSelector>();
            bt
                .AddChild(Create<BTActionSequence>()
                    .SetPrecondition((BTPreconditionNot) Create<BTPreconditionNot>()
                        .AddChild(Create<CON_HasReachedTarget>()))
                    .AddChild(Create<NOD_TurnTo>())
                    .AddChild(Create<NOD_MoveTo>()))
                .AddChild(Create<BTActionSequence>()
                    .AddChild(Create<NOD_TurnTo>())
                    .AddChild(Create<NOD_Attack>()));
            info.RootNode = bt;
            info.Init();
            return info;
        }

        public static T Create<T>() where T : BTNode, new(){
            return BTFactory.CreateNode<T>();
        }
    }

    [AINode("HasReachedTarget", EBTTypeIdxAI.CON_HasReachedTarget)]
    public partial class CON_HasReachedTarget : BTConditionAI {
        public override bool IsTrue(BTWorkingData wData){
            var thisData = wData.As<AIEntityWorkingData>();
            var targetPos = thisData.entity.GetBBValue(CBrain.BBKEY_NEXTMOVINGPOSITION, LVector2.zero);
            var currentPos = thisData.entityTF.pos;
            return (targetPos - currentPos).sqrMagnitude < 1;
        }
    }

    [AINode("Attack", EBTTypeIdxAI.NOD_Attack)]
    public unsafe partial class NOD_Attack : BTActionLeafAI {
        public LFloat waitTime;

        public override object[] GetRuntimeData(BTWorkingData wData){
            return new object[] {
                *(BTCActionLeaf*) wData.GetContext(_uniqueKey),
                *(UserContextData*) GetUserContextData(wData)
            };
        }

        protected override int MemSize => sizeof(UserContextData) + base.MemSize;
        public override Type DataType => typeof(UserContextData);

        [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
        public unsafe partial struct UserContextData {
            internal LFloat attackingTime;
        }

        protected override void OnEnter(BTWorkingData wData){
            var thisData = wData.As<AIEntityWorkingData>();
            var userData = (UserContextData*) GetUserContextData(wData);
            userData->attackingTime = waitTime;
            thisData.entity.PlayAnimation("Attack");
        }

        protected override int OnExecute(BTWorkingData wData){
            var thisData = wData.As<AIEntityWorkingData>();
            var userData = (UserContextData*) GetUserContextData(wData);
            if (userData->attackingTime > 0) {
                userData->attackingTime -= thisData.deltaTime;
                if (userData->attackingTime <= 0) {
                    thisData.EntityAnimatorView.SetInteger("DeadRnd", Random.Range(0, 3));
                    thisData.entity.PlayAnimation("Dead");
                    //thisData.entity.isDead = true;
                }
            }

            return BTRunningStatus.EXECUTING;
        }
    }

    [AINode("NOD_MoveTo", EBTTypeIdxAI.NOD_MoveTo)]
    public partial class NOD_MoveTo : BTActionLeafAI {
        public string namess = "asdfasdf";

        protected override void OnEnter(BTWorkingData wData){
            var thisData = wData.As<AIEntityWorkingData>();
            if (thisData.entity.isDead) {
                thisData.entity.PlayAnimation("Reborn");
            }
            else {
                thisData.entity.PlayAnimation("Walk");
            }
        }

        protected override int OnExecute(BTWorkingData wData){
            var thisData = wData.As<AIEntityWorkingData>();
            var targetPos = thisData.entity.GetBBValue(CBrain.BBKEY_NEXTMOVINGPOSITION, LVector2.zero);
            var currentPos = thisData.entityTF.pos;
            var distToTarget = (targetPos - currentPos).magnitude;
            if (distToTarget < 1) {
                thisData.entityTF.pos = targetPos;
                return BTRunningStatus.FINISHED;
            }
            else {
                int ret = BTRunningStatus.EXECUTING;
                var toTarget = (targetPos - currentPos).normalized;
                var movingStep = 2 * thisData.deltaTime;
                if (movingStep > distToTarget) {
                    movingStep = distToTarget;
                    ret = BTRunningStatus.FINISHED;
                }

                thisData.entityTF.pos = thisData.entityTF.pos + toTarget * movingStep;
                return ret;
            }
        }
    }

    [AINode("NOD_TurnTo", EBTTypeIdxAI.NOD_TurnTo)]
    public partial class NOD_TurnTo : BTActionLeafAI {
        public string rebornAnimName = "Reborn";

        protected override void OnEnter(BTWorkingData wData){
            AIEntityWorkingData thisData = wData.As<AIEntityWorkingData>();
            if (thisData.entity.isDead) {
                thisData.entity.PlayAnimation("Reborn");
            }
            else {
                thisData.entity.PlayAnimation("Walk");
            }
        }

        protected override int OnExecute(BTWorkingData wData){
            AIEntityWorkingData thisData = wData.As<AIEntityWorkingData>();
            var targetPos = thisData.entity.GetBBValue(CBrain.BBKEY_NEXTMOVINGPOSITION, LVector2.zero);
            var currentPos = thisData.entityTF.pos;
            if ((targetPos - currentPos).sqrMagnitude < 1) {
                return BTRunningStatus.FINISHED;
            }
            else {
                var turnVal = 150 * thisData.deltaTime;
                bool isReachTargetDeg = false;
                var fdeg = CTransform2D.TurnToward(targetPos, currentPos, thisData.entityTF.deg, turnVal,
                    out isReachTargetDeg);
                thisData.entityTF.deg = fdeg;
                if (isReachTargetDeg) {
                    return BTRunningStatus.FINISHED;
                }
            }

            return BTRunningStatus.EXECUTING;
        }
    }
}