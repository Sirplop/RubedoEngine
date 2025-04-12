//#define GRID
//#define PILLAR
//#define PYRAMID
//#define DROP
//#define STAIRCASE
//#define RAMPS
#define PLATFORM
//#define SIDES
//#define BIG_PYRAMID

using Rubedo;
using Rubedo.Object;
using Microsoft.Xna.Framework;
using Rubedo.Tests;
using Microsoft.Xna.Framework.Input;
using Rubedo.Physics2D;
using PhysicsEngine2D;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Collision.Shapes;

namespace Learninging.Gameplay;

/// <summary>
/// I am TestState, and this is my summary.
/// </summary>
public class TestState : GameState
{
    TestPhysicsWorld shapes;

    public TestState(StateManager sm, InputManager ih) : base(sm, ih) 
    {
        _name = "ball";
    }

    public override void LoadContent()
    {
        base.LoadContent();
        shapes = new TestPhysicsWorld(this);
        Add(shapes);
        Entity entity;
        Collider comp;
        RubedoEngine.SizeOfMeter = 1;
        RubedoEngine.Instance.World.ResetGravity();
        RubedoEngine.Instance.Camera.SetZoom(24);
        float meter = RubedoEngine.SizeOfMeter;

        PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f);


#if STAIRCASE
        for (int x = 0, y = 0; x < 10 && y < 10; x++, y++)
        {
            entity = new Entity(new Vector2(50 - (30 * x), 180 - (30 * y)), 0);
            comp = Collider.CreateBox(60, 30);
            shapes.MakeBody(this, entity, material, comp, true);
        }
#endif
#if RAMPS
        entity = new Entity(new Vector2(-3.5f * meter, 0), -22.5f);
        comp = Collider.CreateBox(6.75f * meter, 0.4f * meter);
        shapes.MakeBody(this, entity, material, comp, true);

        entity = new Entity(new Vector2(3.5f * meter, 2.3f * meter), 22.5f);
        comp = Collider.CreateBox(6.75f * meter, 0.4f * meter);
        shapes.MakeBody(this, entity, material, comp, true);
#endif
#if PLATFORM
        //Polygon polygon = new Polygon(12 * meter, 1 * meter);
        //shapes.MakeBody(polygon, material, new Vector2(0, -5 * meter), 0f, true);
        
        entity = new Entity(new Vector2(0, -5 * meter));
        comp = Collider.CreateBox(24 * meter, 2 * meter);
        shapes.MakeBody(this, entity, material, comp, true);
#endif
#if SIDES
        entity = new Entity(new Vector2(-360, 0));
        comp = Collider.CreateBox(60, 720);
        shapes.MakeBody(this, entity, material, comp, true);
        entity = new Entity(new Vector2(360, 0));
        comp = Collider.CreateBox(60, 720);
        shapes.MakeBody(this, entity, material, comp, true);
#endif
#if GRID
        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                polygon = new Polygon(0.5f, 0.5f);
                shapes.MakeBody(polygon, material, new Vector2(x * (1.1f * meter) - (11 * meter), y * meter - (3.5f * meter)), 0, false);
                /*entity = new Entity(new Vector2(x * 35 - 330, y * 30 - 105));
                comp = Collider.CreateUnitShape(ShapeType.Box, 4);
                shapes.MakeBody(this, entity, material, comp, false);
                */
            }
        }
#endif
#if PILLAR
        for (int y = 0; y < 5; y++)
        {
            entity = new Entity(new Vector2(-5 * meter, y * meter - (3.5f * meter)));
            comp = Collider.CreateUnitShape(ShapeType.Box);
            shapes.MakeBody(this, entity, material, comp, false);
        }

        for (int y = 0; y < 5; y++)
        {
            entity = new Entity(new Vector2(5 * meter, y * meter - (3.5f * meter)));
            comp = Collider.CreateUnitShape(ShapeType.Box);
            shapes.MakeBody(this, entity, material, comp, false);
        }
#endif
#if PYRAMID
        float indent = 0;
        float left = 100;
        for (int y = 0; y < 11; y++)
        {
            for (int x = y; x < 11; x++)
            {
                entity = new Entity(new Vector2(x * 35 - indent - left, y * 30 - 105));
                comp = Collider.CreateUnitShape(ShapeType.Box);
                shapes.MakeBody(this, entity, material, comp, false);
            }
            indent += Collider.UNIT_BOX_SIDE * 0.5f;
        }
