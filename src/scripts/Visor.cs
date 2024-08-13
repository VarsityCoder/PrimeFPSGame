using Godot;
namespace PrimeFPSGame.Scripts;

public partial class Visor : MeshInstance3D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

  public void killParent() {
    GetParent<CharacterBody3D>().QueueFree();
  }
}
