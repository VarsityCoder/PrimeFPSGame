using Godot;

namespace PrimeFPSGame.Assets;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
  public Vector2 mouseMotion = Vector2.Zero;
  public Node3D cameraPivot = new Node3D();
  [Export] public float jumpHeight = 1.0f;
  public static readonly int MaxHitpoints = 100;
  public double gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
  public float fallMultiplier = 2.0f;
  private int hitpoints = MaxHitpoints;
  private AnimationPlayer animationPlayer = new AnimationPlayer();

  public bool isCrouching;

  public int Hitpoints {
    get => hitpoints;
    set {hitpoints = value;
      GD.Print(hitpoints);
      if (hitpoints <= 0) {
        GetTree().Quit();
      }
    }
  }
  public override void _Ready() {
    cameraPivot = GetNode<Node3D>("CameraPivot");
    animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _PhysicsProcess(double delta)
	{
    handleCameraRotation();
		var velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
      if (velocity.Y >= 0) {
        velocity += GetGravity() * (float)delta;
      }
      else {
        velocity.Y -= (float)gravity * (float)delta * fallMultiplier;
      }

		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
      velocity.Y = Mathf.Sqrt(jumpHeight * 2.0f * (float)gravity);
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseMotion) {
      if (Input.MouseMode == Input.MouseModeEnum.Captured) {
        var m = (InputEventMouseMotion)@event;
        mouseMotion = -m.Relative * 0.001f;
      }
    }
    if (@event.IsActionPressed("ui_cancel")) {
      Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    else if(@event.IsActionPressed("fire")) {
      Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    if (@event.IsActionPressed("crouch")) {
      toggleCrouch();
    }
  }

  private void handleCameraRotation() {
    cameraPivot.RotateX(mouseMotion.Y);
    var tempVector3 = cameraPivot.RotationDegrees;
    tempVector3.X= Mathf.Clamp(cameraPivot.RotationDegrees.X, -90.0f, 90.0f);
    RotateY(mouseMotion.X);
    mouseMotion = Vector2.Zero;
    cameraPivot.RotationDegrees = tempVector3;
  }

  private void toggleCrouch() {
    if (isCrouching) {
      GD.Print("Crouch Off");

    } else if (isCrouching == false) {
      animationPlayer.Play("Crouch");
    }

    isCrouching = !isCrouching;
  }
}