#endif
#if BIG_PYRAMID

        RubedoEngine.SizeOfMeter = 5f;
        //RubedoEngine.Instance.World.ResetGravity();
        RubedoEngine.Instance.Camera.SetZoom(4);

        /*
        entity = new Entity(new Vector2(0, -45));
        comp = Collider.CreateBox(200, 10);
        shapes.MakeBody(this, entity, material, comp, true);*/

        Polygon polygon = new Polygon(100, 5);
        shapes.MakeBody(polygon, material, new Vector2(0, -45), 0f, true);


        float width = 100;
        float height = width / RubedoEngine.Instance.GraphicsDevice.Viewport.AspectRatio;
        Vector2 pX = new Vector2(-width / 2 + 3, (-height / 2 + 1.5f) - 10f);

        const int N = 20;

        Polygon box = new Polygon(2.5f, 2.5f);
        for (int i = 0; i < N; ++i)
        {
            Vector2 y = pX;
            for (int j = i; j < N; ++j)
            {
                shapes.MakeBody(box.Clone(), material, y, 0f, false);
                /*entity = new Entity(y);
                comp = Collider.CreateUnitShape(ShapeType.Box);
                shapes.MakeBody(this, entity, material, comp, false);*/

                y += Vector2.UnitX * 1.125f * 5;//Collider.UNIT_BOX_SIDE;
            }

            // x += Vector2(0.5625f, 1.125f);
            pX += new Vector2(0.5625f, 1.0f) * 5f;//Collider.UNIT_BOX_SIDE;
        }
#endif
#if DROP
        for (int i = 0; i < 5; i++)
        {
            entity = new Entity(new Vector2((i * 50) - 150, i * 5));
            comp = Collider.CreateUnitShape(ShapeType.Circle);
            shapes.MakeBody(this, entity, material, comp, false);
        }
#endif
    }


    private bool shapeSet = true;
    public override void HandleInput()
    {
        base.HandleInput();
        if (inputManager.MousePressed(InputManager.MouseButtons.Mouse1))
        {
            PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f, 0, 0.5f);

            //Circle circle = new Circle(0.5f);
            //shapes.MakeBody(circle, material, inputManager.MouseWorldPosition(), 45, false);
            Entity entity = new Entity(inputManager.MouseWorldPosition());
            Collider comp = Collider.CreateUnitShape(shapeSet ? ShapeType.Circle : ShapeType.Capsule);
            shapes.MakeBody(this, entity, material, comp, false);
        }
        if (inputManager.MousePressed(InputManager.MouseButtons.Mouse2))
        {
            PhysicsMaterial material = new PhysicsMaterial(1, 0.5f, 0.5f);
            //Polygon polygon = new Polygon(0.5f, 0.5f);
            //shapes.MakeBody(polygon, material, inputManager.MouseWorldPosition(), 45, false);
            Entity entity = new Entity(inputManager.MouseWorldPosition());
            Collider comp = Collider.CreateUnitShape(shapeSet ? ShapeType.Box : ShapeType.Polygon, 3);
            shapes.MakeBody(this, entity, material, comp, false);
        }
        if (inputManager.MousePressed(InputManager.MouseButtons.Mouse3))
        {
            shapeSet = !shapeSet;
        }

        if (inputManager.KeyPressed(Keys.Z))
            RubedoEngine.Instance.physicsOn = !RubedoEngine.Instance.physicsOn;
        if (inputManager.KeyPressed(Keys.X))
            RubedoEngine.Instance.stepPhysics = true;

        /*if (inputManager.MouseDown(InputManager.MouseButtons.Mouse1))
        {
            material.Position = inputManager.MouseWorldPosition();
            entityCollider.shape.TransformUpdateRequired = true;
            entityCollider.shape.BoundsUpdateRequired = true;
        }
        if (inputManager.MouseDown(InputManager.MouseButtons.Mouse2))
        {
            material.Rotation += 60 * RubedoEngine.DeltaTime;
            entityCollider.shape.TransformUpdateRequired = true;
            entityCollider.shape.BoundsUpdateRequired = true;
        }*/

        /*
        if (inputManager.KeyDown(Keys.A))
            ball.MoveLeft();
        if (inputManager.KeyDown(Keys.D))
            ball.MoveRight();
        if (inputManager.KeyDown(Keys.W))
            ball.MoveUp();
        if (inputManager.KeyDown(Keys.S))
            ball.MoveDown();*/

        if (inputManager.KeyDown(Keys.J))
            RubedoEngine.Instance.Camera.Move(new Vector2(-0.3f * RubedoEngine.SizeOfMeter, 0));
        if (inputManager.KeyDown(Keys.L))
            RubedoEngine.Instance.Camera.Move(new Vector2(0.3f * RubedoEngine.SizeOfMeter, 0));
        if (inputManager.KeyDown(Keys.K))
            RubedoEngine.Instance.Camera.Move(new Vector2(0, -0.3f * RubedoEngine.SizeOfMeter));
        if (inputManager.KeyDown(Keys.I))
            RubedoEngine.Instance.Camera.Move(new Vector2(0, 0.3f * RubedoEngine.SizeOfMeter));

        if (inputManager.KeyPressed(Keys.E))
        {
            RubedoEngine.Instance.Camera.IncZoom();
        }
        if (inputManager.KeyPressed(Keys.Q))
        {
            RubedoEngine.Instance.Camera.DecZoom();
        }
    }
}