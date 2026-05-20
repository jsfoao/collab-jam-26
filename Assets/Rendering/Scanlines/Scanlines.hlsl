// Scanlines.hlsl
// Horizontal scanline effect with density and speed control.

void Scanlines_float(
    float2 UV,
    float Density,
    float Intensity,
    float Speed,
    float3 InputColor,
    out float3 Out)
{
    // Pixel-space Y coordinate for crisp lines
    float pixelY = UV.y * _ScreenParams.y;

    // Scrolling offset (set Speed to 0 for static lines)
    float scroll = _Time.y * Speed;

    // Sine wave scanline pattern
    float scanline = sin((pixelY + scroll) * Density * 3.14159);
    scanline = scanline * 0.5 + 0.5;
    scanline = scanline * scanline;

    // Blend: 1 = no darkening, 0 = full black lines
    float factor = 1.0 - Intensity * (1.0 - scanline);

    Out = InputColor * factor;
}