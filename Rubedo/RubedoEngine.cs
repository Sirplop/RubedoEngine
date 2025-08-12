using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog;
using Rubedo.UI;
using Rubedo.Input;
using Rubedo.EngineDebug;
using Rubedo.Physics2D.Common;
using Rubedo.Graphics;
using Rubedo.Audio;
using System;
using Rubedo.Lib.Coroutines;

namespace Rubedo;

/// <summary>
/// Base game class for Rubedo. Extend this with your own stuff.
/// </summary>
public class RubedoEngine : Game
{
    public static float SizeOfMeter = 30f;

    public static RubedoEngine Instance { get; private set; }
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static AudioCore Audio { get; protected set; }
    public static StateManager StateManager => Instance._stateManager;
    public static GameState CurrentState => Instance._stateManager.CurrentState();

    public Renderer Renderer { get; protected set; }
    protected StateManager _stateManager;
    protected PhysicsWorld _physicsWorld;
    protected internal CoroutineManager _coroutineManager;

    public PhysicsWorld World => _physicsWorld;
    public Timer _physicsTimer;

    public RubedoEngine()
    {
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Audio = new AudioCore();

        SetupLogger();

        _stateManager = new StateManager();
        _physicsWorld = new PhysicsWorld();
        _coroutineManager = new CoroutineManager();

        Assets.Initialize("Content");

        IsMouseVisible = true;
        IsFixedTimeStep = false;
        Graphics.SynchronizeWithVerticalRetrace = true; //vsync
        this.Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        Renderer = new Renderer(this);

        GUI.Setup(this);
        _physicsTimer = new Timer();

        Lib.SDL2Extern.SDL_SetWindowMinimumSize(Window.Handle, 320, 320);

        base.Initialize(); //this calls LoadContent, so it must happen last.
    }

    public bool physicsOn = true;
    protected override void Update(GameTime gameTime)
    {
        Time.UpdateTime(gameTime);

        if (IsActive)
        {
            InputManager.Update();
            GUI.Root?.UpdateStart(GUI.DoUIInput);
        }

        FixedUpdate(Time.DeltaTime);
        _stateManager.Update();
        _coroutineManager.Update();

        base.Update(gameTime);
    }

    private double accumulatedDelta = 0;
    protected void FixedUpdate(float dt)
    {
        accumulatedDelta += dt;

        // Avoid accumulator death spiral
        if (accumulatedDelta > Time.FixedDeltaTime * 5)
            accumulatedDelta = Time.FixedDeltaTime * 5;

        _physicsTimer.Start();
        while (accumulatedDelta > Time.FixedDeltaTime)
        {
            if (physicsOn)
                _physicsWorld.Step(Time.FixedDeltaTime);

            _stateManager.FixedUpdate();
            accumulatedDelta -= Time.FixedDeltaTime;
        }
        _physicsTimer.Stop();
    }


    protected override void Draw(GameTime gameTime)
    {
        _stateManager.Draw(Renderer);

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
        SimpleLayout layout = new SimpleLayout("[${longdate}] [${level:uppercase=true}] ${literal:text=\\:} ${message:withexception=true}");

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
        Log.Logger = NLog.LogManager.GetCurrentClassLogger();
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
        base.OnActivated(sender, args);
        Audio.outputBus.UnpausePlayback();
    }
    protected override void OnDeactivated(object sender, EventArgs args)
    {
        base.OnActivated(sender, args);
        Audio.outputBus.PausePlayback();
    }
}