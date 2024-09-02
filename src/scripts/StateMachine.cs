using Godot;

namespace PrimeFPSGame.Scripts;

using System.Collections.Generic;

public partial class StateMachine : Node {
  [Export] private State _currentState = new State();
  private readonly Dictionary<string, State> _statesDictionary = new Dictionary<string, State>();

  public override void _Ready() {
    foreach (var node in GetChildren() ) {
      var child = (State)node;
      if (child != null) {
        _statesDictionary[child.Name] = child;
        child.Transition += OnChildTransition;
      }
      else {
        GD.PushWarning("State machine contains incompatible child node");
      }
    }
    //TODO Deal with null literal
    Owner.Ready += () => _currentState.Enter(null!);
  }

  public override void _Process(double delta) {
    _currentState.Update((float)delta);
    if (Global.DebugPanelGlobal != null)
    {
      Global.DebugPanelGlobal.AddProperty("Current State", _currentState.Name, 1);
    }
  }

  public override void _PhysicsProcess(double delta) => _currentState.PhysicsUpdate((float)delta);

  private void OnChildTransition(string newStateName) {
    var newState = _statesDictionary[newStateName];
    if (newState != _currentState) {
        _currentState.Exit();
        newState.Enter(_currentState);
        _currentState = newState;
    }
    else {
      
    }
  }
  

}
