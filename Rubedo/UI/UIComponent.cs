using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubedo.UI;

public enum Anchor
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}

/// <summary>
/// The base component type of all UI components.
/// </summary>
public abstract class UIComponent()
{
    #region Info
    public bool isActive = true;
    public bool isVisible = true;

    public UIComponent? Parent { get; set; } = null;
    protected internal readonly List<UIComponent> _children = new List<UIComponent>();

    public ReadOnlyCollection<UIComponent> Children => _children.AsReadOnly();

    public bool IsLayoutDirty => _isLayoutDirty;
    protected bool _isLayoutDirty = true;
    protected int _parentIndex = -1;

    /// <summary>
    /// Updated every time the clip gets updated, so children will know that it's been updated and they should too.
    /// </summary>
    internal uint _clipVersion = 0;
    /// <summary>
    /// Last known parent's clip version. Will update clip if does not match <see cref="_clipVersion"/>.
    /// </summary>
    private uint _parentLastClipVersion = 0;


    protected float _width = 100;
    public float Width
    {
        get => _width;
        set
        {
            if (_width != value)
            {
                _width = value;
                if (_minSize.HasValue && _minSize.Value.X > -1)
                    _width = MathF.Max(value, _minSize.Value.X);
                if (_maxSize.HasValue && _maxSize.Value.X > -1)
                    _width = MathF.Min(value, _maxSize.Value.X);
            }
        }
    }
    protected float _height = 100;
    public float Height
    {
        get => _height;
        set
        {
            if (_height != value)
            {
                _height = value;
                if (_minSize.HasValue && _minSize.Value.Y > -1)
                    _height = MathF.Max(value, _minSize.Value.Y);
                if (_maxSize.HasValue && _maxSize.Value.Y > -1)
                    _height = MathF.Min(value, _maxSize.Value.Y);
            }
        }
    }

    protected Vector2 _offset;
    /// <summary>
    /// Offset from the anchor position, relative to the parent.
    /// </summary>
    public Vector2 Offset
    {
        get => _offset;
        set => SetOffset(value);
    }

    protected Anchor _anchor;
    /// <summary>
    /// Anchor from which to position this entity.
    /// </summary>
    public Anchor Anchor
    {
        get => _anchor;
        set => SetAnchor(value);
    }
    #endregion

    /// <summary>
    /// The render destination of this component, clipped by its parent.
    /// </summary>
    public Rectangle Clip
    {
        get 
        {
            UpdateClipIfDirty();
            return _clip;
        }
    }
    protected Rectangle _clip;

    private Vector2? _minSize;
    /// <summary>
    /// Limits the size of this component to not go below this value.
    /// </summary>
    public Vector2? MinSize
    {
        get { return _minSize; }
        set { 
            if (_minSize != value) 
            { 
                _minSize = value;
                MarkLayoutAsDirty(); 
            } 
        }
    }

    private Vector2? _maxSize;
    /// <summary>
    /// Limits the size of this component to not go over this value.
    /// </summary>
    public Vector2? MaxSize
    {
        get { return _maxSize; }
        set { 
            if (_maxSize != value) 
            { 
                _maxSize = value;
                MarkLayoutAsDirty(); 
            } 
        }
    }
    /// <summary>
    /// Set the position and anchor of this entity.
    /// </summary>
    /// <param name="anchor">New anchor to set.</param>
    /// <param name="offset">Offset from new anchor position.</param>
    public void SetAnchorAndOffset(Anchor anchor, Vector2 offset)
    {
        SetAnchor(anchor);
        SetOffset(offset);
    }

    /// <summary>
    /// Set the anchor of this entity.
    /// </summary>
    /// <param name="anchor">New anchor to set.</param>
    private void SetAnchor(Anchor anchor)
    {
        if (_anchor != anchor)
        {
            _anchor = anchor;
            MarkLayoutAsDirty();
        }
    }

    /// <summary>
    /// Set the offset of this entity.
    /// </summary>
    /// <param name="offset">New offset to set.</param>
    private void SetOffset(Vector2 offset)
    {
        if (_offset != offset)
        {
            _offset = offset;
            MarkLayoutAsDirty();
        }
    }

    public virtual void UpdateClipIfDirty()
    {
        if (Parent != null && (_isLayoutDirty || _parentLastClipVersion != Parent._clipVersion))
        {
            CreateClipRect();
        }
    }

    protected virtual void UpdateClip()
    {
        CreateClipRect();
        _isLayoutDirty = false; 
        _clipVersion++;
        if (Parent != null)
        {
            _parentLastClipVersion = Parent._clipVersion;
        }
    }

