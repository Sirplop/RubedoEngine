using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Components;
using Rubedo.Internal.Assets;
using Rubedo.Rendering;
using System.Collections.Generic;

namespace Rubedo.EngineDebug;

public class DebugText
{
    public static DebugText Instance
    {
        get
        {
            if (instance == null)
                instance = new DebugText(AssetManager.CreateNewFontSystem("consolas", "fonts/Consolas.ttf"), Color.AntiqueWhite);
            return instance;
        }
    }
    private static DebugText instance = null;

    protected FontSystem font;
    public string text;
    public Color color;

    List<TextData> drawData = new List<TextData>();
    List<Renderer.Space> drawDataSpace = new List<Renderer.Space>();
    public DebugText(FontSystem font, Color color)
    {
        instance = this;
        this.font = font;
        this.color = color;
    }
    private Vector2 stackPosition = new Vector2(30, 5);

    public void DrawText(Vector2 position, float scale, string text, int fontSize, Renderer.Space space)
    {
        drawData.Add(new TextData() { position = position, text = text, scale = scale, fontSize = fontSize });
        drawDataSpace.Add(space);
    }
    public void DrawTextStack(string text)
    {
        drawData.Add(new TextData() { position = stackPosition, text = text, scale = 1f, fontSize = 16 });
        drawDataSpace.Add(Renderer.Space.Screen);
        stackPosition += new Vector2(0, 20);
    }
    public void Draw(Renderer sb)
    {
        for (int i = 0;  i < drawData.Count; i++)
        {
            var fontR = font.GetFont(drawData[i].fontSize);
            TextData data = drawData[i];
            if (drawDataSpace[i] == Renderer.Space.World)
                sb.DrawString(fontR, Renderer.Space.World, data.text, data.position, color, 0, data.scale, SpriteEffects.None);
            else
                sb.DrawString(fontR, Renderer.Space.Screen, data.text, RubedoEngine.Instance.Camera.ScreenToWorldPoint(data.position), color, 0, data.scale / RubedoEngine.Instance.Camera.GetZoom(), SpriteEffects.None);
        }
        drawData.Clear();
        drawDataSpace.Clear();
        stackPosition = new Vector2(30, 5);
    }

    public void Clear()
    {
        drawData.Clear();
        drawDataSpace.Clear();
    }

    public struct TextData
    {
        public string text;
        public Vector2 position;
        public float scale;
        public int fontSize;
    }
}