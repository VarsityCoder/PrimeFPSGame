using System;
using Godot;
namespace PrimeFPSGame.Scripts.Portal;

public partial class PortalCamera : Camera3D
{
    private const float ExitScale = 1.0f;
    private const float ExitCameraNearMin = 0.01f;
    [Export] private float _exitNearSubtract = 0.05f;
    private Portal? _portal;
    private Aabb _meshAabb;
    private Portal? _exitPortal;

    public override void _Ready()
    {
        SetProcess(false);
        _portal = GetParent().GetParent() as Portal;
        if (_portal != null) _meshAabb = _portal.GetNode<MeshInstance3D>("Mesh").Mesh.GetAabb();
    }

    public override void _Process(double delta)
    {
        if (_portal != null && _exitPortal != null && _exitPortal.Camera3D != null && Global.PlayerFpsController != null 
            && Global.PlayerFpsController.CameraController != null)
        {
            var t = _portal.GlobalTransform.AffineInverse() * Global.PlayerFpsController.CameraController.GlobalTransform;
            t = t.Rotated(Vector3.Up, Single.Pi);
            t = _exitPortal.Camera3D.GlobalTransform * t;
            _exitPortal.Camera3D.GlobalTransform = t;
        }

        if (_exitPortal != null && _exitPortal.Camera3D != null)
        {
            var corner1 = _exitPortal.ToGlobal(new Vector3(_meshAabb.Position.X, _meshAabb.Position.Y, 0) * ExitScale);
            var corner2 = _exitPortal.ToGlobal(new Vector3(_meshAabb.Position.X + _meshAabb.Size.X, _meshAabb.Position.Y, 0) * ExitScale);
            var corner3 = _exitPortal.ToGlobal(new Vector3(_meshAabb.Position.X + _meshAabb.Size.X, _meshAabb.Position.Y + _meshAabb.Size.Y, 0) * ExitScale);
            var corner4 = _exitPortal.ToGlobal(new Vector3(_meshAabb.Position.X, _meshAabb.Position.Y + _meshAabb.Size.Y, 0) * ExitScale);

            var cameraForward = -_exitPortal.GlobalTransform.Basis.Z.Normalized();

            var d1 = (corner1 - _exitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
            var d2 = (corner2 - _exitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
            var d3 = (corner3 - _exitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
            var d4 = (corner4 - _exitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
        
            _exitPortal.Camera3D.Near = float.Max(ExitCameraNearMin, float.Min(float.Min(d1,d2), float.Min(d3,d4)) - _exitNearSubtract);
        }
    }
}
