using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Core;
using Engine.Global;

namespace Engine.Tilemaps;

public abstract class TilemapSprite : Sprite 
{
    // Child Parameters
    protected float speed;          // The speed the object moves between tiles
    protected double moveCooldown;  // How long the object waits at a point on the tilemap before starting to check for directions.
    
    // Base Parameters
    protected Tilemap tilemap;
    protected double moveCooldownFinished = -1;

    public Point tilemapPosition;
    public Point previousTilemapPosition;

    protected Vector2 currentTargetPosition;
    protected Vector2 previousTargetPosition;

    protected float moveLerp = 1;

    protected Vector2 currentDirection;

    // The base constructor (sprite) takes an actual position, not a tilemapPosition. Hence this solution is presented.
    public TilemapSprite(Point tilemapPosition, string spriteName, Tilemap tilemap) : base(new Vector2(0, 0), spriteName, Globals.contentManager)
    {
        this.tilemap = tilemap;
        this.tilemapPosition = tilemapPosition;
        previousTargetPosition = new Vector2(0, 0);

        Tile tile = tilemap.GetTile(tilemapPosition);
        position = tile.position;
        tile.SetOccupyingSprite(this);

        currentTargetPosition = position;
        currentDirection = Vector2.Zero;
    }

    // This accounts for both base constructors for the sprite class
    public TilemapSprite(Point tilemapPosition, string spriteName, int spriteWidth, int spriteHeight, Tilemap tilemap) : 
        base(new Vector2(0, 0), spriteName, spriteWidth, spriteHeight, Globals.contentManager)
    {
        this.tilemap = tilemap;
        this.tilemapPosition = tilemapPosition;
        previousTargetPosition = new Vector2(0, 0);

        Tile tile = tilemap.GetTile(tilemapPosition);
        position = tile.position;
        tile.SetOccupyingSprite(this);

        currentTargetPosition = position;
        currentDirection = Vector2.Zero;
    }

    public virtual void Update(GameTime gameTime)
    {
        Move(gameTime);
    }

    // This assumes the object moves single tiles on the tilemap
    protected void Move(GameTime gameTime)
    {
        // The sprite is at the destination tile (the actual position matches their tilemapPosition).
        if (moveLerp == 1) {
            if (currentDirection == Vector2.Zero) {
                currentDirection = GetDirection(gameTime);
            }
            else {
                previousTargetPosition = position;
                currentTargetPosition = tilemap.GetTile(tilemapPosition).position;
                moveLerp = 0;
            }         
        }
        // The movement has been completed (the lerp value has overshot).
        // The way this is coded at the moment seems to allow a little bit of jittery-ness, which I like. - Make it a little less jittery...
        else if (moveLerp > 1) {
            moveLerp = 1;
            currentDirection = Vector2.Zero;
            position = currentTargetPosition;

            moveCooldownFinished = gameTime.TotalGameTime.TotalSeconds + moveCooldown;
        }
        // The sprite is currently moving towards their tilemapPosition.
        else {
            moveLerp += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position = Vector2.Lerp(previousTargetPosition, currentTargetPosition, moveLerp);
        }
    }

    public abstract Vector2 ChooseDirection(GameTime gameTime);

    // Write code that returns the direction the character should move on the tilemap. 
    protected virtual Vector2 GetDirection(GameTime gameTime)
    {
        Vector2 direction = Vector2.Zero;

        // This creates a small delay once they arrive at their destination.
        if (moveCooldownFinished >= gameTime.TotalGameTime.TotalSeconds) {
            return direction;
        }

        direction = ChooseDirection(gameTime);

        Point testPos = new Point((int)direction.X, (int)direction.Y);
        Tile tile = tilemap.GetTile(tilemapPosition + testPos);

        // If this runs, the target tile is either outside of the tilemap or has the blockMovement flag set to true, 
        // and therefore sprites should not move;
        if (tile.tilemapPosition != tilemapPosition + testPos || tile.blockMovement) {
            return Vector2.Zero;
        }

        // If the tile contains any sprites, don't move there.
        if (CheckMovement(direction, tilemapPosition + testPos) == Vector2.Zero) return Vector2.Zero;

        // Set the occupying sprite of the new tile to this sprite.
        tilemap.GetTile(tilemapPosition).RemoveOccupyingSprite();
        tilemapPosition += testPos;
        tile.SetOccupyingSprite(this);
        
        return direction;
    }

    protected Vector2 CheckMovement(Vector2 direction, Point tilemapPosition)
    {
        if (tilemap.GetTile(tilemapPosition).occupyingSprite != null) return Vector2.Zero;

        return direction;
    }
}