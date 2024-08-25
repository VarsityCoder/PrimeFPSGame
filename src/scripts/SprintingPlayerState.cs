using Godot;

namespace PrimeFPSGame.Scripts;

public partial class SprintingPlayerState : PlayerMovementState {

  [Export] private AnimationPlayer? _animationPlayer;
  [Export] private float _topAnimationSpeed = 1.6f;
  [Export] private float _speedDefault = 100.0f;
  [Export] private float _acceleration = 0.1f;
  [Export] private float _deceleration = 0.25f;

  public override async void Enter(State currentState) {
    if (_animationPlayer != null)
    {
      if (_animationPlayer.IsPlaying() && _animationPlayer.CurrentAnimation == "JumpEnd")
      {
        await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _animationPlayer.Play("Sprinting");
      }
      else
      {
        _animationPlayer.Play("Sprinting");
      }
    }
  }
  
  public override void Exit()
  {
    if (_animationPlayer != null)
    {
      _animationPlayer.SpeedScale = 1.0f;
    }
  }

  public override void Update(float delta)
  {
    if (PlayerFpsController != null)
    {
      PlayerFpsController.UpdateGravity(delta);
      PlayerFpsController.UpdateInput(_speedDefault, _acceleration, _deceleration);
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
      if (PlayerFpsController.Velocity.Y < -3f && !PlayerFpsController.IsOnFloor())
      {
        EmitSignal(State.SignalName.Transition, "FallingPlayerState");
      }
    }
  }

  private void SetAnimationSpeed(float speed) {
    var alpha = Mathf.Remap(speed, 0.0, _speedDefault, 0.0, 1.0);
    if (_animationPlayer != null) {
      _animationPlayer.SpeedScale = Mathf.Lerp(0.0f, _topAnimationSpeed, (float)alpha);
    }
  }
  public override void _Input(InputEvent @event) {

  }
}
