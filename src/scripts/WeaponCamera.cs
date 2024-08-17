using Godot;

namespace PrimeFPSGame.Scripts;
public partial class WeaponCamera : Camera3D
{
    [Export] public Node3D? MainCamera;

    public override void _Process(double delta)
    {
        if (MainCamera != null)
        {
            GlobalTransform = MainCamera.GlobalTransform;
        }
    }
}
