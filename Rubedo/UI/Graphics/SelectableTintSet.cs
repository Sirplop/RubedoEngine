using Microsoft.Xna.Framework;
using Rubedo.Lib.Extensions;
using Rubedo.UI.Input;

namespace Rubedo.UI.Graphics;

/// <summary>
/// Collection of color tints designed to apply to <see cref="Selectable"/> elements.
/// </summary>
public class SelectableTintSet : UIComponent
{
    public Image target;

    public Color normal = Color.White;
    public Color disabled = Color.DarkGray;
    public Color focused = Color.LightYellow;
    public Color pressed = Color.Yellow;

    public float colorMultiplier = 1f;
    private Color transitionTint;

    public SelectableTintSet(Image target, float colorMultiplier = 1f)
    {
        this.target = target;
        this.colorMultiplier = colorMultiplier;

        this.normal = Color.White;
        this.disabled = Color.DarkGray;
        this.pressed = Color.Gray;
        this.focused = Color.Yellow;

        transitionTint = normal;
    }

    public SelectableTintSet(Image target, Color normal, Color disabled, Color pressed, Color focused, float colorMultiplier)
    {
        this.target = target;
        this.colorMultiplier = colorMultiplier;
        this.normal = normal;
        this.disabled = disabled;
        this.pressed = pressed;
        this.focused = focused;

        transitionTint = normal;
    }


    public Color GetTintColor(Selectable target, bool pressed)
    {
        if (target == null) //missing target.
            return Color.Black;
        if (!target.IsActive())
            return CalculateColor(in disabled);
        if (pressed)
        {
            if (target.IsFocused)
            {
                return CalculateColor(focused.Multiply(this.pressed));
            }
            return CalculateColor(in this.pressed);
        }
        if (target.IsFocused && !(!target.IsHovered && GUI.MouseControlsEnabled))
        {
            return CalculateColor(in focused);
        }
        return CalculateColor(in normal);
    }

    private Color CalculateColor(in Color color)
    {
        return transitionTint.Multiply(color) * colorMultiplier;
    }

    public override void Update()
    {
        base.Update();
        if (Parent is Selectable sel)
        {
            target.Color = GetTintColor(sel, sel is Button b && b.Clicked);
        }
        else
        {
            throw new System.InvalidOperationException("Component not parented by object of type Button.");
        }
    }
}