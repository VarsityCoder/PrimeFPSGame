using Godot;

namespace PrimeFPSGame.Scripts;

[GlobalClass]
public partial class PickupComponent : Node
{

    [Export] private Vector3 _pickupDistance = new Vector3(0,0,-1);
    private Camera3D? _camera;
    [Export] private PlayerFpsController _playerFpsController;

    private InteractionComponent? _parent;
    [Export] private Node3D? _object;

    private bool _pickedUp;
    private const float PickupLerp = 0.3f;
    


    public override void _Ready()
    {
        _parent = GetParent() as InteractionComponent;
        if (_parent != null)
        {
            _parent.Connect("PlayerInteracted", new Callable(this, "UpdateState"));
        }
    }
    
    public override string[] _GetConfigurationWarnings()
    {
        if (GetParent() is not InteractionComponent)
        {
            return ["This node must have a InteractionComponent parent!"];
        }

        return [];
    }

    private void UpdateState(RigidBody3D? interactable)
    {
        if (_pickedUp && interactable != null) 
        {
            _pickedUp = false;
            _object = null;
            interactable.Freeze = false;
            GD.Print("We are in the Update State Function if");
        }
        else
        {
            _object = interactable;
            if (interactable != null) interactable.Freeze = true;
            _pickedUp = true;
            GD.Print("We are in the Update State Function else");
        }

    }

    public override void _Notification(int what)
    {
        if (what == NotificationEnterTree)
        {
            _parent = GetParent() as InteractionComponent;
            UpdateConfigurationWarnings();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_pickedUp && _object != null && _playerFpsController.CameraController != null)
        {
            var cameraTransform = _playerFpsController.CameraController.GlobalTransform;
            _object.GlobalTransform = _object.GlobalTransform.InterpolateWith(cameraTransform.TranslatedLocal(_pickupDistance), PickupLerp);
            GD.Print("We are in the picked up physics process");
        }
    }
}
