using Godot;

namespace PrimeFPSGame.Scripts;

public partial class PlayerMovementState : State
{
    protected PlayerFpsController? PlayerFpsController;
    private AnimationPlayer? _animationPlayer;
    protected InitialWeapon? WeaponController;
    public override void _Ready()
    {
        Owner.Ready += () => PlayerFpsController = Owner as PlayerFpsController;
        _animationPlayer = PlayerFpsController?.AnimationPlayer;
        WeaponController = PlayerFpsController?.WeaponController;
    }
}
