// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Transparent/Cutout/Diffuse Shake CullOff" {
Properties {
 _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
 _Cutoff ("Alpha cutoff", Range(0.000000,1.000000)) = 0.500000
 _ShakeDisplacement ("Displacement", Range(0.000000,1.000000)) = 1.000000
 _ShakeTime ("Shake Time", Range(0.000000,1.000000)) = 1.000000
 _ShakeWindspeed ("Shake Windspeed", Range(0.000000,1.000000)) = 1.000000
 _ShakeBending ("Shake Bending", Range(0.000000,1.000000)) = 1.000000
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="TransparentCutout" }
  Cull Off
  ColorMask RGB
  GpuProgramID 27081
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
//uniform float4 _Time;
//uniform float4 unity_SHBr;
//uniform float4 unity_SHBg;
//uniform float4 unity_SHBb;
//uniform float4 unity_SHC;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
uniform float _ShakeTime;
uniform float _ShakeWindspeed;
uniform float _ShakeBending;
uniform float4 _MainTex_ST;
//uniform float4 _WorldSpaceLightPos0;
//uniform float4 unity_SHAr;
//uniform float4 unity_SHAg;
//uniform float4 unity_SHAb;
uniform float4 _LightColor0;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _Cutoff;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float3 xlv_TEXCOORD3 :TEXCOORD3;
    float4 xlv_TEXCOORD6 :TEXCOORD6;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD3 :TEXCOORD3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    float4 tmpvar_4;
    tmpvar_4.yw = in_v.vertex.yw;
    float3 waveMove_5;
    float4 s_6;
    float4 waves_7;
    waves_7 = (in_v.vertex.x * float4(0.048, 0.06, 0.24, 0.096));
    waves_7 = (waves_7 + (in_v.vertex.z * float4(0.024, 0.08, 0.08, 0.2)));
    waves_7 = (waves_7 + (((_Time.x * ((1 - (_ShakeTime * 2)) - in_v.color.z)) * float4(1.2, 2, 1.6, 4.8)) * (_ShakeWindspeed + in_v.color.y)));
    float4 tmpvar_8;
    tmpvar_8 = frac(waves_7);
    waves_7 = tmpvar_8;
    float4 val_9;
    val_9 = ((tmpvar_8 * 6.408849) - 3.141593);
    float4 tmpvar_10;
    tmpvar_10 = (val_9 * val_9);
    float4 tmpvar_11;
    tmpvar_11 = (tmpvar_10 * val_9);
    float4 tmpvar_12;
    tmpvar_12 = (tmpvar_11 * tmpvar_10);
    s_6 = ((((val_9 + (tmpvar_11 * (-0.1616162))) + (tmpvar_12 * 0.0083333)) + ((tmpvar_12 * tmpvar_10) * (-0.00019841))) * (in_v.texcoord.y * (in_v.color.w + _ShakeBending)));
    s_6 = (s_6 * float4(0.2153874, 0.3589791, 0.2871833, 0.8615498));
    s_6 = (s_6 * s_6);
    s_6 = (s_6 * s_6);
    waveMove_5.y = 0;
    waveMove_5.x = dot(s_6, float4(0.024, 0.04, (-0.12), 0.096));
    waveMove_5.z = dot(s_6, float4(0.006, 0.02, (-0.02), 0.1));
    float3x3 tmpvar_13;
    tmpvar_13[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
    tmpvar_13[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
    tmpvar_13[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
    tmpvar_4.xz = (in_v.vertex.xz - mul(tmpvar_13, waveMove_5).xz);
    float4 tmpvar_14;
    tmpvar_14.w = 1;
    tmpvar_14.xyz = tmpvar_4.xyz;
    float4 v_15;
    v_15.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_15.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_15.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_15.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_16;
    v_16.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_16.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_16.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_16.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_17;
    v_17.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_17.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_17.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_17.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_18;
    tmpvar_18 = normalize((((v_15.xyz * in_v.normal.x) + (v_16.xyz * in_v.normal.y)) + (v_17.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_18;
    tmpvar_2 = worldNormal_1;
    float3 normal_19;
    normal_19 = worldNormal_1;
    float3 x1_20;
    float4 tmpvar_21;
    tmpvar_21 = (normal_19.xyzz * normal_19.yzzx);
    x1_20.x = dot(unity_SHBr, tmpvar_21);
    x1_20.y = dot(unity_SHBg, tmpvar_21);
    x1_20.z = dot(unity_SHBb, tmpvar_21);
    out_v.vertex = UnityObjectToClipPos(tmpvar_14);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, tmpvar_4).xyz;
    out_v.xlv_TEXCOORD3 = (x1_20 + (unity_SHC.xyz * ((normal_19.x * normal_19.x) - (normal_19.y * normal_19.y))));
    out_v.xlv_TEXCOORD6 = tmpvar_3;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 tmpvar_3;
    float3 lightDir_4;
    float3 tmpvar_5;
    tmpvar_5 = _WorldSpaceLightPos0.xyz;
    lightDir_4 = tmpvar_5;
    tmpvar_3 = in_f.xlv_TEXCOORD1;
    float4 tmpvar_6;
    tmpvar_6 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color);
    float x_7;
    x_7 = (tmpvar_6.w - _Cutoff);
    if((x_7<0))
    {
        discard;
    }
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_4;
    float3 normalWorld_8;
    normalWorld_8 = tmpvar_3;
    float3 ambient_9;
    float4 tmpvar_10;
    tmpvar_10.w = 1;
    tmpvar_10.xyz = float3(normalWorld_8);
    float3 x_11;
    x_11.x = dot(unity_SHAr, tmpvar_10);
    x_11.y = dot(unity_SHAg, tmpvar_10);
    x_11.z = dot(unity_SHAb, tmpvar_10);
    ambient_9 = max(((1.055 * pow(max(float3(0, 0, 0), (in_f.xlv_TEXCOORD3 + x_11)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    float4 c_12;
    float4 c_13;
    float diff_14;
    float tmpvar_15;
    tmpvar_15 = max(0, dot(tmpvar_3, tmpvar_2));
    diff_14 = tmpvar_15;
    c_13.xyz = ((tmpvar_6.xyz * tmpvar_1) * diff_14);
    c_13.w = tmpvar_6.w;
    c_12.w = c_13.w;
    c_12.xyz = (c_13.xyz + (tmpvar_6.xyz * ambient_9));
    out_f.color = c_12;
    return out_f;
}


ENDCG

}
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
  ZWrite Off
  Cull Off
  Blend One One
  ColorMask RGB
  GpuProgramID 97627
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
//uniform float4 _Time;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
uniform float _ShakeTime;
uniform float _ShakeWindspeed;
uniform float _ShakeBending;
uniform float4 _MainTex_ST;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _LightTexture0;
uniform float4x4 unity_WorldToLight;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _Cutoff;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.yw = in_v.vertex.yw;
    float3 waveMove_4;
    float4 s_5;
    float4 waves_6;
    waves_6 = (in_v.vertex.x * float4(0.048, 0.06, 0.24, 0.096));
    waves_6 = (waves_6 + (in_v.vertex.z * float4(0.024, 0.08, 0.08, 0.2)));
    waves_6 = (waves_6 + (((_Time.x * ((1 - (_ShakeTime * 2)) - in_v.color.z)) * float4(1.2, 2, 1.6, 4.8)) * (_ShakeWindspeed + in_v.color.y)));
    float4 tmpvar_7;
    tmpvar_7 = frac(waves_6);
    waves_6 = tmpvar_7;
    float4 val_8;
    val_8 = ((tmpvar_7 * 6.408849) - 3.141593);
    float4 tmpvar_9;
    tmpvar_9 = (val_8 * val_8);
    float4 tmpvar_10;
    tmpvar_10 = (tmpvar_9 * val_8);
    float4 tmpvar_11;
    tmpvar_11 = (tmpvar_10 * tmpvar_9);
    s_5 = ((((val_8 + (tmpvar_10 * (-0.1616162))) + (tmpvar_11 * 0.0083333)) + ((tmpvar_11 * tmpvar_9) * (-0.00019841))) * (in_v.texcoord.y * (in_v.color.w + _ShakeBending)));
    s_5 = (s_5 * float4(0.2153874, 0.3589791, 0.2871833, 0.8615498));
    s_5 = (s_5 * s_5);
    s_5 = (s_5 * s_5);
    waveMove_4.y = 0;
    waveMove_4.x = dot(s_5, float4(0.024, 0.04, (-0.12), 0.096));
    waveMove_4.z = dot(s_5, float4(0.006, 0.02, (-0.02), 0.1));
    float3x3 tmpvar_12;
    tmpvar_12[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
    tmpvar_12[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
    tmpvar_12[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
    tmpvar_3.xz = (in_v.vertex.xz - mul(tmpvar_12, waveMove_4).xz);
    float4 tmpvar_13;
    tmpvar_13.w = 1;
    tmpvar_13.xyz = tmpvar_3.xyz;
    float4 v_14;
    v_14.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_14.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_14.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_14.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_15;
    v_15.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_15.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_15.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_15.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_16;
    v_16.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_16.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_16.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_16.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_17;
    tmpvar_17 = normalize((((v_14.xyz * in_v.normal.x) + (v_15.xyz * in_v.normal.y)) + (v_16.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_17;
    tmpvar_2 = worldNormal_1;
    out_v.vertex = UnityObjectToClipPos(tmpvar_13);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, tmpvar_3).xyz;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 tmpvar_3;
    float3 lightDir_4;
    float3 tmpvar_5;
    tmpvar_5 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD2));
    lightDir_4 = tmpvar_5;
    tmpvar_3 = in_f.xlv_TEXCOORD1;
    float4 tmpvar_6;
    tmpvar_6 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color);
    float x_7;
    x_7 = (tmpvar_6.w - _Cutoff);
    if((x_7<0))
    {
        discard;
    }
    float4 tmpvar_8;
    tmpvar_8.w = 1;
    tmpvar_8.xyz = in_f.xlv_TEXCOORD2;
    float3 tmpvar_9;
    tmpvar_9 = mul(unity_WorldToLight, tmpvar_8).xyz;
    float tmpvar_10;
    tmpvar_10 = dot(tmpvar_9, tmpvar_9);
    float tmpvar_11;
    tmpvar_11 = tex2D(_LightTexture0, float2(tmpvar_10, tmpvar_10)).w;
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_4;
    tmpvar_1 = (tmpvar_1 * tmpvar_11);
    float4 c_12;
    float4 c_13;
    float diff_14;
    float tmpvar_15;
    tmpvar_15 = max(0, dot(tmpvar_3, tmpvar_2));
    diff_14 = tmpvar_15;
    c_13.xyz = ((tmpvar_6.xyz * tmpvar_1) * diff_14);
    c_13.w = tmpvar_6.w;
    c_12.w = c_13.w;
    c_12.xyz = c_13.xyz;
    out_f.color = c_12;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
  Cull Off
  GpuProgramID 185056
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
//uniform float4 _Time;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
uniform float _ShakeTime;
uniform float _ShakeWindspeed;
uniform float _ShakeBending;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _Cutoff;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.yw = in_v.vertex.yw;
    float3 waveMove_4;
    float4 s_5;
    float4 waves_6;
    waves_6 = (in_v.vertex.x * float4(0.048, 0.06, 0.24, 0.096));
    waves_6 = (waves_6 + (in_v.vertex.z * float4(0.024, 0.08, 0.08, 0.2)));
    waves_6 = (waves_6 + (((_Time.x * ((1 - (_ShakeTime * 2)) - in_v.color.z)) * float4(1.2, 2, 1.6, 4.8)) * (_ShakeWindspeed + in_v.color.y)));
    float4 tmpvar_7;
    tmpvar_7 = frac(waves_6);
    waves_6 = tmpvar_7;
    float4 val_8;
    val_8 = ((tmpvar_7 * 6.408849) - 3.141593);
    float4 tmpvar_9;
    tmpvar_9 = (val_8 * val_8);
    float4 tmpvar_10;
    tmpvar_10 = (tmpvar_9 * val_8);
    float4 tmpvar_11;
    tmpvar_11 = (tmpvar_10 * tmpvar_9);
    s_5 = ((((val_8 + (tmpvar_10 * (-0.1616162))) + (tmpvar_11 * 0.0083333)) + ((tmpvar_11 * tmpvar_9) * (-0.00019841))) * (in_v.texcoord.y * (in_v.color.w + _ShakeBending)));
    s_5 = (s_5 * float4(0.2153874, 0.3589791, 0.2871833, 0.8615498));
    s_5 = (s_5 * s_5);
    s_5 = (s_5 * s_5);
    waveMove_4.y = 0;
    waveMove_4.x = dot(s_5, float4(0.024, 0.04, (-0.12), 0.096));
    waveMove_4.z = dot(s_5, float4(0.006, 0.02, (-0.02), 0.1));
    float3x3 tmpvar_12;
    tmpvar_12[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
    tmpvar_12[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
    tmpvar_12[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
    tmpvar_3.xz = (in_v.vertex.xz - mul(tmpvar_12, waveMove_4).xz);
    float4 tmpvar_13;
    tmpvar_13.w = 1;
    tmpvar_13.xyz = tmpvar_3.xyz;
    float4 v_14;
    v_14.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_14.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_14.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_14.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_15;
    v_15.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_15.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_15.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_15.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_16;
    v_16.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_16.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_16.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_16.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_17;
    tmpvar_17 = normalize((((v_14.xyz * in_v.normal.x) + (v_15.xyz * in_v.normal.y)) + (v_16.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_17;
    tmpvar_2 = worldNormal_1;
    out_v.vertex = UnityObjectToClipPos(tmpvar_13);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, tmpvar_3).xyz;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 res_1;
    float3 tmpvar_2;
    tmpvar_2 = in_f.xlv_TEXCOORD1;
    float x_3;
    x_3 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color).w - _Cutoff);
    if((x_3<0))
    {
        discard;
    }
    res_1.xyz = float3(((tmpvar_2 * 0.5) + 0.5));
    res_1.w = 0;
    out_f.color = res_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
  ZWrite Off
  Cull Off
  GpuProgramID 245575
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
//uniform float4 _Time;
//uniform float4 _ProjectionParams;
//uniform float4 unity_SHAr;
//uniform float4 unity_SHAg;
//uniform float4 unity_SHAb;
//uniform float4 unity_SHBr;
//uniform float4 unity_SHBg;
//uniform float4 unity_SHBb;
//uniform float4 unity_SHC;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
uniform float _ShakeTime;
uniform float _ShakeWindspeed;
uniform float _ShakeBending;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform sampler2D _LightBuffer;
uniform float _Cutoff;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.yw = in_v.vertex.yw;
    float3 waveMove_4;
    float4 s_5;
    float4 waves_6;
    waves_6 = (in_v.vertex.x * float4(0.048, 0.06, 0.24, 0.096));
    waves_6 = (waves_6 + (in_v.vertex.z * float4(0.024, 0.08, 0.08, 0.2)));
    waves_6 = (waves_6 + (((_Time.x * ((1 - (_ShakeTime * 2)) - in_v.color.z)) * float4(1.2, 2, 1.6, 4.8)) * (_ShakeWindspeed + in_v.color.y)));
    float4 tmpvar_7;
    tmpvar_7 = frac(waves_6);
    waves_6 = tmpvar_7;
    float4 val_8;
    val_8 = ((tmpvar_7 * 6.408849) - 3.141593);
    float4 tmpvar_9;
    tmpvar_9 = (val_8 * val_8);
    float4 tmpvar_10;
    tmpvar_10 = (tmpvar_9 * val_8);
    float4 tmpvar_11;
    tmpvar_11 = (tmpvar_10 * tmpvar_9);
    s_5 = ((((val_8 + (tmpvar_10 * (-0.1616162))) + (tmpvar_11 * 0.0083333)) + ((tmpvar_11 * tmpvar_9) * (-0.00019841))) * (in_v.texcoord.y * (in_v.color.w + _ShakeBending)));
    s_5 = (s_5 * float4(0.2153874, 0.3589791, 0.2871833, 0.8615498));
    s_5 = (s_5 * s_5);
    s_5 = (s_5 * s_5);
    waveMove_4.y = 0;
    waveMove_4.x = dot(s_5, float4(0.024, 0.04, (-0.12), 0.096));
    waveMove_4.z = dot(s_5, float4(0.006, 0.02, (-0.02), 0.1));
    float3x3 tmpvar_12;
    tmpvar_12[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
    tmpvar_12[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
    tmpvar_12[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
    tmpvar_3.xz = (in_v.vertex.xz - mul(tmpvar_12, waveMove_4).xz);
    float4 tmpvar_13;
    float4 tmpvar_14;
    tmpvar_14.w = 1;
    tmpvar_14.xyz = tmpvar_3.xyz;
    tmpvar_13 = UnityObjectToClipPos(tmpvar_14);
    float4 o_15;
    float4 tmpvar_16;
    tmpvar_16 = (tmpvar_13 * 0.5);
    float2 tmpvar_17;
    tmpvar_17.x = tmpvar_16.x;
    tmpvar_17.y = (tmpvar_16.y * _ProjectionParams.x);
    o_15.xy = (tmpvar_17 + tmpvar_16.w);
    o_15.zw = tmpvar_13.zw;
    tmpvar_1.zw = float2(0, 0);
    tmpvar_1.xy = float2(0, 0);
    float4 v_18;
    v_18.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_18.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_18.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_18.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_19;
    v_19.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_19.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_19.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_19.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_20;
    v_20.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_20.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_20.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_20.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float4 tmpvar_21;
    tmpvar_21.w = 1;
    tmpvar_21.xyz = normalize((((v_18.xyz * in_v.normal.x) + (v_19.xyz * in_v.normal.y)) + (v_20.xyz * in_v.normal.z)));
    float4 normal_22;
    normal_22 = tmpvar_21;
    float3 res_23;
    float3 x_24;
    x_24.x = dot(unity_SHAr, normal_22);
    x_24.y = dot(unity_SHAg, normal_22);
    x_24.z = dot(unity_SHAb, normal_22);
    float3 x1_25;
    float4 tmpvar_26;
    tmpvar_26 = (normal_22.xyzz * normal_22.yzzx);
    x1_25.x = dot(unity_SHBr, tmpvar_26);
    x1_25.y = dot(unity_SHBg, tmpvar_26);
    x1_25.z = dot(unity_SHBb, tmpvar_26);
    res_23 = (x_24 + (x1_25 + (unity_SHC.xyz * ((normal_22.x * normal_22.x) - (normal_22.y * normal_22.y)))));
    float _tmp_dvx_523 = max(((1.055 * pow(max(res_23, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    res_23 = float3(_tmp_dvx_523, _tmp_dvx_523, _tmp_dvx_523);
    tmpvar_2 = res_23;
    out_v.vertex = tmpvar_13;
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, tmpvar_3).xyz;
    out_v.xlv_TEXCOORD2 = o_15;
    out_v.xlv_TEXCOORD3 = tmpvar_1;
    out_v.xlv_TEXCOORD4 = tmpvar_2;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 c_2;
    float4 light_3;
    float4 tmpvar_4;
    tmpvar_4 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color);
    float x_5;
    x_5 = (tmpvar_4.w - _Cutoff);
    if((x_5<0))
    {
        discard;
    }
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_LightBuffer, in_f.xlv_TEXCOORD2);
    light_3 = tmpvar_6;
    light_3 = (-log2(max(light_3, float4(0.001, 0.001, 0.001, 0.001))));
    light_3.xyz = (light_3.xyz + in_f.xlv_TEXCOORD4);
    float4 c_7;
    c_7.xyz = (tmpvar_4.xyz * light_3.xyz);
    c_7.w = tmpvar_4.w;
    c_2 = c_7;
    tmpvar_1 = c_2;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "SHADOWCASTER"
  Tags { "LIGHTMODE"="SHADOWCASTER" "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="TransparentCutout" }
  Cull Off
  GpuProgramID 352935
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
//uniform float4 _Time;
//uniform float4 _WorldSpaceLightPos0;
//uniform float4 unity_LightShadowBias;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
//uniform float4x4 unity_MatrixVP;
uniform float _ShakeTime;
uniform float _ShakeWindspeed;
uniform float _ShakeBending;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _Cutoff;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float2 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2.yw = in_v.vertex.yw;
    float3 waveMove_3;
    float4 s_4;
    float4 waves_5;
    waves_5 = (in_v.vertex.x * float4(0.048, 0.06, 0.24, 0.096));
    waves_5 = (waves_5 + (in_v.vertex.z * float4(0.024, 0.08, 0.08, 0.2)));
    waves_5 = (waves_5 + (((_Time.x * ((1 - (_ShakeTime * 2)) - in_v.color.z)) * float4(1.2, 2, 1.6, 4.8)) * (_ShakeWindspeed + in_v.color.y)));
    float4 tmpvar_6;
    tmpvar_6 = frac(waves_5);
    waves_5 = tmpvar_6;
    float4 val_7;
    val_7 = ((tmpvar_6 * 6.408849) - 3.141593);
    float4 tmpvar_8;
    tmpvar_8 = (val_7 * val_7);
    float4 tmpvar_9;
    tmpvar_9 = (tmpvar_8 * val_7);
    float4 tmpvar_10;
    tmpvar_10 = (tmpvar_9 * tmpvar_8);
    s_4 = ((((val_7 + (tmpvar_9 * (-0.1616162))) + (tmpvar_10 * 0.0083333)) + ((tmpvar_10 * tmpvar_8) * (-0.00019841))) * (in_v.texcoord.y * (in_v.color.w + _ShakeBending)));
    s_4 = (s_4 * float4(0.2153874, 0.3589791, 0.2871833, 0.8615498));
    s_4 = (s_4 * s_4);
    s_4 = (s_4 * s_4);
    waveMove_3.y = 0;
    waveMove_3.x = dot(s_4, float4(0.024, 0.04, (-0.12), 0.096));
    waveMove_3.z = dot(s_4, float4(0.006, 0.02, (-0.02), 0.1));
    float3x3 tmpvar_11;
    tmpvar_11[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
    tmpvar_11[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
    tmpvar_11[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
    tmpvar_2.xz = (in_v.vertex.xz - mul(tmpvar_11, waveMove_3).xz);
    tmpvar_1 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    float3 tmpvar_12;
    tmpvar_12 = mul(unity_ObjectToWorld, tmpvar_2).xyz;
    float3 vertex_13;
    vertex_13 = tmpvar_2.xyz;
    float4 clipPos_14;
    if((unity_LightShadowBias.z!=0))
    {
        float4 tmpvar_15;
        tmpvar_15.w = 1;
        tmpvar_15.xyz = float3(vertex_13);
        float3 tmpvar_16;
        tmpvar_16 = mul(unity_ObjectToWorld, tmpvar_15).xyz;
        float4 v_17;
        v_17.x = conv_mxt4x4_0(unity_WorldToObject).x;
        v_17.y = conv_mxt4x4_1(unity_WorldToObject).x;
        v_17.z = conv_mxt4x4_2(unity_WorldToObject).x;
        v_17.w = conv_mxt4x4_3(unity_WorldToObject).x;
        float4 v_18;
        v_18.x = conv_mxt4x4_0(unity_WorldToObject).y;
        v_18.y = conv_mxt4x4_1(unity_WorldToObject).y;
        v_18.z = conv_mxt4x4_2(unity_WorldToObject).y;
        v_18.w = conv_mxt4x4_3(unity_WorldToObject).y;
        float4 v_19;
        v_19.x = conv_mxt4x4_0(unity_WorldToObject).z;
        v_19.y = conv_mxt4x4_1(unity_WorldToObject).z;
        v_19.z = conv_mxt4x4_2(unity_WorldToObject).z;
        v_19.w = conv_mxt4x4_3(unity_WorldToObject).z;
        float3 tmpvar_20;
        tmpvar_20 = normalize((((v_17.xyz * in_v.normal.x) + (v_18.xyz * in_v.normal.y)) + (v_19.xyz * in_v.normal.z)));
        float tmpvar_21;
        tmpvar_21 = dot(tmpvar_20, normalize((_WorldSpaceLightPos0.xyz - (tmpvar_16 * _WorldSpaceLightPos0.w))));
        float4 tmpvar_22;
        tmpvar_22.w = 1;
        tmpvar_22.xyz = (tmpvar_16 - (tmpvar_20 * (unity_LightShadowBias.z * sqrt((1 - (tmpvar_21 * tmpvar_21))))));
        clipPos_14 = mul(unity_MatrixVP, tmpvar_22);
    }
    else
    {
        float4 tmpvar_23;
        tmpvar_23.w = 1;
        tmpvar_23.xyz = float3(vertex_13);
        clipPos_14 = UnityObjectToClipPos(tmpvar_23);
    }
    float4 clipPos_24;
    clipPos_24.xyw = clipPos_14.xyw;
    clipPos_24.z = (clipPos_14.z + clamp((unity_LightShadowBias.x / clipPos_14.w), 0, 1));
    clipPos_24.z = lerp(clipPos_24.z, max(clipPos_24.z, (-clipPos_14.w)), unity_LightShadowBias.y);
    out_v.vertex = clipPos_24;
    out_v.xlv_TEXCOORD1 = tmpvar_1;
    out_v.xlv_TEXCOORD2 = tmpvar_12;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float x_1;
    x_1 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD1) * _Color).w - _Cutoff);
    if((x_1<0))
    {
        discard;
    }
    out_f.color = float4(0, 0, 0, 0);
    return out_f;
}


ENDCG

}
}
Fallback "Transparent/Cutout/VertexLit"
}