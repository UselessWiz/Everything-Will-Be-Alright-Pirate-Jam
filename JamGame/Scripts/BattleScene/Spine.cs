using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Core;
using System.Collections;
using System.Collections.Generic;

namespace JamGame;

public class Spine : Sprite
{
	public Enemy enemy;

	public Vector2 direction;
	public float speed;

	public float timer = 0;
	public bool timeUp = false;

	public RectangleCollider collider;

	public Spine(Enemy enemy, Vector2 position, Vector2 direction, float speed, string spriteName, ContentManager contentManager) : 
		base(position, spriteName, contentManager) 
	{
		this.enemy = enemy;
		this.direction = direction;
		this.speed = speed;

		switch (spriteName) {
			case "Sprite/Spine Projectile": this.collider = new RectangleCollider(this, new Point(-2, -3), new Point(5, 5)); break;
			case "Sprite/Spine Projectile Left Facing": this.collider = new RectangleCollider(this, new Point(-1, -2), new Point(5, 5)); break;
			case "Sprite/Spine Projectile Right Facing": this.collider = new RectangleCollider(this, new Point(-4, -2), new Point(5, 5)); break;
		}
		
	}

	public void Update(GameTime gameTime)
	{
		position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

		collider.UpdateTrigger();

		timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

		if (timer >= 10f) {
			timeUp = true;
		}
	}
}