
namespace Rubedo.Physics2D.Dynamics.Shapes;
public readonly struct EdgeAdjacency
{
    public static readonly EdgeAdjacency None = new EdgeAdjacency(false, -1, -1);

    public readonly bool IsInternal;
    public readonly int NeighborChildIndex;
    public readonly int NeighborEdgeIndex;

    public EdgeAdjacency(bool isInternal, int neighborChildIndex, int neighborEdgeIndex)
    {
        IsInternal = isInternal;
        NeighborChildIndex = neighborChildIndex;
        NeighborEdgeIndex = neighborEdgeIndex;
    }
}
