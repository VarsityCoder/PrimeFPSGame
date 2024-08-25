using Godot;
namespace PrimeFPSGame.Scripts;

public partial class SlidingPlayerState : PlayerMovementState
{
    [Export] private AnimationPlayer? _animationPlayer;
    [Export] private float _slideAnimationSpeed = 2.2f;
    [Export] private float _speedDefault = 5.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _tiltAmount = 0.09f;
    [Export] private ShapeCast3D? _crouchShapeCast;

    public override void Enter(State currentState)
    {
        if (PlayerFpsController != null && _animationPlayer != null)
        {
            _animationPlayer.SpeedScale = 1.0f;
            SetTilt(PlayerFpsController.CurrentRotation);
            _animationPlayer.GetAnimation("Sliding").TrackSetKeyValue(4,0,PlayerFpsController.Velocity.Length());
            _animationPlayer.Play("Sliding", 1.0f, _slideAnimationSpeed);
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
        if (_animationPlayer != null)
        {
            var tilt = Vector3.Zero;
            tilt.Z = Mathf.Clamp(_tiltAmount * playerRotation, -0.1f, 0.1f);
            if (tilt.Z == 0f)
            {
                tilt.Z = 0.05f;
            }
            _animationPlayer.GetAnimation("Sliding").TrackSetKeyValue(7,1,tilt);
            _animationPlayer.GetAnimation("Sliding").TrackSetKeyValue(7,2,tilt);
        }
    }
    private void Finish()
    {
        EmitSignal(State.SignalName.Transition, "CrouchingPlayerState");
    }
}
