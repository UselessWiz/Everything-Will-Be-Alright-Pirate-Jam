using System;
using System.IO;
using System.Text; // When in doubt steal the using statements from the tutorial.
using Microsoft.Xna.Framework;
using Engine.Tilemaps;

namespace Engine.Serialization;

public static class BinarySerializer
{
	private enum tilemapParameters 
	{
		blockMovement = 128,
	}

	public enum SerializerType
	{
		TILEMAP,
	}

	// It's possible to decrease the space even further, but i'm not worried enough to bother right now. Maybe if we get to
	// open-world testing and find we need like 200 tilemap chunks we might make it better, but for now it's fine.
	public static void WriteTilemap(string path, Tilemap tilemap)
	{
		path = FormatPath(path, SerializerType.TILEMAP);

		using (FileStream stream = File.Open(path, FileMode.Create)) {
			using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false)) {
				// Actual tilemap class data
				writer.Write((int)tilemap.width);
				writer.Write((int)tilemap.height);
				writer.Write((int)tilemap.tileSize);
				writer.Write((int)tilemap.numberOfLayers);
				writer.Write(tilemap.tilesheetName); // load the actual sprite using monogame's contentloader.

				// Data for each individual tile.
				for (int i = 0; i < tilemap.width; i++) {
					for (int j = 0; j < tilemap.height; j++) {
						// Set the tilemap position of each tile
						writer.Write(tilemap.tiles[i,j].tilemapPosition.X);
						writer.Write(tilemap.tiles[i,j].tilemapPosition.Y);

						// Set the layers parameter of each tile (tilesize is handled by the tilemap data above).
						for (int k = 0; k < tilemap.numberOfLayers; k++) {
							writer.Write(tilemap.tiles[i,j].layers[k].X);
							writer.Write(tilemap.tiles[i,j].layers[k].Y);
						}

						// Format the parameters
						byte parameters = 0;

						if (tilemap.tiles[i,j].blockMovement) parameters += (int)tilemapParameters.blockMovement;

						writer.Write(parameters);
					}
				}
			}
		}
	}

	public static Tilemap ReadTilemap(string path)
	{
		path = FormatPath(path, SerializerType.TILEMAP);
		Tilemap tilemap;

		using (FileStream stream = File.Open(path, FileMode.Open)) {
			using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false)) {
				tilemap = new Tilemap(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadString());
				tilemap.tiles = new Tile[tilemap.width, tilemap.height];

				for (int i = 0; i < tilemap.width; i++) {
					for (int j = 0; j < tilemap.height; j++) {
						// Get the tilemap position of each tile
						Point tilemapPosition = new Point(reader.ReadInt32(), reader.ReadInt32());

						// Set the layers parameter of each tile (tilesize is handled by the tilemap data above).
						Rectangle[] layers = new Rectangle[tilemap.numberOfLayers];
						
						for (int k = 0; k < tilemap.numberOfLayers; k++) {
							int layerX = reader.ReadInt32();
							int layerY = reader.ReadInt32();

							if (layerX == 0 && layerY == 0) layers[k] = new Rectangle(0, 0, 0, 0);
							else layers[k] = new Rectangle(layerX, layerY, tilemap.tileSize, tilemap.tileSize);
						}

						// Unformat the parameters. There's probably a better way to do this using bitwise operators, 
						// but I'm not too worried at the moment with only the one parameter.
						byte parameters = reader.ReadByte();
						bool blockMovement = false;

						if (parameters >= (int)tilemapParameters.blockMovement) {
							parameters -= (int)tilemapParameters.blockMovement;
							blockMovement = true;
						}

						tilemap.tiles[i,j] = new Tile(tilemap, tilemapPosition, layers, blockMovement);
					}
				}
			}
		}

		return tilemap;
	}

	// This takes a monogame-style string and rewrites it in the correct form.
	public static string FormatPath(string path, SerializerType type)
	{
		switch (type) {
			case SerializerType.TILEMAP: return $"Content/{path}.tmap";
		}

		return path;
	}
}