using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog;
using Rubedo.UI;
using Rubedo.Rendering;
using Rubedo.Input;
using Rubedo.EngineDebug;
using Rubedo.Internal.Assets;
using Rubedo.Physics2D.Common;
using System;
using Rubedo.Rendering.Viewports;

namespace Rubedo;

/// <summary>
/// Base game class for Rubedo. Extend this with your own stuff.
/// </summary>
public class RubedoEngine : Game
{
    public static float SizeOfMeter = 30f;

    public static RubedoEngine Instance { get; private set; }
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static NLog.Logger Logger { get; protected set; }

    protected internal Renderer _renderer;
    protected NeoCamera _camera;
    protected StateManager _stateManager;
    protected PhysicsWorld _physicsWorld;

    public NeoCamera Camera => _camera;
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
        Graphics.SynchronizeWithVerticalRetrace = true; //vsync
        this.Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _renderer = new Renderer(this);
        _camera = new NeoCamera(new PixelViewport(GraphicsDevice, Window, 500, 500), 0);

        GUI.Setup(this, _camera);
        GUI.Root = new GUIRoot();

        _physicsTimer = new Timer();

        base.Initialize(); //this calls LoadContent, so it must happen last.
    }

    public bool physicsOn = true;
    public bool stepPhysics = false;
    protected override void Update(GameTime gameTime)
    {
        Time.UpdateTime(gameTime);

        if (IsActive)
        {
            InputManager.Update();
            GUI.Root?.UpdateStart(GUI.DoUIInput);
        }

        _stateManager.Update();
        _physicsTimer.Start();
        if (physicsOn || stepPhysics)
        {
            _physicsWorld.Tick(Time.DeltaTime);
            stepPhysics = false;
        }
        _physicsTimer.Stop();

        GUI.Root?.UpdateEnd();

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        _camera.SetViewport();
        GraphicsDevice.Clear(Color.Black);
        _renderer.Begin(_camera, SamplerState.PointClamp);
        _stateManager.Draw(_renderer);
        _renderer.End();
        _camera.ResetViewport();
        GUI.Root.Draw();

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