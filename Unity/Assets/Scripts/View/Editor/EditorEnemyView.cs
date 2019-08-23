using Lockstep.Logic;
using Lockstep.Math;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyView))]
public class EditorEnemyView : Editor {
    private EnemyView owner;
    public LVector3 force;
    public LFloat resetYSpd;
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = target as EnemyView;
        force = EditorGUILayoutExt.Vector3Field("force", force);
        if (GUILayout.Button("AddImpulse")) {
            owner.owner.rigidbody.AddImpulse(force);
        }

        resetYSpd = EditorGUILayoutExt.FloatField("resetYSpd", resetYSpd);
        if (GUILayout.Button("ResetSpeed")) {
            owner.owner.rigidbody.ResetSpeed(resetYSpd);
        }
    }
}