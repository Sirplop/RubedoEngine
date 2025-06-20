using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rubedo.Graphics;

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
    #region Keyboard
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
    #endregion
    #region Mouse
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

    /// <summary>
    /// Gets the mouse cursor's position in world-space, according to a camera.
    /// </summary>
    /// <param name="camera">The camera to position by. If null, the <see cref="GameState.MainCamera"/> will be used.</param>
    /// <returns></returns>
    public static Vector2 MouseWorldPosition(Camera camera = null)
    {
        if (camera == null)
            return RubedoEngine.CurrentState.MainCamera.ScreenToWorld(_currentMouseState.Position.ToVector2());
        return camera.ScreenToWorld(_currentMouseState.Position.ToVector2());
    }
    /// <summary>
    /// Gets the mouse cursor's position in screen-space, according to a camera.
    /// </summary>
    /// <param name="camera">The camera to position by. If null, the <see cref="GameState.MainCamera"/> will be used.</param>
    /// <returns></returns>
    public static Vector2 MouseScreenPosition(Camera camera = null)
    {
        if (camera == null)
            camera = RubedoEngine.CurrentState.MainCamera;
        return _currentMouseState.Position.ToVector2() - camera.VirtualViewport.XY;
    }
    /// <summary>
    /// Position-agnostic screen-space movement vector for the mouse.
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetMouseMovement()
    {
        return (_currentMouseState.Position - _previousMouseState.Position).ToVector2();
    }
    #endregion
}