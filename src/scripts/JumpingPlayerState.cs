using Godot;
namespace PrimeFPSGame.Scripts;

public partial class JumpingPlayerState : PlayerMovementState
{
    [Export] public AnimationPlayer? AnimationPlayer;
    [Export] public float InputMultiplier = 0.85f;
    [Export] public float SpeedDefault = 6.0f;
    [Export] public float Acceleration = 0.1f;
    [Export] public float Deceleration = 0.25f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float DoubleJumpVelocity = 4.5f;

    private bool _isDoubleJump;

    public override void Enter(State currentState)
    {
        if (AnimationPlayer != null && PlayerFpsController != null)
        {
            var tempPlayerVelocity = PlayerFpsController.Velocity;
            tempPlayerVelocity.Y += JumpVelocity;
            PlayerFpsController.Velocity = tempPlayerVelocity;
            AnimationPlayer.Play("JumpStart");
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
            PlayerFpsController.UpdateInput(SpeedDefault * InputMultiplier, Acceleration, Deceleration);
            PlayerFpsController.UpdateVelocity();
            PlayerFpsController.UpdateGravity(delta);

            if (Input.IsActionJustPressed("jump") && _isDoubleJump == false)
            {
                _isDoubleJump = true;
                var tempPlayerVelocity = PlayerFpsController.Velocity;
                tempPlayerVelocity.Y = DoubleJumpVelocity;
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
                AnimationPlayer.Play("JumpEnd");
                await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
                EmitSignal(State.SignalName.Transition, "IdlePlayerState");
            }
            if (PlayerFpsController.Velocity.Y < -3f && !PlayerFpsController.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transition, "FallingPlayerState");
            }
        }
    }
}
