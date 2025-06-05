using Microsoft.Xna.Framework;
using Rubedo.Input;
using Rubedo.Rendering.Viewports;
using System;

namespace Rubedo.UI;

/// <summary>
/// The root component for all UI. Updates and layouts are propogated from this. Should (probably) only ever have one.
/// </summary>
public class GUIRoot : UIComponent, IDisposable
{
    private bool _disposed;

    public Selectable CurrentFocus => _currentFocus;
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

    public override void Draw()
    {
        GUI.UICamera.SetViewport();
        base.Draw();
        GUI.UICamera.ResetViewport();
    }
}
