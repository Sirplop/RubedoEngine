using Microsoft.Xna.Framework;
using Rubedo.Input;
using Rubedo.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rubedo.UI;

/// <summary>
/// Selectable <see cref="UIComponent"/>. Controls navigation and focus.
/// </summary>
public abstract class Selectable : UIComponent
{
    protected const int NOT_SELECTABLE_INDEX = -1;

    protected readonly static List<Selectable> allSelectables = new List<Selectable>();
    protected readonly static List<Selectable> activeSelectables = new List<Selectable>();

    /// <summary>
    /// Called every frame while this <see cref="Selectable"/> is hovered over.
    /// </summary>
    public event Action<Selectable> OnHover;
    /// <summary>
    /// Called the frame this <see cref="Selectable"/> is no longer hovered.
    /// </summary>
    public event Action<Selectable> OnHoverLost;
    /// <summary>
    /// Called when this selectable becomes focused.
    /// </summary>
    public event Action<Selectable> OnFocused;
    /// <summary>
    /// Called when this selectable loses focus.
    /// </summary>
    public event Action<Selectable> OnFocusLost;

    public bool IsHovered => isHovered;
    protected bool isHovered = false;

    protected bool isFocused;
    public virtual bool IsFocused
    {
        get => isFocused;
        set
        {
            if (value == isFocused)
                return; //already un/focused

            if (value && GUI.Root.CurrentFocus != this)
            {
                GUI.Root.CurrentFocus?.OnFocusLost?.Invoke(GUI.Root.CurrentFocus);
                GUI.Root.GrabFocusInternal(this);
                OnFocused?.Invoke(this);
            }
            else if (!value && GUI.Root.CurrentFocus == this)
            {
                OnFocusLost?.Invoke(this);
                GUI.Root.GrabFocusInternal(null);
            }
            isFocused = value;
        }
    }

    public enum NavDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public bool allowUp = true;
    public bool allowDown = true;
    public bool allowLeft = true;
    public bool allowRight = true;
    protected List<Selectable> upNavigation;
    protected List<Selectable> downNavigation;
    protected List<Selectable> leftNavigation;
    protected List<Selectable> rightNavigation;

    protected int _selectedIndex = NOT_SELECTABLE_INDEX;
    public Selectable()
    {
        allSelectables.Add(this);
        if (IsActiveAndVisible())
            AddToSelectableList();
    }

    public override void UpdateInput()
    {
        //focused element navigation controls
        if (IsFocused)
        {
            if (allowUp && NavControls.NavUp.Pressed())
                Navigate(NavDirection.Up);
            else if (allowDown && NavControls.NavDown.Pressed())
                Navigate(NavDirection.Down);
            else if (allowLeft && NavControls.NavLeft.Pressed())
                Navigate(NavDirection.Left);
            else if (allowRight && NavControls.NavRight.Pressed())
                Navigate(NavDirection.Right);
        }

        isHovered = Clip.Contains(InputManager.MouseScreenPosition()); //TODO: Add controller support for a selector to trigger hovers.
        if (isHovered)
            OnHover?.Invoke(this);
        else
            OnHoverLost?.Invoke(this);
        base.UpdateInput();
    }

    /// <summary>
    /// Adds a <paramref name="selectable"/> to this component's navigation list for the given <paramref name="direction"/>, at an optional <paramref name="index"/>.
    /// </summary>
    /// <param name="selectable"></param>
    /// <param name="direction"></param>
    /// <param name="index"></param>
    public void AddNavigation(Selectable selectable, NavDirection direction, int index = -1)
    {
        switch (direction)
        {
            case NavDirection.Up:
                upNavigation ??= new List<Selectable>();
                if (index == -1 || index >= upNavigation.Count)
                    upNavigation.Add(selectable);
                else
                    upNavigation.Insert(index, selectable);
                break;
            case NavDirection.Down:
                downNavigation ??= new List<Selectable>();
                if (index == -1 || index >= downNavigation.Count)
                    downNavigation.Add(selectable);
                else
                    downNavigation.Insert(index, selectable);
                break;
            case NavDirection.Left:
                leftNavigation ??= new List<Selectable>();
                if (index == -1 || index >= leftNavigation.Count)
                    leftNavigation.Add(selectable);
                else
                    leftNavigation.Insert(index, selectable);
                break;
            case NavDirection.Right:
                rightNavigation ??= new List<Selectable>();
                if (index == -1 || index >= rightNavigation.Count)
                    rightNavigation.Add(selectable);
                else
                    rightNavigation.Insert(index, selectable);
                break;
        }
    }

