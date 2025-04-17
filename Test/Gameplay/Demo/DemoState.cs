using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsEngine2D;
using Rubedo;
using Rubedo.Debug;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Math;
using Rubedo.Render;
using System;
using System.Collections.Generic;

namespace Test.Gameplay.Demo;

/// <summary>
/// I am Demo, and this is my summary.
/// </summary>
internal class DemoState : GameState
{
    public static bool colorVelocity = false;
    public static bool showVelocity = false;
    public static bool showAABB = false;
    public static bool fastPlace = false;

    private Shapes shapes;

    private DemoBase[] demos = new DemoBase[]
    {
        new Demo1(),
        new Demo2(),
        new Demo3(),
        new Demo4()
    };

    private int selectedDemo = 0;

    public DemoState(StateManager sm, InputManager ih) : base(sm, ih)
    {
        shapes = new Shapes(RubedoEngine.Instance);
        _name = "DemoState";
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Enter()
    {
        base.Enter();

        RubedoEngine.SizeOfMeter = 1;
        RubedoEngine.Instance.World.ResetGravity();
        RubedoEngine.Instance.Camera.SetZoom(24);

        demos[selectedDemo].Initialize(this);
    }

    public override void HandleInput()
    {
        base.HandleInput();
        demos[selectedDemo].HandleInput(this);

        if (inputManager.KeyPressed(Keys.Z))
            RubedoEngine.Instance.physicsOn = !RubedoEngine.Instance.physicsOn;
        if (inputManager.KeyPressed(Keys.X))
            RubedoEngine.Instance.stepPhysics = true;
        if (inputManager.KeyPressed(Keys.S))
            showVelocity = !showVelocity;
        if (inputManager.KeyPressed(Keys.C))
            colorVelocity = !colorVelocity;
        if (inputManager.KeyPressed(Keys.A))
            showAABB = !showAABB;
        if (inputManager.KeyPressed(Keys.O))
            PhysicsWorld.showContacts = !PhysicsWorld.showContacts;
        if (inputManager.KeyPressed(Keys.B))
            PhysicsWorld.bruteForce = !PhysicsWorld.bruteForce;
        if (inputManager.KeyPressed(Keys.D))
            PhysicsWorld.drawBroadphase = !PhysicsWorld.drawBroadphase;
        if (inputManager.KeyPressed(Keys.F))
            fastPlace = !fastPlace;

        if (inputManager.KeyPressed(Keys.Left))
        {
            if (selectedDemo == 0)
                selectedDemo = demos.Length - 1;
            else
                selectedDemo--;
            RubedoEngine.Instance.World.Clear();
            foreach (Entity ent in Entities)
                Entities.Remove(ent);
            demos[selectedDemo].Initialize(this);
        }
        if (inputManager.KeyPressed(Keys.Right))
        {
            selectedDemo = (selectedDemo + 1) % demos.Length;
            RubedoEngine.Instance.World.Clear();
            foreach (Entity ent in Entities)
                Entities.Remove(ent);
            demos[selectedDemo].Initialize(this);
        }
    }

    public PhysicsBody MakeBody(Entity entity, PhysicsMaterial material, Collider collider, bool isStatic)
    {
        PhysicsBody body = new PhysicsBody(collider, material, true, true);
        if (isStatic)
            body.SetStatic();
        entity.Add(body);
        entity.Add(collider);
        RubedoEngine.Instance.World.AddBody(body);
        this.Add(entity);
        return body;
    }

    public override void Update()
    {
        base.Update();
        demos[selectedDemo].Update(this);
        DeleteIfTooFar();
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

    public override void Draw(Renderer sb)
    {
        shapes.Begin(RubedoEngine.Instance.Camera);

        for (int i = 0; i < RubedoEngine.Instance.World.bodies.Count; i++)
        {
            //RubedoEngine.Instance.World.GetBody(i, out PhysicsBody body);
            PhysicsBody body = RubedoEngine.Instance.World.bodies[i];
            Color speedColor = Color.Black;
            if (colorVelocity)
            {
                if (body.isStatic)
                    speedColor = new Color(50, 50, 50);
                else
                {
                    float val = body.LinearVelocity.Length() * 20f;
                    float vel = 220 - System.MathF.Min(val, 220) % 360;
                    MathColor.HsvToRgb(vel, 1, 1, out int r, out int g, out int b);
                    speedColor = new Color(r, g, b);
                }
            }
            if (showAABB)
            {
                AABB bounds = body.bounds;
                shapes.DrawBox(bounds.min, bounds.max, Color.Green);
            }

            switch (body.collider.shape.type)
            {
                case ShapeType.Circle:
                    Circle shape = (Circle)body.collider.shape;
                    Vector2 vA = shape.transform.Position;
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
                    shapes.DrawCapsuleFill(capsule.transform, capsule.transRadius, capsule.transStart, capsule.transEnd, 20, speedColor);
                    shapes.DrawCapsule(capsule.transform, capsule.transRadius, capsule.transStart, capsule.transEnd, 20, Color.White);
                    break;
                case ShapeType.Polygon:
                    Polygon polygon = (Polygon)body.collider.shape;
                    shapes.DrawPolygonFill(polygon.vertices, ShapeUtility.ComputeTriangles(polygon.VertexCount), polygon.transform, speedColor);
                    shapes.DrawPolygon(polygon.vertices, polygon.transform, Color.White);
                    break;
            }
            if (showVelocity)
                shapes.DrawLine(body.transform.Position, body.transform.Position + body.LinearVelocity, Color.Aquamarine);
        }

        //foreach (Entity ent in Entities)
        //    shapes.DrawBox(ent.transform, 0.25f, 0.25f, Color.Yellow);

        RubedoEngine.Instance.World.DebugDraw(shapes);
        shapes.End();

        //draw text
        DebugText debugText = DebugText.Instance;
        debugText.DrawTextStack($"Bodies: {RubedoEngine.Instance.World.bodies.Count} " +
            $"| Physics time: {RubedoEngine.Instance.physicsWatch.Elapsed.TotalMilliseconds.ToString("0.00")}");
        debugText.DrawTextStack($"Selected Demo: {demos[selectedDemo].description}");
        debugText.DrawTextStack($"Demo {selectedDemo + 1} of {demos.Length}");
        debugText.DrawTextStack($"(C)olor velocity: {(colorVelocity ? "Yes" : "No")}");
        debugText.DrawTextStack($"(S)how velocity: {(showVelocity ? "Yes" : "No")}");
        debugText.DrawTextStack($"(A)ABBs visible: {(showAABB ? "Yes" : "No")}");
        debugText.DrawTextStack($"C(o)ntacts visible: {(PhysicsWorld.showContacts ? "Yes" : "No")}");
        debugText.DrawTextStack($"(D)raw Broadphase: {(PhysicsWorld.drawBroadphase ? "Yes" : "No")}");
        debugText.DrawTextStack($"(B)rute force: {(PhysicsWorld.bruteForce ? "Yes" : "No")}");
        debugText.DrawTextStack($"(F)ast Place: {(fastPlace ? "On" : "Off")}");
        debugText.Draw(sb);
    }
}