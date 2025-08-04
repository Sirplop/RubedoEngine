using Rubedo.Graphics;
using System.Collections.Generic;

namespace Rubedo;

/// <summary>
/// Manages which <see cref="GameState"/> is currently loaded, and which ones can be loaded.
/// </summary>
public class StateManager
{
    protected Dictionary<string, GameState> _states;
    protected GameState _currentState;

    public string Current => _currentState?.Name ?? "";

    public StateManager()
    {
        _states = new Dictionary<string, GameState>();
    }

    public void AddState(GameState state)
    {
        _states.Add(state.Name, state);
    }

    public void SwitchState(string name)
    {
        if (_states.ContainsKey(name))
        {
            if (_currentState != null) 
                _currentState.Exit();

            _currentState = _states[name];
            _currentState.LoadContent();
            _currentState.Enter();
            //immediately update the lists to make sure everything spawns when the state is activated.
            _currentState.Entities.UpdateLists();
        }
    }

    public GameState CurrentState()
    {
        return _currentState;
    }

    public void Update()
    {
        if (_currentState != null)
            _currentState.Update();
    }
    public void FixedUpdate()
    {
        if (_currentState != null)
            _currentState.FixedUpdate();
    }

    public void Draw(Renderer sb)
    {
        if (_currentState != null)
            _currentState.Draw(sb);
    }
}