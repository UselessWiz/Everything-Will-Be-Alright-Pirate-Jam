using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Engine.Tilemaps;

namespace Engine.Global;

public static class Globals
{
	// Monogame Globals
	public static Point windowSize;
	public static ContentManager contentManager;
	public static SpriteBatch spriteBatch;
	public static SpriteBatch uiSpriteBatch;
	public static GraphicsDevice graphicsDevice;
	
	public static int targetFPS;
}
