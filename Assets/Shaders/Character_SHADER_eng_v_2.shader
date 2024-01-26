// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Character_SHADER_eng" {
Properties {
 _Shininess ("Shininess", Range(0.010000,1.000000)) = 0.500000
 _MainTex ("Diffuse", 2D) = "white" { }
 _BumpMap ("Normal", 2D) = "bump" { }
 _Power ("Power", Range(0.010000,10.000000)) = 0.050000
 _Color ("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _EmTex ("Emission", 2D) = "white" { }
 _SpecularControlTex ("Spec (RGB)", 2D) = "white" { }
}
SubShader { 
 LOD 150
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
  GpuProgramID 27401
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
//uniform float4 unity_WorldTransformParams;
uniform float4 _MainTex_ST;
uniform float4 _EmTex_ST;
//uniform float3 _WorldSpaceCameraPos;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _SpecularControlTex;
uniform float _Shininess;
uniform sampler2D _EmTex;
uniform float _Power;
uniform float4 _Color;
struct appdata_t
{
    float4 tangent :TANGENT;
    float4 vertex :POSITION;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float tangentSign_1;
    float3 worldTangent_2;
    float3 worldNormal_3;
    float4 tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5.w = 1;
    tmpvar_5.xyz = in_v.vertex.xyz;
    tmpvar_4.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _EmTex);
    float3 tmpvar_6;
    tmpvar_6 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_8;
    v_8.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_8.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_8.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_8.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_9;
    v_9.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_9.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_9.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_9.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_10;
    tmpvar_10 = normalize((((v_7.xyz * in_v.normal.x) + (v_8.xyz * in_v.normal.y)) + (v_9.xyz * in_v.normal.z)));
    worldNormal_3 = tmpvar_10;
    float3x3 tmpvar_11;
    tmpvar_11[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_11[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_11[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_12;
    tmpvar_12 = normalize(mul(tmpvar_11, in_v.tangent.xyz));
    worldTangent_2 = tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = (in_v.tangent.w * unity_WorldTransformParams.w);
    tangentSign_1 = tmpvar_13;
    float3 tmpvar_14;
    tmpvar_14 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
    float4 tmpvar_15;
    tmpvar_15.x = worldTangent_2.x;
    tmpvar_15.y = tmpvar_14.x;
    tmpvar_15.z = worldNormal_3.x;
    tmpvar_15.w = tmpvar_6.x;
    float4 tmpvar_16;
    tmpvar_16.x = worldTangent_2.y;
    tmpvar_16.y = tmpvar_14.y;
    tmpvar_16.z = worldNormal_3.y;
    tmpvar_16.w = tmpvar_6.y;
    float4 tmpvar_17;
    tmpvar_17.x = worldTangent_2.z;
    tmpvar_17.y = tmpvar_14.z;
    tmpvar_17.z = worldNormal_3.z;
    tmpvar_17.w = tmpvar_6.z;
    out_v.vertex = UnityObjectToClipPos(tmpvar_5);
    out_v.xlv_TEXCOORD0 = tmpvar_4;
    out_v.xlv_TEXCOORD1 = tmpvar_15;
    out_v.xlv_TEXCOORD2 = tmpvar_16;
    out_v.xlv_TEXCOORD3 = tmpvar_17;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 worldN_1;
    float4 c_2;
    float3 worldViewDir_3;
    float3 lightDir_4;
    float3 tmpvar_5;
    tmpvar_5.x = in_f.xlv_TEXCOORD1.w;
    tmpvar_5.y = in_f.xlv_TEXCOORD2.w;
    tmpvar_5.z = in_f.xlv_TEXCOORD3.w;
    float3 tmpvar_6;
    tmpvar_6 = _WorldSpaceLightPos0.xyz;
    lightDir_4 = tmpvar_6;
    float3 tmpvar_7;
    tmpvar_7 = normalize((_WorldSpaceCameraPos - tmpvar_5));
    worldViewDir_3 = tmpvar_7;
    float3 tmpvar_8;
    tmpvar_8 = normalize((worldViewDir_3 + lightDir_4));
    worldViewDir_3 = tmpvar_8;
    float3 tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
    float4 tmpvar_11;
    tmpvar_11 = tex2D(_SpecularControlTex, in_f.xlv_TEXCOORD0.xy);
    float4 tmpvar_12;
    tmpvar_12 = (tmpvar_10 * _Color);
    float3 tmpvar_13;
    tmpvar_13 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.xy).xyz * 2) - 1);
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_EmTex, in_f.xlv_TEXCOORD0.zw);
    float3 tmpvar_15;
    tmpvar_15 = ((tmpvar_12 * tmpvar_14.w) * _Power).xyz;
    tmpvar_9 = tmpvar_15;
    float tmpvar_16;
    tmpvar_16 = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_13);
    worldN_1.x = tmpvar_16;
    float tmpvar_17;
    tmpvar_17 = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_13);
    worldN_1.y = tmpvar_17;
    float tmpvar_18;
    tmpvar_18 = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_13);
    worldN_1.z = tmpvar_18;
    float4 c_19;
    float spec_20;
    float tmpvar_21;
    tmpvar_21 = max(0, dot(worldN_1, tmpvar_8));
    float tmpvar_22;
    tmpvar_22 = (pow(tmpvar_21, (_Shininess * 128)) * tmpvar_11.w);
    spec_20 = tmpvar_22;
    c_19.xyz = ((((tmpvar_10.xyz * _LightColor0.xyz) * max(0.5, dot(worldN_1, lightDir_4))) + (_LightColor0.xyz * spec_20)) * 2);
    c_19.w = 0;
    c_2.xyz = (c_19.xyz + tmpvar_9);
    c_2.w = 1;
    out_f.color = c_2;
    return out_f;
}


ENDCG

}
}
Fallback "Mobile/VertexLit"
}