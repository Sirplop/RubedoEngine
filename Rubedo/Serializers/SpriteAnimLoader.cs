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

        //get information out
        obj.TryGetPropertyValue("name", out node);
        string name = node.GetValue<string>();
        obj.TryGetPropertyValue("loops", out node);
        bool loops = node.GetValue<bool>();
        obj.TryGetPropertyValue("pingpong", out node);
        bool pingpong = node.GetValue<bool>();

        obj.TryGetPropertyValue("frames", out node);
        JsonArray frameArray = node.AsArray();
        SpriteAnimationFrame[] frames = new SpriteAnimationFrame[frameArray.Count];
        for (int i = 0; i < frameArray.Count; i++)
        {
            JsonObject arrObj = frameArray[i].AsObject();
            arrObj.TryGetPropertyValue("name", out node);
            int frameIndex = node.GetValue<int>();
            arrObj.TryGetPropertyValue("duration", out node);
            int duration = node.GetValue<int>();

            frames[i] = new SpriteAnimationFrame(frameIndex, duration / 1000f); //duration is stored in ms, runtime it's seconds.
        }

        return new SpriteAnimation(name, atlas, frames, loops, false, pingpong);
    }
}