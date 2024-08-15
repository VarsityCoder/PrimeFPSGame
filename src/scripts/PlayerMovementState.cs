using Godot;

namespace PrimeFPSGame.Scripts;

public partial class PlayerMovementState : State
{
    protected PlayerFpsController? PlayerFpsController;
    private AnimationPlayer? _animationPlayer;
    public override void _Ready()
    {
        Owner.Ready += () => PlayerFpsController = Owner as PlayerFpsController;
        _animationPlayer = PlayerFpsController?.AnimationPlayer;
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
