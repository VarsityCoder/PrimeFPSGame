namespace PrimeFPSGame.Scripts;

using Godot;

public partial class WalkingPlayerState : PlayerMovementState {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float TopAnimationSpeed = 2.2f;
  [Export] public float SpeedDefault = 5.0f;
  [Export] public float Acceleration = 0.1f;
  [Export] public float Deceleration = 0.25f;

  public override async void Enter(State currentState) {
    if (AnimationPlayer != null)
    {
      if (AnimationPlayer.IsPlaying() && AnimationPlayer.CurrentAnimation == "JumpEnd")
      {
        await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        AnimationPlayer.Play("Walking");
      }
      else
      {
        AnimationPlayer.Play("Walking");
      }

    }

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
      SetAnimationSpeed(Global.PlayerFpsController.Velocity.Length());
      if (PlayerFpsController?.Velocity.Length() == 0.0f)
      {
        EmitSignal(State.SignalName.Transition, "IdlePlayerState");
      }
      if (Input.IsActionJustPressed("jump") && Global.PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "JumpingPlayerState");
      }

      if (Input.IsActionPressed("sprint") && Global.PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "SprintingPlayerState");
      }
      if (Input.IsActionJustPressed("crouch") && Global.PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "CrouchingPlayerState");
      }
 
      if (PlayerFpsController.Velocity.Y < -3f && !PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "FallingPlayerState");
      }
    }


  }

  private void SetAnimationSpeed(float speed) {
    var alpha = Mathf.Remap(speed, 0.0, SpeedDefault, 0.0, 1.0);
    if (AnimationPlayer != null) {
      AnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TopAnimationSpeed, (float)alpha);
    }
  }
}
