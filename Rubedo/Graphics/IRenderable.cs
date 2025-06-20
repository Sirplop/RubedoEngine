using Rubedo.Lib;

namespace Rubedo.Graphics;

/// <summary>
/// When applied to a <see cref="Components.Component"/>, 
/// </summary>
public interface IRenderable
{
    /// <summary>
    /// The bounds of this renderable object, used for camera culling.
    /// </summary>
    RectF Bounds { get; }

    /// <summary>
    /// Determines if this should be rendered.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// The depth of this renderable inside its layer. Higher is in front.
    /// </summary>
    /// <remarks>Range is from ~-400,000 to ~400,000.</remarks>
    int LayerDepth { get; set; }
    /// <summary>
    /// The render layer for this renderable. If you're looking for in-layer sorting, you want <seealso cref="LayerDepth"/>.
    /// </summary>
    int RenderLayer { get; set; }

    /// <summary>
    /// Determines if the object is visible to the camera.
    /// </summary>
    bool IsVisibleToCamera(Camera camera);

    /// <summary>
    /// Renders the object to the <paramref name="camera"/>.
    /// </summary>
    public void Render(Renderer renderer, Camera camera);
}