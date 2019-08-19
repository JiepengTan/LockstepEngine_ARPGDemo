using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CAnimation))]
public class SkillContainerEditor : Editor {
    private CAnimation owner;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
    }
}