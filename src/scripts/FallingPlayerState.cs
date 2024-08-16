using Godot;
namespace PrimeFPSGame.Scripts;

public partial class FallingPlayerState : PlayerMovementState
{
    [Export] public AnimationPlayer? AnimationPlayer;
    [Export] public float SpeedDefault = 5f;
    [Export] public float Acceleration = 0.1f;
    [Export] public float Deceleration = 0.25f;
    [Export] public float DoubleJumpVelocity = 4.5f;

    private bool _isDoubleJump;

    public override void Enter(State currentState)
    {
        if (AnimationPlayer != null)
        {
            AnimationPlayer.Pause();
        }
    }

    public override void Exit()
    {
        _isDoubleJump = false;
    }

    public override async void Update(float delta)
    {
        if (PlayerFpsController != null && AnimationPlayer != null)
        {
            PlayerFpsController.UpdateInput(SpeedDefault, Acceleration, Deceleration);
            PlayerFpsController.UpdateVelocity();
            PlayerFpsController.UpdateGravity(delta);
            
            if (Input.IsActionJustPressed("jump") && _isDoubleJump == false)
            {
                _isDoubleJump = true;
                var tempPlayerVelocity = PlayerFpsController.Velocity;
                tempPlayerVelocity.Y = DoubleJumpVelocity;
                PlayerFpsController.Velocity = tempPlayerVelocity;
            }
            
            if (PlayerFpsController.IsOnFloor())
            {
                AnimationPlayer.Play("JumpEnd");
                await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
                EmitSignal(State.SignalName.Transition, "IdlePlayerState");
            }
        }
    }
}
