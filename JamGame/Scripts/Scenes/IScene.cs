using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamGame;

public interface IScene
{
	public Game1 gameManager {get; set;}

	void LoadContent();

	void Update(GameTime gameTime);

	void Draw(SpriteBatch _spriteBatch);

	void DrawUI(SpriteBatch _spriteBatch);

	void DrawDebug(GameTime gameTime);
}