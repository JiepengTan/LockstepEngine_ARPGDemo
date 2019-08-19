using UnityEditor;
using Lockstep.Logic;

namespace AIToolkitDemo.Editor {
    [CustomEditor(typeof(EnemyView))]
    public class EditorAIEntity : UnityEditor.Editor {
        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            //var owner = (target as EnemyView).owner.aiAgent;
            //AIEditor.Instance?.SetRunTimeInfo(owner.BTInfo,owner.BehaviorWorkingData);
        }
    }
}