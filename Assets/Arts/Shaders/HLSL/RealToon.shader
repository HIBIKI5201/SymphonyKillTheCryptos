Shader "Custom/HDRP_RealToon"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _RampTex("Ramp", 2D) = "white" {}
        _RimPower("Rim Power", Range(0.1,8)) = 3
        _RimIntensity("Rim Intensity", Range(0,2)) = 1
        _SpecPower("Spec Power", Range(8,128)) = 32
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode"="ForwardOnly" }

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            // ★ これが最重要
            #include "Packages/com.unity.render-pipelines.highlyenderrp/Runtime/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_RampTex);
            SAMPLER(sampler_RampTex);

            float4 _BaseColor;
            float _RimPower;
            float _RimIntensity;
            float _SpecPower;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS);
                float3 normalWS   = TransformObjectToWorldNormal(IN.normalOS);

                OUT.positionWS = positionWS;
                OUT.normalWS   = normalWS;
                OUT.positionCS = TransformWorldToHClip(positionWS);

                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDir  = normalize(GetCameraPositionWS() - IN.positionWS);

                // 仮ディレクショナルライト（確認用）
                float3 lightDir = normalize(float3(0.4,1,0.3));
                float NdotL = saturate(dot(normalWS, lightDir));

                float2 rampUV = float2(NdotL, 0.5);
                float3 ramp = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampUV).rgb;

                float3 diffuse = _BaseColor.rgb * ramp;

                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(saturate(dot(normalWS, halfDir)), _SpecPower);

                float rim = pow(1 - saturate(dot(normalWS, viewDir)), _RimPower) * _RimIntensity;

                return float4(diffuse + spec + rim, 1);
            }

            ENDHLSL
        }
    }
}