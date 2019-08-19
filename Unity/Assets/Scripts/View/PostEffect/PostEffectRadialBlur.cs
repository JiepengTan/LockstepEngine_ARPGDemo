using UnityEngine;
using System.Collections;

public class PostEffectRadialBlur : PostEffectBase<PostEffectRadialBlur.ParamsInfo>
{

    [System.Serializable]
    public struct ParamsInfo : IPostEffectLerpableVal<ParamsInfo>
    {
        public float SampleDist;
        public float SampleStrength;
        public ParamsInfo(float SampleDist, float SampleStrength)
        {
            this.SampleDist = SampleDist;
            this.SampleStrength = SampleStrength;
        }

        public ParamsInfo Lerp(ParamsInfo src, ParamsInfo dst, float t)
        {
            return new ParamsInfo(
                Mathf.Lerp(src.SampleDist, dst.SampleDist, t),
                Mathf.Lerp(src.SampleStrength, dst.SampleStrength, t)
                );
        }
    }
    /// <summary>
    /// 初始化结束值
    /// </summary>
    protected override void InitStopVal() { stopVal = new ParamsInfo(0, 0); }

    /// <summary>
    /// 创建时候回调
    /// </summary>
    public override void OnInstance(){
        shader = Resources.Load<Shader>("Shaders/PostEffect/RadialBlur");
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var sampleDist = CurVal.SampleDist;
        var sampleStrength = CurVal.SampleStrength;
        if (sampleDist != 0 && sampleStrength != 0 && Material!= null)
        {

            int rtW = src.width / 8;
            int rtH = src.height / 8;

            Material.SetFloat("_SampleDist", sampleDist);
            Material.SetFloat("_SampleStrength", sampleStrength);


            RenderTexture rtTempA = RenderTexture.GetTemporary(rtW, rtH, 0, RenderTextureFormat.Default);
            rtTempA.filterMode = FilterMode.Bilinear;
            Graphics.Blit(src, rtTempA);
            RenderTexture rtTempB = RenderTexture.GetTemporary(rtW, rtH, 0, RenderTextureFormat.Default);
            rtTempB.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rtTempA, rtTempB, Material, 0);
            Material.SetTexture("_BlurTex", rtTempB);
            Graphics.Blit(src, dest, Material, 1);
            RenderTexture.ReleaseTemporary(rtTempA);
            RenderTexture.ReleaseTemporary(rtTempB);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
