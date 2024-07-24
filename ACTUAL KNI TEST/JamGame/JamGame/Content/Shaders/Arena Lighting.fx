float3x3 WorldViewProjection;

float2 LightPosition;
float LightBrightness;

struct VertexShaderInput
{
	float3 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float3 transformedPos = mul(input.Position, WorldViewProjection);
	//output.Position = float4(transformedPos, 1.0);
	output.Color = float4(1.0, 1.0, 1.0, 1.0);

	return output;
}

float4 MainPS(VertexShaderOutput input, float4 PixPos : SV_POSITION0) : COLOR
{
	float2 Distance = PixPos.xy - LightPosition;
	float Strength = (LightBrightness / 6) / (sqrt(Distance.x * Distance.x + Distance.y * Distance.y + LightBrightness * LightBrightness)
		 - LightBrightness);
	float4 PixelColour = input.Color;
	return float4(input.Color.xyz * Strength, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile vs_3_0_level_9_1 MainVS();
		PixelShader = compile ps_3_0_level_9_1 MainPS();
	}
};