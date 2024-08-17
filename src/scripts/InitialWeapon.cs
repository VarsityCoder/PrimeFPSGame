using Godot;
namespace PrimeFPSGame.Scripts;
public partial class InitialWeapon : Node3D
{

    [Export] public WeaponsResource? WeaponType;
    private MeshInstance3D? _weaponMesh;
    private MeshInstance3D _weaponShadow = new MeshInstance3D();
    
    public override void _Ready()
    {
        _weaponMesh = GetNode<MeshInstance3D>("blasterN");
        LoadWeapon();
    }

    private void LoadWeapon()
    {
        if (_weaponMesh != null && WeaponType != null)
        {
            _weaponMesh.Mesh = WeaponType.Mesh;
            Position = WeaponType.Position;
            RotationDegrees = WeaponType.Rotation;
            _weaponShadow.Visible = WeaponType.Shadow;
        }

    }
}
