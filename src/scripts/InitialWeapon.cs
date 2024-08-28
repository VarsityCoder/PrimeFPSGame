using Godot;
namespace PrimeFPSGame.Scripts;
public partial class InitialWeapon : Node3D
{
    [Signal]
    public delegate void RecoilEventHandler();

    [Export] private WeaponsResource? _weaponType;

    public WeaponsResource? WeaponType
    {
        get => _weaponType;
        set
        {
            _weaponType = value;
            if (Engine.IsEditorHint())
            {
                LoadWeapon();
            }
        }
    }

    [Export] private bool _reset;

    public bool Reset
    {
        get => _reset;
        set
        {
            _reset = value;
            if (Engine.IsEditorHint())
            {
                LoadWeapon();
            }
        }
    }
    
    
    private MeshInstance3D? _weaponMesh;
    private MeshInstance3D _weaponShadow = new MeshInstance3D();
    [Export] private NoiseTexture2D? _swayNoise;
    [Export] private float _swaySpeed = 1.2f;
    [Export] private Camera3D? _attackCamera;
    private PackedScene _raycastTest = GD.Load<PackedScene>("res://src/Assets/Flag.tscn");
    private Tween? _decalTween;

    private float _randomSwayX;
    private float _randomSwayY;
    private float _randomSwayAmount;
    private float _time;
    private float _idleSwayAdjustment;
    private float _idleSwayRotationStrength;
    private Vector2 _weaponBobAmount = Vector2.Zero;
    

    private Vector2 _mouseMovement;
    
    public override void _Ready()
    {
        _weaponMesh = GetNode<MeshInstance3D>("WeaponMesh");
        _decalTween = GetTree().CreateTween();
        _decalTween.Stop();
        LoadWeapon();
    }

    private void LoadWeapon()
    {
        if (_weaponMesh != null && _weaponType != null)
        {
            _weaponMesh.Mesh = _weaponType.Mesh;
            Position = _weaponType.Position;
            RotationDegrees = _weaponType.Rotation;
            _weaponShadow.Visible = _weaponType.Shadow;
            _idleSwayAdjustment = _weaponType.IdleSwayAdjustment;
            _idleSwayRotationStrength = _weaponType.IdleSwayRotationStrength;
            _randomSwayAmount = _weaponType.RandomSwayAmount;
        }

    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("weapon1"))
        {
            WeaponType = GD.Load<WeaponsResource>("res://src/Assets/WeaponResource.tres");
            LoadWeapon();
        }

        if (@event.IsActionPressed("weapon2"))
        {
            WeaponType = GD.Load<WeaponsResource>("res://src/Assets/WeaponResource2.tres");
            LoadWeapon();
        }

