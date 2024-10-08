﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float TotalGameTime;
int bigRandom;
Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input, float2 pixPos : SV_POSITION0) : COLOR
{
	float amount = 0.8;

	float randomIntensity = frac(10000 * sin(pixPos.x + pixPos.y * TotalGameTime) * 200000);

    amount *= randomIntensity;

	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color + amount;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};