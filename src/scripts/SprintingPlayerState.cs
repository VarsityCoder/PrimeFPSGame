using Godot;

namespace PrimeFPSGame.Scripts;

public partial class SprintingPlayerState : PlayerMovementState {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float TopAnimationSpeed = 1.6f;
  [Export] public float SpeedDefault = 100.0f;
  [Export] public float Acceleration = 0.1f;
  [Export] public float Deceleration = 0.25f;

  public override void Enter(State currentState) {
    AnimationPlayer?.Play("Sprinting", 0.5f);
  }
  
  public override void Exit()
  {
    if (AnimationPlayer != null)
    {
      AnimationPlayer.SpeedScale = 1.0f;
    }
  }

  public override void Update(float delta)
  {
    if (PlayerFpsController != null)
    {
      PlayerFpsController.UpdateGravity(delta);
      PlayerFpsController.UpdateInput(SpeedDefault, Acceleration, Deceleration);
      PlayerFpsController.UpdateVelocity();
      SetAnimationSpeed(PlayerFpsController.Velocity.Length());

      if (Input.IsActionJustReleased("sprint") || PlayerFpsController.Velocity.Length() == 0f)
      {
        EmitSignal(State.SignalName.Transition, "WalkingPlayerState");
      }

      if (Input.IsActionJustReleased("crouch") && PlayerFpsController.Velocity.Length() > 6f)
      {
        EmitSignal(State.SignalName.Transition, "SlidingPlayerState");
      }
      if (Input.IsActionJustPressed("jump") && Global.PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "JumpingPlayerState");
      }
    }
  }

  private void SetAnimationSpeed(float speed) {
    var alpha = Mathf.Remap(speed, 0.0, SpeedDefault, 0.0, 1.0);
    if (AnimationPlayer != null) {
      AnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TopAnimationSpeed, (float)alpha);
    }
  }
  public override void _Input(InputEvent @event) {

  }
}
