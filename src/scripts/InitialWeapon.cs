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
    
    private MeshInstance3D? _weaponMesh;
    private MeshInstance3D _weaponShadow = new MeshInstance3D();
    [Export] private NoiseTexture2D? _swayNoise;

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

    private void SwayWeapon(float delta)
    {
        if (_weaponType != null)
        {
            var tempPosition = Position;
            _mouseMovement = _mouseMovement.Clamp(_weaponType.SwayMin, _weaponType.SwayMax);
            tempPosition.X = Mathf.Lerp(Position.X, _weaponType.Position.X - (_mouseMovement.X * _weaponType.SwayAmountPosition) 
                * delta, _weaponType.SwaySpeedPosition);
            
            tempPosition.Y = Mathf.Lerp(Position.Y, _weaponType.Position.Y + (_mouseMovement.Y * _weaponType.SwayAmountPosition) 
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

    public override void _PhysicsProcess(double delta)
    {
        SwayWeapon((float)delta);
    }
}
