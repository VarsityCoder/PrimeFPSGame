using Godot;

namespace PrimeFPSGame.Scripts;
public partial class State : Node {

  [Signal]
  public delegate void TransitionEventHandler(string newStateName);

  public virtual void Enter(State currentState) {

  }

  public virtual void Exit() {

  }

  public virtual void Update(float delta) {

  }

  public void PhysicsUpdate(float delta) {

  }


}
