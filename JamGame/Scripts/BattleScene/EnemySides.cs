using Microsoft.Xna.Framework;
using Engine.Core;
using System;

namespace JamGame;

public class EnemySides : Sprite
{
	public Vector2 offset;
	public Vector2 direction;
	private Enemy enemy;

	public EnemySides(Enemy enemy, bool leftSide) : base(Vector2.Zero, "Sprite/Enemy Side Left", enemy.scene.gameManager.Content)
	{
		this.enemy = enemy;

		if (leftSide) {
			this.offset = new Vector2(-100, 0);
		}
		else {
			this.offset = new Vector2(100, 0);
			LoadContent(enemy.scene.gameManager.Content, "Sprite/Enemy Side Right");
		}

		this.position = enemy.position + this.offset;
	}

	public void Update(GameTime gameTime)
	{
		if (enemy.charging && enemy.health <= 8) {
			position += direction * enemy.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
		else if (enemy.charging) {
			Random random = new Random();
			position = enemy.screenOffset + enemy.scene.camera.position + offset + new Vector2(150 * (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds + MathHelper.Pi),
                10 * (float)Math.Sin((2 * gameTime.TotalGameTime.TotalSeconds + MathHelper.Pi) / 2)) + new Vector2(random.Next(-1, 1), random.Next(-1, 1));
		}
		else {
			Random random = new Random();
			position = enemy.position + offset + new Vector2(random.Next(-1, 1), random.Next(-1, 1));
		}
	}
}