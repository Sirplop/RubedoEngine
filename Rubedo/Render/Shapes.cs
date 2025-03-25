using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib;
using Rubedo.Object;
using System;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;

namespace Rubedo.Render;

/// <summary>
/// Draw primitive shapes for testing.
/// </summary>
public sealed class Shapes : IDisposable
{
    RubedoEngine game;
    private bool started;
    private bool isDisposed;

    private BasicEffect effect;
    private VertexPositionColor[] vertices;
    private int[] indices;
    private int shapeCount;
    private int indexCount;
    private int vertexCount;

    public Shapes(RubedoEngine game)
    {
        this.game = game ?? throw new ArgumentNullException("game");

        this.effect = new BasicEffect(this.game.GraphicsDevice);
        this.effect.FogEnabled = false;
        this.effect.LightingEnabled = false;
        this.effect.TextureEnabled = false;
        this.effect.VertexColorEnabled = true;
        this.effect.PreferPerPixelLighting = false;

        this.vertices = new VertexPositionColor[1024];
        this.indices = new int[this.vertices.Length * 3];

        this.shapeCount = 0;
        this.vertexCount = 0;
        this.indexCount = 0;

        started = false;
        isDisposed = false;
    }

    ~Shapes()
    {
        this.Dispose(false);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.effect?.Dispose();
            this.effect = null;
        }

