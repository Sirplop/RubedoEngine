using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Components;
using Rubedo.Render;
using System.Collections.Generic;

namespace Rubedo.Debug;

public class DebugText
{
    public static DebugText Instance
    {
        get
        {
            if (instance == null)
                instance = new DebugText(AssetManager.LoadFont("Consolas"), Color.AntiqueWhite);
            return instance;
        }
    }
    private static DebugText instance = null;

    protected SpriteFont font;
    public string text;
    public Color color;

    List<TextData> drawData = new List<TextData>();
    List<bool> drawDataSpace = new List<bool>();
    public DebugText(SpriteFont font, Color color)
    {
        instance = this;
        this.font = font;
        this.color = color;
    }
    private Vector2 stackPosition = new Vector2(30, 20);

    public void DrawText(Vector2 position, float scale, string text, bool worldOrScreen)
    {
        drawData.Add(new TextData() { position = position, text = text, scale = scale });
        drawDataSpace.Add(worldOrScreen);
    }
    public void DrawTextStack(string text)
    {
        drawData.Add(new TextData() { position = stackPosition, text = text, scale = 1f });
        drawDataSpace.Add(true);
        stackPosition += new Vector2(0, 20);
    }
    public void Draw(Renderer sb)
    {
        for (int i = 0;  i < drawData.Count; i++)
        {
            TextData data = drawData[i];
            if (drawDataSpace[i])
                sb.DrawString(font, data.text, RubedoEngine.Instance.Camera.ScreenToWorldPoint(data.position), color, 0, data.scale / RubedoEngine.Instance.Camera.GetZoom(), SpriteEffects.None);
            else
                sb.DrawString(font, data.text, data.position, color, 0, data.scale, SpriteEffects.None);
        }
        drawData.Clear();
        drawDataSpace.Clear();
        stackPosition = new Vector2(30, 20);
    }

    public struct TextData
    {
        public string text;
        public Vector2 position;
        public float scale;
    }
}