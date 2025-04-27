using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Math;
using Rubedo.Rendering;

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

    /*public void MakeBody(Shape shape, PhysicsMaterial material, Vector2 pos, float rotation, bool isStatic)
    {
        PhysicsBody b = new PhysicsBody(shape, pos, rotation, material);
        if (isStatic)
            b.SetStatic();
        RubedoEngine.Instance.World.AddBody(b);
    }*/

    
    public void MakeBody(GameState state, Entity entity, PhysicsMaterial material, Collider collider, bool isStatic)
    {
        PhysicsBody body = new PhysicsBody(collider, material, true, true);
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

        for (int i = 0; i < RubedoEngine.Instance.World.bodies.Count; i++)
        {
            //RubedoEngine.Instance.World.GetBody(i, out PhysicsBody body);
            PhysicsBody body = RubedoEngine.Instance.World.bodies[i];
            Color speedColor;
            if (body.isStatic)
                speedColor = new Color(50, 50, 50);
            else
            {
                float val = body.LinearVelocity.Length() * 5f;
                float vel = 220 - System.MathF.Min(val, 220) % 360;
                MathColor.HsvToRgb(vel, 1, 1, out int r, out int g, out int b);
                speedColor = new Color(r, g, b);
            }

            AABB bounds = body.bounds;
            shapes.DrawBox(bounds.min, bounds.max, Color.Green);

            switch (body.collider.shape.type)
            {
                case ShapeType.Circle:
                    Circle shape = (Circle)body.collider.shape;
                    Vector2 vA = body.position;
                    Vector2 vB = shape.transform.LocalToWorldPosition(Vector2.UnitY * shape.radius);

                    shapes.DrawCircleFill(shape.transform, shape.radius, 32, speedColor);
                    shapes.DrawLine(vA, vB, Color.White);
                    shapes.DrawCircle(shape.transform, shape.radius, 32, Color.White);
                    break;
                case ShapeType.Box:
                    Box box = (Box)body.collider.shape;
                    shapes.DrawBoxFill(box.transform, box.width, box.height, speedColor);
                    shapes.DrawBox(box.transform, box.width, box.height, Color.White);
                    break;
                case ShapeType.Capsule:
                    Capsule capsule = (Capsule)body.collider.shape;
                    capsule.TransformPoints();
                    shapes.DrawCapsuleFill(capsule.transform, capsule.transRadius, capsule.transStart, capsule.transEnd, 11, speedColor);
                    shapes.DrawCapsule(capsule.transform, capsule.transRadius, capsule.transStart, capsule.transEnd, 11, Color.White);
                    break;
                case ShapeType.Polygon:
                    Polygon polygon = (Polygon)body.collider.shape;
                    shapes.DrawPolygonFill(polygon.vertices, ShapeUtility.ComputeTriangles(polygon.VertexCount), polygon.transform, speedColor);
                    shapes.DrawPolygon(polygon.vertices, polygon.transform, Color.White);
                    break;
            }
            shapes.DrawLine(body.compTransform.Position, body.compTransform.Position + body.LinearVelocity, Color.Aquamarine);
        }

        RubedoEngine.Instance.World.DebugDraw(shapes);
        shapes.End();
    }

    public void DeleteIfTooFar()
    {
        if (RubedoEngine.Instance.World.bodies.Count == 0)
            return;
        RubedoEngine.Instance.Camera.GetExtents(out Vector2 min, out Vector2 max);

        for (int i = 0; i < RubedoEngine.Instance.World.bodies.Count; i++)
        {
            PhysicsBody body = RubedoEngine.Instance.World.bodies[i];
            if (body.isStatic)
                continue;

            AABB bounds = body.bounds;

            if (bounds.max.Y < min.Y)
            {
                RubedoEngine.Instance.World.RemoveBody(body);
                //if (!RubedoEngine.Instance.World.RemoveBody(body))
                //    throw new System.Exception("FUQ");
                //body.Entity.State.Remove(body.Entity);
            }
        }
    }
}