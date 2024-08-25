using Godot;

namespace PrimeFPSGame.Scripts;
public partial class WeaponCamera : Camera3D
{
    [Export] private Node3D? _mainCamera;

    public override void _Process(double delta)
    {
        if (_mainCamera != null)
        {
            GlobalTransform = _mainCamera.GlobalTransform;
        }
    }
}
