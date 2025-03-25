using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Components;
using Rubedo.Render;
using System.Collections.Generic;

namespace Rubedo.Debug;

/// <summary>
/// I am DebugText, and this is my summary.
/// </summary>
public class DebugText : Text
{
    List<TextData> drawData = new List<TextData>();
    List<bool> drawDataSpace = new List<bool>();
    public DebugText(SpriteFont font, Color color) : base(font, "", color, true, true) { }

    public void DrawText(Vector2 position, string text, bool worldOrScreen)
    {
        drawData.Add(new TextData() { position = position, text = text });
        drawDataSpace.Add(worldOrScreen);
    }

    public override void Draw(Renderer sb)
    {
        Vector2 sub = new Vector2(7.5f, 10);
        for (int i = 0;  i < drawData.Count; i++)
        {
            TextData data = drawData[i];
            if (drawDataSpace[i])
                sb.DrawString(font, data.text, RubedoEngine.Instance.Camera.ScreenToWorldPoint(data.position), color, 0, 1f / RubedoEngine.Instance.Camera.GetZoom(), SpriteEffects.None);
            else
                sb.DrawString(font, data.text, data.position - sub, color, 0, 1f, SpriteEffects.None);
        }
        drawData.Clear();
        drawDataSpace.Clear();
    }

    public struct TextData
    {
        public string text;
        public Vector2 position;
    }
}