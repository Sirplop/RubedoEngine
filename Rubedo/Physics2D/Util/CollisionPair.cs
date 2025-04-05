using System;
using Rubedo.Physics2D.Dynamics;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// I am CollisionPair, and this is my summary.
/// </summary>
public class CollisionPair : IEquatable<CollisionPair>
{
    public readonly PhysicsBody A;
    public readonly PhysicsBody B;

    public CollisionPair(PhysicsBody A, PhysicsBody B)
    {
        this.A = A;
        this.B = B;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as CollisionPair);
    }

    public bool Equals(CollisionPair other)
    {
        if (other == null)
            return false;

        return (this.A.Equals(other.A) && this.B.Equals(other.B)) ||
               (this.A.Equals(other.B) && this.B.Equals(other.A));
    }

    public override int GetHashCode()
    {
        int hashA = A.GetHashCode();
        int hashB = B.GetHashCode();
        return hashA ^ hashB;
    }

    public static bool operator ==(CollisionPair left, CollisionPair right)
    {
        return left is not null && left.Equals(right);
    }

    public static bool operator !=(CollisionPair left, CollisionPair right)
    {
        return !(left == right);
    }

}