using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Global;

namespace Engine.Core;

public class Camera : Transform
{
	public Matrix translation;

	public Camera() : base(Vector2.Zero) {}

	public void Update(GameTime gameTime)
	{
		CalculateTransformation();
	}

	private void CalculateTransformation()
	{
		float dx = (Globals.windowSize.X / 2) - position.X;
		float dy = (Globals.windowSize.Y / 2) - position.Y;
		translation = Matrix.CreateTranslation(dx, dy, 0f);
	}
}