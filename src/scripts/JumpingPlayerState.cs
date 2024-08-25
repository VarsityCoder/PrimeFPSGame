using Godot;
namespace PrimeFPSGame.Scripts;

public partial class JumpingPlayerState : PlayerMovementState
{
    [Export] private AnimationPlayer? _animationPlayer;
    [Export] private float _inputMultiplier = 0.85f;
    [Export] private float _speedDefault = 6.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _jumpVelocity = 4.5f;
    [Export] private float _doubleJumpVelocity = 4.5f;

    private bool _isDoubleJump;

    public override void Enter(State currentState)
    {
        if (_animationPlayer != null && PlayerFpsController != null)
        {
            var tempPlayerVelocity = PlayerFpsController.Velocity;
            tempPlayerVelocity.Y += _jumpVelocity;
            PlayerFpsController.Velocity = tempPlayerVelocity;
            _animationPlayer.Play("JumpStart");
        }
    }

    public override void Exit()
    {
        _isDoubleJump = false;
    }

    public override async void Update(float delta)
    {
        if (PlayerFpsController != null && _animationPlayer != null)
        {
            PlayerFpsController.UpdateInput(_speedDefault * _inputMultiplier, _acceleration, _deceleration);
            PlayerFpsController.UpdateVelocity();
            PlayerFpsController.UpdateGravity(delta);

            if (Input.IsActionJustPressed("jump") && _isDoubleJump == false)
            {
                _isDoubleJump = true;
                var tempPlayerVelocity = PlayerFpsController.Velocity;
                tempPlayerVelocity.Y = _doubleJumpVelocity;
                PlayerFpsController.Velocity = tempPlayerVelocity;
            }

            if (Input.IsActionJustReleased("jump"))
            {
                if (PlayerFpsController.Velocity.Y > 0f)
                {
                    var tempPlayerVelocity = PlayerFpsController.Velocity;
                    tempPlayerVelocity.Y = PlayerFpsController.Velocity.Y / 2f;
                    PlayerFpsController.Velocity = tempPlayerVelocity;
                }
            }
            if (PlayerFpsController.IsOnFloor())
            {
                _animationPlayer.Play("JumpEnd");
                await ToSignal(_animationPlayer, AnimationMixer.SignalName.AnimationFinished);
                EmitSignal(State.SignalName.Transition, "IdlePlayerState");
            }
            if (PlayerFpsController.Velocity.Y < -3f && !PlayerFpsController.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transition, "FallingPlayerState");
            }
        }
    }
}
