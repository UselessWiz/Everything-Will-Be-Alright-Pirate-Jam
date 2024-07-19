using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Core;
using Engine.Global;
using Engine.Serialization;

namespace Engine.Tilemaps;

public class Tilemap
{
	public Tile[,] tiles;

	public int numberOfLayers;

	// The first sprite (at position [0, 0]) MUST ALWAYS BE NOTHING. Position [0, 0] is used to check if a layer is visible or not,
	// as Point is a struct and cannot be nullable (see Tile.layers).
	public Sprite tilesheet;
	public string tilesheetName;

	public int width;
	public int height;

	public int tileSize;

	public Camera camera;
	private Point topLeftTileOnScreen;
	private Point botrightTileOnScreen;

	public Tilemap(int width, int height, int tileSize, int numberOfLayers, string tilesheetName) //string tilemapDataFile
	{
		tiles = new Tile[width, height];

		this.width = width;
		this.height = height;
		this.tileSize = tileSize;
		this.numberOfLayers = numberOfLayers;
		this.tilesheetName = tilesheetName;

		this.tilesheet = new Sprite(Vector2.Zero, tilesheetName, tileSize, tileSize, Globals.contentManager);
	}

	private void LoadContent(ContentManager contentManager)
	{

	}

	public void Update(GameTime gameTime)
	{
		// Calculate the range of tiles that need to be drawn
		// This is done here to avoid placing logic in the Draw function.
		topLeftTileOnScreen = new Point(((int)camera.position.X - Globals.windowSize.X / 2) / tileSize - 2, 
			((int)camera.position.Y - Globals.windowSize.Y / 2) / tileSize - 2);
		botrightTileOnScreen = new Point(((int)camera.position.X + Globals.windowSize.X / 2) / tileSize + 2, 
			((int)camera.position.Y + Globals.windowSize.Y / 2) / tileSize + 2);

		topLeftTileOnScreen.X = Math.Clamp(topLeftTileOnScreen.X, 0, width);
		topLeftTileOnScreen.Y = Math.Clamp(topLeftTileOnScreen.Y, 0, height);
		botrightTileOnScreen.X = Math.Clamp(botrightTileOnScreen.X, 0, width);
		botrightTileOnScreen.Y = Math.Clamp(botrightTileOnScreen.Y, 0, height);
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		// Takes the range of tiles calculated above and draws only them.
		// Hugely better than it was before (its drawing like 100 tiles max instead of 10000)
		for (int i = topLeftTileOnScreen.X; i < botrightTileOnScreen.X; i++) {
			for (int j = topLeftTileOnScreen.Y; j < botrightTileOnScreen.Y; j++) {
				tiles[i,j].Draw(_spriteBatch);
			}
		}
	}

	public Tile GetTile(Point tilemapPosition)
	{
		if (tilemapPosition.X < 0 || tilemapPosition.X > width || tilemapPosition.Y < 0 || tilemapPosition.Y > height) {
			return new Tile();
		}
		else {
			return tiles[tilemapPosition.X,tilemapPosition.Y];
		}
	}

	public override string ToString()
	{
		return $"Width: {this.width}\nHeight: {this.height}\nTileSize: {this.tileSize}\nTilesheetName: {this.tilesheetName}";
	}
}
