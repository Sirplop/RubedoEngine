using Microsoft.Xna.Framework.Content;
using Rubedo.Graphics.Animation;
using Rubedo.Graphics.Sprites;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Rubedo.Serializers;

/// <summary>
/// TODO: I am SpriteAnimLoader, and I don't have a summary yet.
/// </summary>
internal static class SpriteAnimLoader
{
    internal static SpriteAnimation Load(string path)
    {
        FileInfo file = new FileInfo(path + ".spriteanim");
        if (!file.Exists)
            throw new ContentLoadException($"Sprite Animation '{path}' does not exist!");

        //we don't need any runtime checks because the content builder assures us that the JSON is well-formed.
        JsonNode node = JsonNode.Parse(File.ReadAllText(file.FullName), documentOptions: new JsonDocumentOptions() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
        JsonObject obj = node.AsObject();

        obj.TryGetPropertyValue("atlas", out JsonNode atlasNode);
        TextureAtlas2D atlas = Assets.LoadAtlas(atlasNode.GetValue<string>());

        bool loops, pingpong, reversed = false;

        //get information out
        if (!obj.TryGetPropertyValue("name", out node))
            throw new JsonException($"Missing section 'name' in sprite animation '{path}'!");
        string name = node.GetValue<string>();
        if (obj.TryGetPropertyValue("loops", out node))
            loops = node.GetValue<bool>();
        else
            loops = false;
        if (obj.TryGetPropertyValue("pingpong", out node))
            pingpong = node.GetValue<bool>();
        else
            pingpong = false;
        if (obj.TryGetPropertyValue("reversed", out node))
            reversed = node.GetValue<bool>();
        else
            reversed = false;

        if (!obj.TryGetPropertyValue("frames", out node))
            throw new JsonException($"Missing section 'frames' in sprite animation '{path}'!");
        JsonArray frameArray = node.AsArray();
        SpriteAnimationFrame[] frames = new SpriteAnimationFrame[frameArray.Count];
        for (int i = 0; i < frameArray.Count; i++)
        {
            JsonObject arrObj = frameArray[i].AsObject();
            if (!arrObj.TryGetPropertyValue("name", out node))
                throw new JsonException($"Malformed frame name at frame {i} in sprite animation '{path}'!");
            int frameIndex = node.GetValue<int>();
            if (!arrObj.TryGetPropertyValue("duration", out node))
                throw new JsonException($"Malformed frame duration at frame {i} in sprite animation '{path}'!");
            int duration = node.GetValue<int>();

            frames[i] = new SpriteAnimationFrame(frameIndex, duration / 1000f); //duration is stored in ms, runtime it's seconds.
        }

        return new SpriteAnimation(name, atlas, frames, loops, reversed, pingpong);
    }
}