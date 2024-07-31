using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine.Global;

namespace Engine.Core;

public class CameraCentre : Transform
{
	private float scrollSensitivity = 200f;

	public CameraCentre(Vector2 position) : base(position) {}

	public void Update(GameTime gameTime)
	{
		MouseState mouseState = Mouse.GetState();
		Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
		Vector2 direction = Vector2.Zero;

		if (mousePos.X == 0) direction += new Vector2(-1, 0);
		if (mousePos.X == Globals.graphicsDevice.Adapter.CurrentDisplayMode.Width - 1) direction += new Vector2(1, 0);
		if (mousePos.Y == 0) direction += new Vector2(0, -1);
		if (mousePos.Y == Globals.graphicsDevice.Adapter.CurrentDisplayMode.Height - 1) direction += new Vector2(0, 1);

		position += direction * scrollSensitivity * (float)gameTime.ElapsedGameTime.TotalSeconds;
	} 
}