using System.Diagnostics;
using Godot;
namespace PrimeFPSGame.Scripts;
[GlobalClass]
public partial class DoorComponent : Node
{
	enum DoorType
	{
		Sliding,
		Rotating
	}

	enum ForwardDirection
	{
		X,
		Y,
		Z
	}

	[Export] private DoorType _doorType;
	[Export] private ForwardDirection _forwardDirection;
	[Export] private Vector3 _direction;
	[Export] private Vector3 _doorSize;
	[Export] private float _speed = .5f;
	[Export] private float _closeTime = 2f;
	[Export] private Tween.TransitionType _transition;
	[Export] private Tween.EaseType _easeType;
	[Export] private Vector3 _rotation = new Vector3(0, 1, 0);
	[Export] private float _rotationAmount = 90f;

	private Vector3 _originalPosition;
	private Node3D? _parent;
	private Vector3 _originalRotation;
	private float _rotationAdjustment;
	private Vector3 _doorDirection;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_parent = GetParent() as Node3D;
		Debug.Assert(_parent != null, nameof(_parent) + " != null");
		_originalPosition = _parent.Position;
		_parent.Ready += ConnectParent;
		_originalRotation = _parent.Rotation;
		EmitSignal(Node.SignalName.Ready);
	}

	private void CheckDoor()
	{
		if (_parent != null)
		{
			if (_forwardDirection == ForwardDirection.X)
			{
				_doorDirection = _parent.GlobalTransform.Basis.X;
			}

			if (_forwardDirection == ForwardDirection.Y)
			{
				_doorDirection = _parent.GlobalTransform.Basis.Y;
			}

			if (_forwardDirection == ForwardDirection.Z)
			{
				_doorDirection = _parent.GlobalTransform.Basis.Z;
			}
			
		}

		if (_parent != null)
		{
			var doorPosition = _parent.GlobalPosition;
			if (Global.PlayerFpsController != null)
			{
				var playerPosition = Global.PlayerFpsController.GlobalPosition;
				var directionToPlayer = doorPosition.DirectionTo(playerPosition);
				var doorDot = directionToPlayer.Dot(_doorDirection);



				if (doorDot < 0)
				{
					_rotationAdjustment = 1;
				}
				else
				{
					_rotationAdjustment = -1;
				}
			}
		}

		OpenDoor();


	}

	private void ConnectParent()
	{
		if (_parent != null)
		{
			_parent.Connect("Interacted", new Callable(this, "CheckDoor"));
		}

	}

	private void OpenDoor()
	{
		var tween = GetTree().CreateTween();
		if (_doorType == DoorType.Sliding)
		{
			tween.TweenProperty(_parent, "position", _originalPosition + (_direction * _doorSize), _speed)
				.SetTrans(_transition).SetEase(_easeType);
		}

		if (_doorType == DoorType.Rotating)
		{
			tween.TweenProperty(_parent, "rotation", _originalRotation + (_rotation * _rotationAdjustment * Mathf.DegToRad(_rotationAmount)),
				_speed).SetTrans(_transition).SetEase(_easeType);
		}
		tween.TweenInterval(_closeTime);
		tween.TweenCallback(new Callable(this, "CloseDoor"));
	}

	private void CloseDoor()
	{
		var tween = GetTree().CreateTween();
		if (_doorType == DoorType.Sliding)
		{
			tween.TweenProperty(_parent, "position", _originalPosition, _speed).SetTrans(_transition).SetEase(_easeType);
		}

		if (_doorType == DoorType.Rotating)
		{
			tween.TweenProperty(_parent, "rotation", _originalRotation, _speed).SetTrans(_transition).SetEase(_easeType);
		}

	}
}
