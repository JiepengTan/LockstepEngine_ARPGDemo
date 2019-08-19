using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Logic {
    public class SkillContainer : MonoBehaviour {
        public List<CSkill> skills = new List<CSkill>();
        public void Update(){
            for (int i = 0; i < skills.Count; i++) {
                if (Input.GetKey(KeyCode.F1 + i)) {
                    skills[i].Fire();
                }
            }
        }
    }
}