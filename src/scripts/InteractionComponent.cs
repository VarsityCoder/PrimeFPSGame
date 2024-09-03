using Godot;
namespace PrimeFPSGame.Scripts;


[GlobalClass]
public partial class InteractionComponent : Node
{
	[Export] private string? _context;
	[Export] private bool _overrideIcon;
	[Export] private Texture2D _newIcon = new Texture2D();
	
	private Node? _parent;
	private Material _highlightMaterial = GD.Load<Material>("res://src/Assets/Materials/InteractableHighlight.tres");
	private MeshInstance3D? _meshInstance;
	
	
	public override void _Ready()
	{
		_parent = GetParent<Node>();
		ConnectParent();
		SetDefaultMesh();
	}
	
	private void InRange()
	{

		if (_meshInstance != null && _context != null)
		{
			Global.UiContext?.Update(_context,_newIcon, _overrideIcon);
			_meshInstance.MaterialOverlay = _highlightMaterial;
			//TODO potentially use messagebus in the future
			//EmitSignal(MessageBus.SignalName.InteractionFocused);

		}
	}

	private void OutOfRange()
	{
		Global.UiContext?.Reset();
		if (_meshInstance != null)
		{
			_meshInstance.MaterialOverlay = null;
			//TODO potentially use messagebus in the future
			//EmitSignal(MessageBus.SignalName.InteractionUnfocused);
		}

	}

	private void OnInteract()
	{
		GD.Print(_parent?.Name);
	}

	private void ConnectParent()
	{
		if (_parent != null)
		{
			_parent.AddUserSignal("Focused");
			_parent.AddUserSignal("Unfocused");
			_parent.AddUserSignal("Interacted");
			_parent.Connect("Focused", new Callable(this, "InRange"));
			_parent.Connect("Unfocused", new Callable(this, "OutOfRange"));
			_parent.Connect("Interacted", new Callable(this, "OnInteract"));
		}
	}

	private void SetDefaultMesh()
	{
		if (_meshInstance != null)
		{
			
		} else
		{
			if (_parent != null)
			{
				foreach (Node? child in _parent.GetChildren())
				{
					{
						if (child is MeshInstance3D instance3D)
						{
							_meshInstance = instance3D;
						}
					}
				}
			}
		}
	}
}
