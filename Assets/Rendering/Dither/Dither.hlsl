// DitherLighting.hlsl
// Applies a Bayer matrix dither to scene lighting/luminance.

void DitherLighting_float(
    float2 UV,
    float DitherScale,
    float DitherSpread,
    float Brightness,
    float3 SceneColor,
    float3 LitColor,
    float3 ShadowColor,
    out float3 Out)
{
    // --- 4x4 Bayer matrix (values 0-15, normalized to 0-1) ---
    float bayer[16] = {
         0.0 / 16.0,  8.0 / 16.0,  2.0 / 16.0, 10.0 / 16.0,
        12.0 / 16.0,  4.0 / 16.0, 14.0 / 16.0,  6.0 / 16.0,
         3.0 / 16.0, 11.0 / 16.0,  1.0 / 16.0,  9.0 / 16.0,
        15.0 / 16.0,  7.0 / 16.0, 13.0 / 16.0,  5.0 / 16.0
    };

    // Pixel coordinates scaled by dither pattern size
    float2 pixelPos = UV * _ScreenParams.xy / max(DitherScale, 0.001);
    int x = int(fmod(abs(pixelPos.x), 4.0));
    int y = int(fmod(abs(pixelPos.y), 4.0));
    float threshold = bayer[y * 4 + x];

    // Scene luminance as the dither input
    float luminance = dot(SceneColor, float3(0.2126, 0.7152, 0.0722));

    // Shift and spread the luminance around the threshold
    float adjusted = (luminance + Brightness - 0.5) / max(DitherSpread, 0.001);
    adjusted = saturate(adjusted);

    // Binary dither: lit or shadow
    float dither = step(threshold, adjusted);
    
    Out = lerp(ShadowColor, LitColor * luminance, dither);
}