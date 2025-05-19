using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rubedo.UI;

/// <summary>
/// Base helper class for controlling the GUI.
/// </summary>
public static class GUI
{
    /// <summary>
    /// Set to false to disable UI input processing.
    /// </summary>
    public static bool DoUIInput { get; set; } = true;

    /// <summary>
    /// Controls when mouse vs keyboard/gamepade controls are used.
    /// </summary>
    public static bool MouseControlsEnabled { get; set; } = true;

    /// <summary>
    /// SpriteBatch used to draw the UI. We don't use a <see cref="Rendering.Renderer"/> because we're rendering screen coordinates.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; set; } = null!;
    /// <summary>
    /// FontSystem used in the UI.
    /// </summary>
    //public static FontSystem FontSystem { get; set; } = null!;
    /// <summary>
    /// Defaults to LinearClamp.
    /// </summary>
    public static SamplerState GuiSampler { get; set; } = SamplerState.PointClamp;

    /// <summary>
    /// Current GUIRoot.
    /// </summary>
    public static GUIRoot Root { get; set; } = null!;

    private static readonly RasterizerState _rasterState = new RasterizerState { ScissorTestEnable = true };
    private static bool _beginCalled = false;
    private static readonly Stack<(Rectangle, bool)> _scissorStack = new Stack<(Rectangle, bool)>();

    public static void Setup(Game game)
    {
        SpriteBatch = new SpriteBatch(game.GraphicsDevice);
    }

    /* //immediate mode scissoring.
    public static void PushScissor(Rectangle r)
    {
        if (!_beginCalled)
        {
            Begin();
            _scissorStack.Push((SpriteBatch.GraphicsDevice.ScissorRectangle, true));
            SpriteBatch.GraphicsDevice.ScissorRectangle = r;
        }
        else
        {
            _scissorStack.Push((SpriteBatch.GraphicsDevice.ScissorRectangle, false));
            SpriteBatch.GraphicsDevice.ScissorRectangle = r;
        }
    }
    public static void PopScissor()
    {
        (SpriteBatch.GraphicsDevice.ScissorRectangle, bool wasBeginCalled) = _scissorStack.Pop();
        if (wasBeginCalled)
            End();
    }*/

    //deferred mode scissoring.
    public static void PushScissor(Rectangle r)
    {
        // TODO: Optimize begin call somehow. Maybe there is no drawing between scissor swaps?
        bool wasBeginCalled = _beginCalled;
        if (wasBeginCalled)
        {
            End();
        }

        _scissorStack.Push((SpriteBatch.GraphicsDevice.ScissorRectangle, wasBeginCalled));
        SpriteBatch.GraphicsDevice.ScissorRectangle = r;
        Begin();
    }
    /// <summary>
    /// Uses a rectangle to limit the area that the spritebatch is allowed to draw to.
    /// </summary>
    public static void PopScissor()
    {
        if (_beginCalled)
            End();

        bool wasBeginCalled;

        (SpriteBatch.GraphicsDevice.ScissorRectangle, wasBeginCalled) = _scissorStack.Pop();
        if (wasBeginCalled)
            Begin();
    }
    /// <summary>
    /// Calls begin on the spritebatch with the UI rasterizer state, transform matrix and sampler state.
    /// </summary>
    private static void Begin()
    {
        SpriteBatch.Begin(sortMode: SpriteSortMode.Deferred, rasterizerState: _rasterState, samplerState: GuiSampler);
        _beginCalled = true;
    }
    /// <summary>
    /// Calls end on the spritebatch.
    /// </summary>
    private static void End()
    {
        SpriteBatch.End();
        _beginCalled = false;
    }
}