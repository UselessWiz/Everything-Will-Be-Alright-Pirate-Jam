using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using Engine.Core;

namespace Engine.Tilemaps;

public class Tile : Transform
{
	public Tilemap tilemap;
	public Point tilemapPosition;

	public Rectangle[] layers;

	public bool blockMovement;

	public TilemapSprite occupyingSprite;

	public Tile() : base(Vector2.Zero)//Point tilemapPosition
	{
		// This create a null tile, which is used to ensure moving off the tilemap does not generate errors.
		//this.tilemapPosition = tilemapPosition;
		//this.position = new Vector2(this.tilemap.tileSize * tilemapPosition.X, this.tilemap.tileSize * tilemapPosition.Y);
	}

	public Tile(Tilemap tilemap, Point tilemapPosition, Rectangle[] layerTilesheetPositions, bool blockMovement) : 
		base(new Vector2(tilemap.tileSize * tilemapPosition.X, tilemap.tileSize * tilemapPosition.Y))
	{
		this.tilemap = tilemap;
		this.tilemapPosition = tilemapPosition;
		this.blockMovement = blockMovement;

		this.layers = layerTilesheetPositions;
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		Vector2 tileOffset = new Vector2(tilemap.tileSize / 2, tilemap.tileSize / 2);
		for (int i = 0; i < layers.Length; i++) {
			if (layers[i] != new Rectangle(0, 0, 0, 0)) _spriteBatch.Draw(tilemap.tilesheet.spritesheet, position - tileOffset, layers[i], Color.White);
		}
	}

	public void RemoveOccupyingSprite()
	{
		occupyingSprite = null;
	}

	public void SetOccupyingSprite(TilemapSprite sprite)
	{
		occupyingSprite = sprite;
	}

	public override string ToString()
	{
		string layerString = "";
		for (int i = 0; i < layers.Length; i++) {
			layerString += layers[i].ToString();
			if (i != layers.Length - 1) layerString += ", ";
		}

		return $"{{position: {tilemapPosition.ToString()}, layers: {layerString}}}";
	}
}