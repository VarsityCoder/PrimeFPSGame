using System.Diagnostics;
using Godot;
namespace PrimeFPSGame.Scripts;
[GlobalClass]
public partial class DoorComponent : Area3D
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

	enum DoorStatus
	{
		Open,
		Closed
	}

	enum DoorOperation
	{
		Manual,
		Close_Automatically,
		Open_Close_Automatically
	}

	[Export] private DoorType _doorType;
	[Export] private ForwardDirection _forwardDirection;
	[Export] private Vector3 _direction;
	[Export] private Vector3 _doorSize;
	[Export] private float _speed = .5f;
	[Export] private float _closeTime = 2f;
	[Export] private Tween.TransitionType _transition;
	[Export] private Tween.EaseType _easeType;
	[Export] private Vector3 _rotationAxis = new Vector3(0, 1, 0);
	[Export] private float _rotationAmount = 90f;
	[Export] private DoorOperation _doorOperation;

	private Vector3 _originalPosition;
	private Node3D? _parent;
	private Vector3 _originalRotation;
	private float _rotationAdjustment;
	private Vector3 _doorDirection;
	private DoorStatus _doorStatus = DoorStatus.Closed;
	
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

			if (_doorStatus == DoorStatus.Closed)
			{
				OpenDoor();
			} 
			if (_doorStatus == DoorStatus.Open)
			{
				CloseDoor();
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
		if (_parent != null && _doorOperation == DoorOperation.Manual)
		{
			_parent.Connect("Interacted", new Callable(this, "CheckDoor"));
		}

	}

	private void OpenDoor()
	{
		_doorStatus = DoorStatus.Open;
		var tween = GetTree().CreateTween();
		if (_doorType == DoorType.Sliding)
		{
			tween.TweenProperty(_parent, "position", _originalPosition + (_direction * _doorSize), _speed)
				.SetTrans(_transition).SetEase(_easeType);
		}

		if (_doorType == DoorType.Rotating)
		{
			tween.TweenProperty(_parent, "rotation", _originalRotation + (_rotationAxis * _rotationAdjustment * Mathf.DegToRad(_rotationAmount)),
				_speed).SetTrans(_transition).SetEase(_easeType);
		}

		if (_doorOperation == DoorOperation.Close_Automatically)
		{
			tween.TweenInterval(_closeTime);
			tween.TweenCallback(new Callable(this, "CloseDoor"));
		}
	}

	private void CloseDoor()
	{
		_doorStatus = DoorStatus.Closed;
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

	private void BodyEnteredCheck(Node3D body)
	{
		if (_doorOperation == DoorOperation.Open_Close_Automatically && body.IsInGroup("player"))
		{
			CheckDoor();
		}
		
	}

	private void BodyExitedCheck(Node3D body)
	{
		if (_doorOperation == DoorOperation.Open_Close_Automatically && body.IsInGroup("player") && _doorStatus == DoorStatus.Open)
		{
			CheckDoor();
		}
		
	}
}
