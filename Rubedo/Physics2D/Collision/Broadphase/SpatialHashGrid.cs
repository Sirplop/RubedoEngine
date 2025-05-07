using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.EngineDebug;
using Rubedo.Lib;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Math;
using Rubedo.Rendering;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rubedo.Physics2D.Collision.Broadphase;

/// <summary>
/// Grid for spatial bucketing.
/// </summary>
internal class SpatialHashGrid : IBroadphase
{
    public AABB GridBounds = new AABB()
    {
        min = Vector2.Zero,
        max = Vector2.Zero
    };

    internal int cellSize;
    internal float invCellSize;
    internal readonly SpatialDictionary cells = new SpatialDictionary();
    internal readonly HashSet<PhysicsBody> bodySet = new HashSet<PhysicsBody>();
    internal HashSet<PhysicsBody> bodyRemove = new HashSet<PhysicsBody>();

    public SpatialHashGrid(int cellSize)
    {
        this.cellSize = cellSize;
        this.invCellSize = 1f / cellSize;
    }

    public void Update(List<PhysicsBody> bodies)
    {
        bodyRemove = new HashSet<PhysicsBody>(bodySet, bodySet.Comparer);
        bodyRemove.ExceptWith(bodies);
        foreach (PhysicsBody body in bodyRemove) 
        { //remove all bodies that aren't present in our active list.
            Remove(body);
        }
        AABB checkBounds = new AABB();
        for (int i = 0; i < bodies.Count; i++)
        {
            if (!bodySet.Contains(bodies[i]))
            { //a new addition!
                Add(bodies[i]);
                continue;
            }

            if (bodies[i].bounds != bodies[i].collider.shape.RegisteredBounds)
            { //update the body in the hash without removing it from cells it's still in.
                PhysicsBody body = bodies[i];
                AABB newBounds = bodies[i].bounds;
                AABB oldBounds = bodies[i].collider.shape.RegisteredBounds;

                Vector2Int o1 = CellCoords(oldBounds.min.X, oldBounds.min.Y);
                Vector2Int o2 = CellCoords(oldBounds.max.X, oldBounds.max.Y);
                Vector2Int n1 = CellCoords(newBounds.min.X, newBounds.min.Y);
                Vector2Int n2 = CellCoords(newBounds.max.X, newBounds.max.Y);

                if (o1 == n1 && o2 == n2)
                    continue; //we're in the same cells still.

                if (!GridBounds.Contains(ref n1))
                    AABB.Union(ref GridBounds, ref n1, out GridBounds);

                if (!GridBounds.Contains(ref n2))
                    AABB.Union(ref GridBounds, ref n2, out GridBounds);

                checkBounds.Set(n1, n2);
                for (int x = o1.X; x <= o2.X; x++)
                {
                    for (int y = o1.Y; y <= o2.Y; y++)
                    {
                        if (checkBounds.Contains(x, y))
                            continue; //still in this cell
                        CellAtPosition(x, y)?.Remove(body);
                    }
                }
                checkBounds.Set(o1, o2);
                for (int x = n1.X; x <= n2.X; x++)
                {
                    for (int y = n1.Y; y <= n2.Y; y++)
                    {
                        if (checkBounds.Contains(x, y))
                            continue; //still in this cell
                        CellAtPosition(x, y, true).Add(body);
                    }
                }

                body.collider.shape.RegisteredBounds = newBounds;
            }
        }
    }
    private readonly HashSet<Manifold> pairs = new HashSet<Manifold>();
    public void ComputePairs(List<Manifold> manifolds, HashSet<Manifold> manifoldSet)
    {
        foreach (List<PhysicsBody> body in cells.cells.Values)
        {
            if (body.Count <= 1) //ignore cells with 1 thing in it.
                continue;
            for (int i = 0; i < body.Count - 1; i++)
            {
                for (int j = i + 1; j < body.Count; j++)
                {
                    if (body[i].isStatic && body[j].isStatic)
                        continue; //static bodies can't collide.
                    if (body[i].bounds.Overlaps(body[j].bounds))
                    {
                        //TODO: ManifoldPool class, please. Too many allocations.
                        Manifold m = new Manifold(body[i], body[j]);
                        if (pairs.Add(m) && manifoldSet.Add(m))
                        {
                            manifolds.Add(m);
                        }
                    }
                }
            }
        }
        /*manifoldSet.RemoveWhere(m => !pairs.Contains(m)); //TODO: Investigate better stale manifold removal.
        manifolds.Clear();
        manifolds.AddRange(manifoldSet);*/
        for (int i = manifolds.Count - 1; i >= 0; i--)
        {
            if (!pairs.Contains(manifolds[i]))
            {
                manifoldSet.Remove(manifolds[i]);
                manifolds.RemoveAt(i);
            }
        }
        pairs.Clear();
    }

