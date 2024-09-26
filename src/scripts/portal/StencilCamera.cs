using Godot;
namespace PrimeFPSGame.Scripts;

public partial class StencilCamera : Camera3D
{
    private NodePath? _mainCamPath;
    [Export] private PlayerFpsController? _player;
    private Camera3D? _mainCam;

    public override void _Ready()
    {
        if (_player?.CameraController != null)
        {
            _mainCam = _player.CameraController;
        }

    }

    public override void _Process(double delta)
    {
        if (_mainCam != null)
        {
            GlobalTransform = _mainCam.GlobalTransform;
        }
    }
}
