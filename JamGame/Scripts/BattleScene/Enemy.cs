using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using Engine.Core;
using Engine.Global;

namespace JamGame;

public class Enemy : Sprite
{
	public BattleScene scene; // This should maybe use the IScene but i cant be bothered it's only appearing in this scene.

	public RectangleCollider collider;

	public float speed = 300f;
	public int health = 12;
	private HealthBar healthBar;

	public Vector2 screenOffset;

	private int attackIndex = 0;

	public bool charging = false;

	public float attackCooldown = 3.0f;
	private float attackCooldownFinished;

	public List<Spine> spines;
	public EnemySides[] sides;

	private string[] battleText;
	private int battleTextIndex = -1;
	private float battleTextTimer = 0;

	public Enemy(BattleScene scene, HealthBar healthBar, ContentManager contentManager) : base(Vector2.Zero, "Sprite/Enemy", contentManager)
	{
		this.scene = scene;
		this.attackCooldownFinished = 5f;//scene.sceneTime;
		this.healthBar = healthBar;
		this.collider = new RectangleCollider(this, new Point(-17, -17), new Point(34, 34));

		this.spines = new List<Spine>();
		this.sides = new EnemySides[] {new EnemySides(this, true), new EnemySides(this, false)};

		this.screenOffset = new Vector2(0, -100);

		this.battleText = new string[] {"You're too weak to\nfight this battle", "Don't even try, it's not worth losing.", 
			"This won't work out for you", "Run away. It's your only chance", "You can't win this, no matter\nhow strong you think you are."};
	}	

	public void Update(GameTime gameTime)
	{
		Random random = new Random();

		if (!charging) {
			position = scene.camera.position + screenOffset + new Vector2(random.Next(-1, 1), random.Next(-1, 1));
		}
		else {
			position += speed * new Vector2(0, 1) * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		for (int i = 0; i < sides.Length; i++) {
			sides[i].Update(gameTime);
		}

		collider.UpdateTrigger();

		if (gameTime.TotalGameTime.TotalSeconds >= attackCooldownFinished) {
			Attack(gameTime);
		}

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

				for (int i = 0; i < sides.Length; i++) {
					sides[i].direction = Vector2.Normalize(scene.player.position - sides[i].position);
				}
				
				break; 
			case 1: // Dummy to reset charge
				charging = false;
				attackIndex += 1;
				attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + 2f;
				break;
			case 2: // Expanding circle
				charging = false;
				attackIndex += 1;
				attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + 2f;
				
				numberOfSpines = (5 + (12 - health));

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

				numberOfSpines = 8 +(12 - health);

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

	public new void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(spritesheet, position - new Vector2(spriteWidth / 2, spriteHeight / 2), currentSprite, Color.White); 
    }

    public void DrawUI(SpriteBatch _spriteBatch)
    {
    	if (scene.sceneTime <= battleTextTimer && health > 0) _spriteBatch.DrawString(scene.bossFont, battleText[battleTextIndex], 
        	new Vector2(40, 60), Color.Magenta);
    }

	public void TakeDamage()
	{
		health -= 1;
		healthBar.currentValue -= 25;
		healthBar.currentSprite = healthBar.ChangeHealthBarValue(healthBar.currentValue);

		if (health % 2 == 0) {
			battleTextTimer = scene.sceneTime + 5f;
			battleTextIndex += 1;
		}

		if (health == 0) {
			scene.BossKilled();
		}
	}

	private Vector2 GetDirection()
	{
		return Vector2.Zero;
	}
}