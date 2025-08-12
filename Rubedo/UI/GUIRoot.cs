using Microsoft.Xna.Framework;
using Rubedo.Graphics;
using Rubedo.Input;
using Rubedo.Lib;
using System;
using System.Collections.Generic;

namespace Rubedo.UI;

/// <summary>
/// The root component for all UI. Updates and layouts are propogated from this. Should (probably) only ever have one.
/// </summary>
public class GUIRoot : UIComponent, IDisposable, IRenderable
{
    #region Properties
    private bool _disposed;

    public Selectable CurrentFocus => _currentFocus;

    public RectF Bounds => RubedoEngine.Graphics.GraphicsDevice.Viewport.Bounds;

    public bool Visible { get => _visible; set => _visible = value; }
    private bool _visible = true;

    public int LayerDepth
    {
        get => _realLayerDepth;
        set
        {
            //I don't like this coupling, but it's just for a value, so it's not THE WORST, I suppose.
            _layerDepth = (value * Components.RenderableComponent.LAYER_SCALE) - 5e-1f;
            _realLayerDepth = value;
        }
    }
    protected int _realLayerDepth = 0;
    protected float _layerDepth = 0;

    public int RenderLayer { get => _renderLayer; set => _renderLayer = value; }
    private int _renderLayer = (int)Rubedo.Graphics.Sprites.RenderLayer.UI;
    public int PixelPerUnit { get; set; }

    public Point TargetResolution 
    {
        get => _targetResolution;
        set
        {
            if (value != _targetResolution)
            {
                _targetResolution = value;
                MarkLayoutAsDirty();
            }
        }
    }
    protected Point _targetResolution;

    public bool DoScaling
    {
        get => _doScaling;
        set => _doScaling = value;
    }
    protected bool _doScaling;

    public float Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                SetMatrix();
                MarkLayoutAsDirty();
            }
        }
    }
    protected float _scale = 1f;

    protected override void SetOffset(Vector2 offset)
    {
        if (_offset != offset)
        {
            _offset = offset;
            SetMatrix();
            MarkLayoutAsDirty();
        }
    }

    private void SetMatrix()
    {
        _uiMatrix = Matrix.CreateScale(_scale, _scale, 1f);
        _invertUIMatrix = Matrix.Invert(_uiMatrix);
    }

    /// <summary>
    /// Used to convert between the screen and UI coordinate system.
    /// </summary>
    public Matrix UIMatrix => _uiMatrix;
    protected Matrix _uiMatrix = Matrix.CreateScale(1f, 1f, 1f);
    public Matrix InvertUIMatrix => _invertUIMatrix;
    protected Matrix _invertUIMatrix = Matrix.CreateScale(1f, 1f, 1f);

    protected Selectable _currentFocus = null;
    private const float MOUSE_DEADZONE = 10f; //squared value
#endregion

    /// <summary>
    /// Construct a new GUI Root node.
    /// </summary>
    /// <param name="targetResolution">The resolution to render to and scale from if scaling is enabled.</param>
    /// <param name="doScaling">If true, will scale the UI to a constant size no matter window size. If false, uses the current viewport.</param>
    public GUIRoot(Point targetResolution, bool doScaling = true) : base()
    {
        _disposed = false;
        _doScaling = doScaling;
        _targetResolution = targetResolution;
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
        Rectangle curClip = CalculateViewRect(out Vector2 offset);

        if (curClip != _clip)
        {
            _clip = curClip;
            _clipVersion++;
        }
        else
        {
            isLayoutDirty = false;
        }
        SetAnchorAndOffset(Anchor.TopLeft, offset);
        Height = _clip.Height;
        Width = _clip.Width;

        foreach (UIComponent c in _children)
        {
            c.UpdateSizes();
            c.UpdateClipIfDirty();
            c.UpdateLayout();
        }
    }


    private Rectangle CalculateViewRect(out Vector2 offset)
    {
        Rectangle curClip = RubedoEngine.Graphics.GraphicsDevice.Viewport.Bounds;
        if (_doScaling)
        {
            float ratioWidth = curClip.Width / (float)_targetResolution.X;
            float ratioHeight = curClip.Height / (float)_targetResolution.Y;

            if (ratioWidth < ratioHeight)
            {
                Scale = ratioWidth;
                curClip.Width = _targetResolution.X;
                curClip.Height = (int)(curClip.Height / ratioWidth);
            }
            else
            {
                Scale = ratioHeight;
                curClip.Width = (int)(curClip.Width / ratioHeight);
                curClip.Height = _targetResolution.Y;
            }
        }
        offset = new Vector2(curClip.X, curClip.Y);
        curClip.X = 0; curClip.Y = 0;
        return curClip;
    }

    public bool IsVisibleToCamera(Camera camera)
    {
        return true; //gui is always rendered.
    }

    public void Render(Renderer renderer, Camera camera)
    {
        UpdateLayout();
        base.Draw();
    }

    public Vector2 ScreenToUI(in Vector2 point)
    {
        return Vector2.Transform(point, InvertUIMatrix);
    }
    public Vector2 UIToScreen(in Vector2 point)
    {
        return Vector2.Transform(point, UIMatrix);
    }

    /// <summary>
    /// Renders a white box around every child component of this UI.
    /// </summary>
    public void DebugRender(Shapes shapes, Camera camera)
    {
        shapes.Begin(camera);
        Queue<UIComponent> draw = new Queue<UIComponent>();
        draw.Enqueue(this);
        Matrix matrix = UIMatrix;
        while (draw.TryDequeue(out UIComponent comp))
        {
            for (int i = 0; i < comp.Children.Count; i++)
                draw.Enqueue(comp.Children[i]);

            //Vector2 topLeft = new Vector2(comp.Clip.Left, comp.Clip.Top);
            //Vector2 bottomRight = new Vector2(comp.Clip.Right, comp.Clip.Bottom);

            Vector2 topLeft = Vector2.Transform(new Vector2(comp.Clip.Left, comp.Clip.Top), matrix) + Offset;
            Vector2 bottomRight = Vector2.Transform(new Vector2(comp.Clip.Right, comp.Clip.Bottom), matrix) + Offset;

            Vector2 worldTopLeft = camera.ScreenToWorld(topLeft);
            Vector2 worldBottomRight = camera.ScreenToWorld(bottomRight);

            shapes.DrawBox(worldTopLeft, worldBottomRight, Color.White);
        }
        shapes.End();
    }
}
