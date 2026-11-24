#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Pixel-perfect sampling shader.
// Antialiased nearest-neighbor filtering based on:
// https://www.shadertoy.com/view/MllGWX (CC0 1.0 Public Domain Dedication)

float2 TexelSize;
float PixelPerfectEnabled;
float texels_per_pixel;

float4x4 MatrixTransform;
    
Texture2D SpriteTexture : register(t0);
sampler2D SpriteTextureSampler : register(s0) = sampler_state
{
    Texture = <SpriteTexture>;
    MipFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

float2 v2len(float2 a, float2 b)
{
    return sqrt(a * a + b * b);
}

float4 textureBlocky(in float2 uv, in float2 res)
{
    uv *= res; // enter texel coordinate space.

    float2 seam = floor(uv + 0.5); // find the nearest seam between texels.

    // here's where the magic happens. scale up the distance to the seam so that all
    // interpolation happens in a one-pixel-wide space.
    uv = (uv - seam) / v2len(ddx(uv), ddy(uv)) + seam;

    uv = clamp(uv, seam - 0.5, seam + 0.5); // clamp to the center of a texel.

    // convert back to 0..1 coordinate space.
    return tex2Dgrad(SpriteTextureSampler, uv / res, float2(0, 0), float2(0, 0));
}
float4 textureUgly(in float2 uv, in float2 res)
{
    return tex2Dgrad(SpriteTextureSampler, (floor(uv * res) + 0.5) / res, float2(0, 0), float2(0, 0));
}

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, MatrixTransform);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    if (PixelPerfectEnabled > 0.5)
    {
        return textureBlocky(input.TextureCoordinates, TexelSize) * input.Color;
    }
    else
    {
        return textureUgly(input.TextureCoordinates, TexelSize) * input.Color;
    }
}

technique SpriteDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};