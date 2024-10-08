using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Engine.Global;

namespace JamGame;

public class Player : Sprite
{
	public BattleScene scene; // This should maybe use the IScene but i cant be bothered it's only appearing in this scene.

	public RectangleCollider collider;

	private Lazer lazer;
	private Enemy enemy;

	public float speed = 200f;
	private float rawLazerStrength = 0f; // This is the time between 0 and 1 that the lazer takes to charge
	public float uniformLazerStrength = 0f; // This is the scaled time, based on a sqrt function.

	public bool chargingAttack = false;

	public bool damaged = false;

	public float attackCooldown = 3.0f;
	private float attackCooldownFinished;

	public float invlunCooldown = 1.5f;
	private float invlunCooldownFinished;

	public Player(BattleScene scene, Lazer lazer, Enemy enemy, ContentManager contentManager) : base(Vector2.Zero, "Sprite/Player", contentManager)
	{
		this.scene = scene;
		this.lazer = lazer;
		this.lazer.player = this;
		this.enemy = enemy;

		this.attackCooldownFinished = scene.sceneTime;
		this.invlunCooldownFinished = scene.sceneTime + 5f;
		this.collider = new RectangleCollider(this, new Point(-7, -7), new Point(14, 14)); 
	}	

	public void Update(GameTime gameTime)
	{
		// Movement Code.
		Vector2 direction = GetDirection();
		Vector2 oldPosition = position; // Stop the player from getting completely stuck if they go outside of the arena
		
		if (direction != Vector2.Zero) Vector2.Normalize(direction);
		position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

		// If the player would move outside of a 320 unit radius from the centre of the arena, don't move.
		if (Math.Sqrt(Math.Pow(position.X, 2) + Math.Pow(position.Y + 50, 2)) > 320) {
			Console.WriteLine(position);
			position = oldPosition;
			Console.WriteLine(position);
		}

		collider.UpdateCollider();

		// Cycle through the enemy/enemy projectiles and do collision checks.
		if ((float)gameTime.TotalGameTime.TotalSeconds >= invlunCooldownFinished) {
			if (collider.CollisionEntered(enemy.collider, gameTime)) {
				TakeDamage(gameTime);
				invlunCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + invlunCooldown;
			}
	
			for (int i = 0; i < enemy.spines.Count; i++) {
				if (collider.CollisionEntered(enemy.spines[i].collider, gameTime)) {
					TakeDamage(gameTime);
					invlunCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + invlunCooldown;
				}
			}
		}

		MouseState mouseState = Mouse.GetState();

		// These changes will need to be made to ScreenScaling and used for all mouse inputs in all games using these tools.
		Vector2 mousePosition = new Vector2((mouseState.X - ScreenScaling.topLeft.X) / ScreenScaling.scale, 
			(mouseState.Y - ScreenScaling.topLeft.Y) / ScreenScaling.scale);

		Vector2 line = mousePosition - new Vector2(160, 170);

		// Set up the lazer's position and rotation.
		lazer.CalculatePosition(line);

		// Make sure charging doesn't happen all the time, reset visuals of charging.
		// All of this code could be set up a little more neatly into a formal state machine, but who cares about best practices?
		// Actually, me next week cares. a lot. because me next week wont be able to read it otherwise 
		// (this ones not too bad but some of the other things i've seen...)
		if ((mouseState.LeftButton == ButtonState.Released || direction != Vector2.Zero) && chargingAttack) {
			attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + attackCooldown;
			chargingAttack = false;
			scene.lazerChargeInstance.Stop();
			uniformLazerStrength = 0f;
			rawLazerStrength = 0f;
		}

		// Check to start charging
		if (!chargingAttack && mouseState.LeftButton == ButtonState.Pressed && 
			(float)gameTime.TotalGameTime.TotalSeconds >= attackCooldownFinished) {
			attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + attackCooldown;
			chargingAttack = true;
			scene.lazerChargeInstance.Play();
		}

		// If the player is charging an attack, increase the charge color strength (this is used as a uniform).
		if (chargingAttack) {
			rawLazerStrength += (float)gameTime.ElapsedGameTime.TotalSeconds;
			uniformLazerStrength = 4 * (float)Math.Pow(rawLazerStrength - 0.5f, 3) + 0.5f; // This will take one second to charge.

			if (uniformLazerStrength >= 1f) {
				Attack(line);
				scene.lazerShoot.Play();
				chargingAttack = false;
				uniformLazerStrength = 0f;
				rawLazerStrength = 0f;
			}
		}
	}

	private void Attack(Vector2 line)
	{
		line *= 200f;

		Vector2[] points = new Vector2[6];

		float uA = 0;
		float uB = 0;

		bool intersecting = false;

		// Prepares points for collision check.
		points[1] = points[5] = new Vector2(enemy.collider.collider.Left, enemy.collider.collider.Top);
		points[2] = new Vector2(enemy.collider.collider.Right, enemy.collider.collider.Top);
		points[3] = new Vector2(enemy.collider.collider.Right, enemy.collider.collider.Bottom);
		points[4] = points[0] = new Vector2(enemy.collider.collider.Left, enemy.collider.collider.Bottom);

		for (int i = 1; i < 5; i++) {
			float denominator = (points[i].Y - points[i - 1].Y) * (line.X - position.X) - 
				(points[i].X - points[i - 1].X) * (line.Y - position.Y);

			// First check if dividing by 0... if so, the lines are parallel and there is a collision
			if (denominator == 0) {
				intersecting = true;
				break;
			}

			// Then check to see if there are any other intersections
			// points[i] = 4, points[i-1] = 3, line = 2, position = 1
			uA = ((points[i].X - points[i - 1].X) * (position.Y - points[i - 1].Y) - 
				(points[i].Y - points[i - 1].Y) * (position.X - points[i - 1].X)) / denominator;
			uB = ((line.X - position.X) * (position.Y - points[i - 1].Y) - 
				(line.Y - position.Y) * (position.X - points[i - 1].X)) / denominator; 

			if (uA > 0 && uA <= 1 && uB > 0 && uB <= 1) intersecting = true;
		}

		if (intersecting) {
			enemy.TakeDamage();
			scene.bossHurt.Play();
		}
	}

	public void TakeDamage(GameTime gameTime)
	{
		scene.playerHurt.Play();
		
		// Stop lazer if charging
		if (chargingAttack) {
			attackCooldownFinished = (float)gameTime.TotalGameTime.TotalSeconds + attackCooldown;
			chargingAttack = false;
			scene.lazerChargeInstance.Stop();
			uniformLazerStrength = 0f;
		}
		
		scene.lightStrength = scene.lightStrength * 0.75f;

		if (scene.lightStrength < 0.5f) scene.lightStrength = 0.5f; // TEST THIS NUMBER.
	}

	private Vector2 GetDirection()
	{
		Vector2 direction = Vector2.Zero;
		KeyboardState keyboardState = Keyboard.GetState();

		if (keyboardState.IsKeyDown(Keybinds.left)) {
			direction += new Vector2(-1, 0);
		}
		if (keyboardState.IsKeyDown(Keybinds.right)) {
			direction += new Vector2(1, 0);
		}
		if (keyboardState.IsKeyDown(Keybinds.up)) {
			direction += new Vector2(0, -1);
		}
		if (keyboardState.IsKeyDown(Keybinds.down)) {
			direction += new Vector2(0, 1);
		}

		return direction;
	}
}