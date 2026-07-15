using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics.Sprites;
using Rubedo.Lib;
using Rubedo.Resources;
using Rubedo.UI.Graphics;
using Rubedo.UI.Input;
using Rubedo.UI.Layout;
using Rubedo.UI.Text;
using System;

namespace Rubedo.UI;
public static class UIComponentExtensions
{
    public static string DefaultFont = "fs-default";

    public static Button AddButton(this UIComponent component, Action<Button> onClick)
    {
        Button button = new Button();
        button.OnReleased += onClick;
        component.AddChild(button);
        return button;
    }

    public static Image AddImage(this UIComponent component, string imagePath, Color color)
    {
        Texture2D texture = Assets.GetResource<Texture2D>(imagePath);
        Image image = new Image(texture, color);
        component.AddChild(image);
        return image;
    }

    public static Image AddImage(this UIComponent component, string imagePath, int xSize, int ySize, Color color)
    {
        Texture2D texture = Assets.GetResource<Texture2D>(imagePath);
        Image image = new Image(texture, xSize, ySize, color);
        component.AddChild(image);
        return image;
    }

    public static NineSliceImage AddNineSlice(this UIComponent component, string imagePath, int xSize, int ySize, Color color, bool fill, float edgePercent)
    {
        TextureRegion2D region = new TextureRegion2D(Assets.GetResource<Texture2D>(imagePath));
        NineSlice slice = region.CreateNineSliceFromUVs(edgePercent);
        NineSliceImage image = new NineSliceImage(slice, xSize, ySize, color);
        image.Image.filled = fill;
        component.AddChild(image);
        return image;
    }

    public static Image AddTiledImage(this UIComponent component, string imagePath, int xSize, int ySize, Color color)
    {
        Image image = component.AddImage(imagePath, xSize, ySize, color);
        image.drawMode = Image.DrawMode.Tiled;
        return image;
    }


    public static Label AddLabel(this UIComponent component, string text, Color color, int fontSize = 12, string font = "")
    {
        FontSystem fs = font == "" ? Assets.GetFont(DefaultFont) : Assets.GetFont(font);
        Label label = new Label(fs, text, color, fontSize);
        component.AddChild(label);

        return label;
    }

    public static Vertical AddVertical(this UIComponent component, int childPadding)
    {
        Vertical vertical = new Vertical();
        vertical.childPadding = childPadding;
        component.AddChild(vertical);
        return vertical;
    }
    public static Vertical AddVertical(this UIComponent component, Padding padding, int childPadding)
    {
        Vertical vertical = new Vertical();
        vertical.paddingBottom = padding.Bottom;
        vertical.paddingLeft = padding.Left;
        vertical.paddingRight = padding.Right;
        vertical.paddingTop = padding.Top;
        vertical.childPadding = childPadding;

        component.AddChild(vertical);
        return vertical;
    }

    public static Horizontal AddHorizontal(this UIComponent component, int childPadding)
    {
        Horizontal horizontal = new Horizontal();
        horizontal.childPadding = childPadding;
        component.AddChild(horizontal);
        return horizontal;
    }
    public static Horizontal AddHorizontal(this UIComponent component, Padding padding, int childPadding)
    {
        Horizontal horizontal = new Horizontal();
        horizontal.paddingBottom = padding.Bottom;
        horizontal.paddingLeft = padding.Left;
        horizontal.paddingRight = padding.Right;
        horizontal.paddingTop = padding.Top;
        horizontal.childPadding = childPadding;

        component.AddChild(horizontal);
        return horizontal;
    }
}
