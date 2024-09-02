using Godot;

namespace PrimeFPSGame.Scripts;

using System.Globalization;


public partial class PlayerFpsController : CharacterBody3D {
  

  private const float JumpVelocity = 4.5f;
  private bool _mouseInput;
  private float _rotationInput;
  private float _tiltInput;
  private Vector3 _cameraRotation;
  private Vector3 _playerRotation;
  private Vector3 _playerRotationVector3;
  private Vector3 _cameraRotationVector3;
  private bool _isCrouching;
  private float _crouchSpeed = 7.0f;
  private Vector3 _mouseRotation;
  private float _startingSpeed;
  public float CurrentRotation;



  [Export] private double _tiltLowerLimit = Mathf.DegToRad(-90.0);
  [Export] private double _tiltUpperLimit = Mathf.DegToRad(90.0);
  [Export] private Camera3D? _cameraController;
  [Export] private float _mouseSensitivity = 0.5f;
  [Export] protected internal AnimationPlayer? AnimationPlayer;
  [Export] private ShapeCast3D? _crouchShapeCast;
  [Export] private bool _toggleCrouchMode = true;
  protected internal InitialWeapon? WeaponController;
  [Export] private float _interactDistance = 10f;
  private Node? _interactCastResult;
  

  public override void _Ready() {
    Global.PlayerFpsController = this;
    Input.MouseMode = Input.MouseModeEnum.Captured;
    _crouchShapeCast?.AddException(this);
    WeaponController = GetNode<Node3D>("CameraController/Recoil/Camera3D/WeaponContainer/WeaponBlaster") as InitialWeapon;
  }

  public override void _PhysicsProcess(double delta) {
		var velocity = Velocity;
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

    if (Global.DebugPanelGlobal != null)
    {
      Global.DebugPanelGlobal.AddProperty("MovementSpeed ", Velocity.Length().ToString(CultureInfo.InvariantCulture), 1);
      Global.DebugPanelGlobal.AddProperty("CrouchShapeCast ", _crouchShapeCast?.IsColliding().ToString()!, 2);
      Global.DebugPanelGlobal.AddProperty("MouseRotation ", _mouseRotation.ToString(), 3);
      Global.DebugPanelGlobal.AddProperty("IsCrouching ", _isCrouching.ToString(), 4);
      Global.DebugPanelGlobal.AddProperty("IsJumping ", (!IsOnFloor()).ToString(), 5);
    }

    UpdateCamera(delta);

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
      Velocity = velocity;
    }
    InteractCast();
	}

  public override void _UnhandledInput(InputEvent @event) {
    _mouseInput = @event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured;
    if (_mouseInput) {
      var m = (InputEventMouseMotion)@event;
      _rotationInput = -m.Relative.X;
      _tiltInput = -m.Relative.Y;
    }


    if (@event.IsActionPressed("ui_cancel")) {
      Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    else if(@event.IsActionPressed("fire")) {
      Input.MouseMode = Input.MouseModeEnum.Captured;
    }
  }

  private void UpdateCamera(double delta)
  {
    CurrentRotation = _rotationInput;
    _mouseRotation.X += _tiltInput * (float)delta;
    _mouseRotation.X = Mathf.Clamp(_mouseRotation.X, (float)_tiltLowerLimit, (float)_tiltUpperLimit);
    _mouseRotation.Y += _rotationInput * (float)delta;

    _playerRotation = new Vector3(0.0f, _mouseRotation.Y, 0.0f);
    _cameraRotation = new Vector3(_mouseRotation.X, 0.0f, 0.0f);

    if (_cameraController != null) {
      var tempTransform3d = _cameraController.Transform;
      tempTransform3d.Basis = Basis.FromEuler(_cameraRotation);
      _cameraController.Transform = tempTransform3d;
    }

    if (_cameraController != null) {
      var tempRotationVector3 = _cameraController.Rotation;
      tempRotationVector3.Z = 0.0f;
      _cameraController.Rotation = tempRotationVector3;
    }

    var tempGlobalTransformBasis = GlobalTransform;
    tempGlobalTransformBasis.Basis = Basis.FromEuler(_playerRotation);
    GlobalTransform = tempGlobalTransformBasis;

    _rotationInput = 0.0f;
    _tiltInput = 0.0f;
  }
  
  private void OnAnimationPlayerAnimationStarter(string animationName) 
  {
    if(animationName == "Crouch") {
      _isCrouching = !_isCrouching;
    }
  }

  public void UpdateGravity(float delta)
  {
      var tempVector3 = Velocity;
      var gravity = GetGravity().Y;
      tempVector3.Y += gravity * delta;
      Velocity = tempVector3;
  }

  public override void _Input(InputEvent @event)
  {
    if (@event.IsActionPressed("interact"))
    {
      Interact();
    }
  }

  public void UpdateInput(float speed, float acceleration, float deceleration) 
  {
    var velocity = Velocity;
    var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
    if (direction != Vector3.Zero)
    {
      velocity.X = Mathf.Lerp(velocity.X, direction.X * speed , acceleration);
      velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed , acceleration);
    }
    else
    {
      velocity.X = Mathf.MoveToward(Velocity.X, 0, deceleration);
      velocity.Z = Mathf.MoveToward(Velocity.Z, 0, deceleration);
    }

    Velocity = velocity;
  }

  private void Interact()
  {
    if (_interactCastResult != null && _interactCastResult.HasUserSignal("Interacted"))
    {
      _interactCastResult.EmitSignal("Interacted");
    }
  }

  private void InteractCast()
  {
    if (_cameraController != null)
    {
      var spaceState = _cameraController.GetWorld3D().DirectSpaceState;
      var screenCenter = GetViewport().GetWindow().Size / 2;
      var origin = _cameraController.ProjectRayOrigin(screenCenter);
      var end = origin + _cameraController.ProjectRayNormal(screenCenter) * _interactDistance;
      var query = PhysicsRayQueryParameters3D.Create(origin, end);
      query.CollideWithBodies = true;
      var result = spaceState.IntersectRay(query);
      if (result.Keys.Contains("collider"))
      {
        var currentCastResult = result["collider"];
        if ((Node)currentCastResult != _interactCastResult)
        {
       
          if (_interactCastResult != null && _interactCastResult.HasUserSignal("Unfocused"))
          {
            _interactCastResult.EmitSignal("Unfocused");
          }
          _interactCastResult = (Node)currentCastResult;
          if (_interactCastResult != null && _interactCastResult.HasUserSignal("Focused"))
          {
            _interactCastResult.EmitSignal("Focused");

          }
        }
      }
    }
  }

  public void UpdateVelocity()
  {
    MoveAndSlide();
  }
}