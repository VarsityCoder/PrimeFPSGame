using Godot;
namespace PrimeFPSGame.Scripts;


[GlobalClass]
public partial class InteractionComponent : Node
{
	private Node? _parent;
	private Material _highlightMaterial = GD.Load<Material>("res://src/Assets/Materials/InteractableHighlight.tres");
	[Export] private MeshInstance3D? _meshInstance;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_parent = GetParent<Node>();
		ConnectParent();
		SetDefaultMesh();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void InRange()
	{
		if (_meshInstance != null)
		{
			GD.Print("We are InRange");
			_meshInstance.MaterialOverlay = _highlightMaterial;
		}
	}

	private void OutOfRange()
	{
		if (_meshInstance != null)
		{
			GD.Print("We are OutRange");
			_meshInstance.MaterialOverlay = null;
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
			GD.Print("The mesh has already been set");
		}
		else
		{
			_meshInstance = GetParent<MeshInstance3D>();
		}
	}
}
