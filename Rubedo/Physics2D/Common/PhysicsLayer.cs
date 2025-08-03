namespace Rubedo.Physics2D.Common;

/// <summary>
/// Provides a way to limit physics interaction on a layer to layer basis. You can have up to 32 layers (0 - 31).
/// </summary>
public static class PhysicsLayer
{
    private static int[] bitMasks = new int[32];

    static PhysicsLayer()
    {
        for (int i = 0; i < 32; i++)
        {
            bitMasks[i] = 1 << i;
        }
    }

    public static void SetCollisionWithLayer(byte layer1, byte layer2, bool collides)
    {
        if (layer1 > 31 || layer2 > 31)
            throw new System.ArgumentOutOfRangeException("Physics layers can't be more than 31!");

        if (collides)
        {
            bitMasks[layer1] |= 1 << layer2;
            bitMasks[layer2] |= 1 << layer1;
        } else
        {
            bitMasks[layer1] &= ~(1 << layer2);
            bitMasks[layer2] &= ~(1 << layer1);
        }
    }

    public static bool LayersCollide(byte layer1, byte layer2)
    {
        if (layer1 > 31 || layer2 > 31)
            throw new System.ArgumentOutOfRangeException("layer", "Physics layers can't be more than 31!");

        int mask = bitMasks[layer1];
        return (mask & (1 << layer2)) != 0;
    }
}