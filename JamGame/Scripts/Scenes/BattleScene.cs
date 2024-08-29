using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Engine.Core;
using Engine.Global;
using Engine.Animations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JamGame;

public class BattleScene : IScene
{
	public Game1 gameManager {get; set;}
	private SpriteFont debugFont;

	public Player player;
	private Enemy enemy;
	private Lazer lazer;
	public Camera camera;

	private Effect lightShader;
	private LightSource[] lights;
	private VertexPosition[] screenRectVertices;
	private short[] indices;

	public float lightStrength = 20f;

	private Effect lazerShader;
	private Effect bossHitShader;

	private HealthBar bossHealthBar;
	private HealthBar[] finalHealthBars;

	private AnimationClip finalHealthBarAnimation;

	private float endTextTimer = 0;
	public SpriteFont bossFont;
	private string[] bossFinalText;
	private Vector2[] bossFinalTextPositions;
	private int bossFinalTextIndex = 0;
	private bool finalHealthBarAnimPlaying = false;

	private bool musicPlayed = false;

	private string[] endText;
	private int endTextIndex = -1;
	private bool endTextShowing = false;

	private bool bossDefeated = false;

	public float sceneTime;

	private SoundEffect battleMusic;
	private SoundEffectInstance battleMusicInstance;

	private SoundEffect endingMusic;

	public SoundEffect lazerCharge;
	public SoundEffectInstance lazerChargeInstance;
	public SoundEffect lazerShoot;
	public SoundEffect playerHurt;
	public SoundEffect bossHurt;

	public BattleScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		gameManager.ChangeResolution(new Point(320, 240));

		this.bossHealthBar = new HealthBar(new Vector2(160, 6), "Sprite/UI/HealthBarFG", 300, gameManager.Content);

		this.lazer = new Lazer(gameManager.Content);
		this.enemy = new Enemy(this, bossHealthBar, gameManager.Content);
		this.player = new Player(this, lazer, enemy, gameManager.Content);
		this.camera = new Camera();

		this.lights = new LightSource[] {
			new LightSource(new Vector2(50, -40), 1f),
			new LightSource(new Vector2(50, 180), 1f),
			new LightSource(new Vector2(270, -40), 1f),
			new LightSource(new Vector2(270, 180), 1f)
		};

		this.screenRectVertices = new VertexPosition[] {
        	new VertexPosition(new Vector3(-Globals.windowSize.X / 2, -Globals.windowSize.Y / 2, 0)), 
        	new VertexPosition(new Vector3( Globals.windowSize.X / 2, -Globals.windowSize.Y / 2, 0)), 
			new VertexPosition(new Vector3( Globals.windowSize.X / 2,  Globals.windowSize.Y / 2, 0)), 
			new VertexPosition(new Vector3(-Globals.windowSize.X / 2,  Globals.windowSize.Y / 2, 0))
		};

		this.indices = new short[] {0, 1, 2, 0, 2, 3};

		PrepareFinalHealthBars();

		LoadContent();

		bossFinalText = ["You're a fool if you\nthink you've won.", 
			"Your hope isn't enough to\nstop these shadows combined...", "FEAR MUST CONSUME!"];
		bossFinalTextPositions = [new Vector2(80, 100), new Vector2(40, 40), 
			new Vector2(74, 110)];
		endText = ["Don't let your light burn out", "Don't let the shadows consume you", 
			"Don't forget that"];
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
		bossFont = gameManager.Content.Load<SpriteFont>("Low Gothic Battle");
		lightShader = gameManager.Content.Load<Effect>("Shaders/Arena Lighting");
		lazerShader = gameManager.Content.Load<Effect>("Shaders/Lazer");
		bossHitShader = gameManager.Content.Load<Effect>("Shaders/Boss Hit");
		
		battleMusic = gameManager.Content.Load<SoundEffect>("Audio/Everything Will Be Consumed");
		battleMusicInstance = battleMusic.CreateInstance();
		battleMusicInstance.IsLooped = true;
		battleMusicInstance.Play();