        this.isDisposed = true;
    }

    public void Begin(Camera camera)
    {
        if (this.started)
        {
            throw new Exception("Batch was already started.\n" +
                                "Batch must call \"End\" before new batching can start.");
        }
        if (camera is null)
        {
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, game.GraphicsDevice.Viewport.Width, 0, game.GraphicsDevice.Viewport.Height, 0, 1.0f);
        }
        else
        {
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = camera.TransformMatrix;
        }

        started = true;
    }
    public void End()
    {
        if (!this.started)
        {
            throw new Exception("Batch was not started.\n" +
                                "Batch must call \"Begin\" before ending the batch.");
        }

        this.Flush();
        this.started = false;
    }
    private void Flush()
    {
        if (this.shapeCount == 0)
        {
            return;
        }

        GraphicsDevice device = this.game.GraphicsDevice;
        int primitiveCount = this.indexCount / 3;

        EffectPassCollection passes = this.effect.CurrentTechnique.Passes;
        for (int i = 0; i < passes.Count; i++)
        {
            EffectPass pass = passes[i];
            pass.Apply();

            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList,
                this.vertices,
                0,
                this.vertexCount,
                this.indices,
                0,
                primitiveCount);
        }

        this.shapeCount = 0;
        this.vertexCount = 0;
        this.indexCount = 0;
    }
    public void EnsureStarted()
    {
        if (!this.started)
        {
            throw new Exception("Shape batching must be started first.");
        }
    }
    private void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
    {
        int maxVertexCount = this.vertices.Length;
        int maxIndexCount = this.indices.Length;

        if (shapeVertexCount > maxVertexCount || shapeIndexCount > maxIndexCount)
        {
            throw new Exception("Max vertex or index count reached for one draw.");
        }

        if (this.vertexCount + shapeVertexCount > maxVertexCount || this.indexCount + shapeIndexCount > maxIndexCount)
        {
            this.Flush();
        }
    }
    #region Box
    public void DrawBoxFill(Vector2 min, Vector2 max, Color[] colors)
    {
        if (colors is null || colors.Length != 4)
        {
            throw new ArgumentOutOfRangeException("colors array must have exactly 4 items.");
        }

        this.EnsureStarted();

        int shapeVertexCount = 4;
        int shapeIndexCount = 6;

        this.EnsureSpace(shapeVertexCount, shapeIndexCount);

        Vector3 a = new Vector3(min.X, max.Y, 0f);
        Vector3 b = new Vector3(max.X, max.Y, 0f);
        Vector3 c = new Vector3(max.X, min.Y, 0f);
        Vector3 d = new Vector3(min.X, min.Y, 0f);

        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 1 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 3 + this.vertexCount;

        this.vertices[this.vertexCount++] = new VertexPositionColor(a, colors[0]);
        this.vertices[this.vertexCount++] = new VertexPositionColor(b, colors[1]);
        this.vertices[this.vertexCount++] = new VertexPositionColor(c, colors[2]);
        this.vertices[this.vertexCount++] = new VertexPositionColor(d, colors[3]);

        this.shapeCount++;
    }

    public void DrawBoxFill(Transform transform, float width, float height, Color color)
    {
        DrawBoxFill(transform.Position, width, height, transform.RotationRadians, transform.Scale, color);
    }
    public void DrawBoxFill(Vector2 center, float width, float height, float rotation, Vector2 scale, Color color)
    {
        this.EnsureStarted();

        int shapeVertexCount = 4;
        int shapeIndexCount = 6;

        this.EnsureSpace(shapeVertexCount, shapeIndexCount);

        float left = -width * 0.5f;
        float right = left + width;
        float bottom = -height * 0.5f;
        float top = bottom + height;

        // Precompute the trig. functions.
        float sin = MathF.Sin(rotation);
        float cos = MathF.Cos(rotation);

        // Vector components:

        float ax = left;
        float ay = top;
        float bx = right;
        float by = top;
        float cx = right;
        float cy = bottom;
        float dx = left;
        float dy = bottom;

        // Scale transform:

        float sx1 = ax * scale.X;
        float sy1 = ay * scale.Y;
        float sx2 = bx * scale.X;
        float sy2 = by * scale.Y;
        float sx3 = cx * scale.X;
        float sy3 = cy * scale.Y;
        float sx4 = dx * scale.X;
        float sy4 = dy * scale.Y;

        // Rotation transform:

        float rx1 = sx1 * cos - sy1 * sin;
        float ry1 = sx1 * sin + sy1 * cos;
        float rx2 = sx2 * cos - sy2 * sin;
        float ry2 = sx2 * sin + sy2 * cos;
        float rx3 = sx3 * cos - sy3 * sin;
        float ry3 = sx3 * sin + sy3 * cos;
        float rx4 = sx4 * cos - sy4 * sin;
        float ry4 = sx4 * sin + sy4 * cos;

        // Translation transform:

        ax = rx1 + center.X;
        ay = ry1 + center.Y;
        bx = rx2 + center.X;
        by = ry2 + center.Y;
        cx = rx3 + center.X;
        cy = ry3 + center.Y;
        dx = rx4 + center.X;
        dy = ry4 + center.Y;

        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 1 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 3 + this.vertexCount;

        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax, ay, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(bx, by, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(cx, cy, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(dx, dy, 0f), color);

        this.shapeCount++;
    }
    #endregion
    #region Quad
    public void DrawQuadFill(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy, Color color)
    {
        this.EnsureStarted();

        const int shapeVertexCount = 4;
        const int shapeIndexCount = 6;

        this.EnsureSpace(shapeVertexCount, shapeIndexCount);

        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 1 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 3 + this.vertexCount;

        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax, ay, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(bx, by, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(cx, cy, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(dx, dy, 0f), color);

        this.shapeCount++;
    }

    public void DrawQuadFill(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
    {
        this.DrawQuadFill(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y, color);
    }
    #endregion
    #region Circle
    public void DrawCircleFill(Transform transform, float radius, int points, Color color)
    {
        this.EnsureStarted();

        const int MinCirclePoints = 3;
        const int MaxCirclePoints = 64;

        int shapeVertexCount = Lib.Math.Clamp(points, MinCirclePoints, MaxCirclePoints);    // The vertex count will just be the number of points requested by the user.
        int shapeTriangleCount = shapeVertexCount - 2;      // The triangle count of a convex polygon is alway 2 less than the vertex count.
        int shapeIndexCount = shapeTriangleCount * 3;       // The indices count will just be 3 times the triangle count.

        this.EnsureSpace(shapeVertexCount, shapeIndexCount);

        float angle = MathHelper.TwoPi / (float)shapeVertexCount;
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        int index = 1;

        // Indicies;
        for (int i = 0; i < shapeTriangleCount; i++)
        {
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = index + this.vertexCount;
            this.indices[this.indexCount++] = index + 1 + this.vertexCount;

            index++;
        }

        // Vertices;

        float ax = radius;
        float ay = 0f;

        // Save all remaining vertices.
        for (int i = 0; i < shapeVertexCount; i++)
        {
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(ax + transform.Position.X, ay + transform.Position.Y, 0f), color);

            float bx = ax * cos - ay * sin;
            float by = ax * sin + ay * cos;

            ax = bx;
            ay = by;
        }

        this.shapeCount++;
    }
    #endregion
    #region Ellipsis
    public void DrawEllipseFill(float x, float y, float xRadius, float yRadius, int points, Color color)
    {
        this.DrawEllipseFill(new Vector2(x, y), new Vector2(xRadius, yRadius), points, color);
    }

    public void DrawEllipseFill(Vector2 center, Vector2 radius, int points, Color color)
    {
        /*
         * How to draw ellipses:
         * https://community.khronos.org/t/how-to-draw-an-oval/13428
         */

        this.EnsureStarted();

        const int MinElipsePoints = 3;
        const int MaxElipsePoints = 256;

        int shapeVertexCount = Lib.Math.Clamp(points, MinElipsePoints, MaxElipsePoints);    // The vertex count will just be the number of points requested by the user.
        int shapeTriangleCount = shapeVertexCount - 2;      // The triangle count of a convex polygon is alway 2 less than the vertex count.
        int shapeIndexCount = shapeTriangleCount * 3;       // The indices count will just be 3 times the triangle count.

        this.EnsureSpace(shapeVertexCount, shapeIndexCount);

        float deltaAngle = MathHelper.TwoPi / (float)shapeVertexCount;
        float angle = 0f;

        int index = 1;

        // Indicies;
        for (int i = 0; i < shapeTriangleCount; i++)
        {
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = index + this.vertexCount;
            this.indices[this.indexCount++] = index + 1 + this.vertexCount;

            index++;
        }

        // Vertices;

        // Save all remaining vertices.
        for (int i = 0; i < shapeVertexCount; i++)
        {
            float x = MathF.Cos(angle) * radius.X + center.X;
            float y = MathF.Sin(angle) * radius.Y + center.Y;

            angle += deltaAngle;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(x, y, 0f), color);
        }

        this.shapeCount++;
    }
    #endregion
    #region Capsule
    public void DrawCapsuleFill(Transform transform, float radius, Vector2 start, Vector2 end, int pointsPerCap, Color color)
    {
        this.EnsureStarted();

        const int MinElipsePoints = 3;
        const int MaxElipsePoints = 64;

        int shapeVertexCount = Lib.Math.Clamp(pointsPerCap, MinElipsePoints, MaxElipsePoints) * 2;    // The vertex count will just be the number of points requested by the user.
        int shapeTriangleCount = (shapeVertexCount) - 2;      // The triangle count of a convex polygon is alway 2 less than the vertex count.
        int shapeIndexCount = (shapeTriangleCount) * 3;       // The indices count will just be 3 times the triangle count.

        this.EnsureSpace(shapeVertexCount, shapeIndexCount);

        float deltaAngle = MathHelper.Pi / ((shapeVertexCount * 0.5f) - 1);
        float angle = MathHelper.Pi;

        int index = 1;

        // Indicies;
        for (int i = 0; i < shapeTriangleCount; i++)
        {
            this.indices[this.indexCount++] = 0 + this.vertexCount;
            this.indices[this.indexCount++] = index + this.vertexCount;
            this.indices[this.indexCount++] = index + 1 + this.vertexCount;

            index++;
        }

        Matrix2D matrix = Matrix2D.CreateRS(transform.Rotation, transform.Scale.X, transform.Scale.Y);

        // Vertices;
        // Save all remaining vertices.
        for (int i = 0; i < shapeVertexCount / 2; i++)
        {
            float x = MathF.Cos(angle) * radius;
            float y = MathF.Sin(angle) * radius;
            Vector2 val = matrix.Transform(x, y) + start;
            angle += deltaAngle;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(val, 0f), color);
        }
        angle = 0;
        for (int i = 0; i < shapeVertexCount / 2; i++)
        {
            float x = MathF.Cos(angle) * radius;
            float y = MathF.Sin(angle) * radius;
            Vector2 val = matrix.Transform(x, y) + end;
            angle += deltaAngle;

            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(val, 0f), color);
        }

        this.shapeCount++;
    }
    #endregion
    #region Polygon
    public void DrawPolygonFill(Vector2[] vertices, int[] triangles, Transform transform, Color color)
    {
        this.EnsureStarted();
        this.EnsureSpace(vertices.Length, indices.Length);

        for (int i = 0; i < triangles.Length; i++)
        {
            this.indices[this.indexCount++] = this.vertexCount + triangles[i];
        }
        //Matrix2D matrix = transform.ToMatrix();
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            //v = matrix.Transform(v);
            this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(v.X, v.Y, 0f), color);
        }

        this.shapeCount++;
    }
    #endregion
    #region Line
    public void DrawLine(Vector2 a, Vector2 b, Color color)
    {
        this.DrawLine(a.X, a.Y, b.X, b.Y, color);
    }
    public void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
        // Default thickness with no zoom.
        float thickness = 2f;

        // If we are using the world camera then we need to adjust the "thickness" of the line
        //  so no matter how far we have "zoomed" into the world the line will look the same.
        /*if (this.usingCamera)
        {
            thickness /= (float)this.camera.Zoom;
        }*/ //implement when camera implemented.

        float halfThickness = thickness * 0.5f;

        // Line edge pointing from "b" to "a".
        float e1x = x2 - x1;
        float e1y = y2 - y1;

        Lib.Math.Normalize(ref e1x, ref e1y);

        float n1x = -e1y;
        float n1y = e1x;

        e1x *= halfThickness;
        e1y *= halfThickness;

        n1x *= halfThickness;
        n1y *= halfThickness;

        float e2x = -e1x;
        float e2y = -e1y;

        float n2x = -n1x;
        float n2y = -n1y;

        float qax = x1 + n1x + e2x;
        float qay = y1 + n1y + e2y;

        float qbx = x2 + n1x + e1x;
        float qby = y2 + n1y + e1y;

        float qcx = x2 + n2x + e1x;
        float qcy = y2 + n2y + e1y;

        float qdx = x1 + n2x + e2x;
        float qdy = y1 + n2y + e2y;

        this.DrawQuadFill(qax, qay, qbx, qby, qcx, qcy, qdx, qdy, color);
    }

    public void GetLine(float x1, float y1, float x2, float y2, Color color, VertexPositionColor[] vertices, int[] indices, ref int vertexCount, ref int indexCount)
    {
        if (vertices == null)
        {
            throw new ArgumentNullException("vertices");
        }

        if (indices == null)
        {
            throw new ArgumentNullException("indices");
        }

        // Default thickness with not zoom.
        float thickness = 2f;

        // If we are using the world camera then we need to adjust the "thickness" of the line
        //  so no matter how far we have "zoomed" into the world the line will look the same.
        /*if (this.usingCamera)
        {
            thickness /= (float)this.camera.Zoom;
        }*/ //implement when camera implemented.

        float halfThickness = thickness * 0.5f;

        // Line edge pointing from "b" to "a".
        float e1x = x2 - x1;
        float e1y = y2 - y1;

        Lib.Math.Normalize(ref e1x, ref e1y);

        float n1x = -e1y;
        float n1y = e1x;

        e1x *= halfThickness;
        e1y *= halfThickness;

        n1x *= halfThickness;
        n1y *= halfThickness;

        float e2x = -e1x;
        float e2y = -e1y;

        float n2x = -n1x;
        float n2y = -n1y;

        float qax = x1 + n1x + e2x;
        float qay = y1 + n1y + e2y;

        float qbx = x2 + n1x + e1x;
        float qby = y2 + n1y + e1y;

        float qcx = x2 + n2x + e1x;
        float qcy = y2 + n2y + e1y;

        float qdx = x1 + n2x + e2x;
        float qdy = y1 + n2y + e2y;

        indices[indexCount++] = vertexCount + 0;
        indices[indexCount++] = vertexCount + 1;
        indices[indexCount++] = vertexCount + 2;
        indices[indexCount++] = vertexCount + 0;
        indices[indexCount++] = vertexCount + 2;
        indices[indexCount++] = vertexCount + 3;

        vertices[vertexCount++] = new VertexPositionColor(new Vector3(qax, qay, 0f), color);
        vertices[vertexCount++] = new VertexPositionColor(new Vector3(qbx, qby, 0f), color);
        vertices[vertexCount++] = new VertexPositionColor(new Vector3(qcx, qcy, 0f), color);
        vertices[vertexCount++] = new VertexPositionColor(new Vector3(qdx, qdy, 0f), color);
    }
    #endregion

    #region Hollow Box
    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        this.DrawLine(a, b, color);
        this.DrawLine(b, c, color);
        this.DrawLine(c, a, color);
    }

    public void DrawBox(Vector2 min, Vector2 max, Color color)
    {
        this.DrawLine(min.X, max.Y, max.X, max.Y, color);
        this.DrawLine(max.X, max.Y, max.X, min.Y, color);
        this.DrawLine(max.X, min.Y, min.X, min.Y, color);
        this.DrawLine(min.X, min.Y, min.X, max.Y, color);
    }

    public void DrawBox(Transform transform, float width, float height, Color color)
    {
        this.DrawBox(transform.Position, width, height, transform.RotationRadians, transform.Scale, color);
    }

    public void DrawBox(float x, float y, float width, float height, Color color)
    {
        Vector2 min = new Vector2(x, y);
        Vector2 max = new Vector2(x + width, y + height);

        this.DrawBox(min, max, color);
    }

    public void DrawBox(Vector2 center, float width, float height, Color color)
    {
        Vector2 min = new Vector2(center.X - width * 0.5f, center.Y - height * 0.5f);
        Vector2 max = new Vector2(min.X + width, min.Y + height);

        this.DrawBox(min, max, color);
    }

    public void DrawBox(Vector2 center, float width, float height, float angle, Vector2 scale, Color color)
    {
        float left = -width * 0.5f * scale.X;
        float right = left + width * scale.X;
        float bottom = -height * 0.5f * scale.Y;
        float top = bottom + height * scale.Y;

        // Precompute the trig. functions.
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        // Vector components:

        float ax = left;
        float ay = top;
        float bx = right;
        float by = top;
        float cx = right;
        float cy = bottom;
        float dx = left;
        float dy = bottom;

        // Rotation transform:

        float rx1 = ax * cos - ay * sin;
        float ry1 = ax * sin + ay * cos;
        float rx2 = bx * cos - by * sin;
        float ry2 = bx * sin + by * cos;
        float rx3 = cx * cos - cy * sin;
        float ry3 = cx * sin + cy * cos;
        float rx4 = dx * cos - dy * sin;
        float ry4 = dx * sin + dy * cos;

        // Translation transform:

        ax = rx1 + center.X;
        ay = ry1 + center.Y;
        bx = rx2 + center.X;
        by = ry2 + center.Y;
        cx = rx3 + center.X;
        cy = ry3 + center.Y;
        dx = rx4 + center.X;
        dy = ry4 + center.Y;

        this.DrawLine(ax, ay, bx, by, color);
        this.DrawLine(bx, by, cx, cy, color);
        this.DrawLine(cx, cy, dx, dy, color);
        this.DrawLine(dx, dy, ax, ay, color);
    }
    #endregion
    #region Hollow Circle
    public void DrawCircle(Transform transform, float radius, int points, Color color)
    {
        const int MinCirclePoints = 3;
        const int MaxCirclePoints = 256;

        points = Lib.Math.Clamp(points, MinCirclePoints, MaxCirclePoints);

        float angle = MathHelper.TwoPi / (float)points;

        // Precalculate the trig. functions.
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        // Initial point location on the unit circle.
        float ax = radius;
        float ay = 0f;

        for (int i = 0; i < points; i++)
        {
            // Perform a 2D rotation transform to get the next point on the circle.
            float bx = ax * cos - ay * sin;
            float by = ax * sin + ay * cos;

            this.DrawLine(ax + transform.Position.X, ay + transform.Position.Y,
                bx + transform.Position.X, by + transform.Position.Y, color);

            // Save the last transform for the next transform in the loop.
            ax = bx;
            ay = by;
        }
    }
    #endregion
    #region Hollow Capsule
    public void DrawCapsule(Transform transform, float radius, Vector2 start, Vector2 end, int pointsPerCap, Color color)
    {
        const int MinCirclePoints = 3;
        const int MaxCirclePoints = 64;

        pointsPerCap = (Lib.Math.Clamp(pointsPerCap, MinCirclePoints, MaxCirclePoints) - 1) * 2;

        float angle = MathHelper.Pi / ((pointsPerCap * 0.5f));

        // Precalculate the trig. functions.
        float sin = -MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        // Initial point location on the unit circle.
        float ax = radius;
        float ay = 0;

        float bx = 0, by = 0;
        Matrix2D matrix = Matrix2D.CreateRS(transform.Rotation, transform.Scale.X, transform.Scale.Y);

        for (int i = 0; i < pointsPerCap / 2; i++)
        {
            // Perform a 2D rotation transform to get the next point on the circle.
            bx = ax * cos - ay * sin;
            by = ax * sin + ay * cos;

            this.DrawLine(
                matrix.Transform(ax, ay) + start,
                matrix.Transform(bx, by) + start,
                color);

            // Save the last transform for the next transform in the loop.
            ax = bx;
            ay = by;
        }

        this.DrawLine(
                matrix.Transform(ax, ay) + start,
                matrix.Transform(bx, by) + end,
            color);
        for (int i = 0; i < pointsPerCap / 2; i++)
        {
            // Perform a 2D rotation transform to get the next point on the circle.
            bx = ax * cos - ay * sin;
            by = ax * sin + ay * cos;

            this.DrawLine(
                matrix.Transform(ax, ay) + end,
                matrix.Transform(bx, by) + end,
                color);

            // Save the last transform for the next transform in the loop.
            ax = bx;
            ay = by;
        }
        this.DrawLine(
                matrix.Transform(ax, ay) + end,
                matrix.Transform(bx, by) + start,
                color);
    }
    #endregion
    #region Hollow Polygon
    
    public void DrawPolygon(Vector2[] vertices, Transform transform, Color color)
    {
        if (vertices is null || vertices.Length < 3)
        {
            return;
        }

        // Now perform the rest of the vertex transforms and draw a line between each, 
        //  except for the final line segment that connects back to the first.
        //Matrix2D matrix = transform.ToMatrix();
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 a = vertices[i];
            Vector2 b = vertices[(i + 1) % vertices.Length];

            //a = matrix.Transform(a);
            //b = matrix.Transform(b);

            this.DrawLine(a, b, color);
        }
    }
    #endregion
}