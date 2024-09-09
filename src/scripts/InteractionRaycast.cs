using Godot;
namespace PrimeFPSGame.Scripts;

public partial class InteractionRaycast : RayCast3D
{
    private Node? _interactCastResult;
    [Export] private Camera3D? _cameraController;
    [Export] private float _interactDistance = 10f;
    private Variant _currentCastResult;
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            Interact();
        }
    }
    
    private void Interact()
    {
        if (_interactCastResult != null && _interactCastResult.HasUserSignal("Interacted"))
        {
            _interactCastResult.EmitSignal("Interacted");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        InteractCast();
    }

    private void InteractCast()
    {
        if (_cameraController != null)
        {
            _currentCastResult = GetCollider();
                if ((Node)_currentCastResult != _interactCastResult )
                {
       
                    if (_interactCastResult != null && _interactCastResult.HasUserSignal("Unfocused"))
                    {
                        _interactCastResult.EmitSignal("Unfocused");
                    }
                    _interactCastResult = (Node)_currentCastResult;
                    if (_interactCastResult != null && _interactCastResult.HasUserSignal("Focused"))
                    {
                        _interactCastResult.EmitSignal("Focused");

                    }
                }
        }
    }
    }
