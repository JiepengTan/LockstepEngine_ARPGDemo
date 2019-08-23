using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    [Serializable]
    public class SkillColliderInfo {
        public LVector2 pos;
        public LVector2 size;
        public LFloat radius;
        public LFloat deg = new LFloat(180);
        public LFloat maxY;

        public bool IsCircle => radius > 0;
    }

    [Serializable]
    public class SkillPart {
        public bool _DebugShow;
        public LFloat startTimer;
        public SkillColliderInfo collider;
        public LVector3 impulseForce;
        public bool needForce;
        public bool isResetForce;

        public LFloat interval;
        public int otherCount;
        public int damage;
        public static LFloat AnimFrameScale = new LFloat(true, 1667);
        [HideInInspector] public LFloat DeadTimer => startTimer + interval * (otherCount + LFloat.half);

        [HideInInspector] public int counter;

        public LFloat NextTriggerTimer(){
            return startTimer + interval * counter;
        }
    }
    
    [Serializable]
    public class SkillInfo  {
        public LFloat CD;
        public LFloat doneDelay;
        public int targetLayer; 
        public List<SkillPart> parts = new List<SkillPart>();
    }
}