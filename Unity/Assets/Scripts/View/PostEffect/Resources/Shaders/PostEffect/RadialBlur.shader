Shader "PostEffect/RadialBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SampleDist ("_SampleDist", Float) = 1.0
		_SampleStrength ("_SampleStrength", Float) = 1.0
	}
	CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		half4 _MainTex_TexelSize;

		sampler2D _BlurTex;
		float _SampleDist;
		float _SampleStrength;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};
		
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;					 
			return o;
		}
		fixed4 fragRadialBlur (v2f i) : COLOR
		{
			fixed2 dir = normalize(0.5-i.uv) * _SampleDist;
			fixed4 
			sum =  tex2D(_MainTex, i.uv - dir*0.01);
			sum += tex2D(_MainTex, i.uv - dir*0.02);
			sum += tex2D(_MainTex, i.uv - dir*0.03);
			sum += tex2D(_MainTex, i.uv - dir*0.05);
			sum += tex2D(_MainTex, i.uv - dir*0.08);
			sum += tex2D(_MainTex, i.uv + dir*0.01);
			sum += tex2D(_MainTex, i.uv + dir*0.02);
			sum += tex2D(_MainTex, i.uv + dir*0.03);
			sum += tex2D(_MainTex, i.uv + dir*0.05);
			sum += tex2D(_MainTex, i.uv + dir*0.08);
			sum *= 0.1;
			
			return sum;
		}
		
		fixed4 fragCombine (v2f i) : COLOR
		{
			fixed dist = length(0.5-i.uv);
			fixed4  col = tex2D(_MainTex, i.uv);
			fixed4  blur = tex2D(_BlurTex, i.uv);
			col=lerp(col, blur,saturate(_SampleStrength*dist));
			return col;
		}
	ENDCG
				
    SubShader
    {   
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
			#pragma vertex vert  
			#pragma fragment fragRadialBlur  
            ENDCG
        }

        Pass
        {
            CGPROGRAM
			#pragma vertex vert  
			#pragma fragment fragCombine  
            ENDCG
        }
	} 
}
