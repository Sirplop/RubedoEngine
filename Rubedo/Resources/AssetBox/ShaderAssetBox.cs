using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubedo.Resources.AssetBox;
public class ShaderAssetBox : WeakAssetBox<Effect>
{
    private const string EXTENSION_DX11 = ".dx11.mgfxo";
    private const string EXTENSION_OPENGL = ".ogl.mgfxo";

    public ShaderAssetBox(string name, string rootPath) : base(name, rootPath) { }

    public override void LoadAsset(string key, string[] extra = null)
    {
        if (key == string.Empty)
            return;

        if (AssetExists(key))
            return;

        string path = Path.Combine(Assets.RootDirectory, RootPath, key);

        string extension = "";

        switch (PlatformInfo.GraphicsBackend)
        {
            case GraphicsBackend.DirectX:
                extension = EXTENSION_DX11;
                break;
            case GraphicsBackend.OpenGL:
                extension = EXTENSION_OPENGL;
                break;
            case GraphicsBackend.Vulkan:
            case GraphicsBackend.Metal:
            case GraphicsBackend.DirectX12:
                throw new NotImplementedException();
        }

        try
        {
            using (Stream stream = TitleContainer.OpenStream(Path.ChangeExtension(path, extension)))
            {
                if (stream != null)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                    Effect effect = new Effect(RubedoEngine.Graphics.GraphicsDevice, buffer);

                    AddAsset(key, effect);
                    return;
                }
            }
        }
        catch { } //TODO: Make a better way to handle this whole thing.

        throw new ContentLoadException($"Shader effect at '{path}' does not exist!");
    }

    public override void UnloadAsset(string key)
    {
        Effect effect = GetAsset(key);
        if (effect != null)
        {
            effect.Dispose();
        }
        RemoveAsset(key);
    }
}
