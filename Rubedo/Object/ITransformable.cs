namespace Rubedo.Object;

/// <summary>
/// Required for a <see cref="Transform"/> to be attached to something.
/// </summary>
public interface ITransformable 
{
    public void TransformChanged();
}
