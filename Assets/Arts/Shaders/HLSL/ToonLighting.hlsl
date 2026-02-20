#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

void ToonLighting_float(
    float3 NormalWS,
    float3 ViewDirWS,
    float3 LightDirWS,
    float3 LightColor,
    float3 BaseColor,
    Texture2D RampTex,
    SamplerState RampSampler,
    float RimPower,
    float SpecPower,
    out float3 OutColor)
{
    NormalWS = normalize(NormalWS);
    ViewDirWS = normalize(ViewDirWS);
    LightDirWS = normalize(LightDirWS);

    float NdotL = saturate(dot(NormalWS, LightDirWS));

    float2 rampUV = float2(NdotL, 0.5);
    float3 ramp = RampTex.Sample(RampSampler, rampUV).rgb;

    float3 diffuse = BaseColor * ramp * LightColor;

    float3 halfDir = normalize(LightDirWS + ViewDirWS);
    float spec = pow(saturate(dot(NormalWS, halfDir)), SpecPower);

    float rim = pow(1 - saturate(dot(NormalWS, ViewDirWS)), RimPower);

    OutColor = diffuse + spec + rim;
}

#endif