using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JamGame;

public class Spine : Sprite
{
	public Vector2 direction;
	public float speed;

	public float timer = 0;

	public Spine(Enemy enemy, Vector2 position, Vector2 direction, float speed, ContentManager contentManager) : base(position, "Sprite/Spine Projectile", contentManager) 
	{
		this.enemy = enemy;
		this.direction = direction;
		this.speed = speed;
	}

	public void Update(GameTime gameTime)
	{
		position += direction * speed * gameTime.ElapsedGameTime.TotalSeconds;

		timer += gameTime.ElapsedGameTime.TotalSeconds;

		if (timer >= 10f) {
			enemy.spines.Remove(this);
		}
	}
}