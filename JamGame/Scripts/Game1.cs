using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine.Global;

namespace JamGame;

public class Game1 : Game
{
    private IScene currentScene;

    private GraphicsDeviceManager _graphics;

    // Sprite Batches
    private SpriteBatch _spriteBatch;
    private SpriteBatch _uiSpriteBatch;

    // Render Targets
    public RenderTarget2D mainRenderTarget;

    private Rectangle upscaledDrawTarget;

    private SpriteFont testFont;

    public Game1()
    {
        // Initialise the Graphics Device Manager (Default)
        _graphics = new GraphicsDeviceManager(this);

        // Start searching for Content in this directory when loading things (Default)
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Set the window size.
        Globals.windowSize = new Point(320, 240);
        _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = true;

        mainRenderTarget = new RenderTarget2D(GraphicsDevice, Globals.windowSize.X, Globals.windowSize.Y, 
            false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

        this._graphics.SynchronizeWithVerticalRetrace = true;

        // Unlock Framerate.
        Globals.targetFPS = 120;
        base.IsFixedTimeStep = false;

        _graphics.ApplyChanges();

        // Set up the screen scaling initially.
        upscaledDrawTarget = ScreenScaling.ChangeResolution(_graphics, Globals.windowSize); // NOTE - THIS MAY NEED TO BE REDONE FOR WEB BUILDS

        // Set the content manager globally.
        Globals.contentManager = Content;
        Globals.graphicsDevice = GraphicsDevice;
        
        // Initialise the starting scene.
        currentScene = new BattleScene(this);//currentScene = new MenuScene(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Set up the sprite batch and set it globally.
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _uiSpriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.spriteBatch = _spriteBatch;
        Globals.uiSpriteBatch = _uiSpriteBatch;

        // TODO: use this.Content to load your game content here
        testFont = this.Content.Load<SpriteFont>("Monogram");
    }

    protected override void Update(GameTime gameTime)
    {
        // Exit the game if escape/back button is pressed.
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }

        KeyboardExtended.SetState();

        // Perform all game update logic
        currentScene.Update(gameTime);

        KeyboardExtended.SetPreviousState();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Start by drawing to a render target at native resolution (typically small pixel art resolution).
        GraphicsDevice.SetRenderTarget(mainRenderTarget);

        // Draw all objects to the RenderTarget2D, which is used to scale from native resolution to screen/window resolution.
        currentScene.Draw(_spriteBatch);

        // Draw the UI elements above the main game elements.
        //currentScene.DrawUI(_uiSpriteBatch);

        // Prepare to draw to the actual window.
        GraphicsDevice.SetRenderTarget(null);

        // Draw the RenderTarget managed above to the screen at the correct resolution.
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

        // Prepare for scaling, then draw debug info above the render target at native resolution.
        _spriteBatch.Draw(mainRenderTarget, upscaledDrawTarget, Color.White);
        currentScene.DrawDebug(gameTime);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void SwitchScene(IScene scene)
    {
        currentScene = scene;
    }
}
