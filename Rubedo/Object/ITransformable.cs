namespace Rubedo.Object;

/// <summary>
/// Required for a <see cref="Transform"/> to be attached to something.
/// </summary>
internal interface ITransformable 
{
    internal void TransformChanged();
}
