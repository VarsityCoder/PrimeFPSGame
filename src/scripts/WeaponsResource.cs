using Godot;
namespace PrimeFPSGame.Scripts;

[GlobalClass]
public partial class WeaponsResource : Resource
{
    [Export] protected internal string? Name;
    [ExportCategory("Weapon Orientation")] 
    [Export] protected internal Vector3 Position;
    [Export] protected internal Vector3 Rotation;
    [Export] protected internal Vector3 Scale;
    [ExportCategory("Weapon Sway")] 
    [Export] protected internal Vector2 SwayMin = new Vector2(-20.0f, -20.0f);

    [Export] protected internal Vector2 SwayMax = new Vector2(20.0f, 20.0f);

    [Export(PropertyHint.Range, "0,0.2,0.01")]
    protected internal float SwaySpeedPosition = 0.07f;

    [Export(PropertyHint.Range, "0,0.2,0.01")]
    protected internal float SwaySpeedRotation = 0.1f;

    [Export(PropertyHint.Range, "0,0.25,0.01")]
    protected internal float SwayAmountPosition = 0.1f;

    [Export(PropertyHint.Range, "0,50,0.01")]
    protected internal float SwayAmountRotation = 30.0f;

    [Export] protected internal float IdleSwayAdjustment = 10.0f;
    [Export] protected internal float IdleSwayRotationStrength = 30.0f;

    [ExportCategory("Visual Settings")] 
    [Export] protected internal Mesh? Mesh;
    [Export] protected internal bool Shadow;


}
