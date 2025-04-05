using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Rubedo.Render;
using Rubedo.Physics2D;
using Rubedo.Debug;
using Rubedo.Object;
using System.Diagnostics;
using System;

namespace Rubedo;

/// <summary>
/// I am Rubedo, and this is my summary.
/// </summary>
public class RubedoEngine : Game
{
    public static float SizeOfMeter = 30f;

    public static RubedoEngine Instance { get; private set; }
    public static GraphicsDeviceManager Graphics { get; private set; }

    protected Renderer _renderer;
    protected Screen _screen;
    protected Camera _camera;
    protected StateManager _stateManager;
    protected InputManager _inputManager;
    protected PhysicsWorld _physicsWorld;

    public static float DeltaTime => Instance.deltaTime;
    public static float RawDeltaTime => Instance.rawDeltaTime;

    public float deltaTime { get; private set; }
    public float rawDeltaTime { get; private set; }
    public float timeRate = 1.0f;

    public Camera Camera => _camera;
    public Screen Screen => _screen;
    public PhysicsWorld World => _physicsWorld;

    public static DebugText debugText;
    private Stopwatch physicsWatch;
    public Stopwatch physicsPhaseWatch;

    public RubedoEngine()
    {
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        _stateManager = new StateManager();
        _inputManager = new InputManager();
        _physicsWorld = new PhysicsWorld();

        AssetManager.Initialize(Content);

        IsMouseVisible = true;
        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        _renderer = new Renderer(this);
        _screen = new Screen(this, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
        _camera = new Camera(_screen);
        _camera.SetZoom(1);

        physicsWatch = Stopwatch.StartNew();
        physicsPhaseWatch = Stopwatch.StartNew();

        base.Initialize(); //this calls LoadContent, so it must happen last.
    }

    public bool physicsOn = true;
    public bool stepPhysics = false;
    protected override void Update(GameTime gameTime)
    {
        rawDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        deltaTime = rawDeltaTime * timeRate;

        if (IsActive)
            _inputManager.Update();

        _stateManager.Update();
        physicsWatch.Restart();
        physicsWatch.Start();
        if (physicsOn || stepPhysics)
        {
            _physicsWorld.Step();
            stepPhysics = false;
        }
        physicsWatch.Stop();
        debugText.DrawText(new Vector2(0, 20), $"Bodies: {_physicsWorld.BodyCount} | Physics time: {physicsWatch.Elapsed.TotalMilliseconds.ToString("0.00")}", true);

        //_stateManager.Update();
        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        _screen.Set();
        GraphicsDevice.Clear(Color.Black);
        _renderer.Begin(_camera, SamplerState.PointClamp);
        _stateManager.Draw(_renderer);
        _renderer.End();
        _screen.Unset();
        _screen.Preset(_renderer, SamplerState.PointClamp);

        base.Draw(gameTime);
    }
}