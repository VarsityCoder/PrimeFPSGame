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

    public override void Enter(State currentState)
    {
        if (AnimationPlayer != null && PlayerFpsController != null)
        {
            var tempPlayerVelocity = PlayerFpsController.Velocity;
            tempPlayerVelocity.Y += JumpVelocity;
            PlayerFpsController.Velocity = tempPlayerVelocity;
            AnimationPlayer.Pause();
        }
    }

    public override void Update(float delta)
    {
        if (PlayerFpsController != null)
        {
            PlayerFpsController.UpdateInput(SpeedDefault * InputMultiplier, Acceleration, Deceleration);
            PlayerFpsController.UpdateVelocity();
            if (PlayerFpsController.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transition, "IdlePlayerState");
            }
        }
    }
}
