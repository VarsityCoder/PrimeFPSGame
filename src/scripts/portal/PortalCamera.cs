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
    public Portal? ExitPortal;

    public override void _Ready()
    {
        GD.Print("Portal Camera exists");
        _portal = GetParent().GetParent() as Portal;
        if (_portal != null) _meshAabb = _portal.GetNode<MeshInstance3D>("PortalMesh").Mesh.GetAabb();
    }

    public override void _Process(double delta)
    {
        if (ExitPortal != null)
        {
            GD.Print("Portal is not null");
        }
        else
        {
            GD.Print("Portal is null");
        }
        if (_portal != null && ExitPortal != null && ExitPortal.Camera3D != null && Global.PlayerFpsController != null 
            && Global.PlayerFpsController.CameraController != null)
        {

            GD.Print("Exit portal transform is working");
            var t = _portal.GlobalTransform.AffineInverse() * Global.PlayerFpsController.CameraController.GlobalTransform;
            t = t.Rotated(Vector3.Up, Single.Pi);
            t = ExitPortal.Camera3D.GlobalTransform * t;
            ExitPortal.Camera3D.GlobalTransform = t;
        }

        if (ExitPortal != null && ExitPortal.Camera3D != null)
        {
            GD.Print("Exit portal corner is working");
            var corner1 = ExitPortal.ToGlobal(new Vector3(_meshAabb.Position.X, _meshAabb.Position.Y, 0) * ExitScale);
            var corner2 = ExitPortal.ToGlobal(new Vector3(_meshAabb.Position.X + _meshAabb.Size.X, _meshAabb.Position.Y, 0) * ExitScale);
            var corner3 = ExitPortal.ToGlobal(new Vector3(_meshAabb.Position.X + _meshAabb.Size.X, _meshAabb.Position.Y + _meshAabb.Size.Y, 0) * ExitScale);
            var corner4 = ExitPortal.ToGlobal(new Vector3(_meshAabb.Position.X, _meshAabb.Position.Y + _meshAabb.Size.Y, 0) * ExitScale);

            var cameraForward = -ExitPortal.GlobalTransform.Basis.Z.Normalized();

            var d1 = (corner1 - ExitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
            var d2 = (corner2 - ExitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
            var d3 = (corner3 - ExitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
            var d4 = (corner4 - ExitPortal.Camera3D.GlobalPosition).Dot(cameraForward);
        
            ExitPortal.Camera3D.Near = float.Max(ExitCameraNearMin, float.Min(float.Min(d1,d2), float.Min(d3,d4)) - _exitNearSubtract);
        }
    }
}
