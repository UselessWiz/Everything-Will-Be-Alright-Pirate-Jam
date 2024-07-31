using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Engine.Core;

namespace JamGame;

public class Lazer : Sprite
{
	public Player player;

	private float rotation;

	public Lazer(ContentManager contentManager) : base(Vector2.Zero, "Sprite/Lazer", contentManager)
	{
	}

	public void Update(GameTime gameTime)
	{
		//Console.WriteLine(position + rotation.ToString());
	}

	public new void Draw(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Draw(spritesheet, position, null, Color.White, rotation, new Vector2(spritesheet.Width / 2, spritesheet.Height / 2),
			new Vector2(1, 1), SpriteEffects.None, 0);
	}

	public void CalculatePosition(Vector2 line)
	{
		line.Normalize();
		position = player.position + line * 150; 

		rotation = (float)Math.Atan2((double)line.Y, (double)line.X);
	}
}