using System.Diagnostics;
using Godot;
namespace PrimeFPSGame.Scripts;
[GlobalClass]
public partial class DoorComponent : Node
{
	[Export] private Vector3 _direction;
	[Export] private Vector3 _doorSize;
	[Export] private float _speed = .5f;
	[Export] private float _closeTime = 2f;
	[Export] private Tween.TransitionType _transition;
	[Export] private Tween.EaseType _easeType;

	private Vector3 _originalPosition;
	private Node3D? _parent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_parent = GetParent() as Node3D;
		Debug.Assert(_parent != null, nameof(_parent) + " != null");
		_originalPosition = _parent.Position;
		_parent.Ready += ConnectParent;
		EmitSignal(Node.SignalName.Ready);
	}

	private void ConnectParent()
	{
		if (_parent != null)
		{
			_parent.Connect("Interacted", new Callable(this, "OpenDoor"));
		}

	}

	private void OpenDoor()
	{
		var tween = GetTree().CreateTween();
		tween.TweenProperty(_parent, "position", _originalPosition + (_direction * _doorSize), _speed)
			.SetTrans(_transition).SetEase(_easeType);
		tween.TweenInterval(_closeTime);
		tween.TweenCallback(new Callable(this, "CloseDoor"));
	}

	private void CloseDoor()
	{
		var tween = GetTree().CreateTween();
		tween.TweenProperty(_parent, "position", _originalPosition, _speed).SetTrans(_transition).SetEase(_easeType);
		
	}
}
