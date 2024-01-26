// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ProgressBar" {
Properties {
 _Color ("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _MainTex ("Main Tex (RGBA)", 2D) = "white" { }
 _Progress ("Progress", Range(0.000000,1.000000)) = 0.000000
}
SubShader { 
 Tags { "QUEUE"="Overlay+1" }
 Pass {
  Tags { "QUEUE"="Overlay+1" }
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 23512
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _Progress;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 color_2;
    float4 tmpvar_3;
    tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    color_2 = tmpvar_3;
    color_2.w = (color_2.w * float((in_f.xlv_TEXCOORD0.x<_Progress)));
    tmpvar_1 = (color_2 * _Color);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
}