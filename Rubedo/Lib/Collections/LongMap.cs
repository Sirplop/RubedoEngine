using System.Runtime.CompilerServices;

namespace Rubedo.Lib.Collections;

/// <summary>
/// A collection dedicated to mapping a long key to an object. Significantly faster than a Dictionary, but limited to long keys.
/// </summary>
public class LongMap<T> where T : class
{
    /// <summary>
    /// The backing key array. Note that this includes empty keys.
    /// </summary>
    public ref long[] Keys => ref keys;
    /// <summary>
    /// The backing values array. Note that this includes empty slots.
    /// </summary>
    public ref T[] Values => ref values;

    int count;
    int capacity;
    int threshold;
    long[] keys;
    T[] values;
    byte[] states; // 0 = empty, 1 = occupied, 2 = tombstone
    const float LOAD_FACTOR = 0.8f;
    const ulong GOLDEN = 11400714819323198485UL; //floor(2^64 / phi), where phi is the golden ratio

    public LongMap(int initialCapacity = 16)
    {
        if (initialCapacity < 8) initialCapacity = 8;
        capacity = Math.Power2Roundup(initialCapacity);
        keys = new long[capacity];
        values = new T[capacity];
        states = new byte[capacity];
        threshold = (int)(capacity * LOAD_FACTOR);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int IndexFor(long key, int mask) => (int)((unchecked((ulong)key) * GOLDEN) & (uint)mask);

    void Resize()
    {
        int newCap = capacity << 1;
        long[] oldKeys = keys;
        T[] oldValues = values;
        byte[] oldStates = states;

        keys = new long[newCap];
        values = new T[newCap];
        states = new byte[newCap];

        int oldCap = capacity;
        capacity = newCap;
        threshold = (int)(capacity * LOAD_FACTOR);
        count = 0;

        int mask = capacity - 1;
        for (int i = 0; i < oldCap; i++)
        {
            if (oldStates[i] == 1)
            {
                long k = oldKeys[i];
                T v = oldValues[i];
                // reinsert
                int idx = (int)((unchecked((ulong)k) * GOLDEN) & (uint)mask);
                while (states[idx] == 1)
                {
                    idx = (idx + 1) & mask;
                }
                keys[idx] = k;
                values[idx] = v;
                states[idx] = 1;
                count++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(long key, out T value)
    {
        int mask = capacity - 1;
        int idx = (int)((unchecked((ulong)key) * GOLDEN) & (uint)mask);

        // Bound the number of probes to capacity to avoid infinite loop when table
        // contains no empty slot (e.g. lots of deletions or near-full).
        for (int probes = 0; probes < capacity; probes++)
        {
            byte s = states[idx];
            if (s == 0) // empty slot -> not present
            {
                value = null;
                return false;
            }
            if (s == 1 && keys[idx] == key)
            {
                value = values[idx];
                return true;
            }
            idx = (idx + 1) & mask;
        }

        // If we've probed 'capacity' times and didn't find an empty slot or the key,
        // the key is not present (prevents infinite looping).
        value = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(long key, T value)
    {
        if (count >= threshold) Resize();

        int mask = capacity - 1;
        int idx = (int)((unchecked((ulong)key) * GOLDEN) & (uint)mask);
        int firstTombstone = -1;

        // Bound probes to capacity to guarantee termination even if there is no empty slot.
        for (int probes = 0; probes < capacity; probes++)
        {
            byte s = states[idx];
            if (s == 0)
            {
                if (firstTombstone != -1) idx = firstTombstone;
                keys[idx] = key;
                values[idx] = value;
                states[idx] = 1;
                count++;
                return;
            }
            if (s == 2)
            {
                if (firstTombstone == -1) firstTombstone = idx;
            }
            else if (s == 1 && keys[idx] == key)
            {
                // overwrite existing
                values[idx] = value;
                return;
            }
            idx = (idx + 1) & mask;
        }

        // If we probed the whole table and only found tombstones, use the first tombstone.
        if (firstTombstone != -1)
        {
            idx = firstTombstone;
            keys[idx] = key;
            values[idx] = value;
            states[idx] = 1;
            count++;
            return;
        }

        // Table truly full (no empty slot and no tombstones) -> resize and insert.
        Resize();
        Add(key, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(long key)
    {
        int mask = capacity - 1;
        int idx = (int)((unchecked((ulong)key) * GOLDEN) & (uint)mask);

        while (true)
        {
            byte s = states[idx];
            if (s == 0) return false;
            if (s == 1 && keys[idx] == key)
            {
                states[idx] = 2; // tombstone
                values[idx] = null;
                count--;
                return true;
            }
            idx = (idx + 1) & mask;
        }
    }

    public void Clear()
    {
        if (count == 0) return;
        for (int i = 0; i < capacity; i++)
        {
            states[i] = 0;
            values[i] = null;
        }
        count = 0;
    }
}
