using Rubedo;
using Rubedo.Components;
using Rubedo.Object;
using Microsoft.Xna.Framework;
using Rubedo.Tests;
using Rubedo.Physics2D;
using Rubedo.Physics2D.ColliderShape;
using Microsoft.Xna.Framework.Input;
using Rubedo.Lib;

namespace Learninging.Gameplay;

/// <summary>
/// I am TestState, and this is my summary.
/// </summary>
public class TestState : GameState
{
    TestBall ball;
    TestPhysicsWorld shapes;

    public TestState(StateManager sm, InputManager ih) : base(sm, ih) 
    {
        _name = "ball";
    }

    public override void LoadContent()
    {
        base.LoadContent();
#if !BALL
        Entity ballEntity = new Entity();
        Sprite sprite = new Sprite("ball", true, true);
        Collider collider = Collider.CreateCircle(ballEntity.transform, Collider.UNIT_CIRCLE_RADIUS);
        PhysicsBody body = new PhysicsBody(collider, new Transform(), 1, 1, false);
        ball = new TestBall(sprite, body, true, true);
        ballEntity.Add(ball);
        ballEntity.Add(body);
        ballEntity.Add(collider);
        ballEntity.Add(sprite);
        Add(ballEntity);
#endif

        shapes = new TestPhysicsWorld(this);
        shapes.colors.Add(Color.Black);
        shapes.outlineColors.Add(Color.Blue);
        Add(shapes);
        RubedoEngine.Instance.World.AddBody(new PhysicsObject(body, collider));

        /*
        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Entity entity = new Entity(new Vector2(x * 30 - 250, y * 30 - 100));
                Collider comp = Collider.CreateUnitShape(entity.transform, ShapeType.Box);
                shapes.MakeBody(this, entity, comp, false);
            }
        }*/
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if (inputManager.MousePressed(InputManager.MouseButtons.Mouse1))
        {
            Entity entity = new Entity(inputManager.MouseWorldPosition());
            Collider comp = Collider.CreateUnitShape(entity.transform, ShapeType.Box);
            shapes.MakeBody(this, entity, comp, false);
        }
        if (inputManager.MousePressed(InputManager.MouseButtons.Mouse2))
        {
            Entity entity = new Entity(inputManager.MouseWorldPosition());
            Collider comp = Collider.CreateUnitShape(entity.transform, ShapeType.Circle);
            shapes.MakeBody(this, entity, comp, false);
        }

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
            RubedoEngine.Instance.Camera.Move(new Vector2(-10, 0));
        if (inputManager.KeyDown(Keys.L))
            RubedoEngine.Instance.Camera.Move(new Vector2(10, 0));
        if (inputManager.KeyDown(Keys.K))
            RubedoEngine.Instance.Camera.Move(new Vector2(0, -10));
        if (inputManager.KeyDown(Keys.I))
            RubedoEngine.Instance.Camera.Move(new Vector2(0, 10));

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