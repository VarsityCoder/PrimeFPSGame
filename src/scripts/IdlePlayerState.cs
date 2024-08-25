namespace PrimeFPSGame.Scripts;

using Godot;

public partial class IdlePlayerState : PlayerMovementState {

  [Export] private AnimationPlayer? _animationPlayer;
  [Export] private float _speedDefault = 5.0f;
  [Export] private float _acceleration = 0.1f;
  [Export] private float _deceleration = 0.25f;

  public override async void Enter(State currentState)
  {
    if (_animationPlayer != null)
    {
      if (_animationPlayer.IsPlaying() && _animationPlayer.CurrentAnimation == "JumpEnd")
      {
        await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _animationPlayer.Pause();
      }
      else
      {
        _animationPlayer.Pause();
      }
    }
  } 

  public override void Update(float delta) {
    if (PlayerFpsController != null)
    {
      PlayerFpsController.UpdateGravity(delta);
      PlayerFpsController.UpdateInput(_speedDefault, _acceleration, _deceleration);
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
