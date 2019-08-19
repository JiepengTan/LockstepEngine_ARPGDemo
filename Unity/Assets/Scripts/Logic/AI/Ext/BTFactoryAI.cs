using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using AIToolkitDemo;
using Lockstep.BehaviourTree;
using Lockstep.Serialization;


namespace AIToolkitDemo {
    public abstract class BTConditionAI : BTCondition {
        public override short NodeId => (short) ((int) (EBTTypeIdxAI) Enum.Parse(typeof(EBTTypeIdxAI), GetType().Name));
    }

    public abstract class BTActionLeafAI : BTActionLeaf {
        public override short NodeId => (short) ((int) (EBTTypeIdxAI) Enum.Parse(typeof(EBTTypeIdxAI), GetType().Name));
    }

    public partial class BTFactoryAI {
        public const short IdxOffset = BTFactory.MaxBuildInIdx;
        public static int MaxCount = BTFactory.MaxBuildInIdx + (int) EBTTypeIdxAI.EnumCount;

        private delegate BTNode FuncCreateNode();

        private static FuncCreateNode[] funcsCreate;

        public static BTNode CreateNode(short idx){
            if (funcsCreate == null) Init();
            if (idx > MaxCount || idx < 0) return null;
            return funcsCreate[idx]?.Invoke();
        }

        public static T CreateNode<T>(short idx) where T : BTNode{
            return CreateNode(idx) as T;
        }
    }
}