using System;
using Godot;

namespace PrimeFPSGame.Scripts;

public partial class Recoil : Node3D
{

    [Export] private Vector3 _recoilAmount;
    [Export] private float _snapAmount;
    [Export] private float _speed;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;
    private InitialWeapon? _weaponController;

    public override void _Ready()
    {
        _weaponController = GetNode<InitialWeapon>("Camera3D/WeaponContainer/WeaponBlaster");
    }

    public override void _Process(double delta)
    {
        _targetRotation = _targetRotation.Lerp(Vector3.Zero, _speed * (float)delta);
        _currentRotation = _currentRotation.Lerp(_targetRotation, _snapAmount * (float)delta);
        Basis = new Basis(Quaternion.FromEuler(_currentRotation));
    }

    private void AddRecoil()
    {
        _targetRotation += new Vector3(GetRandomNumber(_recoilAmount.X, _recoilAmount.X), GetRandomNumber(-_recoilAmount.Y, _recoilAmount.Y), 
            GetRandomNumber(-_recoilAmount.Z, _recoilAmount.Z));
    }

    private float GetRandomNumber(float min, float max)
    {
        Random random = new Random();
        return random.NextSingle() * (max - min) + min;
    }
}
