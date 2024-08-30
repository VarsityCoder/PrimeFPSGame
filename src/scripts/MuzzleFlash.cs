using Godot;

namespace PrimeFPSGame.Scripts;



public partial class MuzzleFlash : Node3D
{
	[Export] private InitialWeapon? _weaponController;
	[Export] private float _flashTime = 0.05f;
	private OmniLight3D? _light3D;
	private GpuParticles3D? _gpuParticles;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_light3D = GetNode<OmniLight3D>("OmniLight3D");
		_gpuParticles = GetNode<GpuParticles3D>("GPUParticles3D");

	}

	private void AddMuzzleFlash()
	{
		if (_light3D != null && _gpuParticles != null)
		{
			_light3D.Visible = true;
			_gpuParticles.Emitting = true;
			GetTree().CreateTimer(_flashTime).Timeout += () => _light3D.Visible = false;
		}
	}
}
