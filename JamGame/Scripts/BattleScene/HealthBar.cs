using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Core;

namespace JamGame;

public class HealthBar : Sprite
{
	public bool drawn = false;

	public Sprite healthBarBG;

	public float maxValue = 300;
	public float minValue = 0;

	public int currentValue;

	// Position is the base position of the health bar - ie. the position of the health bar background.
	public HealthBar(Vector2 position, string spriteName, int currentValue, ContentManager contentManager) : 
		base(position, spriteName, contentManager)
	{
		this.healthBarBG = new Sprite(new Vector2(position.X, position.Y + 1), "Sprite/UI/HealthBarBG", contentManager);
		this.currentValue = currentValue;

		this.currentSprite = ChangeHealthBarValue(currentValue);
	}

	public Rectangle ChangeHealthBarValue(int newValue)
	{
		return new Rectangle(0, 0, currentValue, spritesheet.Height);
	}

	public void DrawUI(SpriteBatch _spriteBatch) 
	{
		if (currentValue > 0) this.healthBarBG.Draw(_spriteBatch);
		this.Draw(_spriteBatch);
	}
}