using Godot;
namespace PrimeFPSGame.Scripts;

public partial class FallingPlayerState : PlayerMovementState
{
    [Export] private AnimationPlayer? _animationPlayer;
    [Export] private float _speedDefault = 5f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _doubleJumpVelocity = 4.5f;

    private bool _isDoubleJump;

    public override void Enter(State currentState)
    {
        if (_animationPlayer != null)
        {
            _animationPlayer.Pause();
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
            PlayerFpsController.UpdateInput(_speedDefault, _acceleration, _deceleration);
            PlayerFpsController.UpdateVelocity();
            PlayerFpsController.UpdateGravity(delta);
            
            if (Input.IsActionJustPressed("jump") && _isDoubleJump == false)
            {
                _isDoubleJump = true;
                var tempPlayerVelocity = PlayerFpsController.Velocity;
                tempPlayerVelocity.Y = _doubleJumpVelocity;
                PlayerFpsController.Velocity = tempPlayerVelocity;
            }
            
            if (PlayerFpsController.IsOnFloor())
            {
                _animationPlayer.Play("JumpEnd");
                await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
                EmitSignal(State.SignalName.Transition, "IdlePlayerState");
            }
        }
    }
}
