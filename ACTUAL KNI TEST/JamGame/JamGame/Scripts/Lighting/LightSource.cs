using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Core;

namespace JamGame;

// This class holds data related to light sources for the lighting system.
public class LightSource : Transform
{
	public bool isActive = true;
	public float brightness;

	public LightSource(Vector2 position, float brightness) : base(position)
	{
		this.brightness = brightness;
	}

	// CHANGE THE SHADER
	public VertexPosition PrepareVertexData(int zCoord)
	{
		return new VertexPosition(new Vector3(position, 0.0f));
	}
}