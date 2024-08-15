namespace PrimeFPSGame.Scripts;

using Godot;

public partial class WalkingPlayerState : PlayerMovementState {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float TopAnimationSpeed = 2.2f;
  [Export] public float SpeedDefault = 5.0f;
  [Export] public float Acceleration = 0.1f;
  [Export] public float Deceleration = 0.25f;

  public override void Enter() {
    AnimationPlayer?.Play("Walking", -1.0f, 1.0f);
  }

  public override void Update(float delta) {
    PlayerFpsController.UpdateGravity(delta);
    PlayerFpsController.UpdateInput(SpeedDefault, Acceleration, Deceleration);
    PlayerFpsController.UpdateVelocity();
    SetAnimationSpeed(Global.PlayerFpsController.Velocity.Length());
    if (PlayerFpsController?.Velocity.Length() == 0.0f) {
      EmitSignal(State.SignalName.Transition, "IdlePlayerState");
    }
    if (Input.IsActionPressed("sprint") && Global.PlayerFpsController.IsOnFloor()) {
      EmitSignal(State.SignalName.Transition, "SprintingPlayerState");
    }
  }

  private void SetAnimationSpeed(float speed) {
    var alpha = Mathf.Remap(speed, 0.0, SpeedDefault, 0.0, 1.0);
    if (AnimationPlayer != null) {
      AnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TopAnimationSpeed, (float)alpha);
    }
  }

}
