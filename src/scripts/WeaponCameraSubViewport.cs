using Godot;

namespace PrimeFPSGame.Scripts;
public partial class WeaponCameraSubViewport : SubViewport
{
    private Vector2 _screenSize;

    public override void _Ready()
    {
        _screenSize = GetWindow().Size;
        Size = (Vector2I)_screenSize;
    }
}
