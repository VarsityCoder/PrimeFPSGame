using Godot;
namespace PrimeFPSGame.Scripts.Portal;

public partial class StencilPortal : MeshInstance3D
{
    [Export] private bool _current;
    [Export] private NodePath? _otherPortalPath;
    [Export] private Camera3D? _portalCamera;
    private MeshInstance3D? _inside;

    private StencilPortal? _otherPortal;
    private Node3D? _helper;


    public override void _Ready()
    {
        _inside = GetNode<MeshInstance3D>("Inside");
        _helper = GetNode<Node3D>("helper");
        if (_otherPortalPath != null && !_otherPortalPath.IsEmpty)
        {
            _otherPortal = GetNode<StencilPortal>(_otherPortalPath);
        }
        if (_current)
        {
            _inside.Visible = true;
        }
    }

    public override void _Process(double delta)
    {
        if (_current && _helper != null && _otherPortal is { _helper: not null })
        {
            var mainCam = GetViewport().GetCamera3D();
            _helper.GlobalTransform = mainCam.GlobalTransform;
            _otherPortal._helper.Transform = _helper.Transform;
            if (_portalCamera != null) _portalCamera.GlobalTransform = _otherPortal._helper.GlobalTransform;
            // var diff = GlobalTransform.Origin - mainCam.GlobalTransform.Origin;
            // var angle = mainCam.GlobalTransform.Basis.Z.AngleTo(diff);
            // var nearDistance = _helper.Transform.Origin.Length() * double.Abs(double.Cos(angle));
            if (!Visible)
            {
                Visible = true;
            }
        }
        else
        {
            if (Visible)
            {
                Visible = false;
            }
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("player"))
        {
            return;
        }

        if (!_current)
        {
            _current = true;
            Visible = true;
        }

        if (_inside != null && _helper != null && _otherPortal is { _helper: not null })
        {
            if (_current && _inside.Visible)
            {
                _helper.GlobalTransform = body.GlobalTransform;
                _otherPortal._helper.Transform = _helper.Transform;
                body.GlobalTransform = _otherPortal._helper.GlobalTransform;
                _current = false;
                _inside.Visible = false;
            }
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (!body.IsInGroup("player"))
        {
            return;
        }

        if (_inside != null)
        {
            if (_current && !_inside.Visible)
            {
                _inside.Visible = true;
            }
        }
    }
}
