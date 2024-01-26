// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox/Procedural" {
Properties {
[KeywordEnum(None, Simple, High Quality)]  _SunDisk ("Sun", Float) = 2.000000
 _SunSize ("Sun Size", Range(0.000000,1.000000)) = 0.040000
 _AtmosphereThickness ("Atmoshpere Thickness", Range(0.000000,5.000000)) = 1.000000
 _SkyTint ("Sky Tint", Color) = (0.500000,0.500000,0.500000,1.000000)
 _GroundColor ("Ground", Color) = (0.369000,0.349000,0.341000,1.000000)
 _Exposure ("Exposure", Range(0.000000,8.000000)) = 1.300000
}
SubShader { 
 Tags { "QUEUE"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
  ZWrite Off
  Cull Off
  GpuProgramID 50816
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)


#define CODE_BLOCK_VERTEX
//uniform float4 _WorldSpaceLightPos0;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
//uniform float4 unity_ColorSpaceDouble;
uniform float _Exposure;
uniform float3 _GroundColor;
uniform float3 _SkyTint;
uniform float _AtmosphereThickness;
struct appdata_t
{
    float4 vertex :POSITION;
};

struct OUT_Data_Vert
{
    float xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float xlv_TEXCOORD0 :TEXCOORD0;
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
    float3 cOut_1;
    float3 cIn_2;
    float far_3;
    float kKr4PI_4;
    float kKrESun_5;
    float3 kSkyTintInGammaSpace_6;
    float tmpvar_7;
    float4 tmpvar_8;
    float4 tmpvar_9;
    tmpvar_9.w = 1;
    tmpvar_9.xyz = in_v.vertex.xyz;
    tmpvar_8 = UnityObjectToClipPos(tmpvar_9);
    float3 tmpvar_10;
    if((unity_ColorSpaceDouble.x>2))
    {
        tmpvar_10 = pow(_SkyTint, float3(0.4545454, 0.4545454, 0.4545454));
    }
    else
    {
        tmpvar_10 = _SkyTint;
    }
    kSkyTintInGammaSpace_6 = tmpvar_10;
    float3 tmpvar_11;
    float _tmp_dvx_552 = (1 / pow(lerp(float3(0.5, 0.42, 0.325), float3(0.8, 0.72, 0.625), (float3(1, 1, 1) - kSkyTintInGammaSpace_6)), float3(4, 4, 4)));
    tmpvar_11 = float3(_tmp_dvx_552, _tmp_dvx_552, _tmp_dvx_552);
    float tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = pow(_AtmosphereThickness, 2.5);
    tmpvar_12 = (0.05 * tmpvar_13);
    kKrESun_5 = tmpvar_12;
    float tmpvar_14;
    tmpvar_14 = (0.03141593 * tmpvar_13);
    kKr4PI_4 = tmpvar_14;
    float3x3 tmpvar_15;
    tmpvar_15[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
    tmpvar_15[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
    tmpvar_15[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
    float3 tmpvar_16;
    tmpvar_16 = normalize(mul(tmpvar_15, in_v.vertex.xyz));
    far_3 = 0;
    if((tmpvar_16.y>=0))
    {
        float3 frontColor_17;
        float3 samplePoint_18;
        far_3 = (sqrt(((1.050625 + (tmpvar_16.y * tmpvar_16.y)) - 1)) - tmpvar_16.y);
        float tmpvar_19;
        tmpvar_19 = (1 - (dot(tmpvar_16, float3(0, 1.0001, 0)) / 1.0001));
        float tmpvar_20;
        tmpvar_20 = (exp((-0.00287 + (tmpvar_19 * (0.459 + (tmpvar_19 * (3.83 + (tmpvar_19 * (-6.8 + (tmpvar_19 * 5.25))))))))) * 0.2460318);
        float tmpvar_21;
        tmpvar_21 = (far_3 / 2);
        float tmpvar_22;
        tmpvar_22 = (tmpvar_21 * 40.00004);
        float3 tmpvar_23;
        tmpvar_23 = (tmpvar_16 * tmpvar_21);
        float3 tmpvar_24;
        tmpvar_24 = (float3(0, 1.0001, 0) + (tmpvar_23 * 0.5));
        float tmpvar_25;
        tmpvar_25 = sqrt(dot(tmpvar_24, tmpvar_24));
        float tmpvar_26;
        tmpvar_26 = exp((160.0002 * (1 - tmpvar_25)));
        float tmpvar_27;
        tmpvar_27 = (1 - (dot(_WorldSpaceLightPos0.xyz, tmpvar_24) / tmpvar_25));
        float tmpvar_28;
        tmpvar_28 = (1 - (dot(tmpvar_16, tmpvar_24) / tmpvar_25));
        float _tmp_dvx_553 = (exp(((-clamp((tmpvar_20 + (tmpvar_26 * ((0.25 * exp((-0.00287 + (tmpvar_27 * (0.459 + (tmpvar_27 * (3.83 + (tmpvar_27 * (-6.8 + (tmpvar_27 * 5.25)))))))))) - (0.25 * exp((-0.00287 + (tmpvar_28 * (0.459 + (tmpvar_28 * (3.83 + (tmpvar_28 * (-6.8 + (tmpvar_28 * 5.25))))))))))))), 0, 50)) * ((tmpvar_11 * kKr4PI_4) + 0.01256637))) * (tmpvar_26 * tmpvar_22));
        frontColor_17 = float3(_tmp_dvx_553, _tmp_dvx_553, _tmp_dvx_553);
        samplePoint_18 = (tmpvar_24 + tmpvar_23);
        float tmpvar_29;
        tmpvar_29 = sqrt(dot(samplePoint_18, samplePoint_18));
        float tmpvar_30;
        tmpvar_30 = exp((160.0002 * (1 - tmpvar_29)));
        float tmpvar_31;
        tmpvar_31 = (1 - (dot(_WorldSpaceLightPos0.xyz, samplePoint_18) / tmpvar_29));
        float tmpvar_32;
        tmpvar_32 = (1 - (dot(tmpvar_16, samplePoint_18) / tmpvar_29));
        frontColor_17 = (frontColor_17 + (exp(((-clamp((tmpvar_20 + (tmpvar_30 * ((0.25 * exp((-0.00287 + (tmpvar_31 * (0.459 + (tmpvar_31 * (3.83 + (tmpvar_31 * (-6.8 + (tmpvar_31 * 5.25)))))))))) - (0.25 * exp((-0.00287 + (tmpvar_32 * (0.459 + (tmpvar_32 * (3.83 + (tmpvar_32 * (-6.8 + (tmpvar_32 * 5.25))))))))))))), 0, 50)) * ((tmpvar_11 * kKr4PI_4) + 0.01256637))) * (tmpvar_30 * tmpvar_22)));
        samplePoint_18 = (samplePoint_18 + tmpvar_23);
        cIn_2 = (frontColor_17 * (tmpvar_11 * kKrESun_5));
        cOut_1 = (frontColor_17 * 0.02);
    }
    else
    {
        float3 frontColor_1_33;
        far_3 = (-0.0001 / min(-0.001, tmpvar_16.y));
        float3 tmpvar_34;
        tmpvar_34 = (float3(0, 1.0001, 0) + (far_3 * tmpvar_16));
        float tmpvar_35;
        float tmpvar_36;
        tmpvar_36 = (1 - dot((-tmpvar_16), tmpvar_34));
        tmpvar_35 = (0.25 * exp((-0.00287 + (tmpvar_36 * (0.459 + (tmpvar_36 * (3.83 + (tmpvar_36 * (-6.8 + (tmpvar_36 * 5.25))))))))));
        float tmpvar_37;
        tmpvar_37 = (1 - dot(_WorldSpaceLightPos0.xyz, tmpvar_34));
        float tmpvar_38;
        tmpvar_38 = (far_3 / 2);
        float3 tmpvar_39;
        tmpvar_39 = (float3(0, 1.0001, 0) + ((tmpvar_16 * tmpvar_38) * 0.5));
        float tmpvar_40;
        tmpvar_40 = exp((160.0002 * (1 - sqrt(dot(tmpvar_39, tmpvar_39)))));
        float3 tmpvar_41;
        float _tmp_dvx_554 = exp(((-clamp(((tmpvar_40 * ((0.25 * exp((-0.00287 + (tmpvar_37 * (0.459 + (tmpvar_37 * (3.83 + (tmpvar_37 * (-6.8 + (tmpvar_37 * 5.25)))))))))) + tmpvar_35)) - (0.9996001 * tmpvar_35)), 0, 50)) * ((tmpvar_11 * kKr4PI_4) + 0.01256637)));
        tmpvar_41 = float3(_tmp_dvx_554, _tmp_dvx_554, _tmp_dvx_554);
        frontColor_1_33 = (tmpvar_41 * (tmpvar_40 * (tmpvar_38 * 40.00004)));
        cIn_2 = (frontColor_1_33 * ((tmpvar_11 * kKrESun_5) + 0.02));
        float3 tmpvar_42;
        tmpvar_42 = clamp(tmpvar_41, float3(0, 0, 0), float3(1, 1, 1));
        cOut_1 = tmpvar_42;
    }
    tmpvar_7 = ((-tmpvar_16.y) / 0.02);
    float3 light_43;
    light_43 = _WorldSpaceLightPos0.xyz;
    float3 ray_44;
    ray_44 = (-tmpvar_16);
    float tmpvar_45;
    tmpvar_45 = dot(light_43, ray_44);
    out_v.vertex = tmpvar_8;
    out_v.xlv_TEXCOORD0 = tmpvar_7;
    out_v.xlv_TEXCOORD1 = (_Exposure * (cIn_2 + (_GroundColor * cOut_1)));
    out_v.xlv_TEXCOORD2 = (_Exposure * (cIn_2 * (0.75 + (0.75 * (tmpvar_45 * tmpvar_45)))));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    tmpvar_1.w = 1;
    float _tmp_dvx_555 = clamp(in_f.xlv_TEXCOORD0, 0, 1);
    tmpvar_1.xyz = lerp(in_f.xlv_TEXCOORD2, in_f.xlv_TEXCOORD1, float3(_tmp_dvx_555, _tmp_dvx_555, _tmp_dvx_555));
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
Fallback Off
}