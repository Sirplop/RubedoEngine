using Microsoft.Xna.Framework;
using Rubedo.Input;
using Rubedo.Lib;
using Rubedo.Rendering;
using Rubedo.Rendering.Viewports;
using System;

namespace Rubedo.UI;

/// <summary>
/// The root component for all UI. Updates and layouts are propogated from this. Should (probably) only ever have one.
/// </summary>
public class GUIRoot : UIComponent, IDisposable, IRenderable
{
    private bool _disposed;

    public Selectable CurrentFocus => _currentFocus;

    public RectF Bounds => RubedoEngine.Graphics.GraphicsDevice.Viewport.Bounds;

    public bool Visible { get => _visible; set => _visible = value; }
    private bool _visible = true;

    public float LayerDepth { get => _layerDepth; set => _layerDepth = value; }
    private float _layerDepth = 0;
    public int RenderLayer { get => _renderLayer; set => _renderLayer = value; }
    private int _renderLayer = (int)Rendering.RenderLayer.UI;

    protected Selectable _currentFocus = null;
    private const float MOUSE_DEADZONE = 10f; //squared value

    public GUIRoot() : base()
    {
        _disposed = false;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sets a <see cref="Selectable"/> to be the active focus. May be set to null to remove focus.
    /// </summary>
    public void GrabFocus(Selectable? c)
    {
        if (CurrentFocus != null)
            CurrentFocus.IsFocused = false;
        if (c != null)
            c.IsFocused = true;
    }
    /// <summary>
    /// Sets the focus directly, used by <see cref="Selectable.IsFocused"/>.
    /// </summary>
    internal void GrabFocusInternal(Selectable? c)
    {
        _currentFocus = c;
    }

    /// <summary>
    /// Called before updates, but after input updates.
    /// </summary>
    /// <param name="processInput"></param>
    public void UpdateStart(bool processInput)
    {
        UpdateSetup();
        if (processInput)
            UpdateInput();
        Update();
    }

    private float _mouseMove;
    public override void UpdateInput()
    {
        if (!GUI.MouseControlsEnabled)
        { //check for when the mouse wakes up.
            Vector2 v = InputManager.GetMouseMovement();
            _mouseMove += v.LengthSquared();
            if (_mouseMove > MOUSE_DEADZONE)
            {
                GUI.MouseControlsEnabled = true;
                _mouseMove = 0;
            }
        }
        base.UpdateInput();
    }

    public override void UpdateLayout()
    {
        // The GUIRoot manages itself.

        Rectangle curClip = RubedoEngine.Graphics.GraphicsDevice.Viewport.Bounds;
        if (curClip != _clip)
        {
            _clip = curClip;
            _clipVersion++;
        }
        else
        {
            isLayoutDirty = false;
        }
        SetAnchorAndOffset(Anchor.TopLeft, Vector2.Zero);
        Height = curClip.Height;
        Width = curClip.Width;

        foreach (UIComponent c in _children)
        {
            c.UpdateSizes();
            c.UpdateClipIfDirty();
            c.UpdateLayout();
        }
    }

    /// <summary>
    /// Called after updates.
    /// </summary>
    public void UpdateEnd()
    {
        UpdateLayout();
    }

    public bool IsVisibleToCamera(Camera camera)
    {
        return true; //gui is always rendered.
    }

    public void Render(Renderer renderer, Camera camera)
    {
        base.Draw();
    }
}
