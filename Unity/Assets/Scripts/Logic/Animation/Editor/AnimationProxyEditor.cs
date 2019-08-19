using System.Collections.Generic;
using Lockstep.Math;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CAnimation))]
public class AnimationProxyEditor : Editor {
    private CAnimation owner;
    private Animation AnimComp => owner.animComp;
    public List<AnimInfo> animInfos => owner.animInfos;
    private Transform rootTrans => owner.rootTrans;
    private Transform transform => owner.transform;
    public List<string> animNames => owner.animNames;

    public int FrameIdx;
    public float debugTimer = 0;
    private int curIdx = 0;
    public bool isGenAnimInfos = false;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = target as CAnimation;
        if (GUILayout.Button("GenList")) {
            isGenAnimInfos = true;
            OnBeginGen();
            EditorApplication.update += UpdateFunc;
        }

        if (GUILayout.Button("SetTimer")) {
            owner.SetTime(owner.debugTimer);
        }

        if (GUILayout.Button("PE_RadialBlur")) {
            PostEffectManager.StartEffect(EPostEffectType.RadialBlur, owner.shaderDuration, owner.shaderFadeInTime,
                owner.shaderFadeOutTime
                ,new PostEffectRadialBlur.ParamsInfo(owner.shadersSampleDist,owner.shadersSampleStrength));
        }
    }

    void UpdateFunc(){
        owner = target as CAnimation;
        SampleFrame();
        if (!isGenAnimInfos) {
            EditorApplication.update = null;
            SampleFrame();
        }
    }

    public AnimInfo CurAnimInfo {
        get => owner.CurAnimInfo;
        set => owner.CurAnimInfo = value;
    }

    public void OnBeginGen(){
        owner.animInfos.Clear();
        animNames.Clear();
        curIdx = 0;
        var count = AnimComp.GetClipCount();
        foreach (AnimationState state in AnimComp) {
            animNames.Add(state.name);
            animInfos.Add(new AnimInfo());
        }

        NextAnim();
    }

    void NextAnim(){
        owner.CurAnimName = animNames[curIdx];
        CurAnimInfo = animInfos[curIdx];
        CurAnimInfo.name = owner.CurAnimName;
        CurAnimInfo.length = AnimComp[owner.CurAnimName].length.ToLFloat();

        AnimComp.Play(owner.CurAnimName);
        FrameIdx = 0;
        curIdx++;
    }

    public void SampleFrame(){
        owner.animState = AnimComp[owner.CurAnimName];
        owner.animLen = owner.animState.length.ToLFloat();
        FrameIdx++;
        if (FrameIdx * AnimatorConfig.FrameInterval >= owner.animLen) {
            for (int i = 0; i < CurAnimInfo.OffsetCount; i++) {
                CurAnimInfo[i] = CurAnimInfo[i] - CurAnimInfo[0];
            }
#if USING_POS_OFFSET
            var accu = new AnimOffsetInfo();
            for (int i = 0; i < _curAnimInfo.Count; i++) {
                _curAnimInfo[i] = _curAnimInfo[i] - accu;
                accu += _curAnimInfo[i];
            }
#endif
            if (curIdx >= animNames.Count) {
                isGenAnimInfos = false;
                return;
            }

            NextAnim();
        }

        owner.Sample(FrameIdx * AnimatorConfig.FrameInterval);
        var tpos = rootTrans.position - transform.position;
        var tdeg = rootTrans.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        CurAnimInfo.Add(new AnimOffsetInfo() {pos = tpos.ToLVector3(), deg = tdeg.ToLFloat()});
    }
}