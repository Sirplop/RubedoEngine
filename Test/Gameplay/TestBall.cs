using Microsoft.Xna.Framework;
using Rubedo;
using Rubedo.Components;
using Rubedo.Physics2D.Dynamics;
using System;

namespace Learninging.Gameplay;

/// <summary>
/// I am TestBall, and this is my summary.
/// </summary>
public class TestBall : Component
{
    private Sprite sprite;
    private PhysicsBody physicsBody;

    private float forceMagnitude = 500f;

    public TestBall(Sprite sprite, PhysicsBody physicsBody, bool active, bool visible) : base(active, visible) 
    {
        this.sprite = sprite;
        sprite.visible = false;
        this.physicsBody = physicsBody;
    }

    public void MoveLeft()
    {
        physicsBody.AddForce(new Vector2(-forceMagnitude, 0));
    }
    public void MoveRight()
    {
        physicsBody.AddForce(new Vector2(forceMagnitude, 0));
    }
    public void MoveUp()
    {
        physicsBody.AddForce(new Vector2(0, forceMagnitude));
    }
    public void MoveDown()
    {
        physicsBody.AddForce(new Vector2(0, -forceMagnitude));
    }
}