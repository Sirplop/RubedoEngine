namespace Test.Gameplay.Demo;

/// <summary>
/// I am DemoBase, and this is my summary.
/// </summary>
internal abstract class DemoBase
{
    public string description;
    public abstract void Initialize(DemoState state);
    public abstract void Update(DemoState state);
    public abstract void HandleInput(DemoState state);
}