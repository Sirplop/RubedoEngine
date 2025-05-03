using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rubedo.Input;

/// <summary>
/// Manages and updates the current and previous frames' input states.
/// </summary>
public static class InputManager
{
    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        Mouse4,
        Mosue5
    }

    private static KeyboardState _previousKeyboardState;
    private static KeyboardState _currentKeyboardState;
    private static MouseState _previousMouseState;
    private static MouseState _currentMouseState;

    public static ushort FrameCounter => frameCounter;
    private static ushort frameCounter = 0;

    public static bool DoInput => doInput;
    private static bool doInput = true;

    public static void SetInputActive(bool set)
    {
        doInput = set;
    }

    public static void Update()
    {
        frameCounter++;

        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> is not currently pressed.
    /// </summary>
    public static bool KeyDown(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && doInput;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> is currently pressed.
    /// </summary>
    public static bool KeyUp(Keys key)
    {
        return _currentKeyboardState.IsKeyUp(key) && doInput;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> was released this frame. Triggers once per press.
    /// </summary>
    public static bool KeyReleased(Keys key)
    {
        return _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key) && doInput;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> was pressed this frame. Triggers once per press.
    /// </summary>
    public static bool KeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key) && doInput;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="key"/> has been held down since last frame.
    /// </summary>
    public static bool KeyHeld(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key) && doInput;
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> is currently pressed.
    /// </summary>
    public static bool MouseDown(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed && doInput;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed && doInput;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed && doInput;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Pressed && doInput;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Pressed && doInput;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> is not currently pressed.
    /// </summary>
    public static bool MouseUp(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Released && doInput;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Released && doInput;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Released && doInput;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Released && doInput;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Released && doInput;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> was released this frame. Triggers once per press.
    /// </summary>
    public static bool MouseReleased(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed && doInput;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed && doInput;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Released && _previousMouseState.MiddleButton == ButtonState.Pressed && doInput;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Released && _previousMouseState.XButton1 == ButtonState.Pressed && doInput;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Released && _previousMouseState.XButton2 == ButtonState.Pressed && doInput;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> has been held down since last frame.
    /// </summary>
    public static bool MouseHeld(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Pressed && doInput;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Pressed && doInput;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Pressed && doInput;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Pressed && _previousMouseState.XButton1 == ButtonState.Pressed && doInput;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Pressed && _previousMouseState.XButton2 == ButtonState.Pressed && doInput;
        }
        return false;
    }
    /// <summary>
    /// Returns whether or not the given <paramref name="button"/> was pressed this frame. Triggers once per press.
    /// </summary>
    public static bool MousePressed(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released && doInput;
            case MouseButtons.Right:
                return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released && doInput;
            case MouseButtons.Middle:
                return _currentMouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Released && doInput;
            case MouseButtons.Mouse4:
                return _currentMouseState.XButton1 == ButtonState.Pressed && _previousMouseState.XButton1 == ButtonState.Released && doInput;
            case MouseButtons.Mosue5:
                return _currentMouseState.XButton2 == ButtonState.Pressed && _previousMouseState.XButton2 == ButtonState.Released && doInput;
        }
        return false;
    }

    public static Vector2 MouseWorldPosition()
    {
        return RubedoEngine.Instance.Camera.ScreenToWorldPoint(_currentMouseState.Position.ToVector2());
    }
    public static Vector2 MouseScreenPosition()
    {
        return _currentMouseState.Position.ToVector2();
    }
}