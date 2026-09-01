using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubedo.Graphics;
public class Material
{
    Texture2D texture;
    Effect shader;

    public Material(Texture2D texture, Effect shader = null)
    {
        this.texture = texture;
        this.shader = shader ?? RubedoEngine.Instance.Renderer.DefaultEffect;
    }

    public Material Clone()
    {
        if (shader == RubedoEngine.Instance.Renderer.DefaultEffect)
            return new Material(texture, null); //don't clone the default effect.
        return new Material(texture, shader.Clone());
    }
}
