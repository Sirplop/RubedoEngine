using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics.Sprites;
using Rubedo.Resources;
using System;
using System.IO;

namespace Rubedo.Resources.Serializers;

/// <summary>
/// Loads a spriteatlas file, and processes it into a useable <see cref="TextureAtlas2D"/>
/// </summary>
internal static class TextureAtlas2DLoader
{
    internal static TextureAtlas2D Load(string path)
    {
        FileInfo atlasMap = new FileInfo(path + ".atlasmap");
        FileInfo atlasFile = new FileInfo(path + ".png");

        if (!atlasMap.Exists || !atlasFile.Exists)
        {
            throw new ContentLoadException($"Atlas '{path}' does not exist!");
        }
        Texture2D texture = null;
        using (Stream stream = TitleContainer.OpenStream(path + ".png"))
        {
            texture = Texture2D.FromStream(RubedoEngine.Graphics.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
        }

        TextureAtlas2D atlas = new TextureAtlas2D(texture, path);
        int lineNumber = 0;
        using (Stream stream = TitleContainer.OpenStream(path + ".atlasmap"))
        {
            using (TextReader file = new StreamReader(stream))
            {
                while (true)
                {
                    string lineRead = file.ReadLine();
                    if (lineRead == null)
                    {
                        break;
                    }
                    lineNumber++;
                    ReadOnlySpan<char> line = lineRead.AsSpan();
                    Range[] ranges = new Range[8];
                    Span<Range> sections = new Span<Range>(ranges); //doing it with spans to save memory - no need to allocate more than the line!
                    line.Split(sections, ',');
                    //time for arbitrary data reading! yippee!
                    //order is: name, sheetNum, x, y, width, height, pivotX, pivotY
                    //TODO: use pivots and sheetNum! multi-bin atlases don't exist yet, and pivots aren't used.
                    if (sections.Length != 8)
                        throw new ArgumentOutOfRangeException($"Malformed atlasmap line, number {lineNumber}! Did you edit it yourself, like a fool?");

                    string name = line[sections[0]].ToString();
                    Rectangle rect = new Rectangle(int.Parse(line[sections[2]]), int.Parse(line[sections[3]]), int.Parse(line[sections[4]]), int.Parse(line[sections[5]]));
                    atlas.CreateRegion(rect, name);
                }
            }
        }

        return atlas;
    }
}