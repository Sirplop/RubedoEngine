using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Rubedo;

/// <summary>
/// I am InputHelper, and this is my summary.
/// </summary>
public class InputManager
{
    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        Mouse4,
        Mosue5
    }

    protected KeyboardState _previousKeyboardState;
    protected KeyboardState _currentKeyboardState;
    protected MouseState _previousMouseState;
    protected MouseState _currentMouseState;
    public InputManager() { }

    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> is not currently pressed.
    /// </summary>
    public bool KeyDown(Keys key)
    {
        return Keyboard.GetState().IsKeyDown(key);
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> is currently pressed.
    /// </summary>
    public bool KeyUp(Keys key)
    {
        return Keyboard.GetState().IsKeyUp(key);
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> was released this frame. Triggers once per press.
    /// </summary>
    public bool KeyReleased(Keys key)
    {
        return _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> was pressed this frame. Triggers once per press.
    /// </summary>
    public bool KeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> is currently pressed.
    /// </summary>
    public bool MouseDown(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Pressed;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Pressed;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> is not currently pressed.
    /// </summary>
    public bool MouseUp(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Released;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Released;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Released;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Released;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Released;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> was released this frame. Triggers once per press.
    /// </summary>
    public bool MouseReleased(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Released && _previousMouseState.MiddleButton == ButtonState.Pressed;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Released && _previousMouseState.XButton1 == ButtonState.Pressed;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Released && _previousMouseState.XButton2 == ButtonState.Pressed;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> was pressed this frame. Triggers once per press.
    /// </summary>
    public bool MousePressed(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Released;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Pressed && _previousMouseState.XButton1 == ButtonState.Released;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Pressed && _previousMouseState.XButton2 == ButtonState.Released;
        }
        return false;
    }

    public Vector2 MouseWorldPosition()
    {
        return RubedoEngine.Instance.Camera.ScreenToWorldPoint(_currentMouseState.Position.ToVector2());
    }
    public Vector2 MouseScreenPosition()
    {
        return _currentMouseState.Position.ToVector2();
    }
}