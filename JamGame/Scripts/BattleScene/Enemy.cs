using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
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

	private Vector2 screenOffset;

	private int attackIndex = 0;

	private bool charging = false;

	public float attackCooldown = 3.0f;
	private float attackCooldownFinished;

	private List<Spine> spines;

	public Enemy(BattleScene scene, HealthBar healthBar, ContentManager contentManager) : base(Vector2.Zero, "Sprite/Enemy", contentManager)
	{
		this.scene = scene;
		this.attackCooldownFinished = scene.sceneTime;
		this.healthBar = healthBar;
		this.collider = new RectangleCollider(this, new Point(3, 3), new Point(34, 34));

		this.spines = new List<Spine>();

		this.screenOffset = new Vector2(0, -80);
	}	

	public void Update(GameTime gameTime)
	{
		if (!charging) {
			position = scene.camera.position + screenOffset + new Vector2(Random.NextInt(-1, 1), Random.NextInt(-1, 1));
		}
		else {
			position += speed * new Vector2(0, 1);
		}

		if (gameTime.TotalGameTime.TotalSeconds >= attackCooldownFinished) {
			Attack(gameTime);
		}

		foreach (Spine spine in spines) {
			spines.Update(gameTime);
		}
	}

	private void Attack(GameTime gameTime)
	{
		switch (attackIndex) {
			case 0: // Charge attack.
				charging = true;
				attackCooldownFinished = gameTime.TotalGameTime.TotalSeconds + 4f;
				break; 
			case 1: // Expanding circle
				charging = false;
				attackCooldownFinished = gameTime.TotalGameTime.TotalSeconds + 5f;
				
				for (int i = 0; i < (5 + 2 * (health - 6)) + 1; i++) { // WORK OUT DIRECTION LATER USE NOTES
					spines.Add(new Spine(position + new Vector2(0, -40), new Vector2(0, -1), 150f, scene.gameManager.Content)); // remove old spines from the list.
				}

				break; 
			case 2: // Toriel
				attackCooldownFinished = gameTime.TotalGameTime.TotalSeconds + 3f;
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