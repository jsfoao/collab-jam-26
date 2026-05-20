// BarrelDistortion.hlsl
// Pure barrel distortion with rounded corner masking.

void BarrelDistortion_float(
    float2 UV,
    float Strength,
    float Zoom,
    out float2 DistortedUV,
    out float Mask)
{
    // Remap UV from 0..1 to -1..1
    float2 centered = UV * 2.0 - 1.0;

    // Distance from center
    float r2 = dot(centered, centered);

    // Barrel distortion
    float2 distorted = centered * (1.0 + Strength * r2);

    // Zoom to compensate for edges pulling inward
    distorted /= max(Zoom, 0.001);

    // Back to 0..1
    DistortedUV = distorted * 0.5 + 0.5;

    // Mask: 1 inside screen bounds, 0 outside (rounded corners)
    Mask = step(0.0, DistortedUV.x) * step(DistortedUV.x, 1.0)
         * step(0.0, DistortedUV.y) * step(DistortedUV.y, 1.0);
}