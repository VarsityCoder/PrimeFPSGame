using Godot;

namespace PrimeFPSGame.Scripts;

public partial class PlayerMovementState : State
{

    private PlayerFpsController? _playerFpsController;
    private AnimationPlayer? _animationPlayer;
    public override void _Ready()
    {
        Owner.Ready += () => _playerFpsController = Owner as PlayerFpsController;
        _animationPlayer = _playerFpsController?.AnimationPlayer;
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
