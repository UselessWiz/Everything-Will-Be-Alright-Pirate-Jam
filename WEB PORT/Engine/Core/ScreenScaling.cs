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
        if (windowSize.X == 320)
        {
            scale = 4;
        }
        else
        {
            scale = 1;
        }

        return new Point(scale * windowSize.X, scale * windowSize.Y);
    }

    public static Point BestRenderTargetDrawLocation(GraphicsDevice graphicsDevice, Point bestResolution)
    {
        topLeft = new Point((graphicsDevice.Adapter.CurrentDisplayMode.Width - bestResolution.X) / 2, 
            (graphicsDevice.Adapter.CurrentDisplayMode.Height - bestResolution.Y) / 2);
        return topLeft;
    }

    public static Rectangle ChangeResolution(GraphicsDeviceManager _graphics, Point windowSize)
    {
        Point bestResolution = BestResolution(_graphics.GraphicsDevice, _graphics.PreferredBackBufferHeight, windowSize);

        return new Rectangle(BestRenderTargetDrawLocation(_graphics.GraphicsDevice, bestResolution), bestResolution);
    }
}