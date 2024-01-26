// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Miami/Transparent/MobileWater_AlphaBumpSpec" {
Properties {
 _Color ("Color Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
 _SpecColor ("Specular Color", Color) = (0.500000,0.500000,0.500000,0.000000)
 _Shininess ("Shininess", Range(0.010000,1.000000)) = 0.078125
 _MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" { }
 _BumpMap ("Normalmap", 2D) = "bump" { }
 _Shift ("Shift", Range(0.000000,10.000000)) = 0.000000
 _Delta ("Delta", Range(0.000000,10.000000)) = 1.000000
 _SpeedShift ("Speed Shift", Range(0.000000,10.000000)) = 1.000000
 _SpeedOffset ("Speed Offset", Range(0.000000,10.000000)) = 1.000000
 _CubemapScale ("CubemapScale", Range(0.000000,100.000000)) = 1.000000
 _ReflectColor ("Reflection Color", Color) = (1.000000,1.000000,1.000000,0.500000)
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" { }
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  GpuProgramID 20608
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
//uniform float4 _SinTime;
//uniform float4 unity_SHBr;
//uniform float4 unity_SHBg;
//uniform float4 unity_SHBb;
//uniform float4 unity_SHC;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
//uniform float4 unity_WorldTransformParams;
uniform float _Shift;
uniform float _Delta;
uniform float _SpeedShift;
uniform float _SpeedOffset;
uniform float4 _MainTex_ST;
uniform float4 _BumpMap_ST;
//uniform float3 _WorldSpaceCameraPos;
//uniform float4 _WorldSpaceLightPos0;
//uniform float4 unity_SHAr;
//uniform float4 unity_SHAg;
//uniform float4 unity_SHAb;
uniform float4 _LightColor0;
uniform samplerCUBE _Cube;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform float4 _Color;
uniform float4 _ReflectColor;
uniform float _CubemapScale;
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
    float4 xlv_TEXCOORD6 :TEXCOORD6;
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
    float4 tmpvar_6;
    float4 tmpvar_7;
    tmpvar_6.xzw = in_v.vertex.xzw;
    tmpvar_7.zw = in_v.texcoord.zw;
    tmpvar_6.y = (in_v.vertex.y + (_Shift * (_SinTime.y * _SpeedShift)));
    float tmpvar_8;
    tmpvar_8 = (_SinTime.y * _SpeedOffset);
    tmpvar_7.x = (in_v.texcoord.x + ((_Delta * sin(in_v.vertex.x)) * tmpvar_8));
    tmpvar_7.y = (in_v.texcoord.y + ((_Delta * sin(in_v.vertex.z)) * tmpvar_8));
    float4 tmpvar_9;
    tmpvar_9.w = 1;
    tmpvar_9.xyz = tmpvar_6.xyz;
    tmpvar_4.xy = TRANSFORM_TEX(tmpvar_7.xy, _MainTex);
    tmpvar_4.zw = TRANSFORM_TEX(tmpvar_7.xy, _BumpMap);
    float3 tmpvar_10;
    tmpvar_10 = mul(unity_ObjectToWorld, tmpvar_6).xyz;
    float4 v_11;
    v_11.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_11.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_11.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_11.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_12;
    v_12.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_12.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_12.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_12.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_13;
    v_13.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_13.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_13.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_13.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_14;
    tmpvar_14 = normalize((((v_11.xyz * in_v.normal.x) + (v_12.xyz * in_v.normal.y)) + (v_13.xyz * in_v.normal.z)));
    worldNormal_3 = tmpvar_14;
    float3x3 tmpvar_15;
    tmpvar_15[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_15[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_15[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_16;
    tmpvar_16 = normalize(mul(tmpvar_15, in_v.tangent.xyz));
    worldTangent_2 = tmpvar_16;
    float tmpvar_17;
    tmpvar_17 = (in_v.tangent.w * unity_WorldTransformParams.w);
    tangentSign_1 = tmpvar_17;
    float3 tmpvar_18;
    tmpvar_18 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
    float4 tmpvar_19;
    tmpvar_19.x = worldTangent_2.x;
    tmpvar_19.y = tmpvar_18.x;
    tmpvar_19.z = worldNormal_3.x;
    tmpvar_19.w = tmpvar_10.x;
    float4 tmpvar_20;
    tmpvar_20.x = worldTangent_2.y;
    tmpvar_20.y = tmpvar_18.y;
    tmpvar_20.z = worldNormal_3.y;
    tmpvar_20.w = tmpvar_10.y;
    float4 tmpvar_21;
    tmpvar_21.x = worldTangent_2.z;
    tmpvar_21.y = tmpvar_18.z;
    tmpvar_21.z = worldNormal_3.z;
    tmpvar_21.w = tmpvar_10.z;
    float3 normal_22;
    normal_22 = worldNormal_3;
    float3 x1_23;
    float4 tmpvar_24;
    tmpvar_24 = (normal_22.xyzz * normal_22.yzzx);
    x1_23.x = dot(unity_SHBr, tmpvar_24);
    x1_23.y = dot(unity_SHBg, tmpvar_24);
    x1_23.z = dot(unity_SHBb, tmpvar_24);
    out_v.vertex = UnityObjectToClipPos(tmpvar_9);
    out_v.xlv_TEXCOORD0 = tmpvar_4;
    out_v.xlv_TEXCOORD1 = tmpvar_19;
    out_v.xlv_TEXCOORD2 = tmpvar_20;
    out_v.xlv_TEXCOORD3 = tmpvar_21;
    out_v.xlv_TEXCOORD4 = (x1_23 + (unity_SHC.xyz * ((normal_22.x * normal_22.x) - (normal_22.y * normal_22.y))));
    out_v.xlv_TEXCOORD6 = tmpvar_5;
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
    float3 worldViewDir_5;
    float3 lightDir_6;
    float3 tmpvar_7;
    float3 tmpvar_8;
    tmpvar_8.x = in_f.xlv_TEXCOORD1.w;
    tmpvar_8.y = in_f.xlv_TEXCOORD2.w;
    tmpvar_8.z = in_f.xlv_TEXCOORD3.w;
    float3 tmpvar_9;
    tmpvar_9 = _WorldSpaceLightPos0.xyz;
    lightDir_6 = tmpvar_9;
    float3 tmpvar_10;
    tmpvar_10 = normalize((_WorldSpaceCameraPos - tmpvar_8));
    worldViewDir_5 = tmpvar_10;
    tmpvar_7 = (-worldViewDir_5);
    float3 tmpvar_11;
    float3 tmpvar_12;
    float4 c_13;
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
    c_13 = tmpvar_14;
    tmpvar_11 = (c_13.xyz * _Color.xyz);
    float4 tmpvar_15;
    float _tmp_dvx_483 = texCUBE(_Cube, tmpvar_7);
    tmpvar_15 = float4(_tmp_dvx_483, _tmp_dvx_483, _tmp_dvx_483, _tmp_dvx_483);
    tmpvar_12 = ((tmpvar_15.xyz * _ReflectColor.xyz) * _CubemapScale);
    float3 tmpvar_16;
    tmpvar_16 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
    float tmpvar_17;
    tmpvar_17 = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_16);
    worldN_3.x = tmpvar_17;
    float tmpvar_18;
    tmpvar_18 = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_16);
    worldN_3.y = tmpvar_18;
    float tmpvar_19;
    tmpvar_19 = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_16);
    worldN_3.z = tmpvar_19;
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_6;
    float3 normalWorld_20;
    normalWorld_20 = worldN_3;
    float3 ambient_21;
    float4 tmpvar_22;
    tmpvar_22.w = 1;
    tmpvar_22.xyz = float3(normalWorld_20);
    float3 x_23;
    x_23.x = dot(unity_SHAr, tmpvar_22);
    x_23.y = dot(unity_SHAg, tmpvar_22);
    x_23.z = dot(unity_SHAb, tmpvar_22);
    ambient_21 = max(((1.055 * pow(max(float3(0, 0, 0), (in_f.xlv_TEXCOORD4 + x_23)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    float4 c_24;
    float4 c_25;
    float diff_26;
    float tmpvar_27;
    tmpvar_27 = max(0, dot(worldN_3, tmpvar_2));
    diff_26 = tmpvar_27;
    c_25.xyz = float3(((tmpvar_11 * tmpvar_1) * diff_26));
    c_25.w = (tmpvar_15.w * _ReflectColor.w);
    c_24.w = c_25.w;
    c_24.xyz = (c_25.xyz + (tmpvar_11 * ambient_21));
    c_4.w = c_24.w;
    c_4.xyz = (c_24.xyz + tmpvar_12);
    out_f.color = c_4;
    return out_f;
}


ENDCG

}
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha One
  ColorMask RGB
  GpuProgramID 111576
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
//uniform float4 _SinTime;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
//uniform float4 unity_WorldTransformParams;
uniform float _Shift;
uniform float _Delta;
uniform float _SpeedShift;
uniform float _SpeedOffset;
uniform float4 _MainTex_ST;
uniform float4 _BumpMap_ST;
//uniform float3 _WorldSpaceCameraPos;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _LightTexture0;
uniform float4x4 unity_WorldToLight;
uniform samplerCUBE _Cube;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform float4 _Color;
uniform float4 _ReflectColor;
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
    float4 tmpvar_6;
    tmpvar_5.xzw = in_v.vertex.xzw;
    tmpvar_6.zw = in_v.texcoord.zw;
    tmpvar_5.y = (in_v.vertex.y + (_Shift * (_SinTime.y * _SpeedShift)));
    float tmpvar_7;
    tmpvar_7 = (_SinTime.y * _SpeedOffset);
    tmpvar_6.x = (in_v.texcoord.x + ((_Delta * sin(in_v.vertex.x)) * tmpvar_7));
    tmpvar_6.y = (in_v.texcoord.y + ((_Delta * sin(in_v.vertex.z)) * tmpvar_7));
    float4 tmpvar_8;
    tmpvar_8.w = 1;
    tmpvar_8.xyz = tmpvar_5.xyz;
    tmpvar_4.xy = TRANSFORM_TEX(tmpvar_6.xy, _MainTex);
    tmpvar_4.zw = TRANSFORM_TEX(tmpvar_6.xy, _BumpMap);
    float4 v_9;
    v_9.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_9.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_9.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_9.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_10;
    v_10.x = conv_mxt4x4_0(unity_WorldToObject).y;
    v_10.y = conv_mxt4x4_1(unity_WorldToObject).y;
    v_10.z = conv_mxt4x4_2(unity_WorldToObject).y;
    v_10.w = conv_mxt4x4_3(unity_WorldToObject).y;
    float4 v_11;
    v_11.x = conv_mxt4x4_0(unity_WorldToObject).z;
    v_11.y = conv_mxt4x4_1(unity_WorldToObject).z;
    v_11.z = conv_mxt4x4_2(unity_WorldToObject).z;
    v_11.w = conv_mxt4x4_3(unity_WorldToObject).z;
    float3 tmpvar_12;
    tmpvar_12 = normalize((((v_9.xyz * in_v.normal.x) + (v_10.xyz * in_v.normal.y)) + (v_11.xyz * in_v.normal.z)));
    worldNormal_3 = tmpvar_12;
    float3x3 tmpvar_13;
    tmpvar_13[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_13[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_13[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_14;
    tmpvar_14 = normalize(mul(tmpvar_13, in_v.tangent.xyz));
    worldTangent_2 = tmpvar_14;
    float tmpvar_15;
    tmpvar_15 = (in_v.tangent.w * unity_WorldTransformParams.w);
    tangentSign_1 = tmpvar_15;
    float3 tmpvar_16;
    tmpvar_16 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
    float3 tmpvar_17;
    tmpvar_17.x = worldTangent_2.x;
    tmpvar_17.y = tmpvar_16.x;
    tmpvar_17.z = worldNormal_3.x;
    float3 tmpvar_18;
    tmpvar_18.x = worldTangent_2.y;
    tmpvar_18.y = tmpvar_16.y;
    tmpvar_18.z = worldNormal_3.y;
    float3 tmpvar_19;
    tmpvar_19.x = worldTangent_2.z;
    tmpvar_19.y = tmpvar_16.z;
    tmpvar_19.z = worldNormal_3.z;
    out_v.vertex = UnityObjectToClipPos(tmpvar_8);
    out_v.xlv_TEXCOORD0 = tmpvar_4;
    out_v.xlv_TEXCOORD1 = tmpvar_17;
    out_v.xlv_TEXCOORD2 = tmpvar_18;
    out_v.xlv_TEXCOORD3 = tmpvar_19;
    out_v.xlv_TEXCOORD4 = mul(unity_ObjectToWorld, tmpvar_5).xyz;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 worldN_3;
    float3 worldViewDir_4;
    float3 lightDir_5;
    float3 tmpvar_6;
    float3 tmpvar_7;
    tmpvar_7 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD4));
    lightDir_5 = tmpvar_7;
    float3 tmpvar_8;
    tmpvar_8 = normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD4));
    worldViewDir_4 = tmpvar_8;
    tmpvar_6 = (-worldViewDir_4);
    float3 tmpvar_9;
    float4 c_10;
    float4 tmpvar_11;
    tmpvar_11 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
    c_10 = tmpvar_11;
    tmpvar_9 = (c_10.xyz * _Color.xyz);
    float3 tmpvar_12;
    tmpvar_12 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
    float4 tmpvar_13;
    tmpvar_13.w = 1;
    tmpvar_13.xyz = in_f.xlv_TEXCOORD4;
    float3 tmpvar_14;
    tmpvar_14 = mul(unity_WorldToLight, tmpvar_13).xyz;
    float tmpvar_15;
    tmpvar_15 = dot(tmpvar_14, tmpvar_14);
    float tmpvar_16;
    tmpvar_16 = tex2D(_LightTexture0, float2(tmpvar_15, tmpvar_15)).w;
    worldN_3.x = dot(in_f.xlv_TEXCOORD1, tmpvar_12);
    worldN_3.y = dot(in_f.xlv_TEXCOORD2, tmpvar_12);
    worldN_3.z = dot(in_f.xlv_TEXCOORD3, tmpvar_12);
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_5;
    tmpvar_1 = (tmpvar_1 * tmpvar_16);
    float4 c_17;
    float4 c_18;
    float diff_19;
    float tmpvar_20;
    tmpvar_20 = max(0, dot(worldN_3, tmpvar_2));
    diff_19 = tmpvar_20;
    c_18.xyz = float3(((tmpvar_9 * tmpvar_1) * diff_19));
    c_18.w = (texCUBE(_Cube, tmpvar_6).w * _ReflectColor.w);
    c_17.w = c_18.w;
    c_17.xyz = c_18.xyz;
    out_f.color = c_17;
    return out_f;
}


ENDCG

}
}
Fallback "Legacy Shaders/Transparent/VertexLit"
}