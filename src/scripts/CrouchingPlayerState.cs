using Godot;
namespace PrimeFPSGame.Scripts;

public partial class CrouchingPlayerState : PlayerMovementState
{
    [Export] public AnimationPlayer? AnimationPlayer;
    [Export] public float TopAnimationSpeed = 2.2f;
    [Export] public float SpeedDefault = 5.0f;
    [Export] public float Acceleration = 0.1f;
    [Export] public float Deceleration = 0.25f;
    [Export] public float CrouchSpeed = 4.0f;
    [Export] public ShapeCast3D? CrouchShapeCast;

    private bool _released;

    public override void Enter(State previousState)
    {
        if (AnimationPlayer != null)
        {
            AnimationPlayer.SpeedScale = 1.0f;
            if (previousState.Name != "SlidingPlayerState")
            {
                AnimationPlayer.Play("Crouch", -1.0, CrouchSpeed);
            } else if (previousState.Name == "SlidingPartyState")
            {
                AnimationPlayer.CurrentAnimation = "Crouch";
                AnimationPlayer.Seek(1.0f, true);
            }
        }
    }

    public override void Exit()
    {
        _released = false;
    }

    public override void Update(float delta)
    {
        if (PlayerFpsController != null)
        {
            PlayerFpsController.UpdateGravity(delta);
            PlayerFpsController.UpdateInput(SpeedDefault, Acceleration, Deceleration);
            PlayerFpsController.UpdateVelocity();
            if (Input.IsActionJustReleased("crouch"))
            {
                Uncrouch();
            } else if (Input.IsActionPressed("crouch") == false && _released == false)
            {
                _released = true;
                Uncrouch();
            }
        }
    }

    private async void Uncrouch()
    {
        if (CrouchShapeCast?.IsColliding() == false && Input.IsActionPressed("crouch") == false)
        {
            AnimationPlayer?.Play("Crouch", -1.0, -CrouchSpeed * 1.5f, true);
            if (AnimationPlayer != null && AnimationPlayer.IsPlaying())
            {
                await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
            }  
            else if (CrouchShapeCast.IsColliding())
            {
                GetTree().CreateTimer(0.1).Timeout += Uncrouch;
            }
            EmitSignal(State.SignalName.Transition, "IdlePlayerState");
        }
    }
}
