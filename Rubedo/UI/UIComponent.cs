using Microsoft.Xna.Framework;
using Rubedo.Lib.Extensions;
using Rubedo.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

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
public abstract class UIComponent : IDestroyable
{

    #region Info
    /// <summary>
    /// If this element does not care about layout information, and provides none itself.
    /// </summary>
    protected bool ignoreLayout = false;
    protected bool isVisible = true;
    protected bool isActive = true;
    protected bool isParentVisible = true;
    protected bool isParentActive = true;

    public UIComponent? Parent { get; set; } = null;
    protected internal readonly List<UIComponent> _children = new List<UIComponent>();

    public ReadOnlyCollection<UIComponent> Children => _children.AsReadOnly();

    protected bool isLayoutDirty = true;
    protected int _parentIndex = -1;

    /// <summary>
    /// Updated every time the clip gets updated, so children will know that it's been updated and they should too.
    /// </summary>
    internal uint _clipVersion = 0;
    /// <summary>
    /// Last known parent's clip version. Will update clip if does not match <see cref="_clipVersion"/>.
    /// </summary>
    private uint _parentLastClipVersion = 0;

    /// <summary>
    /// If this element does not care about layout information, and provides none itself.
    /// </summary>
    public bool IgnoresLayout => ignoreLayout;
    public void SetIgnoresLayout(bool val)
    {
        ignoreLayout = val;
    }

    public bool IsVisible()
    {
        return isVisible && isParentVisible;
    }
    public bool IsActive()
    {
        return isActive && isParentActive;
    }
    public bool IsActiveAndVisible()
    {
        return isVisible && isParentVisible && isActive && isParentActive;
    }

    public virtual void SetVisible(bool vis)
    {
        isVisible = vis;
        if (Parent == null)
            isParentVisible = vis;
        foreach (UIComponent child in Children)
            child.SetParentVisible(isParentVisible && vis);
    }
    public virtual void SetActive(bool active)
    {
        isActive = active;
        if (Parent == null)
            isParentActive = active;
        foreach (UIComponent child in Children)
            child.SetParentActive(isParentActive && active);
    }
    protected virtual void SetParentVisible(bool vis)
    {
        if (isParentVisible)
            return; //still the same, don't update anything.
        isParentVisible = vis;
        foreach (UIComponent child in Children)
            child.SetParentVisible(vis && isVisible);
    }
    protected virtual void SetParentActive(bool active)
    {
        if (isParentActive == active)
            return; //still the same, don't update anything.
        isParentActive = active;
        foreach (UIComponent child in Children)
            child.SetParentActive(active && isParentActive);
    }

