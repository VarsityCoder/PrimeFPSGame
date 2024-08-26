using Godot;
namespace PrimeFPSGame.Scripts;
public partial class InitialWeapon : Node3D
{

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

        if (@event is InputEventMouseMotion)
        {
            var m = (InputEventMouseMotion)@event;
            _mouseMovement = m.Relative;
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
}
