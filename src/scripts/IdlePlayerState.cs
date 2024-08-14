namespace PrimeFPSGame.Scripts;

using Godot;

public partial class IdlePlayerState : PlayerMovementState {

  [Export] public AnimationPlayer? AnimationPlayer;

  public override void Enter() => AnimationPlayer?.Pause();

  public override void Update(float delta) {
    if (Global.PlayerFpsController.Velocity.Length() > 0.0f && Global.PlayerFpsController.IsOnFloor()) {
      EmitSignal(State.SignalName.Transition, "WalkingPlayerState");
    }
  }
}
