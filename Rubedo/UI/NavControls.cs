using Microsoft.Xna.Framework.Input;
using Rubedo.Input;
using Rubedo.Input.Conditions;

namespace Rubedo.UI;

/// <summary>
/// The controls for navigation UI elements. Can be overriden.
/// </summary>
public static class NavControls
{
    public static ICondition MouseInteract { get; set; } = new MouseCondition(InputManager.MouseButtons.Left);
    public static ICondition ButtonInteract { get; set; } =
        new AnyCondition(
            new KeyCondition(Keys.Space),
            new KeyCondition(Keys.Enter)
        );
    public static ICondition NavLeft { get; set; } = new KeyCondition(Keys.Left);
    public static ICondition NavRight { get; set; } = new KeyCondition(Keys.Right);
    public static ICondition NavUp { get; set; } = new KeyCondition(Keys.Up);
    public static ICondition NavDown { get; set; } = new KeyCondition(Keys.Down);
    public static ICondition Back {  get; set; } = new KeyCondition(Keys.Escape);
}