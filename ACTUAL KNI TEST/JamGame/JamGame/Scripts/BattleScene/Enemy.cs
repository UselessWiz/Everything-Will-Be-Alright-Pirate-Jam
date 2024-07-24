using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Global;

namespace JamGame;

public class Enemy : Sprite
{
	public BattleScene scene; // This should maybe use the IScene but i cant be bothered it's only appearing in this scene.

	public RectangleCollider collider;

	public float speed = 2f;
	public int health = 6;
	private HealthBar healthBar;

	private Vector2 screenOffset;

	private int attackIndex = 0;

	private bool charging = false;

	public float attackCooldown = 3.0f;
	private float attackCooldownFinished;

	public List<Spine> spines;

	public Enemy(BattleScene scene, HealthBar healthBar, ContentManager contentManager) : base(Vector2.Zero, "Sprite/Enemy", contentManager)
	{
		this.scene = scene;
		this.attackCooldownFinished = 5f;//scene.sceneTime;
		this.healthBar = healthBar;
		this.collider = new RectangleCollider(this, new Point(-17, -17), new Point(34, 34));

		this.spines = new List<Spine>();

		this.screenOffset = new Vector2(0, -100);
	}	

	public void Update(GameTime gameTime)
	{
		Random random = new Random();

		//Console.WriteLine($"{gameTime.TotalGameTime.TotalSeconds} - {attackCooldownFinished}");

		if (!charging) {
			position = scene.camera.position + screenOffset;// + new Vector2(random.Next(-1, 1), random.Next(-1, 1));
		}
		else {
			position += speed * new Vector2(0, 1);
		}

		collider.UpdateTrigger();
		//Console.WriteLine($"{position} - {collider.collider.Location}");

		if (gameTime.TotalGameTime.TotalSeconds >= attackCooldownFinished) {
			Attack(gameTime);
		}

		// I believe this code functions.
		for (int i = 0; i < spines.Count; i++) {
			spines[i].Update(gameTime);
			
			if (spines[i].timeUp) {
				spines.RemoveAt(i);
				i -= 1;
			}

			if (i >= spines.Count - 1) break;
		}
	}

	private void Attack(GameTime gameTime)
	{
		int numberOfSpines;

		switch (attackIndex) {
			case 0: // Charge attack.
				charging = true;
				attackIndex += 1;
				attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + 2f;
				break; 
			case 1: // Dummy to reset charge
				charging = false;
				attackIndex += 1;
				attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + 2f;
				break;
			case 2: // Expanding circle
				charging = false;
				attackIndex += 1;
				attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + 5f;
				
				numberOfSpines = (5 + 2 * (6 - health));

				for (int i = 0; i < numberOfSpines + 1; i++) { 
					Vector2 direction = Vector2.Normalize(new Vector2((float)Math.Cos(MathHelper.Pi / 3 + (i * (MathHelper.Pi / 3) / numberOfSpines)),
						(float)Math.Sin(MathHelper.Pi / 3 + (i * (MathHelper.Pi / 3) / numberOfSpines))));
					spines.Add(new Spine(this, position + new Vector2(0, 40), direction, 100f + 10 * (health - 6), 
						"Sprite/Spine Projectile", scene.gameManager.Content)); 
				}

				break; 
			case 3: // Toriel
				attackIndex = 0;
				attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + 3f;

				numberOfSpines = 8 + 2 * (6 - health);

				for (int i = 0; i < numberOfSpines; i++) {
					Vector2 projectilePosition = position + new Vector2(-155, 65 + i * (170 / numberOfSpines));
					spines.Add(new Spine(this, projectilePosition, Vector2.Normalize(scene.player.position - projectilePosition), 
						120f + 10 * (health - 6), "Sprite/Spine Projectile Right Facing", scene.gameManager.Content));

					projectilePosition = position + new Vector2(155, 65 + i * ((235 - 65) / numberOfSpines));
					spines.Add(new Spine(this, projectilePosition, Vector2.Normalize(scene.player.position - projectilePosition), 
						120f + 10 * (health - 6), "Sprite/Spine Projectile Left Facing", scene.gameManager.Content));
				}

				break; 
		}
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