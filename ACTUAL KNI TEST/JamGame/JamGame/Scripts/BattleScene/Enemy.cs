using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using Engine.Core;
using Engine.Global;

namespace JamGame;

public class Enemy : Sprite
{
	public BattleScene scene; // This should maybe use the IScene but i cant be bothered it's only appearing in this scene.

	public RectangleCollider collider;

	public float speed = 200f;
	public int health = 6;
	private HealthBar healthBar;

	public float attackCooldown = 3.0f;
	private float attackCooldownFinished;

	public Enemy(BattleScene scene, HealthBar healthBar, ContentManager contentManager) : base(Vector2.Zero, "Sprite/Enemy", contentManager)
	{
		this.scene = scene;
		this.attackCooldownFinished = scene.sceneTime;
		this.healthBar = healthBar;
		this.collider = new RectangleCollider(this, new Point(3, 3), new Point(34, 34)); 
	}	

	public void Update(GameTime gameTime)
	{
		Vector2 direction = GetDirection();
		if (direction != Vector2.Zero) Vector2.Normalize(direction);

		position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
	}

	private void Attack()
	{
		
	}

	public void TakeDamage()
	{
		health -= 1;
		healthBar.currentValue -= 50;
		healthBar.currentSprite = healthBar.ChangeHealthBarValue(healthBar.currentValue);
		Console.WriteLine(health);

		if (health == 0) {
			scene.BossKilled();
		}
	}

	private Vector2 GetDirection()
	{
		return Vector2.Zero;
	}
}