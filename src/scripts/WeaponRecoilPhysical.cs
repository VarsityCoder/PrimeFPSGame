using Godot;

namespace PrimeFPSGame.Scripts;

public partial class WeaponRecoilPhysical : Node3D
{

	[Export] private Vector3 _recoilAmount;
	[Export] private float _snapAmount;
	[Export] private float _speed;
	[Export] private InitialWeapon? _weaponController;

	private Vector3 _currentPosition;
	private Vector3 _targetPosition;
	
	public override void _Ready()
	{
		
	}
	public override void _Process(double delta)
	{
		_targetPosition = _targetPosition.Lerp(Vector3.Zero, _speed * (float)delta);
		_currentPosition = _currentPosition.Lerp(_targetPosition, _snapAmount * (float)delta);
		Position = _currentPosition;
	}

	private void AddRecoil()
	{
		Recoil recoil = new Recoil();
		_targetPosition += new Vector3(recoil.GetRandomNumber(_recoilAmount.X, _recoilAmount.X * 2), 
			recoil.GetRandomNumber(_recoilAmount.Y, _recoilAmount.Y * 2), recoil.GetRandomNumber(-_recoilAmount.Z, _recoilAmount.Z));
	}
}
