// SobelEdge.hlsl
// Detects edges from both depth AND world normals, with thickness control.

void SobelEdge_float(
    float2 UV,
    float Thickness,
    float DepthThreshold,
    float NormalThreshold,
    out float Out)
{
    float2 offsets[9] = {
        float2(-1, -1), float2(0, -1), float2(1, -1),
        float2(-1,  0), float2(0,  0), float2(1,  0),
        float2(-1,  1), float2(0,  1), float2(1,  1)
    };

    float sobelX[9] = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
    float sobelY[9] = { -1, -2, -1,  0, 0, 0,  1, 2, 1 };

    // --- Depth edge detection ---
    float depthGx = 0.0;
    float depthGy = 0.0;

    // --- Normal edge detection (per channel) ---
    float3 normalGx = float3(0, 0, 0);
    float3 normalGy = float3(0, 0, 0);

    for (int i = 0; i < 9; i++)
    {
        float texelWidth = 1.0 / _ScreenParams.x;
        float texelHeight = 1.0 / _ScreenParams.y;
        float2 sampleUV = UV + offsets[i] * float2(texelWidth, texelHeight) * Thickness;

        // Sample depth
        float depth = LinearEyeDepth(
            SHADERGRAPH_SAMPLE_SCENE_DEPTH(sampleUV),
            _ZBufferParams
        );
        depthGx += depth * sobelX[i];
        depthGy += depth * sobelY[i];

        // Sample world normal
        float3 normal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(sampleUV);
        normalGx += normal * sobelX[i];
        normalGy += normal * sobelY[i];
    }

    // Depth edge magnitude
    float depthEdge = sqrt(depthGx * depthGx + depthGy * depthGy);

    // Normal edge magnitude (combine all 3 channels)
    float normalEdge = sqrt(
        dot(normalGx, normalGx) + dot(normalGy, normalGy)
    );

    // Combine: edge if either depth or normal discontinuity is detected
    float edge = max(
        step(DepthThreshold, depthEdge),
        step(NormalThreshold, normalEdge)
    );

    Out = edge;
}