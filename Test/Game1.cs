using Learninging.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rubedo.Debug;
using Rubedo.Object;
using Rubedo;
using System.IO;

namespace Learninging;

public class Game1 : Rubedo.RubedoEngine
{
    public Game1() : base() { }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        _stateManager.AddState(new TestState(_stateManager, _inputManager));

        _stateManager.SwitchState("ball");

        debugText = new DebugText(AssetManager.LoadFont("Consolas"), Color.White);
        Entity textEnt = new Entity
        {
            debugText
        };
        _stateManager.CurrentState().Add(textEnt);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected Texture2D LoadTextureThing(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            return Texture2D.FromStream(GraphicsDevice, fs);
        }
    }
}