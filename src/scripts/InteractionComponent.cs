using Godot;
namespace PrimeFPSGame.Scripts;


[GlobalClass]
public partial class InteractionComponent : Node
{
	private Node? _parent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_parent = GetParent<Node>();
		ConnectParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void InRange()
	{
		GD.Print(_parent);
	}

	private void OutOfRange()
	{
		GD.Print("We are out of range");
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
}
