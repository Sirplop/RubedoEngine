using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Input;
using Rubedo.Lib;
using Rubedo.Object;
using System;
using System.Collections.Generic;

namespace Rubedo.UI;

/// <summary>
/// The root component for all UI. Updates and layouts are propogated from this. Should (probably) only ever have one.
/// </summary>
public class GUIRoot : UIComponent
{
    public Selectable CurrentFocus => _currentFocus;
    protected Selectable _currentFocus = null;
    private const float MOUSE_DEADZONE = 10f; //squared value

    public GUIRoot() : base()
    {

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

        _clip = new Rectangle(0, 0, RubedoEngine.Instance.Screen.Width, RubedoEngine.Instance.Screen.Height);
        SetAnchorAndOffset(Anchor.TopLeft, Vector2.Zero);
        Height = RubedoEngine.Instance.Screen.Height;
        Width = RubedoEngine.Instance.Screen.Width;
        isLayoutDirty = false; //this doesn't cause layout problems. Or should it? TODO.

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
        foreach (var c in _children)
            c.Draw();
    }

    public void Clear()
    {
        for (int i = 0; i < _children.Count; i++)
            _children[i].Destroy();
        _children.Clear();
    }
}
