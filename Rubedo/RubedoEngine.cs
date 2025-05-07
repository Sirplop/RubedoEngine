using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using PhysicsEngine2D;
using Rubedo.UI;
using Rubedo.Rendering;
using Rubedo.Input;
using Rubedo.EngineDebug;
using NLog;
using NLog.Targets;
using NLog.Config;
using Rubedo.Internal.Assets;
using NLog.Layouts;

namespace Rubedo;

/// <summary>
/// I am Rubedo, and this is my summary.
/// </summary>
public class RubedoEngine : Game
{
    public static float SizeOfMeter = 30f;

    public static RubedoEngine Instance { get; private set; }
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static NLog.Logger Logger { get; protected set; }

    protected Renderer _renderer;
    protected Screen _screen;
    protected Camera _camera;
    protected StateManager _stateManager;
    protected PhysicsWorld _physicsWorld;

    public static float DeltaTime => Instance.deltaTime;
    public static float RawDeltaTime => Instance.rawDeltaTime;
    public static double RawTime => Instance.rawTime;

    protected float deltaTime;
    protected float rawDeltaTime;
    protected double rawTime;
    public float timeRate = 1.0f;

    public Camera Camera => _camera;
    public Screen Screen => _screen;
    public PhysicsWorld World => _physicsWorld;

    public Timer _physicsTimer;

    public RubedoEngine()
    {
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);

        SetupLogger();

        _stateManager = new StateManager();
        _physicsWorld = new PhysicsWorld();

        AssetManager.Initialize("Content");

        IsMouseVisible = true;
        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        _renderer = new Renderer(this);
        _screen = new Screen(this, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
        //_screen = new Screen(this, 640, 480);
        _camera = new Camera(_screen);
        _camera.SetZoom(1);

        GUI.Setup(this);
        GUI.Root = new GUIRoot();

        _physicsTimer = new Timer();

        base.Initialize(); //this calls LoadContent, so it must happen last.
    }

    public bool physicsOn = true;
    public bool stepPhysics = false;
    protected override void Update(GameTime gameTime)
    {
        rawTime = gameTime.TotalGameTime.TotalMilliseconds;
        rawDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        deltaTime = rawDeltaTime * timeRate;

        if (IsActive)
        {
            InputManager.Update();
            GUI.Root?.UpdateStart(GUI.DoUIInput);
        }

        _stateManager.Update();
        _physicsTimer.Start();
        if (physicsOn || stepPhysics)
        {
            _physicsWorld.Update(deltaTime);
            stepPhysics = false;
        }
        _physicsTimer.Stop();

        GUI.Root?.UpdateEnd();

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        _screen.Set();
        GraphicsDevice.Clear(Color.Black);
        _renderer.Begin(_camera, SamplerState.PointClamp);
        _stateManager.Draw(_renderer);
        _renderer.End();
        GUI.Root.Draw();
        _screen.Unset();
        _screen.Preset(_renderer, SamplerState.PointClamp);

        base.Draw(gameTime);
    }

    protected override void EndRun()
    {
        NLog.LogManager.Shutdown();
        base.EndRun();
    }

    /// <summary>
    /// Creates the rules and config for the NLog logger.
    /// </summary>
    protected virtual void SetupLogger()
    {
        LoggingConfiguration config = new NLog.Config.LoggingConfiguration();

        // Nicer log output.
        SimpleLayout layout = new SimpleLayout() { Text = "[${longdate}] [${level:uppercase=true}] ${literal:text=\\:} ${message:withexception=true}" };

        // Targets where to log to: File and Console
        //FileTarget logfile = new NLog.Targets.FileTarget("logfile") { FileName = "gamelog.txt" };
#if DEBUG
        ConsoleTarget logconsole = new NLog.Targets.ConsoleTarget("logconsole");
        DebuggerTarget logDebugConsole = new NLog.Targets.DebuggerTarget("debugconsole");
        logconsole.Layout = layout;
        logDebugConsole.Layout = layout;

        // Rules for mapping loggers to targets            
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logDebugConsole);
#endif
        //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

        // Apply config           
        NLog.LogManager.Configuration = config;

        //Load the logger.
        Logger = NLog.LogManager.GetCurrentClassLogger();
    }
}