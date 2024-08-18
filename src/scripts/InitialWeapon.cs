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
    }
}
