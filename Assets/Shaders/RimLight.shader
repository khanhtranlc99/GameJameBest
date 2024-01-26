// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Example/Rim" {
Properties {
 _MainTex ("Texture", 2D) = "white" { }
 _BumpMap ("Bumpmap", 2D) = "bump" { }
 _RimColor ("Rim Color", Color) = (0.260000,0.190000,0.160000,0.000000)
 _RimPower ("Rim Power", Range(0.500000,8.000000)) = 3.000000
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
  GpuProgramID 23141
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
//uniform float4 unity_WorldTransformParams;
uniform float4 _MainTex_ST;
uniform float4 _BumpMap_ST;
//uniform float3 _WorldSpaceCameraPos;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform float4 _RimColor;
uniform float _RimPower;
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
    float3 xlv_TEXCOORD4 :TEXCOORD4;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
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
    tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
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
    float3 normal_18;
    normal_18 = worldNormal_3;
    float4 tmpvar_19;
    tmpvar_19.w = 1;
    tmpvar_19.xyz = float3(normal_18);
    float3 res_20;
    float3 x_21;
    x_21.x = dot(unity_SHAr, tmpvar_19);
    x_21.y = dot(unity_SHAg, tmpvar_19);
    x_21.z = dot(unity_SHAb, tmpvar_19);
    float3 x1_22;
    float4 tmpvar_23;
    tmpvar_23 = (normal_18.xyzz * normal_18.yzzx);
    x1_22.x = dot(unity_SHBr, tmpvar_23);
    x1_22.y = dot(unity_SHBg, tmpvar_23);
    x1_22.z = dot(unity_SHBb, tmpvar_23);
    res_20 = (x_21 + (x1_22 + (unity_SHC.xyz * ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y)))));
    float _tmp_dvx_475 = max(((1.055 * pow(max(res_20, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    res_20 = float3(_tmp_dvx_475, _tmp_dvx_475, _tmp_dvx_475);
    out_v.vertex = UnityObjectToClipPos(tmpvar_5);
    out_v.xlv_TEXCOORD0 = tmpvar_4;
    out_v.xlv_TEXCOORD1 = tmpvar_15;
    out_v.xlv_TEXCOORD2 = tmpvar_16;
    out_v.xlv_TEXCOORD3 = tmpvar_17;
    out_v.xlv_TEXCOORD4 = max(float3(0, 0, 0), res_20);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 worldN_3;
    float4 c_4;
    float3 viewDir_5;
    float3 worldViewDir_6;
    float3 lightDir_7;
    float3 tmpvar_8;
    float3 tmpvar_9;
    tmpvar_9.x = in_f.xlv_TEXCOORD1.w;
    tmpvar_9.y = in_f.xlv_TEXCOORD2.w;
    tmpvar_9.z = in_f.xlv_TEXCOORD3.w;
    float3 tmpvar_10;
    tmpvar_10 = _WorldSpaceLightPos0.xyz;
    lightDir_7 = tmpvar_10;
    float3 tmpvar_11;
    tmpvar_11 = normalize((_WorldSpaceCameraPos - tmpvar_9));
    worldViewDir_6 = tmpvar_11;
    float3 tmpvar_12;
    tmpvar_12 = (((in_f.xlv_TEXCOORD1.xyz * worldViewDir_6.x) + (in_f.xlv_TEXCOORD2.xyz * worldViewDir_6.y)) + (in_f.xlv_TEXCOORD3.xyz * worldViewDir_6.z));
    viewDir_5 = tmpvar_12;
    tmpvar_8 = viewDir_5;
    float3 tmpvar_13;
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
    float3 tmpvar_15;
    tmpvar_15 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
    float tmpvar_16;
    tmpvar_16 = clamp(dot(normalize(tmpvar_8), tmpvar_15), 0, 1);
    float tmpvar_17;
    tmpvar_17 = (1 - tmpvar_16);
    float tmpvar_18;
    tmpvar_18 = pow(tmpvar_17, _RimPower);
    tmpvar_13 = (_RimColor.xyz * tmpvar_18);
    float tmpvar_19;
    tmpvar_19 = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_15);
    worldN_3.x = tmpvar_19;
    float tmpvar_20;
    tmpvar_20 = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_15);
    worldN_3.y = tmpvar_20;
    float tmpvar_21;
    tmpvar_21 = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_15);
    worldN_3.z = tmpvar_21;
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_7;
    float4 c_22;
    float4 c_23;
    float diff_24;
    float tmpvar_25;
    tmpvar_25 = max(0, dot(worldN_3, tmpvar_2));
    diff_24 = tmpvar_25;
    c_23.xyz = ((tmpvar_14.xyz * tmpvar_1) * diff_24);
    c_23.w = 0;
    c_22.w = c_23.w;
    c_22.xyz = (c_23.xyz + (tmpvar_14.xyz * in_f.xlv_TEXCOORD4));
    c_4.xyz = (c_22.xyz + tmpvar_13);
    c_4.w = 1;
    out_f.color = c_4;
    return out_f;
}


ENDCG

}
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Blend One One
  GpuProgramID 99502
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
uniform float4 _BumpMap_ST;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _LightTexture0;
uniform float4x4 unity_WorldToLight;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
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
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float3 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float3 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
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
    tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_8;
    v_8.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_8.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_8.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_8.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_9;
    tmpvar_9 = normalize((((v_6.xyz * in_v.normal.x) + (v_7.xyz * in_v.normal.y)) + (v_8.xyz * in_v.normal.z)));
    worldNormal_3 = tmpvar_9;
    float3x3 tmpvar_10;
    tmpvar_10[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_10[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_10[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_11;
    tmpvar_11 = normalize(mul(tmpvar_10, in_v.tangent.xyz));
    worldTangent_2 = tmpvar_11;
    float tmpvar_12;
    tmpvar_12 = (in_v.tangent.w * unity_WorldTransformParams.w);
    tangentSign_1 = tmpvar_12;
    float3 tmpvar_13;
    tmpvar_13 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
    float3 tmpvar_14;
    tmpvar_14.x = worldTangent_2.x;
    tmpvar_14.y = tmpvar_13.x;
    tmpvar_14.z = worldNormal_3.x;
    float3 tmpvar_15;
    tmpvar_15.x = worldTangent_2.y;
    tmpvar_15.y = tmpvar_13.y;
    tmpvar_15.z = worldNormal_3.y;
    float3 tmpvar_16;
    tmpvar_16.x = worldTangent_2.z;
    tmpvar_16.y = tmpvar_13.z;
    tmpvar_16.z = worldNormal_3.z;
    out_v.vertex = UnityObjectToClipPos(tmpvar_5);
    out_v.xlv_TEXCOORD0 = tmpvar_4;
    out_v.xlv_TEXCOORD1 = tmpvar_14;
    out_v.xlv_TEXCOORD2 = tmpvar_15;
    out_v.xlv_TEXCOORD3 = tmpvar_16;
    out_v.xlv_TEXCOORD4 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 worldN_3;
    float4 c_4;
    float3 lightDir_5;
    float3 tmpvar_6;
    tmpvar_6 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD4));
    lightDir_5 = tmpvar_6;
    float3 tmpvar_7;
    tmpvar_7 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
    float4 tmpvar_8;
    tmpvar_8.w = 1;
    tmpvar_8.xyz = in_f.xlv_TEXCOORD4;
    float3 tmpvar_9;
    tmpvar_9 = mul(unity_WorldToLight, tmpvar_8).xyz;
    float tmpvar_10;
    tmpvar_10 = dot(tmpvar_9, tmpvar_9);
    float tmpvar_11;
    tmpvar_11 = tex2D(_LightTexture0, float2(tmpvar_10, tmpvar_10)).w;
    worldN_3.x = dot(in_f.xlv_TEXCOORD1, tmpvar_7);
    worldN_3.y = dot(in_f.xlv_TEXCOORD2, tmpvar_7);
    worldN_3.z = dot(in_f.xlv_TEXCOORD3, tmpvar_7);
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_5;
    tmpvar_1 = (tmpvar_1 * tmpvar_11);
    float4 c_12;
    float4 c_13;
    float diff_14;
    float tmpvar_15;
    tmpvar_15 = max(0, dot(worldN_3, tmpvar_2));
    diff_14 = tmpvar_15;
    c_13.xyz = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz * tmpvar_1) * diff_14);
    c_13.w = 0;
    c_12.w = c_13.w;
    c_12.xyz = c_13.xyz;
    c_4.xyz = c_12.xyz;
    c_4.w = 1;
    out_f.color = c_4;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "RenderType"="Opaque" }
  GpuProgramID 150635
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
uniform float4 _BumpMap_ST;
uniform sampler2D _BumpMap;
struct appdata_t
{
    float4 tangent :TANGENT;
    float4 vertex :POSITION;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
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
    tmpvar_4.w = 1;
    tmpvar_4.xyz = in_v.vertex.xyz;
    float3 tmpvar_5;
    tmpvar_5 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_8;
    v_8.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_8.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_8.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_8.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_9;
    tmpvar_9 = normalize((((v_6.xyz * in_v.normal.x) + (v_7.xyz * in_v.normal.y)) + (v_8.xyz * in_v.normal.z)));
    worldNormal_3 = tmpvar_9;
    float3x3 tmpvar_10;
    tmpvar_10[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_10[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_10[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_11;
    tmpvar_11 = normalize(mul(tmpvar_10, in_v.tangent.xyz));
    worldTangent_2 = tmpvar_11;
    float tmpvar_12;
    tmpvar_12 = (in_v.tangent.w * unity_WorldTransformParams.w);
    tangentSign_1 = tmpvar_12;
    float3 tmpvar_13;
    tmpvar_13 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
    float4 tmpvar_14;
    tmpvar_14.x = worldTangent_2.x;
    tmpvar_14.y = tmpvar_13.x;
    tmpvar_14.z = worldNormal_3.x;
    tmpvar_14.w = tmpvar_5.x;
    float4 tmpvar_15;
    tmpvar_15.x = worldTangent_2.y;
    tmpvar_15.y = tmpvar_13.y;
    tmpvar_15.z = worldNormal_3.y;
    tmpvar_15.w = tmpvar_5.y;
    float4 tmpvar_16;
    tmpvar_16.x = worldTangent_2.z;
    tmpvar_16.y = tmpvar_13.z;
    tmpvar_16.z = worldNormal_3.z;
    tmpvar_16.w = tmpvar_5.z;
    out_v.vertex = UnityObjectToClipPos(tmpvar_4);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
    out_v.xlv_TEXCOORD1 = tmpvar_14;
    out_v.xlv_TEXCOORD2 = tmpvar_15;
    out_v.xlv_TEXCOORD3 = tmpvar_16;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 res_1;
    float3 worldN_2;
    float3 tmpvar_3;
    tmpvar_3 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0).xyz * 2) - 1);
    float tmpvar_4;
    tmpvar_4 = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_3);
    worldN_2.x = tmpvar_4;
    float tmpvar_5;
    tmpvar_5 = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_3);
    worldN_2.y = tmpvar_5;
    float tmpvar_6;
    tmpvar_6 = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_3);
    worldN_2.z = tmpvar_6;
    res_1.xyz = float3(((worldN_2 * 0.5) + 0.5));
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
  GpuProgramID 203054
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
//uniform float3 _WorldSpaceCameraPos;
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
//uniform float4 unity_WorldTransformParams;
uniform float4 _MainTex_ST;
uniform float4 _BumpMap_ST;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform float4 _RimColor;
uniform float _RimPower;
uniform sampler2D _LightBuffer;
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
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float4 xlv_TEXCOORD4 :TEXCOORD4;
    float3 xlv_TEXCOORD5 :TEXCOORD5;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD5 :TEXCOORD5;
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
    float3 tmpvar_5;
    float4 tmpvar_6;
    float3 tmpvar_7;
    float4 tmpvar_8;
    float4 tmpvar_9;
    tmpvar_9.w = 1;
    tmpvar_9.xyz = in_v.vertex.xyz;
    tmpvar_8 = UnityObjectToClipPos(tmpvar_9);
    tmpvar_4.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    tmpvar_4.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
    float3 tmpvar_10;
    tmpvar_10 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    float4 v_11;
    float tmpvar_12;
    tmpvar_12 = conv_mxt4x4_0(unity_WorldToObject).x;
    v_11.x = tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = conv_mxt4x4_1(unity_WorldToObject).x;
    v_11.y = tmpvar_13;
    float tmpvar_14;
    tmpvar_14 = conv_mxt4x4_2(unity_WorldToObject).x;
    v_11.z = tmpvar_14;
    float tmpvar_15;
    tmpvar_15 = conv_mxt4x4_3(unity_WorldToObject).x;
    v_11.w = tmpvar_15;
    float4 v_16;
    float tmpvar_17;
    tmpvar_17 = conv_mxt4x4_0(unity_WorldToObject).y;
    v_16.x = tmpvar_17;
    float tmpvar_18;
    tmpvar_18 = conv_mxt4x4_1(unity_WorldToObject).y;
    v_16.y = tmpvar_18;
    float tmpvar_19;
    tmpvar_19 = conv_mxt4x4_2(unity_WorldToObject).y;
    v_16.z = tmpvar_19;
    float tmpvar_20;
    tmpvar_20 = conv_mxt4x4_3(unity_WorldToObject).y;
    v_16.w = tmpvar_20;
    float4 v_21;
    float tmpvar_22;
    tmpvar_22 = conv_mxt4x4_0(unity_WorldToObject).z;
    v_21.x = tmpvar_22;
    float tmpvar_23;
    tmpvar_23 = conv_mxt4x4_1(unity_WorldToObject).z;
    v_21.y = tmpvar_23;
    float tmpvar_24;
    tmpvar_24 = conv_mxt4x4_2(unity_WorldToObject).z;
    v_21.z = tmpvar_24;
    float tmpvar_25;
    tmpvar_25 = conv_mxt4x4_3(unity_WorldToObject).z;
    v_21.w = tmpvar_25;
    float3 tmpvar_26;
    tmpvar_26 = normalize((((v_11.xyz * in_v.normal.x) + (v_16.xyz * in_v.normal.y)) + (v_21.xyz * in_v.normal.z)));
    worldNormal_3 = tmpvar_26;
    float3x3 tmpvar_27;
    tmpvar_27[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_27[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_27[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_28;
    tmpvar_28 = normalize(mul(tmpvar_27, in_v.tangent.xyz));
    worldTangent_2 = tmpvar_28;
    float tmpvar_29;
    tmpvar_29 = (in_v.tangent.w * unity_WorldTransformParams.w);
    tangentSign_1 = tmpvar_29;
    float3 tmpvar_30;
    tmpvar_30 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
    float3 tmpvar_31;
    tmpvar_31 = (_WorldSpaceCameraPos - tmpvar_10);
    float tmpvar_32;
    tmpvar_32 = dot(tmpvar_31, worldTangent_2);
    tmpvar_5.x = tmpvar_32;
    float tmpvar_33;
    tmpvar_33 = dot(tmpvar_31, tmpvar_30);
    tmpvar_5.y = tmpvar_33;
    float tmpvar_34;
    tmpvar_34 = dot(tmpvar_31, worldNormal_3);
    tmpvar_5.z = tmpvar_34;
    float4 o_35;
    float4 tmpvar_36;
    tmpvar_36 = (tmpvar_8 * 0.5);
    float2 tmpvar_37;
    tmpvar_37.x = tmpvar_36.x;
    tmpvar_37.y = (tmpvar_36.y * _ProjectionParams.x);
    o_35.xy = (tmpvar_37 + tmpvar_36.w);
    o_35.zw = tmpvar_8.zw;
    tmpvar_6.zw = float2(0, 0);
    tmpvar_6.xy = float2(0, 0);
    float4 v_38;
    v_38.x = tmpvar_12;
    v_38.y = tmpvar_13;
    v_38.z = tmpvar_14;
    v_38.w = tmpvar_15;
    float4 v_39;
    v_39.x = tmpvar_17;
    v_39.y = tmpvar_18;
    v_39.z = tmpvar_19;
    v_39.w = tmpvar_20;
    float4 v_40;
    v_40.x = tmpvar_22;
    v_40.y = tmpvar_23;
    v_40.z = tmpvar_24;
    v_40.w = tmpvar_25;
    float4 tmpvar_41;
    tmpvar_41.w = 1;
    tmpvar_41.xyz = normalize((((v_38.xyz * in_v.normal.x) + (v_39.xyz * in_v.normal.y)) + (v_40.xyz * in_v.normal.z)));
    float4 normal_42;
    normal_42 = tmpvar_41;
    float3 res_43;
    float3 x_44;
    x_44.x = dot(unity_SHAr, normal_42);
    x_44.y = dot(unity_SHAg, normal_42);
    x_44.z = dot(unity_SHAb, normal_42);
    float3 x1_45;
    float4 tmpvar_46;
    tmpvar_46 = (normal_42.xyzz * normal_42.yzzx);
    x1_45.x = dot(unity_SHBr, tmpvar_46);
    x1_45.y = dot(unity_SHBg, tmpvar_46);
    x1_45.z = dot(unity_SHBb, tmpvar_46);
    res_43 = (x_44 + (x1_45 + (unity_SHC.xyz * ((normal_42.x * normal_42.x) - (normal_42.y * normal_42.y)))));
    float _tmp_dvx_467 = max(((1.055 * pow(max(res_43, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    res_43 = float3(_tmp_dvx_467, _tmp_dvx_467, _tmp_dvx_467);
    tmpvar_7 = res_43;
    out_v.vertex = tmpvar_8;
    out_v.xlv_TEXCOORD0 = tmpvar_4;
    out_v.xlv_TEXCOORD1 = tmpvar_10;
    out_v.xlv_TEXCOORD2 = tmpvar_5;
    out_v.xlv_TEXCOORD3 = o_35;
    out_v.xlv_TEXCOORD4 = tmpvar_6;
    out_v.xlv_TEXCOORD5 = tmpvar_7;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 c_2;
    float4 light_3;
    float3 viewDir_4;
    float3 tmpvar_5;
    float3 tmpvar_6;
    tmpvar_6 = normalize(in_f.xlv_TEXCOORD2);
    viewDir_4 = tmpvar_6;
    tmpvar_5 = viewDir_4;
    float3 tmpvar_7;
    float3 tmpvar_8;
    tmpvar_8 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
    float tmpvar_9;
    tmpvar_9 = clamp(dot(normalize(tmpvar_5), tmpvar_8), 0, 1);
    float tmpvar_10;
    tmpvar_10 = (1 - tmpvar_9);
    float tmpvar_11;
    tmpvar_11 = pow(tmpvar_10, _RimPower);
    tmpvar_7 = (_RimColor.xyz * tmpvar_11);
    float4 tmpvar_12;
    tmpvar_12 = tex2D(_LightBuffer, in_f.xlv_TEXCOORD3);
    light_3 = tmpvar_12;
    light_3 = (-log2(max(light_3, float4(0.001, 0.001, 0.001, 0.001))));
    light_3.xyz = (light_3.xyz + in_f.xlv_TEXCOORD5);
    float4 c_13;
    c_13.xyz = (tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).xyz * light_3.xyz);
    c_13.w = 0;
    c_2 = c_13;
    c_2.xyz = (c_2.xyz + tmpvar_7);
    c_2.w = 1;
    tmpvar_1 = c_2;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
Fallback "Diffuse"
}