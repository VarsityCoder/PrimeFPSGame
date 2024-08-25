using Godot;

namespace PrimeFPSGame.Scripts;

using Assets;

public partial class Enemy : CharacterBody3D
{ 
	public const float Speed = 5.0f; 
	public const float JumpVelocity = 4.5f;
	public const float attackRange = 1.5f;

  public NavigationAgent3D navigationAgent3DEnemy = new NavigationAgent3D();
  public Player player = new Player();
  public bool isProvoked;
  public float aggroRange = 12.0f;
  [Export] public int attackDamage = 12;

  [Export] public int maxHitPoints = 100;

  public AnimationPlayer animationPlayer = new AnimationPlayer();


  public void checkHealth() {
    if (maxHitPoints <= 0) {
      QueueFree();
    }
  }
  public override void _Ready() {
    player = (Player)GetTree().GetFirstNodeInGroup("Player");
    navigationAgent3DEnemy = GetNode<NavigationAgent3D>("NavigationAgent3D");
    animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
  }

  public override void _Process(double delta) {
    checkHealth();
    if (isProvoked) {
      navigationAgent3DEnemy.TargetPosition = player.GlobalPosition;
    }
  }

  public override void _PhysicsProcess(double delta) {
    var nextPosition = navigationAgent3DEnemy.GetNextPathPosition();
		var velocity = Velocity;
    var distance = GlobalPosition.DistanceTo(player.GlobalPosition);
    if (distance <= aggroRange) {
      isProvoked = true;
    }

    if (isProvoked) {
      if (distance <= attackRange) {
        animationPlayer.Play("Attack");
      }
    }

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
    var direction = GlobalPosition.DirectionTo(nextPosition);
		if (direction != Vector3.Zero)
		{
      lookAtTarget(direction);
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

  private void lookAtTarget(Vector3 Direction) {
    var adjustedDirection = Direction;
    adjustedDirection.Y = 0;
    LookAt(GlobalPosition + adjustedDirection, Vector3.Up, true);

  }
  private void attack() {
    GD.Print("Enemy Attack");
    player.Hitpoints -= attackDamage;
  }
}
