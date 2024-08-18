using Godot;
namespace PrimeFPSGame.Scripts;

[GlobalClass]
public partial class WeaponsResource : Resource
{
    [Export] public string? Name;
    [ExportCategory("Weapon Orientation")] 
    [Export] public Vector3 Position;
    [Export] public Vector3 Rotation;
    [ExportCategory("Visual Settings")] 
    [Export] public Mesh? Mesh;
    [Export] public bool Shadow;
}
