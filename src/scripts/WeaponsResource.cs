using Godot;
namespace PrimeFPSGame.Scripts;

[GlobalClass]
public partial class WeaponsResource : Resource
{
    [Export] public string? Name;
    [ExportCategory("Weapon Orientation")] 
    [Export] public Vector3 Position;
    [Export] public Vector3 Rotation;
    [ExportCategory("Weapon Sway")] 
    [Export] public Vector2 SwayMin = new Vector2(-20.0f, -20.0f);

    [Export] public Vector2 SwayMax = new Vector2(20.0f, 20.0f);

    [Export(PropertyHint.Range, "0,0.2,0.01")]
    public float SwaySpeedPosition = 0.07f;

    [Export(PropertyHint.Range, "0,0.2,0.01")]
    public float SwaySpeedRotation = 0.1f;

    [Export(PropertyHint.Range, "0,0.25,0.01")]
    public float SwayAmountPosition = 0.1f;

    [Export(PropertyHint.Range, "0,50,0.01")]
    public float SwayAmountRotation = 30.0f;

    [ExportCategory("Visual Settings")] 
    [Export] public Mesh? Mesh;
    [Export] public bool Shadow;


}
