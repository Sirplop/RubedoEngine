//#define CONSTANT_SHAPE
//#define RANDOM_SHAPE

using Microsoft.Xna.Framework;
using Rubedo.Components;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.ColliderShape;
using Rubedo.Render;
using System.Collections.Generic;

namespace Rubedo.Tests;

/// <summary>
/// Debug testing for drawing and colliding various collider shapes.
/// </summary>
public class TestPhysicsWorld : Entity
{
    private int bodyCount = 25;
    private Shapes shapes;
    public List<Color> colors = new List<Color>();
    public List<Color> outlineColors =  new List<Color>();

    public TestPhysicsWorld(GameState state)
    {
        shapes = new Shapes(RubedoEngine.Instance);

#if CONSTANT_SHAPE
        int i = 0;
        MakeBody(state, ShapeType.Circle, 50 * i++ - 250, 0, false);
        MakeBody(state, ShapeType.Box, 50 * i++ - 250, 0, false);
        MakeBody(state, ShapeType.Capsule, 50 * i++ - 250, 0, false);
        MakeBody(state, ShapeType.Polygon, 50 * i++ - 250, 0, false);
        MakeBody(state, ShapeType.Circle, 50 * i++ - 250, 0, true);
        MakeBody(state, ShapeType.Box, 50 * i++ - 250, 0, true);
        MakeBody(state, ShapeType.Capsule, 50 * i++ - 250, 0, true);
        MakeBody(state, ShapeType.Polygon, 50 * i++ - 250, 0, true);
#elif RANDOM_SHAPE
        for (int i = 0; i < bodyCount; i++)
        {
            int type = Random.Range(0, 4);
            float x = Random.Range(-350, 350);
            float y = Random.Range(-200, 200);

            MakeBody(state, (ShapeType)type, x, y, Random.Flip);
        }
#else

        Entity entity = new Entity(new Vector2(-100, 0), -22.5f);
        Collider comp = Collider.CreateBox(entity.transform, 200, 10);
        MakeBody(state, entity, comp, true);

        entity = new Entity(new Vector2(100, 70), 22.5f);
        comp = Collider.CreateBox(entity.transform, 200, 10);
        MakeBody(state, entity, comp, true);

        entity = new Entity(new Vector2(0, -150));
        comp = Collider.CreateBox(entity.transform, 720, 60);
        MakeBody(state, entity, comp, true);
#endif
    }

    public void MakeBody(GameState state, Entity entity, Collider comp, bool isStatic)
    {
        PhysicsBody body = new PhysicsBody(comp, new Transform(), 1, 0.5f, isStatic, true, true);
        RubedoEngine.Instance.World.AddBody(new PhysicsObject(body, comp));
        entity.Add(body);
        entity.Add(comp);
        Text text = new Text(AssetManager.LoadFont("Consolas"), RubedoEngine.Instance.World.BodyCount.ToString(), Color.White, true, true);
        text.centerMode = Text.CenterText.None;
        entity.Add(text);
        state.Add(entity);
        if (isStatic)
        {
            colors.Add(Color.DarkGray);
            outlineColors.Add(Color.Red);
        } else
        {
            colors.Add(Random.Color());
            outlineColors.Add(Color.White);
        }
    }

    public override void Update()
    {
        base.Update();
        DeleteIfTooFar();
    }

    public override void Draw(Renderer sb)
    {
        shapes.Begin(RubedoEngine.Instance.Camera);
        RubedoEngine.Instance.World.DebugDraw(shapes);

        for (int i = 0; i < RubedoEngine.Instance.World.BodyCount; i++)
        {
            RubedoEngine.Instance.World.GetBody(i, out PhysicsObject body);
            switch (body.collider.shape.ShapeType)
            {
                case ShapeType.Circle:
                    CircleShape shape = (CircleShape)body.collider.shape;
                    shapes.DrawCircleFill(shape.Transform, shape.Radius, 32, colors[i]);
                    shapes.DrawCircle(shape.Transform, shape.Radius, 32, outlineColors[i]);
                    break;
                case ShapeType.Box:
                    BoxShape box = (BoxShape)body.collider.shape;
                    shapes.DrawBoxFill(box.Transform, box.width, box.height, colors[i]);
                    shapes.DrawBox(box.Transform, box.width, box.height, outlineColors[i]);
                    break;
                case ShapeType.Capsule:
                    CapsuleShape capsule = (CapsuleShape)body.collider.shape;
                    shapes.DrawCapsuleFill(capsule.Transform, capsule.Radius, capsule.Start, capsule.End, 11, colors[i]);
                    shapes.DrawCapsule(capsule.Transform, capsule.Radius, capsule.Start, capsule.End, 11, outlineColors[i]);
                    break;
                case ShapeType.Polygon:
                    PolygonShape polygon = (PolygonShape)body.collider.shape;
                    shapes.DrawPolygonFill(polygon.TransformedVertices, new int[] { 0, 1, 2 }, polygon.Transform, colors[i]);
                    shapes.DrawPolygon(polygon.TransformedVertices, polygon.Transform, outlineColors[i]);
                    break;
            }
        }
        for (int i = 0; i < RubedoEngine.Instance.World.contactPoints.Count; i++)
        {
            shapes.DrawBoxFill(RubedoEngine.Instance.World.contactPoints[i], 5, 5, 0, Vector2.One, Color.Red);
            shapes.DrawBox(RubedoEngine.Instance.World.contactPoints[i], 5, 5, 0, Vector2.One, Color.White);
        }
        shapes.End();
    }

    public void DeleteIfTooFar()
    {
        if (RubedoEngine.Instance.World.BodyCount == 0)
            return;
        RubedoEngine.Instance.Camera.GetExtents(out Vector2 min, out Vector2 max);

        for (int i = 0; i < RubedoEngine.Instance.World.BodyCount; i++)
        {
            if (!RubedoEngine.Instance.World.GetBody(i, out PhysicsObject body))
                throw new System.Exception();
            if (body.body.isStatic) continue;

            AABB bounds = body.collider.shape.Bounds;

            if (bounds.Max.Y < min.Y)
            {
                if (!RubedoEngine.Instance.World.RemoveBody(body))
                    throw new System.Exception("FUQ");
                colors.RemoveAt(i);
                outlineColors.RemoveAt(i);
                body.body.Entity.State.Remove(body.body.Entity);
            }
        }
    }

   /* public void WrapScreen()
    {
        if (RubedoEngine.Instance.World.BodyCount == 0)
            return;
        RubedoEngine.Instance.Camera.GetExtents(out Vector2 min, out Vector2 max);

        float viewWidth = max.X - min.X;
        float viewHeight = max.Y - min.Y;

        for (int i = 0; i < RubedoEngine.Instance.World.BodyCount; i++)
        {
            if (!RubedoEngine.Instance.World.GetBody(i, out PhysicsBodyComponent body))
                throw new System.Exception();

            if (body.worldTransform.Position.X < min.X) body.Entity.transform.Position += new Vector2(viewWidth, 0f);
            if (body.worldTransform.Position.X > max.X) body.Entity.transform.Position -= new Vector2(viewWidth, 0f);
            if (body.worldTransform.Position.Y < min.Y) body.Entity.transform.Position += new Vector2(0f, viewHeight);
            if (body.worldTransform.Position.Y > max.Y) body.Entity.transform.Position -= new Vector2(0f, viewHeight);
        }
    }*/
}