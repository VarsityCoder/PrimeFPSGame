using Godot;

namespace PrimeFPSGame.Scripts;

using System.Globalization;

public partial class PlayerFpsController : CharacterBody3D {
  private const float JUMP_VELOCITY = 4.5f;
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
  public float _startingSpeed;


  [Export] public double TiltLowerLimit = Mathf.DegToRad(-90.0);
  [Export] public double TiltUpperLimit = Mathf.DegToRad(90.0);
  [Export] public Camera3D? CameraController;
  [Export] public float MouseSensitivity = 0.5f;
  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public ShapeCast3D? CrouchShapeCast;
  [Export] public bool ToggleCrouchMode = true;
  [Export] public float SpeedSprinting = 7.0f;
  [Export] public float SpeedDefault = 5.0f;
  [Export] public float SpeedCrouch = 2.0f;
  [Export] public float Acceleration = 0.1f;
  [Export] public float Deceleration = 0.25f;


  public override void _Ready() {
    Global.PlayerFpsController = this;
    Input.MouseMode = Input.MouseModeEnum.Captured;
    CrouchShapeCast?.AddException(this);
    _startingSpeed = SpeedDefault;
  }

  public override void _PhysicsProcess(double delta) {
		var velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
    Global.DebugPanelGlobal.AddProperty("MovementSpeed ", Velocity.Length().ToString(CultureInfo.InvariantCulture), 1);
    Global.DebugPanelGlobal.AddProperty("CrouchShapeCast ", CrouchShapeCast?.IsColliding().ToString()!, 2);
    Global.DebugPanelGlobal.AddProperty("MouseRotation ", _mouseRotation.ToString(), 3);
    Global.DebugPanelGlobal.AddProperty("IsCrouching ", _isCrouching.ToString(), 4);
    Global.DebugPanelGlobal.AddProperty("IsJumping ", (!IsOnFloor()).ToString(), 5);

    UpdateCamera(delta);

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor() && _isCrouching == false)
		{
			velocity.Y = JUMP_VELOCITY;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * _startingSpeed , Acceleration);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * _startingSpeed , Acceleration);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Deceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Deceleration);
		}

		Velocity = velocity;
		MoveAndSlide();
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

  private void UpdateCamera(double delta) {
    _mouseRotation.X += _tiltInput * (float)delta;
    _mouseRotation.X = Mathf.Clamp(_mouseRotation.X, (float)TiltLowerLimit, (float)TiltUpperLimit);
    _mouseRotation.Y += _rotationInput * (float)delta;

    _playerRotation = new Vector3(0.0f, _mouseRotation.Y, 0.0f);
    _cameraRotation = new Vector3(_mouseRotation.X, 0.0f, 0.0f);

    if (CameraController != null) {
      var tempTransform3d = CameraController.Transform;
      tempTransform3d.Basis = Basis.FromEuler(_cameraRotation);
      CameraController.Transform = tempTransform3d;
    }

    if (CameraController != null) {
      var tempRotationVector3 = CameraController.Rotation;
      tempRotationVector3.Z = 0.0f;
      CameraController.Rotation = tempRotationVector3;
    }

    var tempGlobalTransformBasis = GlobalTransform;
    tempGlobalTransformBasis.Basis = Basis.FromEuler(_playerRotation);
    GlobalTransform = tempGlobalTransformBasis;

    _rotationInput = 0.0f;
    _tiltInput = 0.0f;
  }

  public override void _Input(InputEvent @event) {

    if (@event.IsActionPressed("crouch") && IsOnFloor() && ToggleCrouchMode) {
      ToggleCrouch();
    }
    if (@event.IsActionPressed("crouch") && IsOnFloor() && ToggleCrouchMode == false && _isCrouching == false) {
      Crouching(true);
    }
    if (@event.IsActionReleased("crouch") && ToggleCrouchMode == false) {
      if (CrouchShapeCast?.IsColliding() == false) {
        Crouching(false);
      }
      else if (CrouchShapeCast?.IsColliding() == true) {
        UncrouchCheck();
      }
    }

  }

  private void ToggleCrouch() {
    if (_isCrouching && CrouchShapeCast?.IsColliding() == false) {
      Crouching(false);
    } else if (_isCrouching == false) {
      Crouching(true);
    }
  }

  private void Crouching(bool state) {
    switch (state) {
      case true:
        AnimationPlayer?.Play("Crouch", 0, _crouchSpeed);
        SetMovementSpeed("crouching");
        break;
      case false:
        AnimationPlayer?.Play("Crouch", 0, -_crouchSpeed, true);
        SetMovementSpeed("default");
        break;
    }
  }
  private void OnAnimationPlayerAnimationStarter(string animationName) {
    if(animationName == "Crouch") {
      _isCrouching = !_isCrouching;
    }
  }
  private void UncrouchCheck() {
    if (CrouchShapeCast?.IsColliding() == false) {
      Crouching(false);
    }
    if (CrouchShapeCast?.IsColliding() == true) {
      var myTimer = new Timer();
      myTimer.SetWaitTime(0.1);
      myTimer.Timeout += UncrouchCheck;
    }
  }

  private void SetMovementSpeed(string state) {
    switch (state) {
      case "default":
        _startingSpeed = SpeedDefault;
        break;
      case "crouching":
        _startingSpeed = SpeedCrouch;
        break;
    }
  }
}
