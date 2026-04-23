#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

void ToonLighting_float(
    float3 NormalWS,
    float3 ViewDirWS,
    float3 LightDirWS,
    float3 LightColor,
    float3 BaseColor,
    UnityTexture2D RampTex,
    float2 RampUV,
    float RimPower,
    float SpecPower,
    out float3 OutColor)
{
    NormalWS = normalize(NormalWS);
    ViewDirWS = normalize(ViewDirWS);
    LightDirWS = normalize(LightDirWS);

    float NdotL = saturate(dot(NormalWS, LightDirWS));

    float2 uv = float2(NdotL, RampUV.y);

    float4 rampSample = SAMPLE_TEXTURE2D(RampTex, RampTex.samplerstate, uv);
    float3 ramp = rampSample.rgb;

    float3 diffuse = BaseColor * ramp * LightColor;

    float3 halfDir = normalize(LightDirWS + ViewDirWS);
    float spec = pow(saturate(dot(NormalWS, halfDir)), SpecPower);

    float rim = pow(1 - saturate(dot(NormalWS, ViewDirWS)), RimPower);

    float3 specColor = spec * LightColor;
    float3 rimColor  = rim * LightColor;

    OutColor = diffuse;
    OutColor += specColor * 0.5;
    OutColor += rimColor * 0.5;
    OutColor = saturate(OutColor);
}

#endif