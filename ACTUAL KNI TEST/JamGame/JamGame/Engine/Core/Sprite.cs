using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Engine.Core;

public class Sprite : Transform
{
    public Texture2D spritesheet;

    public int spriteWidth;
    public int spriteHeight;

    public Rectangle currentSprite;

    // Use this for a single sprite
    public Sprite(Vector2 position, string spriteName, ContentManager contentManager) : base(position)
    {
        this.position = position;
        LoadContent(contentManager, spriteName);

        this.spriteWidth = spritesheet.Width;
        this.spriteHeight = spritesheet.Height;
        this.currentSprite = new Rectangle(0, 0, spriteWidth, spriteHeight);
    }

    // Use this for sprites on a spritesheet.
    public Sprite(Vector2 position, string spritesheetName, int spriteWidth, int spriteHeight, ContentManager contentManager) : base(position)
    {
        this.position = position;
        LoadContent(contentManager, spritesheetName);

        this.spriteWidth = spriteWidth;
        this.spriteHeight = spriteHeight;
        this.currentSprite = new Rectangle(0, 0, spriteWidth, spriteHeight);
    } 

    private Texture2D LoadContent(ContentManager contentManager, string spriteName)
    {
        Texture2D texture = contentManager.Load<Texture2D>(spriteName);
        this.spritesheet = texture;
        return texture;
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(spritesheet, position - new Vector2(spriteWidth / 2, spriteHeight / 2), currentSprite, Color.White); 
    }

    public void Draw(SpriteBatch _spriteBatch, Point selectedSprite) // This override is used mainly for selecting tiles off a tilemap.
    {
        _spriteBatch.Draw(spritesheet, position - new Vector2(spriteWidth / 2, spriteHeight / 2), new Rectangle(new Point(spriteWidth, spriteHeight), selectedSprite), Color.White); 
    }

    // Important methods
    public void SetSprite(Point spriteLocation)
    {
        currentSprite.Location = new Point(spriteLocation.X * spriteWidth, spriteLocation.Y * spriteHeight);
    }
}