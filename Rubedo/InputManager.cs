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
        Mouse1,
        Mouse2,
        Mouse3,
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

    public bool KeyDown(Keys key)
    {
        return Keyboard.GetState().IsKeyDown(key);
    }

    public bool KeyUp(Keys key)
    {
        return Keyboard.GetState().IsKeyUp(key);
    }
    public bool KeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    public bool MouseDown(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Mouse1:
                return _currentMouseState.LeftButton == ButtonState.Pressed;
            case MouseButtons.Mouse2:
                return _currentMouseState.RightButton == ButtonState.Pressed;
            case MouseButtons.Mouse3:
                return _currentMouseState.MiddleButton == ButtonState.Pressed;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Pressed;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Pressed;
        }
        return false;
    }
    public bool MouseUp(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Mouse1:
                return _currentMouseState.LeftButton == ButtonState.Released;
            case MouseButtons.Mouse2:
                return _currentMouseState.RightButton == ButtonState.Released;
            case MouseButtons.Mouse3:
                return _currentMouseState.MiddleButton == ButtonState.Released;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Released;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Released;
        }
        return false;
    }
    public bool MousePressed(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Mouse1:
                return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            case MouseButtons.Mouse2:
                return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
            case MouseButtons.Mouse3:
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
}