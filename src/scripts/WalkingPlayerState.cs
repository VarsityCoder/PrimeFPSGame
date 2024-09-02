namespace PrimeFPSGame.Scripts;

using Godot;

public partial class WalkingPlayerState : PlayerMovementState {

  [Export] private AnimationPlayer? _animationPlayer;
  [Export] private float _topAnimationSpeed = 2.2f;
  [Export] private float _speedDefault = 5.0f;
  [Export] private float _acceleration = 0.1f;
  [Export] private float _deceleration = 0.25f;
  [Export] private float _bobSpeed = 5f;
  [Export] private float _horizontalBobAmount = 1f;
  [Export] private float _verticalBobAmount = 0.5f;

  public override async void Enter(State currentState) {
    if (_animationPlayer != null)
    {
      if (_animationPlayer.IsPlaying() && _animationPlayer.CurrentAnimation == "JumpEnd")
      {
        await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _animationPlayer.Play("Walking");
      }
      else
      {
        _animationPlayer.Play("Walking");
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
      PlayerFpsController.WeaponController?.SwayWeapon(delta, false);
      PlayerFpsController.WeaponController?.WeaponBob(delta,_bobSpeed, _horizontalBobAmount,_verticalBobAmount);
      if (Global.PlayerFpsController != null)
      {
        SetAnimationSpeed(Global.PlayerFpsController.Velocity.Length());
        if (Input.IsActionJustPressed("jump") && Global.PlayerFpsController.IsOnFloor())
        {
          EmitSignal(State.SignalName.Transition, "JumpingPlayerState");
        }
        if (PlayerFpsController?.Velocity.Length() == 0.0f)
        {
          EmitSignal(State.SignalName.Transition, "IdlePlayerState");
        }
        if (Input.IsActionPressed("sprint") && Global.PlayerFpsController.IsOnFloor())
        {
          EmitSignal(State.SignalName.Transition, "SprintingPlayerState");
        }
        if (Input.IsActionJustPressed("crouch") && Global.PlayerFpsController.IsOnFloor())
        {
          EmitSignal(State.SignalName.Transition, "CrouchingPlayerState");
        }

      }

      if (PlayerFpsController is { Velocity.Y: < -3f } && !PlayerFpsController.IsOnFloor())
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
}
