using System.Runtime.CompilerServices;

namespace Rubedo.Object;

/// <summary>
/// Provides a method to destroy this object, similar to an <see cref="System.IDisposable"/>.
/// </summary>
public interface IDestroyable
{
    public bool IsDestroyed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; protected set; }
    public void Destroy();
}