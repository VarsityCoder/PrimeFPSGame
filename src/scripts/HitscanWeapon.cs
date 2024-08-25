using Godot;


namespace PrimeFPSGame.Scripts;


public partial class HitscanWeapon : Node3D {

  [Export] private float _fireRate = 14.0f;
  private Enemy _enemy = new Enemy();

  [Export] private float _recoil = 0.5f;
  [Export] private int _weaponDamage = 10;

  [Export] private Node3D _weaponMesh = new Node3D();
  private Vector3 _weaponPosition;

  private RayCast3D _rayCast3DWeapon = new RayCast3D();

  private Timer _timer = new Timer();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
    _timer = GetNode<Timer>("CooldownTimer");
    _rayCast3DWeapon = GetNode<RayCast3D>("RayCast3D");
    _weaponPosition = _weaponMesh.Position;
  }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    if (Input.IsActionPressed("fire")) {
      if (_timer.IsStopped()) {
        shoot();
      }
    }
    _weaponMesh.Position = _weaponMesh.Position.Lerp(_weaponPosition, (float)delta * 10.0f);
  }

  public void shoot() {
    var collider = _rayCast3DWeapon.GetCollider();
    _timer.Start(1.0f/_fireRate);
    GD.Print(collider);
    var temporaryWeaponPosition = _weaponMesh.Position;
    temporaryWeaponPosition.Z += _recoil;
    _weaponMesh.Position = temporaryWeaponPosition;
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