    protected virtual void CreateClipRect()
    {
        Rectangle parentClip;
        if (Parent == null)
        { //no parent, use the root size
            parentClip = GUI.Root.Clip;
        }
        else
        {
            Parent?.UpdateClipIfDirty();
            parentClip = Parent.Clip;
        }

        Rectangle bounds = new Rectangle(0, 0, (int)Width, (int)Height);

        int parent_left = parentClip.X;
        int parent_top = parentClip.Y;
        int parent_right = parent_left + parentClip.Width;
        int parent_bottom = parent_top + parentClip.Height;
        int parent_center_x = parent_left + parentClip.Width / 2;
        int parent_center_y = parent_top + parentClip.Height / 2;

        switch (_anchor)
        {
            case Anchor.TopLeft:
                bounds.X = parent_left + (int)_offset.X;
                bounds.Y = parent_top + (int)_offset.Y;
                break;

            case Anchor.Top:
                bounds.X = parent_center_x - bounds.Width / 2 + (int)_offset.X;
                bounds.Y = parent_top + (int)_offset.Y;
                break;

            case Anchor.TopRight:
                bounds.X = parent_right - bounds.Width - (int)_offset.X;
                bounds.Y = parent_top + (int)_offset.Y;
                break;

            case Anchor.Left:
                bounds.X = parent_left + (int)_offset.X;
                bounds.Y = parent_center_y - bounds.Height / 2 + (int)_offset.Y;
                break;

            case Anchor.Center:
                bounds.X = parent_center_x - bounds.Width / 2 + (int)_offset.X;
                bounds.Y = parent_center_y - bounds.Height / 2 + (int)_offset.Y;
                break;

            case Anchor.Right:
                bounds.X = parent_right - bounds.Width - (int)_offset.X;
                bounds.Y = parent_center_y - bounds.Height / 2 + (int)_offset.Y;
                break;

            case Anchor.BottomLeft:
                bounds.X = parent_left + (int)_offset.X;
                bounds.Y = parent_bottom - bounds.Height - (int)_offset.Y;
                break;

            case Anchor.Bottom:
                bounds.X = parent_center_x - bounds.Width / 2 + (int)_offset.X;
                bounds.Y = parent_bottom - bounds.Height - (int)_offset.Y;
                break;

            case Anchor.BottomRight:
                bounds.X = parent_right - bounds.Width - (int)_offset.X;
                bounds.Y = parent_bottom - bounds.Height - (int)_offset.Y;
                break;
        }

        RectangleExtensions.Clip(ref bounds, ref parentClip, out _clip);
    }

    #region Updates
    /// <summary>
    /// Update before inputs are processed.
    /// </summary>
    public virtual void UpdateSetup()
    {
        foreach (UIComponent c in _children)
            if (c.isVisible)
                c.UpdateSetup();
    }
    /// <summary>
    /// Layout size updates.
    /// </summary>
    public virtual void UpdateSizes() { }
    /// <summary>
    /// Input processing.
    /// </summary>
    public virtual void UpdateInput()
    {
        foreach (var c in _children)
            if (c.isActive && c.isVisible)
                c.UpdateInput();
    }
    /// <summary>
    /// General updates.
    /// </summary>
    public virtual void Update()
    {
        foreach (var c in _children)
            if (c.isVisible)
                c.Update();
    }
    /// <summary>
    /// Draws the component. The component should draw itself within it's clip rectangle.
    /// </summary>
    public virtual void Draw()
    {
        Rectangle clip = Clip;
        GUI.PushScissor(clip);
        foreach (var c in _children)
        {
            if (c.isVisible && clip.Intersects(c.Clip))
                c.Draw();
        }
        GUI.PopScissor();
    }
    #endregion

    public void AddChild(UIComponent component, int index = -1) 
    {
        if (component.Parent != null)
        {
            component.RemoveChild(component);
        }
        component.Parent = this;
        if (index >= 0)
        {
            _children.Insert(index, component);

            // update siblings after this one.
            for (int i = index + 1; i < _children.Count; i++)
            {
                _children[i]._parentIndex += 1;
            }
        } else
        {
            component._parentIndex = _children.Count;
            _children.Add(component);
        }

        MarkLayoutAsDirty();
        component.MarkLayoutAsDirty();
    }
    public void RemoveChild(UIComponent component) 
    {
        component.Parent = null;
        _children.RemoveAt(component._parentIndex);
        // update siblings after this one.
        for (int i = component._parentIndex; i < _children.Count; i++)
        {
            _children[i]._parentIndex -= 1;
        }

        MarkLayoutAsDirty();
        component.MarkLayoutAsDirty();
    }
    /// <summary>
    /// Applies a layout to this component.
    /// </summary>
    public virtual void UpdateLayout() { }

    /// <summary>
    /// Marks this component, and all parents up to the root, as dirty, so they will update their layouts.
    /// </summary>
    public void MarkLayoutAsDirty()
    {
        _isLayoutDirty = true;
    }
}