#include "Packages/com.jimmycushnie.noisynodes/NoiseShader/HLSL/ClassicNoise2D.hlsl"

// octaves - the number of levels of details.
// persistence - how much each octave contributes to the overall shape (adjusts amplitude).
// lacunarity -how much detail is added or removed at each octave (adjusts frequency).
void FractalNoise2D_float(float2 samplingCoords, uint octaves, float persistence, float lacunarity, out float Out) {
    float amplitude = 1;
    float frequency = 1;
    float range = 1;
    Out = 0;

    Out += cnoise(samplingCoords * frequency) * amplitude;

    for (uint i = 1; i < octaves; ++i) {
        amplitude *= persistence;
        frequency *= lacunarity;
        range += amplitude;

        Out += cnoise(samplingCoords * frequency) * amplitude;
    }
    Out /= range;
}