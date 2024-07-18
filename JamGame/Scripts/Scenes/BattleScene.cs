using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Global;

namespace JamGame;

public class BattleScene : IScene
{
	public Game1 gameManager {get; set;}

	private SpriteFont debugFont;

	public BattleScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		LoadContent();
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
	}

	public void Update(GameTime gameTime)
	{
		return;
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		return;
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		return;
	}

	public void DrawDebug(GameTime gameTime)
    {
        Globals.spriteBatch.DrawString(debugFont, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2 (10, 10), Color.Red);    
    }
}