		lazerCharge = gameManager.Content.Load<SoundEffect>("Audio/SFX/Lazer Charge");
		lazerChargeInstance = lazerCharge.CreateInstance();
		lazerShoot = gameManager.Content.Load<SoundEffect>("Audio/SFX/Lazer Shoot");
		playerHurt = gameManager.Content.Load<SoundEffect>("Audio/SFX/Player Hurt");
		bossHurt = gameManager.Content.Load<SoundEffect>("Audio/SFX/Boss Hurt");

		endingMusic = gameManager.Content.Load<SoundEffect>("Audio/Everything Will Be Alright");
	}

	public void Update(GameTime gameTime)
	{
		sceneTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

		if (!bossDefeated) {
			player.Update(gameTime);
			enemy.Update(gameTime);
			lazer.Update(gameTime);

			camera.position = player.position + new Vector2(0, -50);
			camera.Update(gameTime);
		}

		else {
			if (finalHealthBarAnimPlaying) {
				finalHealthBarAnimation.Update(gameTime);
				if (sceneTime >= endTextTimer) {
					if (!musicPlayed) {
						endingMusic.Play();
						musicPlayed = true;
						endTextTimer += 5;
					}
					else {
						endTextShowing = true;
						endTextIndex += 1;

						endTextTimer += 14.3f;
						if (endTextIndex == endText.Length - 1) endTextTimer  = 5000000000000000f;
					}
				}
			}

			for (int i = 0; i < 49; i++) {
				finalHealthBars[i].currentValue = (int)finalHealthBarAnimation.values[i];
				finalHealthBars[i].currentSprite = finalHealthBars[i].ChangeHealthBarValue(finalHealthBars[i].currentValue);
			}

			if (sceneTime >= endTextTimer) {
				playerHurt.Play();
				endTextTimer += 5f;
				bossFinalTextIndex += 1;

				// Yeah yeah this is really really bad code theres only 2 days to go and i don't give a fuck anymore.
				if (bossFinalTextIndex == bossFinalText.Length) {
					finalHealthBarAnimation.playing = true;
					finalHealthBarAnimPlaying = true;
				}
			}
		}
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		// Yeah yeah this is bad code yada yada don't care.
		List<Spine> spines = enemy.spines;

		// Clear this buffer.
        gameManager.GraphicsDevice.Clear(Color.Black);

        // Draw the player and spines. They use the same effects (base sprite).
        _spriteBatch.Begin(transformMatrix: camera.translation);
        player.Draw(_spriteBatch);
        
        for (int i = 0; i < spines.Count; i++) {
        	spines[i].Draw(_spriteBatch);
        }
        _spriteBatch.End();

        // Draw the lights
        gameManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        lightShader.Parameters["WorldViewProjection"].SetValue(camera.translation * 
        	Matrix.CreateOrthographicOffCenter(0, Globals.windowSize.X, Globals.windowSize.Y, 0, 0, -1));

        gameManager.GraphicsDevice.BlendState = BlendState.Additive;

        // Prepare the light shader.
		VertexBuffer vertexBuffer = new VertexBuffer(gameManager.GraphicsDevice, typeof(VertexPosition), 6, BufferUsage.WriteOnly);
        vertexBuffer.SetData<VertexPosition>(screenRectVertices);
		gameManager.GraphicsDevice.SetVertexBuffer(vertexBuffer);

		IndexBuffer indexBuffer = new IndexBuffer(Globals.graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
		indexBuffer.SetData(indices);
		gameManager.GraphicsDevice.Indices = indexBuffer;

        foreach (LightSource light in lights) {
        	lightShader.Parameters["LightPosition"].SetValue(light.position - camera.position);
			lightShader.Parameters["LightBrightness"].SetValue(lightStrength);

			foreach (EffectPass pass in lightShader.CurrentTechnique.Passes)
        	{
            	pass.Apply();
            	gameManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        	}
        }

        gameManager.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

        // Draw the enemy
		bossHitShader.Parameters["hitStrength"].SetValue(enemy.hitAnimation.values[0]);
        _spriteBatch.Begin(transformMatrix: camera.translation,effect: bossHitShader);
        enemy.Draw(_spriteBatch);
        for (int i = 0; i < enemy.sides.Length; i++) {
			enemy.sides[i].Draw(_spriteBatch);
		}
        _spriteBatch.End();

        // Prepare the lazer shader and draw the lazer using it.
        lazerShader.Parameters["LazerStrength"].SetValue(player.uniformLazerStrength);
        _spriteBatch.Begin(transformMatrix: camera.translation, effect: lazerShader);
        lazer.Draw(_spriteBatch);
        _spriteBatch.End();
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Begin();
		bossHealthBar.DrawUI(_spriteBatch);
		enemy.DrawUI(_spriteBatch);
		_spriteBatch.End();

		if (!bossDefeated) return;

		_spriteBatch.Begin();

		foreach (HealthBar bar in finalHealthBars) {
			bar.drawn = false;
		}

		foreach (HealthBar bar in finalHealthBars) {
			if (bar.currentValue == 300) {
				bar.DrawUI(_spriteBatch);
				bar.drawn = true;
			}
		}

		foreach (HealthBar bar in finalHealthBars) {
			if (!bar.drawn) {
				bar.DrawUI(_spriteBatch);
			}
		}

		if (!finalHealthBarAnimPlaying) {
			_spriteBatch.DrawString(bossFont, bossFinalText[bossFinalTextIndex], bossFinalTextPositions[bossFinalTextIndex], Color.Magenta);
		}

		if (endTextShowing) {
			if (endTextIndex < endText.Length - 1) {
				_spriteBatch.DrawString(bossFont, endText[endTextIndex], new Vector2((180 - bossFont.MeasureString(endText[endTextIndex]).X / 2) - 20, 
					(120 - bossFont.MeasureString(endText[endTextIndex]).Y / 2) - 10), Color.White);
			}
			else {
				_spriteBatch.DrawString(bossFont, endText[endTextIndex], new Vector2((180 - bossFont.MeasureString(endText[endTextIndex]).X / 2) - 20, 
					(120 - bossFont.MeasureString(endText[endTextIndex]).Y / 2) - 10), Color.White);
				_spriteBatch.DrawString(bossFont, "Everything Will Be Alright", new Vector2((180 - bossFont.MeasureString(endText[endTextIndex]).X / 2) - 54, 
					(120 - bossFont.MeasureString(endText[endTextIndex]).Y / 2) + 10), Color.White);
			}
		}

		_spriteBatch.End();
	}

	public void DrawDebug(GameTime gameTime)
    {
        Globals.spriteBatch.DrawString(debugFont, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2 (10, 10), Color.Red);    
    }

    private void PrepareFinalHealthBars()
    {
    	this.finalHealthBars = new HealthBar[49];
    	Vector2[] finalHealthBarPositions = new Vector2[49];

    	// Set up positions and offsets for each healthbar
    	for (int i = 0; i < 25; i++) {
    		finalHealthBarPositions[i] = new Vector2(160, 10 * i);
    	}

    	for (int i = 25; i < 48; i++) {
			finalHealthBarPositions[i] = new Vector2(160, (10 * (i - 24)) - 5);
    	}

    	finalHealthBarPositions[48] = new Vector2(160, 245);

    	// Set up the animation data
    	float[] keyframeTimes = new float[49];
    	Animation[][] keyframeData = new Animation[49][];

    	for (int i = 0; i < 49; i++) {
    		finalHealthBars[i] = new HealthBar(finalHealthBarPositions[i], "Sprite/UI/HealthBarFG", 0, gameManager.Content);
    		keyframeTimes[i] = 0.5f * i;
    		keyframeData[i] = new Animation[1];
    		keyframeData[i][0] = new Animation(i, 0f, 300f, 0.5f);
    	}

    	// Create the animation
    	this.finalHealthBarAnimation = new AnimationClip(49, keyframeTimes, keyframeData, 24.5f, true); 
    }

    public void BossKilled()
    {
    	bossDefeated = true;
    	battleMusicInstance.Stop();
    	endTextTimer = sceneTime + 5f;
    	playerHurt.Play();
    }
}