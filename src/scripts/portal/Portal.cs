using System;
using Godot;

namespace PrimeFPSGame.Scripts.Portal;

public partial class Portal : Area3D
{
    private static bool _teleporting;
    [Export] private int _verticalViewportResolution = 512;
    private int _id = -1;
    private Tween? _tween;
    public PortalCamera? _camera3D;
    private SubViewport? _subViewport;
    private Portal _exitPortal = new Portal();

    public Portal ExitPortal
    {
        get => _exitPortal;
        set
        {
            _exitPortal = value;
            if (_exitPortal._camera3D != null)
            {
                if (Global.PlayerFpsController?.CameraController != null)
                {
                    _exitPortal._camera3D.Far = Global.PlayerFpsController.CameraController.Far;
                    _exitPortal._camera3D.Fov = Global.PlayerFpsController.CameraController.Fov;
                    _exitPortal._camera3D.KeepAspect = Global.PlayerFpsController.CameraController.KeepAspect;
                }
            }
            _camera3D?.SetProcess(true);
        }
    }
    
    public override void _Ready()
    {
        _subViewport = GetNode<SubViewport>("SubViewport");
        _camera3D = GetNode<PortalCamera>("SubViewport/Camera3D");
        Connect("BodyEntered", new Callable(this, "OnBodyEntered"));
        Connect("BodyExited", new Callable(this, "OnBodyExited"));
        SetupViewportSize();
    }

    private void SetupViewportSize()
    {
        var viewportSize = GetViewport().GetWindow().Size;
        var aspectRatio = (float)viewportSize.X  / viewportSize.Y;
        if (_subViewport != null)
            _subViewport.Size = new Vector2I((int)(_verticalViewportResolution * aspectRatio + 0.5f),
                _verticalViewportResolution);
    }

    private void OnBodyEntered(Node3D body)
    {
        var characterBodyNode = body as CharacterBody3D;
        if (characterBodyNode != null && characterBodyNode.IsInGroup("Player"))
        {
            var h = characterBodyNode.GetNode<Node3D>("PlayerFPSController/CameraController");
            var c = h.GetNode<Camera3D>("Recoil/Camera3D");
            var inForward = -GlobalTransform.Basis.Z;
            var outForward = -_exitPortal.GlobalTransform.Basis.Z;
            var inUp = GlobalTransform.Basis.Y;
            var outUp = _exitPortal.GlobalTransform.Basis.Y;
            var initCamRot = c.Rotation;
            var initYaw = c.GlobalTransform.Basis.Z.SignedAngleTo(inForward, inUp);
            var outIsHorizontal = outForward.Dot(Vector3.Up) > 0.9;

            var posDiff = characterBodyNode.GlobalPosition - GlobalPosition;
            body.GlobalRotation = _exitPortal.GlobalPosition + posDiff;
            
            c.Rotation = Vector3.Zero;
            h.LookAt(body.GlobalPosition + outForward, outUp);
            var tempVector3 = h.Rotation;
            tempVector3.Y -= initYaw;
            h.Rotation = tempVector3;
            var pitch = Vector3.Up.SignedAngleTo(h.GlobalTransform.Basis.Y, h.GlobalTransform.Basis.X);
            h.RotateX(pitch);
            c.Rotation = initCamRot;
            c.Rotation += Vector3.Right * inUp.SignedAngleTo(outUp, _exitPortal.GlobalTransform.Basis.X);
            
            if(!outIsHorizontal)
            {
                if (c.RotationDegrees.X < -100.0)
                {
                    var tempVector3CRotation = c.Rotation;
                    tempVector3CRotation.X += Single.Pi;
                    c.Rotation = tempVector3CRotation;
                } 
                else if (c.RotationDegrees.X > 100.0)
                {
                    var tempVector3CRotation = c.Rotation;
                    tempVector3CRotation.X -= Single.Pi;
                    c.Rotation = tempVector3CRotation;
                }
                var p = new Plane(-outForward);
                var velXz = p.Project(characterBodyNode.Velocity);
                var velY = characterBodyNode.Velocity.Project(inUp);
                characterBodyNode.Velocity = velXz.Length() * -c.GlobalTransform.Basis.Z + velY * outUp;
            }
            else
            {
                characterBodyNode.Velocity = outForward * characterBodyNode.Velocity.Length();
            }
            if (outForward.Dot(Vector3.Up) > 0.9)
            {
                characterBodyNode.Velocity += Vector3.Up * 0.1f;
                characterBodyNode.GlobalTranslate(Vector3.Up * 0.3f);
            }
        }
    }

    private void OnBodyExited(Node3D? body)
    {
        body = body as CharacterBody3D;
        if (body != null && body.IsInGroup("player"))
        {
            _teleporting = false;
        }
    }
}