        if (@event is InputEventMouseMotion motion)
        {
            _mouseMovement = motion.Relative;
        }
    }

    public void SwayWeapon(float delta, bool isIdle)
    {
        if (_weaponType != null)
        {
            _mouseMovement = _mouseMovement.Clamp(_weaponType.SwayMin, _weaponType.SwayMax);
        }
        if (isIdle && _weaponType != null)
        {
            var swayRandom = GetSwayNoise();
            var swayRandomAdjusted = swayRandom * _idleSwayAdjustment;

            _time += delta * (_swaySpeed * swayRandom);
            _randomSwayX = Mathf.Sin(_time * 1.5f + swayRandomAdjusted) / _randomSwayAmount;
            _randomSwayY = Mathf.Sin(_time - swayRandomAdjusted) / _randomSwayAmount;
            var tempPosition = Position;
            tempPosition.X = Mathf.Lerp(Position.X, _weaponType.Position.X - (_mouseMovement.X * _weaponType.SwayAmountPosition + _randomSwayX) 
                * delta, _weaponType.SwaySpeedPosition);
            
            tempPosition.Y = Mathf.Lerp(Position.Y, _weaponType.Position.Y + (_mouseMovement.Y * _weaponType.SwayAmountPosition + _randomSwayY) 
                * delta, _weaponType.SwaySpeedPosition);
            Position = tempPosition;

            var tempRotationDegrees = RotationDegrees;
            tempRotationDegrees.Y = Mathf.Lerp(RotationDegrees.Y,
                _weaponType.Rotation.Y + (_mouseMovement.Y * _weaponType.SwayAmountRotation + (_randomSwayY * _idleSwayRotationStrength)) * delta,
                _weaponType.SwaySpeedRotation);

            tempRotationDegrees.X = Mathf.Lerp(RotationDegrees.X,
                _weaponType.Rotation.X - (_mouseMovement.X * _weaponType.SwayAmountRotation + (_randomSwayX * _idleSwayRotationStrength)) * delta,
                _weaponType.SwaySpeedRotation);
            RotationDegrees = tempRotationDegrees;
        }
        else
        {
            if (_weaponType != null)
            {
                var tempPosition = Position;
                tempPosition.X = Mathf.Lerp(Position.X, _weaponType.Position.X - (_mouseMovement.X * _weaponType.SwayAmountPosition + _weaponBobAmount.X) 
                    * delta, _weaponType.SwaySpeedPosition);
            
                tempPosition.Y = Mathf.Lerp(Position.Y, _weaponType.Position.Y + (_mouseMovement.Y * _weaponType.SwayAmountPosition + _weaponBobAmount.Y) 
                    * delta, _weaponType.SwaySpeedPosition);
                Position = tempPosition;

                var tempRotationDegrees = RotationDegrees;
                tempRotationDegrees.Y = Mathf.Lerp(RotationDegrees.Y,
                    _weaponType.Rotation.Y + (_mouseMovement.Y * _weaponType.SwayAmountRotation) * delta,
                    _weaponType.SwaySpeedRotation);

                tempRotationDegrees.X = Mathf.Lerp(RotationDegrees.X,
                    _weaponType.Rotation.X - (_mouseMovement.X * _weaponType.SwayAmountRotation) * delta,
                    _weaponType.SwaySpeedRotation);
                RotationDegrees = tempRotationDegrees;
            
            }
        }
    }

    public void WeaponBob(float delta, float bobSpeed, float horizontalBobAmount, float verticalBobAmount)
    {
        _time += delta;
        _weaponBobAmount.X = Mathf.Sin(_time * bobSpeed) * horizontalBobAmount;
        _weaponBobAmount.Y = Mathf.Abs(Mathf.Cos(_time * bobSpeed) * verticalBobAmount);
    }
    private float GetSwayNoise()
    {
        var playerPosition = Vector3.Zero;
        if (!Engine.IsEditorHint())
        {
            playerPosition = Global.PlayerFpsController.GlobalPosition;
        }

        var noiseLocation = 0f;
        if (_swayNoise != null)
        {
            noiseLocation = _swayNoise.Noise.GetNoise2D(playerPosition.X, playerPosition.Y);
        }
        return noiseLocation;
    }

    public void Attack()
    {
        EmitSignal(SignalName.Recoil);
        if (_attackCamera != null)
        {
            var spaceState = _attackCamera.GetWorld3D().DirectSpaceState;
            var screenCenter = GetViewport().GetWindow().Size / 2;
            var origin = _attackCamera.ProjectRayOrigin(screenCenter);
            var end = origin + _attackCamera.ProjectRayNormal(screenCenter ) * 1000;
            var query = PhysicsRayQueryParameters3D.Create(origin, end);
            query.CollideWithBodies = true;
            var result = spaceState?.IntersectRay(query);
            if (result != null)
            {
                DecalSpawn((Vector3)result["position"], (Vector3)result["normal"]);
            }
        }
    }

    private void DecalSpawn(Vector3 position, Vector3 normal)
    {
        var instance = _raycastTest.Instantiate() as Decal;
        GetTree().Root.AddChild(instance);
        if (instance != null)
        {
            instance.GlobalPosition = position;
            if (normal != Vector3.Up)
            {
                instance.LookAt(instance.GlobalTransform.Origin + normal, Vector3.Up);
            }


            if (normal != Vector3.Up && normal != Vector3.Down)
            {
                instance.RotateObjectLocal(new Vector3(1,0,0), 90);
            }

            if (_decalTween != null && !_decalTween.IsRunning())
            {
                //TODO need to figure out why not tweening
                _decalTween.TweenProperty(instance, "modulate:a", 0, 1.5);
                GetTree().CreateTimer(2).Timeout += () => instance.QueueFree();
            }
        }

    }
}
