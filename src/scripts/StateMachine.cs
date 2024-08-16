using Godot;

namespace PrimeFPSGame.Scripts;

using System.Collections.Generic;

public partial class StateMachine : Node {
  [Export] public State CurrentState = new State();
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
    Owner.Ready += () => CurrentState.Enter(null!);
  }

  public override void _Process(double delta) {
    CurrentState.Update((float)delta);
    Global.DebugPanelGlobal.AddProperty("Current State", CurrentState.Name, 1);
  }

  public override void _PhysicsProcess(double delta) => CurrentState.PhysicsUpdate((float)delta);

  private void OnChildTransition(string newStateName) {
    var newState = _statesDictionary[newStateName];
    if (newState != CurrentState) {
        CurrentState.Exit();
        newState.Enter(CurrentState);
        CurrentState = newState;
    }
    else {
      
    }
  }
  

}
