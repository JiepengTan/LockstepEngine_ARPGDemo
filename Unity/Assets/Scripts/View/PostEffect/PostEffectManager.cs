using System;
using Lockstep.Math;
using UnityEngine;

public enum EPostEffectType {
    RadialBlur,
    MaxCount
}

public static class GameObjectExt {
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component{
        var t = obj.GetComponent<T>();
        if (t != null) return t;
        return obj.AddComponent<T>();
    }
}

public class PostEffectManager : MonoBehaviour {
    public static PostEffectManager Instance { get; private set; }

    private GameObject _mainCamera;
    private IPostEffect[] _allEffects;
    static int MAX_EFFECT_COUNT = (int) EPostEffectType.MaxCount;

    public IPostEffect GetOrCreateEffect(EPostEffectType type){
        var idx = (int) type;
        if (idx < 0 || idx >= MAX_EFFECT_COUNT) {
            Debug.LogError("EPostEffectType out of range" + type.ToString());
            return null;
        }

        IPostEffect comp = null;
        if (_allEffects[idx] == null) {
            switch (type) {
                case EPostEffectType.RadialBlur:
                    comp = _mainCamera.GetOrAddComponent<PostEffectRadialBlur>();
                    break;
                default:
                    break;
            }

            comp.OnInstance();
            if (comp.shader == null) {
                Debug.LogError("Can not find PostEffect shader " + type.ToString());
                return null;
            }

            _allEffects[idx] = comp;
        }

        return _allEffects[idx];
    }

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        DoInit();
    }

    private void Update(){
        DoUpdate(Time.deltaTime);
    }

    public void DoInit(){
        _mainCamera = Camera.main.gameObject;
        _allEffects = new IPostEffect[MAX_EFFECT_COUNT];
    }

    public void DoDestroy(){
        if (_allEffects == null)
            return;
        StopAllEffect();
        var count = _allEffects.Length;
        for (int i = 0; i < count; i++) {
            _allEffects[i] = null;
        }
    }

    public void DoUpdate(float deltaTime){
        if (_allEffects == null)
            return;
        var count = _allEffects.Length;
        for (int i = 0; i < count; i++) {
            var comp = _allEffects[i];
            if (comp != null) {
                comp.DoUpdate(deltaTime);
            }
        }
    }

    public static void StartEffect(EPostEffectType type, float duration = 0.1f, float fadeInTime = 0,
        float fadeOutTime = 0, object param = null){
        Instance?._StartEffect(type,duration,fadeInTime,fadeOutTime,param);
    }

    public static void StopEffect(EPostEffectType type){
        Instance?._StopEffect(type);
    }

    public static void StopAllEffect(){
        Instance?._StopAllEffect();
        
    }

    private void _StartEffect(EPostEffectType type, float duration = 0.1f, float fadeInTime = 0,
        float fadeOutTime = 0, object param = null){
        var comp = GetOrCreateEffect(type);
        if (comp == null) {
            return;
        }

        comp.enabled = true;
        comp.StartEffect(duration, fadeInTime, fadeOutTime, param);
    }

    private void _StopEffect(EPostEffectType type){
        var comp = GetOrCreateEffect(type);
        if (comp == null) {
            return;
        }

        comp.enabled = false;
        comp.StopEffect();
    }

    private void _StopAllEffect(){
        var count = _allEffects.Length;
        for (int i = 0; i < count; i++) {
            if (_allEffects[i] != null) {
                _allEffects[i].StopEffect();
            }
        }
    }
}