    public void Add(PhysicsBody body)
    {
        AABB bounds = body.collider.shape.GetBoundingBox();
        body.collider.shape.RegisteredBounds = bounds;
        Vector2Int p1 = CellCoords(bounds.min.X, bounds.min.Y);
        Vector2Int p2 = CellCoords(bounds.max.X, bounds.max.Y);
        
        if (!GridBounds.Contains(ref p1))
            AABB.Union(ref GridBounds, ref p1, out GridBounds);

        if (!GridBounds.Contains(ref p2))
            AABB.Union(ref GridBounds, ref p2, out GridBounds);

        for (int x = p1.X; x <= p2.X; x++)
        {
            for (int y = p1.Y; y <= p2.Y; y++)
            {
                CellAtPosition(x, y, true).Add(body);
            }
        }
        bodySet.Add(body);
    }
    public void Remove(PhysicsBody body)
    {
        AABB bounds = body.collider.shape.RegisteredBounds;
        Vector2Int p1 = CellCoords(bounds.min.X, bounds.min.Y);
        Vector2Int p2 = CellCoords(bounds.max.X, bounds.max.Y);

        for (int x = p1.X; x <= p2.X; x++)
        {
            for (int y = p1.Y; y <= p2.Y; y++)
            {
                // the cell should probably always exist since this collider should be in all queried cells
                CellAtPosition(x, y)?.Remove(body);
            }
        }
        bodySet.Remove(body);
    }

    public bool Raycast(Math.Ray2D ray, float distance, out RaycastResult result)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        cells.Clear();
    }
    public void DebugDraw(Rendering.Shapes shapes)
    {
        for (int x = Lib.Math.FloorToInt(GridBounds.min.X); x <= Lib.Math.CeilToInt(GridBounds.max.X); x++)
        {
            for (int y = Lib.Math.FloorToInt(GridBounds.min.Y); y <= Lib.Math.CeilToInt(GridBounds.max.Y); y++)
            {
                List<PhysicsBody> cell = CellAtPosition(x, y);
                if (cell != null && cell.Count > 0)
                    DebugDrawCellDetails(shapes, x, y, cell.Count);
            }
        }
    }


    void DebugDrawCellDetails(Rendering.Shapes shapes, float x, float y, int cellCount)
    {
        shapes.DrawBox(new Vector2(x * cellSize, y * cellSize), new Vector2((x + 1) * cellSize, (y + 1) * cellSize), Color.DarkBlue);

        if (cellCount > 0)
        {
            Vector2 textPosition = new Vector2((float)x * cellSize + 0.25f * cellSize,
                (float)y * cellSize + 0.75f * cellSize);
            DebugText.Instance.DrawText(textPosition, 0.05f, cellCount.ToString(), 24, Renderer.Space.World);
        }
    }

    #region Helper
    /// <summary>
    /// Gets the cell coordinate for a world-space x,y value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Vector2Int CellCoords(float x, float y)
    {
        return new Vector2Int(Lib.Math.FloorToInt(x * invCellSize), Lib.Math.FloorToInt(y * invCellSize));
    }
    /// <summary>
    /// Gets the cell at the world-space x,y value. If the cell is empty and createCellIfEmpty is true a new cell will be created.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    List<PhysicsBody> CellAtPosition(int x, int y, bool createCellIfEmpty = false)
    {
        if (!cells.TryGetValue(x, y, out List<PhysicsBody> cell))
        {
            if (createCellIfEmpty)
            {
                cell = new List<PhysicsBody>();
                cells.Add(x, y, cell);
            }
        }

        return cell;
    }
    #endregion
    #region SpatialDictionary
    internal class SpatialDictionary
    {
        internal readonly Dictionary<long, List<PhysicsBody>> cells = new Dictionary<long, List<PhysicsBody>>();

        /// <summary>
        /// Computes and returns a hash key by packing the x and y coordinates into a long.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long GetKey(int x, int y)
        {
            return unchecked((long)x << 32 | (uint)y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int x, int y, List<PhysicsBody> list)
        {
            cells.Add(GetKey(x, y), list);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(PhysicsBody body)
        {
            foreach (List<PhysicsBody> list in cells.Values)
            {
                if (list.Contains(body))
                    list.Remove(body);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(int x, int y, out List<PhysicsBody> list)
        {
            return cells.TryGetValue(GetKey(x, y), out list);
        }

        public void Clear()
        {
            cells.Clear();
        }
    }
    #endregion
}