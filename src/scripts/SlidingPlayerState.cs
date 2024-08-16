using Godot;
namespace PrimeFPSGame.Scripts;

public partial class SlidingPlayerState : PlayerMovementState
{
    [Export] public AnimationPlayer? AnimationPlayer;
    [Export] public float SlideAnimationSpeed = 2.2f;
    [Export] public float SpeedDefault = 5.0f;
    [Export] public float Acceleration = 0.1f;
    [Export] public float Deceleration = 0.25f;
    [Export] public float TiltAmount = 0.09f;
    [Export] public ShapeCast3D? CrouchShapeCast;

    public override void Enter(State currentState)
    {
        if (PlayerFpsController != null && AnimationPlayer != null)
        {
            AnimationPlayer.SpeedScale = 1.0f;
            SetTilt(PlayerFpsController.CurrentRotation);
            AnimationPlayer.GetAnimation("Sliding").TrackSetKeyValue(4,0,PlayerFpsController.Velocity.Length());
            AnimationPlayer.Play("Sliding", 1.0f, SlideAnimationSpeed);
        }
    }

    public override void Update(float delta)
    {
        if (PlayerFpsController != null)
        {
            PlayerFpsController.UpdateGravity(delta);
            PlayerFpsController.UpdateVelocity();
            if (Input.IsActionJustPressed("jump") && Global.PlayerFpsController.IsOnFloor())
            {
                EmitSignal(State.SignalName.Transition, "JumpingPlayerState");
            }
        }

    }
    
    private void SetTilt(float playerRotation)
    {
        if (AnimationPlayer != null)
        {
            var tilt = Vector3.Zero;
            tilt.Z = Mathf.Clamp(TiltAmount * playerRotation, -0.1f, 0.1f);
            if (tilt.Z == 0f)
            {
                tilt.Z = 0.05f;
            }
            AnimationPlayer.GetAnimation("Sliding").TrackSetKeyValue(7,1,tilt);
            AnimationPlayer.GetAnimation("Sliding").TrackSetKeyValue(7,2,tilt);
        }
    }
    private void Finish()
    {
        EmitSignal(State.SignalName.Transition, "CrouchingPlayerState");
    }
}
