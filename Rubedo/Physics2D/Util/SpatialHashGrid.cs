using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Render;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// I am SpatialHashGrid, and this is my summary.
/// </summary>
public class SpatialHashGrid
{
    /*
    public AABB GridBounds = new AABB()
    {
        Min = Vector2.Zero,
        Max = Vector2.Zero
    };

    int _cellSize;
    float _inverseCellSize;
    HashDictionary _cells = new HashDictionary();

    public SpatialHashGrid(int cellSize = 100)
    {
        _cellSize = cellSize;
        _inverseCellSize = 1f / cellSize;
    }
    
    /// <summary>
    /// Gets the cell coordinate for a world-space x,y value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Vector2Int CellCoords(float x, float y)
    {
        return new Vector2Int(Math.FloorToInt(x * _inverseCellSize), Math.FloorToInt(y * _inverseCellSize));
    }

    /// <summary>
    /// Gets the cell at the world-space x,y value. If the cell is empty and createCellIfEmpty is true a new cell will be created.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    List<PhysicsBody> CellAtPosition(int x, int y, bool createCellIfEmpty = false)
    {
        if (!_cells.TryGetValue(x, y, out List<PhysicsBody> cell))
        {
            if (createCellIfEmpty)
            {
                cell = new List<PhysicsBody>();
                _cells.Add(x, y, cell);
            }
        }

        return cell;
    }

    public void Update(PhysicsBody collider)
    {
        Remove(collider);
        Add(collider);
    }

    /// <summary>
    /// adds the object to the SpatialHash
    /// </summary>
    /// <param name="collider">Object.</param>
    public void Add(PhysicsBody collider)
    {
        AABB bounds = collider.collider.shape.Bounds;
        collider.collider.shape.RegisteredBounds = bounds;
        Vector2Int p1 = CellCoords(bounds.Min.X, bounds.Min.Y);
        Vector2Int p2 = CellCoords(bounds.Max.X, bounds.Max.Y);

        // update our bounds to keep track of our grid size
        if (!GridBounds.Contains(ref p1))
            AABB.Union(ref GridBounds, ref p1, out GridBounds);

        if (!GridBounds.Contains(ref p2))
            AABB.Union(ref GridBounds, ref p2, out GridBounds);

        for (int x = p1.X; x <= p2.X; x++)
        {
            for (int y = p1.Y; y <= p2.Y; y++)
            {
                // we need to create the cell if there is none
                CellAtPosition(x, y, true).Add(collider);
            }
        }
    }

    /// <summary>
    /// Removes the object from the SpatialHash
    /// </summary>
    /// <param name="collider">Collider.</param>
    public void Remove(PhysicsBody collider)
    {
        AABB bounds = collider.collider.shape.RegisteredBounds;
        Vector2Int p1 = CellCoords(bounds.Min.X, bounds.Min.Y);
        Vector2Int p2 = CellCoords(bounds.Max.X, bounds.Max.Y);

        for (int x = p1.X; x <= p2.X; x++)
        {
            for (int y = p1.Y; y <= p2.Y; y++)
            {
                // the cell should probably always exist since this collider should be in all queryed cells
                CellAtPosition(x, y)?.Remove(collider);
            }
        }
    }

    public void Clear()
    {
        _cells.Clear();
        GridBounds = new AABB()
        {
            Min = Vector2.Zero,
            Max = Vector2.Zero
        };
    }


    /// <summary>
    /// Returns all valid objects in cells that the object's bounding box intersects.
    /// </summary>
    public void AABB_Broadphase(in PhysicsBody inObject, in HashSet<(PhysicsBody, PhysicsBody)> pairSet, in List<CollisionPair> pairList)
    {
        AABB bounds = inObject.collider.shape.Bounds;
        Vector2Int p1 = CellCoords(bounds.Min.X, bounds.Min.Y);
        Vector2Int p2 = CellCoords(bounds.Max.X, bounds.Max.Y);

        for (int x = p1.X; x <= p2.X; x++)
        {
            for (int y = p1.Y; y <= p2.Y; y++)
            {
                List<PhysicsBody> cell = CellAtPosition(x, y);
                if (cell == null || cell.Count == 0)
                    continue;

                // we have an occupied cell. loop through and fetch all the Colliders
                for (int i = 0; i < cell.Count; i++)
                {
                    PhysicsBody obj = cell[i];

                    // skip this collider if it's the thing we're checking or if it doesnt match our layerMask
                    if (obj == inObject)// || !Flags.IsFlagSet(layerMask, collider.PhysicsLayer))
                        continue;

                    if (bounds.Intersects(ref obj.collider.shape.Bounds))
                    {
                        if (pairSet.Add((inObject, obj)))
                        {
                            pairList.Add(new CollisionPair(inObject, obj));
                        }
                    }
                }
            }
        }
    }


    public void DebugDraw(Shapes shapes, float textScale = 1f)
    {
        for (int x = Math.FloorToInt(GridBounds.Min.X); x <= Math.CeilToInt(GridBounds.Max.X); x++)
        {
            for (int y = Math.FloorToInt(GridBounds.Min.Y); y <= Math.CeilToInt(GridBounds.Max.Y); y++)
            {
                List<PhysicsBody> cell = CellAtPosition(x, y);
                if (cell != null && cell.Count > 0)
                    DebugDrawCellDetails(shapes, x, y, cell.Count, textScale);
            }
        }
    }


    void DebugDrawCellDetails(Shapes shapes, float x, float y, int cellCount, float textScale = 1f)
    {
        shapes.DrawBox(new Vector2(x * _cellSize, y * _cellSize), new Vector2((x + 1) * _cellSize, (y + 1) * _cellSize), Color.DarkBlue);

        if (cellCount > 0)
        {
            Vector2 textPosition = new Vector2((float)x * _cellSize + 0.5f * _cellSize,
                (float)y * _cellSize + 0.5f * _cellSize);
            RubedoEngine.debugText.DrawText(textPosition, cellCount.ToString(), false);
        }
    }


    class HashDictionary
    {
        internal readonly Dictionary<long, List<PhysicsBody>> map = new Dictionary<long, List<PhysicsBody>>();


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
            map.Add(GetKey(x, y), list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(PhysicsBody obj)
        {
            foreach (var list in map.Values)
            {
                if (list.Contains(obj))
                    list.Remove(obj);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(int x, int y, out List<PhysicsBody> list)
        {
            return map.TryGetValue(GetKey(x, y), out list);
        }

        public void Clear()
        {
            map.Clear();
        }
    }*/
}