using Godot;


namespace PrimeFPSGame.Scripts;


public partial class HitscanWeapon : Node3D {

  [Export]
  public float fireRate = 14.0f;
  private Enemy enemy = new Enemy();

  [Export] public float recoil = 0.5f;
  [Export] public int weaponDamage = 10;

  [Export] public Node3D weaponMesh = new Node3D();
  public Vector3 weaponPosition;

  public RayCast3D rayCast3DWeapon = new RayCast3D();

  public Timer timer = new Timer();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
    timer = GetNode<Timer>("CooldownTimer");
    rayCast3DWeapon = GetNode<RayCast3D>("RayCast3D");
    weaponPosition = weaponMesh.Position;
  }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    if (Input.IsActionPressed("fire")) {
      if (timer.IsStopped()) {
        shoot();
      }
    }
    weaponMesh.Position = weaponMesh.Position.Lerp(weaponPosition, (float)delta * 10.0f);
  }

  public void shoot() {
    var collider = rayCast3DWeapon.GetCollider();
    timer.Start(1.0f/fireRate);
    GD.Print(collider);
    var temporaryWeaponPosition = weaponMesh.Position;
    temporaryWeaponPosition.Z += recoil;
    weaponMesh.Position = temporaryWeaponPosition;
    switch (collider)
    {
      case WeakPoint weakPoint:
        weakPoint.test();
        GD.Print("You are hitting the visor");
        break;
      case Enemy enemy1:
        enemy1.maxHitPoints -= 10;
        enemy1.isProvoked = true;
        GD.Print("Targeting enemy body");
        break;
    }
  }

  }
