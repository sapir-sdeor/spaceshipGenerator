// Use with CustomFunction node, with input:
// - Float Value, the value to bin.
// And 9 outputs:
// - Float - the weight of each bin: 1/16. 1/8, 1/4, 1/2, 1, 2, 4, 8, 16.
// Function name is LogBin.


float2 GradientNoise_Dir_float(float2 p) {
    // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
    p = p % 289;
    // need full precision, otherwise half overflows when p > 1
    float x = float(34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float GradientNoise_float(float2 UV, float Scale) {
    float2 p = UV * Scale;
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(GradientNoise_Dir_float(ip), fp);
    float d01 = dot(GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
}

void LogBinNoise_float(float2 uv, float scale, out float res) {
    const float valIndex = log2(max(0.001, scale));
    const int index = floor(valIndex);
    float firstScale = pow(2, index - 1);
    res = GradientNoise_float(uv, firstScale) * (1 - valIndex + index)
        + GradientNoise_float(uv, firstScale * 2) * (2 - valIndex + index)
        + GradientNoise_float(uv, firstScale * 4) * (1 + valIndex - index)
        + GradientNoise_float(uv, firstScale * 8) * (valIndex - index);
    res *= 0.25;
}

void LogBin_float(
        float val,
        out float _1_16,
        out float _1_8,
        out float _1_4,
        out float _1_2,
        out float _1,
        out float _2,
        out float _4,
        out float _8,
        out float _16) {
    const float valIndex = clamp(log2(val), -3, 2);
    const int index = floor(valIndex);
    float res[9] = {0,0,0,0,0,0,0,0,0};
    res[index + 3] = 1 - valIndex + index;
    res[index + 4] = 2 - valIndex + index;
    res[index + 5] = 1 + valIndex - index;
    res[index + 6] = valIndex - index;
    _1_16 = res[0];
    _1_8 = res[1];
    _1_4 = res[2];
    _1_2 = res[3];
    _1 = res[4];
    _2 = res[5];
    _4 = res[6];
    _8 = res[7];
    _16 = res[8];
}
