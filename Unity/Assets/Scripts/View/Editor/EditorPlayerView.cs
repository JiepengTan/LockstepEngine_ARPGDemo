
    using System.Collections.Generic;
    using System.Linq;
    using Lockstep.Logic;
    using Lockstep.Collision2D;
    using Lockstep.Math;
    using UnityEditor;
    using UnityEngine;


    //[CustomEditor(typeof(PlayerView))]
    //public class EditorPlayerView : Editor {
    //    private PlayerView owner;
    //    public LVector3 force;
    //    public LFloat resetYSpd;
    //    public override void OnInspectorGUI(){
    //        base.OnInspectorGUI();
    //        owner = target as PlayerView;
    //        if (GUILayout.Button("CopySkills")) {
    //            var config = owner.skillConfig;
    //            if (config != null) {
    //                var skills = owner.GetComponents<CSkill>();
    //               config.skillInfos = new List<SkillInfo>();
    //               foreach (var skill in skills) {
    //                   var info = new SkillInfo();
    //                   info.parts = skill.parts;
    //                   info.CD = skill.CD;
    //                   info.doneDelay = skill.doneDelay;
    //                   info.targetLayer = skill.targetLayer;
    //                   config.skillInfos.Add(info);
    //               }
    //            }
    //        }
//
    //    }
    //}