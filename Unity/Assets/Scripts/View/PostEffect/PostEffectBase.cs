using UnityEngine;
using System.Collections;

public interface IPostEffectLerpableVal<T> {
    T Lerp(T src, T dst, float t);
}

public interface IPostEffect {
    Shader shader { get; set; }
    bool enabled { get; set; }
    void DoUpdate(float deltaTime);
    void StopEffect();
    void StartEffect(float duration, float fadeinTime = 0, float fadeoutTime = 0, object param = null);
    void OnInstance();
}

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public abstract class PostEffectBase<T> : MonoBehaviour, IPostEffect
    where T : IPostEffectLerpableVal<T> {
    public Shader _shader;

    [SerializeField]
    public Shader shader {
        get => _shader;
        set {
            _shader = value;
            CheckShaderAndCreateMaterial(_shader, _material);
        }
    }

    protected void Start(){
        CheckResources();
    }

    private Material _material = null;

    public Material Material {
        get {
            if (_material == null) {
                _material = CheckShaderAndCreateMaterial(shader, _material);
            }

            return _material;
        }
    }

    protected void CheckResources(){
        if (SystemInfo.supportsImageEffects == false) {
            Debug.LogWarning("This platform does not support image effects.");
            enabled = false;
            return;
        }
    }

    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material){
        if (shader == null) {
            return null;
        }

        if (shader.isSupported && material && material.shader == shader)
            return material;

        if (!shader.isSupported) {
            return null;
        }
        else {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }

    void Awake(){
        InitStopVal();
    }


    protected float fadeInTime;
    protected float fadeOutTime;
    protected float duration;
    protected float timer = 0;

    protected T stopVal;
    protected T srcVal;
    protected T dstVal;
    protected T curVal;
#if UNITY_EDITOR
    [SerializeField] protected T EditorCurVal;
#endif
    protected T CurVal {
        get {
#if UNITY_EDITOR
            return Application.isPlaying ? curVal : EditorCurVal;
#else
            return curVal; 
#endif
        }
    }

    public virtual void DoUpdate(float deltaTime){
        if (!enabled) {
            return;
        }

        timer += deltaTime;
        if (timer < fadeInTime) {
            curVal = LerpParams(srcVal, dstVal, Mathf.Min(timer / fadeInTime, 1));
        }
        else if (timer < (fadeInTime + duration)) {
            curVal = dstVal;
        }
        else {
            var val = timer - (fadeInTime + duration);
            curVal = LerpParams(dstVal, stopVal, Mathf.Min(val / fadeOutTime, 1));
        }

        if (timer >= (fadeInTime + duration + fadeOutTime)) {
            StopEffect();
        }
    }

    public virtual void StartEffect(float duration, float fadeinTime = 0, float fadeoutTime = 0, object param = null){
        srcVal = enabled ? curVal : stopVal;
        dstVal = (T) param;
        enabled = true;
        this.duration = duration;
        fadeInTime = fadeinTime;
        fadeOutTime = fadeoutTime;
        timer = 0;
    }

    public virtual void OnInstance(){ }

    public virtual void StopEffect(){
        enabled = false;
    }

    protected T LerpParams(T src, T dst, float progress){
        return stopVal.Lerp(src, dst, progress);
    }

    protected abstract void InitStopVal();
}