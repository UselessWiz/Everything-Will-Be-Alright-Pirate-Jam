#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float3x3 WorldViewProjection;

float2 LightPosition;
float LightBrightness;

struct VertexShaderInput
{
	float3 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float3 transformedPos = mul(input.Position, WorldViewProjection);
	output.Position = float4(transformedPos, 1.0);
	output.Color = float4(1.0, 1.0, 1.0, 1.0);

	return output;
}

float4 MainPS(VertexShaderOutput input, float2 PixPos : SV_POSITION0) : COLOR
{
	float2 Distance = PixPos - LightPosition;
	float Strength = (LightBrightness / 6) / (sqrt(Distance.x * Distance.x + Distance.y * Distance.y + LightBrightness * LightBrightness)
		 - LightBrightness);
	float4 PixelColour = input.Color;
	return float4(input.Color.xyz * Strength, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};