    protected float _width = 100;
    public float Width
    {
        get => _width;
        set
        {
            if (_width != value)
            {
                MarkLayoutAsDirty();
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
                MarkLayoutAsDirty();
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

    /// <summary>
    /// Gets the sum of offsets, aka this component's position on screen.
    /// </summary>
    /// <returns></returns>
    public Vector2 ScreenPosition()
    {
        Point clipPoint = Clip.Location;
        return new Vector2(clipPoint.X, clipPoint.Y);
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

    public bool IsDestroyed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; }

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
    /// Sets this component's offset relative to the height/width of the given rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to fit relatively to.</param>
    /// <param name="offset">The offset, from 0 to 1.</param>
    /// <remarks>This will account for the component's anchor point.</remarks>
    public void SetRelativeOffset(in Rectangle rect, Vector2 offset)
    {
        switch (_anchor)
        {
            case Anchor.TopLeft:
                SetOffset(new Vector2(offset.X * rect.Width, offset.Y * rect.Height));
                break;
            case Anchor.Top:
                SetOffset(new Vector2((offset.X - 0.5f) * rect.Width, offset.Y * rect.Height));
                break;
            case Anchor.TopRight:
                SetOffset(new Vector2((offset.X - 1f) * rect.Width, offset.Y * rect.Height));
                break;
            case Anchor.Left:
                SetOffset(new Vector2(offset.X * rect.Width, (offset.Y - 0.5f) * rect.Height));
                break;
            case Anchor.Center:
                SetOffset(new Vector2((offset.X - 0.5f) * rect.Width, (offset.Y - 0.5f) * rect.Height));
                break;
            case Anchor.Right:
                SetOffset(new Vector2((offset.X - 1f) * rect.Width, (offset.Y - 0.5f) * rect.Height));
                break;
            case Anchor.BottomLeft:
                SetOffset(new Vector2(offset.X * rect.Width, (offset.Y - 1f) * rect.Height));
                break;
            case Anchor.Bottom:
                SetOffset(new Vector2((offset.X - 0.5f) * rect.Width, (offset.Y - 1) * rect.Height));
                break;
            case Anchor.BottomRight:
                SetOffset(new Vector2((offset.X - 1f) * rect.Width, (offset.Y - 1f) * rect.Height));
                break;
        }
    }

    /// <summary>
    /// Set the anchor of this entity.
    /// </summary>
    /// <param name="anchor">New anchor to set.</param>
    protected virtual void SetAnchor(Anchor anchor)
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
    protected virtual void SetOffset(Vector2 offset)
    {
        if (_offset != offset)
        {
            _offset = offset;
            MarkLayoutAsDirty();
        }
    }

    public virtual void UpdateClipIfDirty()
    {
        if (Parent != null && (isLayoutDirty || _parentLastClipVersion != Parent._clipVersion))
        {
            UpdateClip();
        }
    }

    protected virtual void UpdateClip()
    {
        CreateClipRect();
        isLayoutDirty = false; 
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
            Parent.UpdateClipIfDirty();
            parentClip = Parent.Clip;
        }

        Rectangle bounds = new Rectangle(0, 0, (int)Width, (int)Height);

        int parent_left = parentClip.X;
        int parent_top = parentClip.Y;
        int parent_right = parent_left + (int)Parent.Width; //use actual width and height instead of clip
        int parent_bottom = parent_top + (int)Parent.Height; //to make anchors properly relative to the parent's full size.
        int parent_center_x = parent_left + (int)Parent.Width / 2;
        int parent_center_y = parent_top + (int)Parent.Height / 2;

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
    public virtual void UpdateSizes()
    {
        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            c.UpdateSizes();
        }
    }
    /// <summary>
    /// Input processing.
    /// </summary>
    public virtual void UpdateInput()
    {
        for (int i = 0; i < _children.Count; i++)
        {
            var c = _children[i];
            if (c.isActive && c.isVisible)
                c.UpdateInput();
        }
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

        component.SetParentVisible(isParentVisible && isVisible);
        component.SetParentActive(isParentActive && isActive);

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

        component.SetParentVisible(component.isVisible);
        component.SetParentActive(component.isActive);

        MarkLayoutAsDirty();
        component.MarkLayoutAsDirty();
    }

    /// <summary>
    /// Applies a layout to this component.
    /// </summary>
    public virtual void UpdateLayout()
    {
        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            c.UpdateClipIfDirty();
            c.UpdateLayout();
        }
    }

    /// <summary>
    /// Marks this component as dirty to update clips and layouts.
    /// </summary>
    public virtual void MarkLayoutAsDirty()
    {
        isLayoutDirty = true;
    }

    /// <summary>
    /// Destroys this and all of its children.
    /// </summary>
    public virtual void Destroy()
    {
        if (!IsDestroyed)
        {
            Parent = null;
            foreach (UIComponent component in _children)
                component.Destroy();
            _children.Clear();
            IsDestroyed = true;
        }
    }

    /// <summary>
    /// Destroys all children of this component, but not the component itself.
    /// </summary>
    public virtual void DestroyChildren()
    {
        for (int i = 0; i < _children.Count; i++)
            _children[i].Destroy();
        _children.Clear();
    }
}