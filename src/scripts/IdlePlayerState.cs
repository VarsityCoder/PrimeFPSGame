namespace PrimeFPSGame.Scripts;

using Godot;

public partial class IdlePlayerState : PlayerMovementState {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float SpeedDefault = 5.0f;
  [Export] public float Acceleration = 0.1f;
  [Export] public float Deceleration = 0.25f;

  public override void Enter(State currentState) => AnimationPlayer?.Pause();

  public override void Update(float delta) {
    if (PlayerFpsController != null)
    {
      PlayerFpsController.UpdateGravity(delta);
      PlayerFpsController.UpdateInput(SpeedDefault, Acceleration, Deceleration);
      PlayerFpsController.UpdateVelocity();
      if (Global.PlayerFpsController.Velocity.Length() > 0.0f && Global.PlayerFpsController.IsOnFloor()) {
        EmitSignal(State.SignalName.Transition, "WalkingPlayerState");
      }
      if (Input.IsActionJustPressed("crouch") && Global.PlayerFpsController.IsOnFloor()) {
        EmitSignal(State.SignalName.Transition, "CrouchingPlayerState");
      }
    }
  }
}
