using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Core;

public class ScreenScaling
{
    public static int scale;
    public static Point topLeft;

	public static Point BestResolution(GraphicsDevice graphicsDevice, int PreferredBackBufferHeight, Point windowSize)
    {
        scale = 4;

        // If there is any issues with scaling (particularly for displays that aren't the same aspect ratio as the internal resolution).
        if (scale * windowSize.X > graphicsDevice.Adapter.CurrentDisplayMode.Width || 
                scale * windowSize.Y > graphicsDevice.Adapter.CurrentDisplayMode.Height) {
            scale -= 1;
        }

        return new Point(scale * windowSize.X, scale * windowSize.Y);
    }

    public static Point BestRenderTargetDrawLocation(GraphicsDevice graphicsDevice, Point bestResolution)
    {
        topLeft = new Point(320, 200);
        return topLeft;
    }

    public static Rectangle ChangeResolution(GraphicsDeviceManager _graphics, Point windowSize)
    {
        Point bestResolution = BestResolution(_graphics.GraphicsDevice, _graphics.PreferredBackBufferHeight, windowSize);

        return new Rectangle(BestRenderTargetDrawLocation(_graphics.GraphicsDevice, bestResolution), bestResolution);
    }
}