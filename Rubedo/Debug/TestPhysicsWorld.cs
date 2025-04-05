using Microsoft.Xna.Framework;
using Rubedo.Components;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Util;
using Rubedo.Render;

namespace Rubedo.Tests;

/// <summary>
/// Debug testing for drawing and colliding various collider shapes.
/// </summary>
public class TestPhysicsWorld : Entity
{
    private Shapes shapes;

    public TestPhysicsWorld(GameState state)
    {
        shapes = new Shapes(RubedoEngine.Instance);
    }

    public void MakeBody(GameState state, Entity entity, PhysicsMaterial material, Collider collider, bool isStatic)
    {
        PhysicsBody body = new PhysicsBody(collider, material);
        if (isStatic)
            body.SetStatic();
        entity.Add(body);
        entity.Add(collider);
        RubedoEngine.Instance.World.AddBody(body);
        state.Add(entity);
    }

    public override void Update()
    {
        base.Update();
        DeleteIfTooFar();
    }

    public override void Draw(Renderer sb)
    {
        shapes.Begin(RubedoEngine.Instance.Camera);

        for (int i = 0; i < RubedoEngine.Instance.World.BodyCount; i++)
        {
            RubedoEngine.Instance.World.GetBody(i, out PhysicsBody body);
            Color speedColor;
            if (body.isStatic)
                speedColor = new Color(50, 50, 50);
            else
            {
                float val = body.LinearVelocity.Length();
                float vel = 220 - System.MathF.Min(val, 220) % 360;
                Lib.Math.HsvToRgb(vel, 1, 1, out int r, out int g, out int b);
                speedColor = new Color(r, g, b);
            }

            AABB bounds = body.collider.shape.Bounds;
            shapes.DrawBox(bounds.Min, bounds.Max, Color.Green);

            switch (body.collider.shape.ShapeType)
            {
                case ShapeType.Circle:
                    Circle shape = (Circle)body.collider.shape;
                    Matrix2D matrix = shape.transform.ToMatrixWorld();
                    Vector2 vA = shape.transform.WorldPosition;
                    Vector2 vB = matrix.Transform(Vector2.UnitY * shape.radius);

                    //shapes.DrawCircleFill(shape.Transform, shape.Radius, 32, speedColor);
                    shapes.DrawLine(vA, vB, Color.White);
                    shapes.DrawCircle(shape.transform, shape.radius, 32, Color.White);
                    break;
                case ShapeType.Box:
                    Box box = (Box)body.collider.shape;
                    //shapes.DrawBoxFill(box.Transform, box.width, box.height, speedColor);
                    shapes.DrawBox(box.transform, box.width, box.height, Color.White);
                    break;
                case ShapeType.Capsule:
                    Capsule capsule = (Capsule)body.collider.shape;
                    capsule.GetTransformedPoints(out Vector2 start, out Vector2 end, out float radius);
                    //shapes.DrawCapsuleFill(capsule.Transform, capsule.Radius, capsule.Start, capsule.End, 11, speedColor);
                    shapes.DrawCapsule(capsule.transform, radius, start, end, 11, Color.White);
                    break;
                case ShapeType.Polygon:
                    Polygon polygon = (Polygon)body.collider.shape;
                    //shapes.DrawPolygonFill(polygon.TransformedVertices, ColliderShapeUtility.ComputeTriangles(polygon.TransformedVertices.Length), polygon.Transform, speedColor);
                    shapes.DrawPolygon(polygon.vertices, polygon.transform, Color.White);
                    break;
            }
            shapes.DrawLine(body.transform.WorldPosition, body.transform.WorldPosition + body.LinearVelocity, Color.Aquamarine);
        }
        RubedoEngine.Instance.World.DebugDraw(shapes);
        shapes.End();
    }

    public void DeleteIfTooFar()
    {
        if (RubedoEngine.Instance.World.BodyCount == 0)
            return;
        RubedoEngine.Instance.Camera.GetExtents(out Vector2 min, out Vector2 max);

        for (int i = 0; i < RubedoEngine.Instance.World.BodyCount; i++)
        {
            if (!RubedoEngine.Instance.World.GetBody(i, out PhysicsBody body))
                throw new System.Exception();
            if (body.isStatic)
                continue;

            AABB bounds = body.collider.shape.Bounds;

            if (bounds.Max.Y < min.Y)
            {
                if (!RubedoEngine.Instance.World.RemoveBody(body))
                    throw new System.Exception("FUQ");
                body.Entity.State.Remove(body.Entity);
            }
        }
    }
}