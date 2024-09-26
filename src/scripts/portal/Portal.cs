using System;
using Godot;

namespace PrimeFPSGame.Scripts.Portal;

public partial class Portal : Area3D
{
    private static bool _teleporting;
    [Export] private int _verticalViewportResolution = 512;
    private int _id = -1;
    private Tween? _tween;
    public PortalCamera? Camera3D;
    private SubViewport? _subViewport;
    private Portal? _exitPortal = null;
    private ShaderMaterial? _material;

    public Portal? ExitPortal
    {
        get => _exitPortal;
        set
        {
            _exitPortal = value;
            if (_exitPortal?.Camera3D != null)
            {
                if (Global.PlayerFpsController?.CameraController != null)
                {
                    _exitPortal.Camera3D.Far = Global.PlayerFpsController.CameraController.Far;
                    _exitPortal.Camera3D.Fov = Global.PlayerFpsController.CameraController.Fov;
                    _exitPortal.Camera3D.KeepAspect = Global.PlayerFpsController.CameraController.KeepAspect;
                    _material?.SetShaderParameter("cam_view_tex", _exitPortal.GetNode<SubViewport>("SubViewport").GetTexture());
                }            
                else
                {
                    _material?.SetShaderParameter("cam_view_tex", new Variant());
                }
            }

            if (Camera3D != null)
            {
                Camera3D.ExitPortal = value;

                Camera3D.SetProcess(value != null);
            }
        }
    }
    
    public override void _Ready()
    {
        
        _subViewport = GetNode<SubViewport>("SubViewport");
        Camera3D = GetNode<PortalCamera>("SubViewport/Camera3D");
        Connect("body_entered", new Callable(this, "OnBodyEntered"));
        Connect("body_exited", new Callable(this, "OnBodyExited"));
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
        GD.Print("In on body entered");
        if (!_teleporting && Global.PlayerFpsController.IsInGroup("player"))
        {
            GD.Print("In if statement");
            _teleporting = true;
            var h = Global.PlayerFpsController.GetNode<Node3D>("CameraController");
            var c = h.GetNode<Camera3D>("Recoil/Camera3D");
            var inForward = -GlobalTransform.Basis.Z;
            if (_exitPortal != null)
            {
                GD.Print("We are in _exitportal if");
                var outForward = -_exitPortal.GlobalTransform.Basis.Z;
                var inUp = GlobalTransform.Basis.Y;
                var outUp = _exitPortal.GlobalTransform.Basis.Y;
                var initCamRot = c.Rotation;
                var initYaw = c.GlobalTransform.Basis.Z.SignedAngleTo(inForward, inUp);
                var outIsHorizontal = outForward.Dot(Vector3.Up) > 0.9;

                var posDiff = Global.PlayerFpsController.GlobalPosition - GlobalPosition;
                Global.PlayerFpsController.GlobalRotation = _exitPortal.GlobalPosition + posDiff;
            
                c.Rotation = Vector3.Zero;
                h.LookAt(  Global.PlayerFpsController.GlobalPosition + outForward, outUp);
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
                    var velXz = p.Project(Global.PlayerFpsController.Velocity);
                    var velY = Global.PlayerFpsController.Velocity.Project(inUp);
                    Global.PlayerFpsController.Velocity = velXz.Length() * -c.GlobalTransform.Basis.Z + velY * outUp;
                }
                else
                {
                    Global.PlayerFpsController.Velocity = outForward * Global.PlayerFpsController.Velocity.Length();
                }
                if (outForward.Dot(Vector3.Up) > 0.9)
                {
                    Global.PlayerFpsController.Velocity += Vector3.Up * 0.1f;
                    Global.PlayerFpsController.GlobalTranslate(Vector3.Up * 0.3f);
                }
            }
        }
    }

    private void OnBodyExited(Node3D? body)
    {
        body = body as CharacterBody3D;
        if (body != null && body.IsInGroup("player"))
        {
            GD.Print("We are in body exited");
            _teleporting = false;
        }
    }

    private void SetExitPortal()
    {
        
    }
}
