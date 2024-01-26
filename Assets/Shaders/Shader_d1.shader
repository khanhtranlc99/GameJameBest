// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Diffuse Intensity" {
Properties {
 _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _BrightnessMult ("Brightness multiplier", Range(0.000000,5.000000)) = 1.000000
}
SubShader { 
 LOD 200
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
  GpuProgramID 14845
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
uniform float4 _MainTex_ST;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _BrightnessMult;
struct appdata_t
{
    float4 vertex :POSITION;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float3 xlv_TEXCOORD3 :TEXCOORD3;
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
    tmpvar_3.w = 1;
    tmpvar_3.xyz = in_v.vertex.xyz;
    float4 v_4;
    v_4.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_4.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_4.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_4.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_5;
    v_5.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_5.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_5.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_5.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_7;
    tmpvar_7 = normalize((((v_4.xyz * in_v.normal.x) + (v_5.xyz * in_v.normal.y)) + (v_6.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_7;
    tmpvar_2 = worldNormal_1;
    float3 normal_8;
    normal_8 = worldNormal_1;
    float4 tmpvar_9;
    tmpvar_9.w = 1;
    tmpvar_9.xyz = float3(normal_8);
    float3 res_10;
    float3 x_11;
    x_11.x = dot(unity_SHAr, tmpvar_9);
    x_11.y = dot(unity_SHAg, tmpvar_9);
    x_11.z = dot(unity_SHAb, tmpvar_9);
    float3 x1_12;
    float4 tmpvar_13;
    tmpvar_13 = (normal_8.xyzz * normal_8.yzzx);
    x1_12.x = dot(unity_SHBr, tmpvar_13);
    x1_12.y = dot(unity_SHBg, tmpvar_13);
    x1_12.z = dot(unity_SHBb, tmpvar_13);
    res_10 = (x_11 + (x1_12 + (unity_SHC.xyz * ((normal_8.x * normal_8.x) - (normal_8.y * normal_8.y)))));
    float _tmp_dvx_449 = max(((1.055 * pow(max(res_10, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    res_10 = float3(_tmp_dvx_449, _tmp_dvx_449, _tmp_dvx_449);
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD3 = max(float3(0, 0, 0), res_10);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float4 c_3;
    float3 tmpvar_4;
    float3 lightDir_5;
    float3 tmpvar_6;
    tmpvar_6 = _WorldSpaceLightPos0.xyz;
    lightDir_5 = tmpvar_6;
    tmpvar_4 = in_f.xlv_TEXCOORD1;
    float3 tmpvar_7;
    float4 tmpvar_8;
    tmpvar_8 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color);
    tmpvar_7 = (tmpvar_8.xyz * _BrightnessMult);
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_5;
    float4 c_9;
    float4 c_10;
    float diff_11;
    float tmpvar_12;
    tmpvar_12 = max(0, dot(tmpvar_4, tmpvar_2));
    diff_11 = tmpvar_12;
    c_10.xyz = float3(((tmpvar_7 * tmpvar_1) * diff_11));
    c_10.w = tmpvar_8.w;
    c_9.w = c_10.w;
    c_9.xyz = (c_10.xyz + (tmpvar_7 * in_f.xlv_TEXCOORD3));
    c_3.xyz = c_9.xyz;
    c_3.w = 1;
    out_f.color = c_3;
    return out_f;
}


ENDCG

}
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Blend One One
  GpuProgramID 123901
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
uniform float4 _MainTex_ST;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _LightTexture0;
uniform float4x4 unity_WorldToLight;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _BrightnessMult;
struct appdata_t
{
    float4 vertex :POSITION;
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
    tmpvar_3.w = 1;
    tmpvar_3.xyz = in_v.vertex.xyz;
    float4 v_4;
    v_4.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_4.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_4.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_4.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_5;
    v_5.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_5.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_5.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_5.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_7;
    tmpvar_7 = normalize((((v_4.xyz * in_v.normal.x) + (v_5.xyz * in_v.normal.y)) + (v_6.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_7;
    tmpvar_2 = worldNormal_1;
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float4 c_3;
    float3 tmpvar_4;
    float3 lightDir_5;
    float3 tmpvar_6;
    tmpvar_6 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD2));
    lightDir_5 = tmpvar_6;
    tmpvar_4 = in_f.xlv_TEXCOORD1;
    float3 tmpvar_7;
    float4 tmpvar_8;
    tmpvar_8 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color);
    tmpvar_7 = (tmpvar_8.xyz * _BrightnessMult);
    float4 tmpvar_9;
    tmpvar_9.w = 1;
    tmpvar_9.xyz = in_f.xlv_TEXCOORD2;
    float3 tmpvar_10;
    tmpvar_10 = mul(unity_WorldToLight, tmpvar_9).xyz;
    float tmpvar_11;
    tmpvar_11 = dot(tmpvar_10, tmpvar_10);
    float tmpvar_12;
    tmpvar_12 = tex2D(_LightTexture0, float2(tmpvar_11, tmpvar_11)).w;
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_5;
    tmpvar_1 = (tmpvar_1 * tmpvar_12);
    float4 c_13;
    float4 c_14;
    float diff_15;
    float tmpvar_16;
    tmpvar_16 = max(0, dot(tmpvar_4, tmpvar_2));
    diff_15 = tmpvar_16;
    c_14.xyz = float3(((tmpvar_7 * tmpvar_1) * diff_15));
    c_14.w = tmpvar_8.w;
    c_13.w = c_14.w;
    c_13.xyz = c_14.xyz;
    c_3.xyz = c_13.xyz;
    c_3.w = 1;
    out_f.color = c_3;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "RenderType"="Opaque" }
  GpuProgramID 149185
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
struct appdata_t
{
    float4 vertex :POSITION;
    float3 normal :NORMAL;
};

struct OUT_Data_Vert
{
    float3 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float3 xlv_TEXCOORD0 :TEXCOORD0;
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
    tmpvar_3.w = 1;
    tmpvar_3.xyz = in_v.vertex.xyz;
    float4 v_4;
    v_4.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_4.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_4.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_4.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_5;
    v_5.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_5.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_5.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_5.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_7;
    tmpvar_7 = normalize((((v_4.xyz * in_v.normal.x) + (v_5.xyz * in_v.normal.y)) + (v_6.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_7;
    tmpvar_2 = worldNormal_1;
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    out_v.xlv_TEXCOORD0 = tmpvar_2;
    out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 res_1;
    float3 tmpvar_2;
    tmpvar_2 = in_f.xlv_TEXCOORD0;
    res_1.xyz = float3(((tmpvar_2 * 0.5) + 0.5));
    res_1.w = 0;
    out_f.color = res_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassFinal" "RenderType"="Opaque" }
  ZWrite Off
  GpuProgramID 241977
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
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
uniform float4 _Color;
uniform float _BrightnessMult;
uniform sampler2D _LightBuffer;
struct appdata_t
{
    float4 vertex :POSITION;
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
    float4 tmpvar_4;
    tmpvar_4.w = 1;
    tmpvar_4.xyz = in_v.vertex.xyz;
    tmpvar_3 = UnityObjectToClipPos(tmpvar_4);
    float4 o_5;
    float4 tmpvar_6;
    tmpvar_6 = (tmpvar_3 * 0.5);
    float2 tmpvar_7;
    tmpvar_7.x = tmpvar_6.x;
    tmpvar_7.y = (tmpvar_6.y * _ProjectionParams.x);
    o_5.xy = (tmpvar_7 + tmpvar_6.w);
    o_5.zw = tmpvar_3.zw;
    tmpvar_1.zw = float2(0, 0);
    tmpvar_1.xy = float2(0, 0);
    float4 v_8;
    v_8.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_8.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_8.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_8.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_9;
    v_9.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_9.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_9.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_9.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_10;
    v_10.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_10.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_10.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_10.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float4 tmpvar_11;
    tmpvar_11.w = 1;
    tmpvar_11.xyz = normalize((((v_8.xyz * in_v.normal.x) + (v_9.xyz * in_v.normal.y)) + (v_10.xyz * in_v.normal.z)));
    float4 normal_12;
    normal_12 = tmpvar_11;
    float3 res_13;
    float3 x_14;
    x_14.x = dot(unity_SHAr, normal_12);
    x_14.y = dot(unity_SHAg, normal_12);
    x_14.z = dot(unity_SHAb, normal_12);
    float3 x1_15;
    float4 tmpvar_16;
    tmpvar_16 = (normal_12.xyzz * normal_12.yzzx);
    x1_15.x = dot(unity_SHBr, tmpvar_16);
    x1_15.y = dot(unity_SHBg, tmpvar_16);
    x1_15.z = dot(unity_SHBb, tmpvar_16);
    res_13 = (x_14 + (x1_15 + (unity_SHC.xyz * ((normal_12.x * normal_12.x) - (normal_12.y * normal_12.y)))));
    float _tmp_dvx_441 = max(((1.055 * pow(max(res_13, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    res_13 = float3(_tmp_dvx_441, _tmp_dvx_441, _tmp_dvx_441);
    tmpvar_2 = res_13;
    out_v.vertex = tmpvar_3;
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD2 = o_5;
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
    float3 tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _Color);
    tmpvar_4 = (tmpvar_5.xyz * _BrightnessMult);
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_LightBuffer, in_f.xlv_TEXCOORD2);
    light_3 = tmpvar_6;
    light_3 = (-log2(max(light_3, float4(0.001, 0.001, 0.001, 0.001))));
    light_3.xyz = (light_3.xyz + in_f.xlv_TEXCOORD4);
    float4 c_7;
    c_7.xyz = (tmpvar_4 * light_3.xyz);
    c_7.w = tmpvar_5.w;
    c_2.xyz = c_7.xyz;
    c_2.w = 1;
    tmpvar_1 = c_2;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
Fallback "Legacy Shaders/VertexLit"
}