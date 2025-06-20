using Microsoft.Xna.Framework;
using Rubedo.Graphics;
using System.Threading.Tasks;

namespace Rubedo.Components;

/// <summary>
/// TODO: I am Parallax, and I don't have a summary yet.
/// </summary>
public class Parallax : Component
{
    public enum ParallaxType
    {
        Horizontal,
        Vertical,
        Both
    }

    private readonly Camera targetCamera;
    public float parallaxY = 0f;
    public float parallaxX = 0f;
    public ParallaxType parallaxType;
    public Vector2 startPos;

    /// <summary>
    /// Parallax value of 0
    /// </summary>
    public Parallax(Camera targetCamera, float parallax, ParallaxType type)
    {
        this.targetCamera = targetCamera;
        this.parallaxX = parallax;
        this.parallaxY = parallax;
        this.parallaxType = type;
    }
    public Parallax(Camera targetCamera, float parallaxX, float parallaxY, ParallaxType type)
    {
        this.targetCamera = targetCamera;
        this.parallaxX = parallaxX;
        this.parallaxY = parallaxY;
        this.parallaxType = type;
    }
    public Parallax(Camera targetCamera, Vector2 parallax, ParallaxType type)
    {
        this.targetCamera = targetCamera;
        this.parallaxX = parallax.X;
        this.parallaxY = parallax.Y;
        this.parallaxType = type;
    }

    public override void EntityAdded(GameState state)
    {
        base.EntityAdded(state);
        startPos = Entity.Transform.Position;
    }

    public override void Update()
    {
        float distance;
        switch (parallaxType)
        {
            case ParallaxType.Horizontal:
                distance = targetCamera.X * parallaxX;
                Entity.Transform.Position = new Vector2(startPos.X + distance, Entity.Transform.Position.Y);
                break;
            case ParallaxType.Vertical:
                distance = targetCamera.Y * parallaxY;
                Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, startPos.Y + distance);
                break;
            case ParallaxType.Both:
                float distX = targetCamera.X * parallaxX;
                float distY = targetCamera.Y * parallaxY;
                Entity.Transform.Position = new Vector2(startPos.X + distX, startPos.Y + distY);
                break;
        }
    }
}