    /// <summary>
    /// Navigates to the best selectable in a direction, and sets it as the current focus.
    /// </summary>
    /// <returns>The selectable chosen, or null if nothing was found.</returns>
    public Selectable Navigate(NavDirection direction)
    {
        Selectable sel = null;
        switch (direction)
        {
            case NavDirection.Up:
                sel = FindSelectableUp();
                break;
            case NavDirection.Down:
                sel = FindSelectableDown();
                break;
            case NavDirection.Left:
                sel = FindSelectableLeft();
                break;
            case NavDirection.Right:
                sel = FindSelectableRight();
                break;
        }
        if (sel != null)
        { //selectable found, set focus.
            GUI.Root.GrabFocus(sel);
        }
        return sel;
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        CheckIfSelectable();
    }
    public override void SetVisible(bool vis)
    {
        base.SetVisible(vis);
        CheckIfSelectable();
    }
    protected override void SetParentActive(bool active)
    {
        base.SetParentActive(active);
        CheckIfSelectable();
    }
    protected override void SetParentVisible(bool vis)
    {
        base.SetParentVisible(vis);
        CheckIfSelectable();
    }
    /// <summary>
    /// Updates this selectable's active state in the list of active selectables.
    /// </summary>
    protected void CheckIfSelectable()
    {
        bool selectable = IsActiveAndVisible();
        if (_selectedIndex == NOT_SELECTABLE_INDEX && selectable)
            AddToSelectableList();
        else if (_selectedIndex != NOT_SELECTABLE_INDEX && !selectable)
            RemoveFromSelectableList();
    }

    protected void AddToSelectableList()
    {
        _selectedIndex = activeSelectables.Count;
        activeSelectables.Add(this);
    }
    protected void RemoveFromSelectableList()
    {
        activeSelectables.RemoveAt(_selectedIndex);
        for (int i = _selectedIndex; i < activeSelectables.Count; i++)
        {
            activeSelectables[i]._selectedIndex--;
        }
        _selectedIndex = NOT_SELECTABLE_INDEX;
    }


    /// <summary>
    /// Gets the first active selectable in this component's up navigation list. If there are no targets, finds one using <see cref="FindSelectableAuto"/>.
    /// </summary>
    /// <returns></returns>
    public Selectable FindSelectableUp()
    {
        if (!allowUp)
            return null;
        Selectable sel = FindFirstSelectable(upNavigation);
        return sel ?? FindSelectableAuto(NavDirection.Up);
    }
    /// <summary>
    /// Gets the first active selectable in this component's down navigation list. If there are no targets, finds one using <see cref="FindSelectableAuto"/>.
    /// </summary>
    /// <returns></returns>
    public Selectable FindSelectableDown()
    {
        if (!allowDown)
            return null;
        Selectable sel = FindFirstSelectable(downNavigation);
        return sel ?? FindSelectableAuto(NavDirection.Down);
    }
    /// <summary>
    /// Gets the first active selectable in this component's left navigation list. If there are no targets, finds one using <see cref="FindSelectableAuto"/>.
    /// </summary>
    /// <returns></returns>
    public Selectable FindSelectableLeft()
    {
        if (!allowUp)
            return null;
        Selectable sel = FindFirstSelectable(leftNavigation);
        return sel ?? FindSelectableAuto(NavDirection.Left);
    }
    /// <summary>
    /// Gets the first active selectable in this component's right navigation list. If there are no targets, finds one using <see cref="FindSelectableAuto"/>.
    /// </summary>
    /// <returns></returns>
    public Selectable FindSelectableRight()
    {
        if (!allowUp)
            return null;
        Selectable sel = FindFirstSelectable(rightNavigation);
        return sel ?? FindSelectableAuto(NavDirection.Right);
    }

    /// <summary>
    /// Gets the first valid <see cref="Selectable"/> out of the given <paramref name="targetList"/>.
    /// </summary>
    /// <returns>First valid selectable, or null if <paramref name="targetList"/> is null or if there are no valid targets.</returns>
    protected static Selectable FindFirstSelectable(List<Selectable> targetList)
    {
        if (targetList == null)
            return null;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] != null && targetList[i].IsActiveAndVisible())
            {
                return targetList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Finds a valid <see cref="Selectable"/> in the UI tree.
    /// </summary>
    protected virtual Selectable FindSelectableAuto(NavDirection direction)
    {
        Vector2 findPos = ScreenPosition();
        float bestDist = float.MaxValue;
        Selectable best = null;
        float testDist;
        for (int i = 0; i < activeSelectables.Count; i++)
        {
            if (i == _selectedIndex)
                continue; //skip ourselves.
            Selectable sel = activeSelectables[i];
            Vector2 pos = sel.ScreenPosition();
            switch (direction)
            {
                case NavDirection.Up:
                    if (pos.Y >= findPos.Y)
                        continue;
                    break;
                case NavDirection.Down:
                    if (pos.Y <= findPos.Y)
                        continue;
                    break;
                case NavDirection.Left:
                    if (pos.X >= findPos.X)
                        continue;
                    break;
                case NavDirection.Right:
                    if (pos.X <= findPos.X)
                        continue;
                    break;
            }
            Vector2.DistanceSquared(ref findPos, ref pos, out testDist);
            if (best == null || testDist < bestDist)
            {
                bestDist = testDist;
                best = sel;
            }
        }
        return best;
    }
}