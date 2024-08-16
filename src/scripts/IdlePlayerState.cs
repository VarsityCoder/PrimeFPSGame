namespace PrimeFPSGame.Scripts;

using Godot;

public partial class IdlePlayerState : PlayerMovementState {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float SpeedDefault = 5.0f;
  [Export] public float Acceleration = 0.1f;
  [Export] public float Deceleration = 0.25f;

  public override async void Enter(State currentState)
  {
    if (AnimationPlayer != null)
    {
      if (AnimationPlayer.IsPlaying() && AnimationPlayer.CurrentAnimation == "JumpEnd")
      {
        await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        AnimationPlayer.Pause();
      }
      else
      {
        AnimationPlayer.Pause();
      }
    }
  } 

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

      if (Input.IsActionJustPressed("jump") && Global.PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "JumpingPlayerState");
      }

      if (PlayerFpsController.Velocity.Y < -3f && !PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "FallingPlayerState");
      }
    }
  }
}
