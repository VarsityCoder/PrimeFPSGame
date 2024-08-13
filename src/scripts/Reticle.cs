using Godot;


namespace PrimeFPSGame.Scripts;

using System.CodeDom.Compiler;
using System.Numerics;
using Godot.Collections;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class Reticle : CenterContainer {

  [Export] public float DotRadius = 3.0f;
  [Export] public Color DotColor = Colors.White;
  [Export] public CharacterBody3D? PlayerController;
  [Export] public float ReticleSpeed = 0.25f;
  [Export] public float ReticleDistance = 2.0f;
  [Export] public Array<Line2D>? ReticleLines;


  public override void _Ready() => QueueRedraw();

  public override void _Process(double delta) => AdjustReticleLines();

  public override void _Draw() {
    DrawCircle(Vector2.Zero, 4, Colors.DimGray);
    DrawCircle(Vector2.Zero, DotRadius, DotColor);
  }

  private void AdjustReticleLines() {
    if (PlayerController != null && ReticleLines != null) {
      var velocity = PlayerController.GetRealVelocity();
      var origin = Vector3.Zero;
      var position = Vector2.Zero;
      var speed = origin.DistanceTo(velocity);

      ReticleLines[0].Position = ReticleLines[0].Position.Lerp(position + new Vector2(0, -speed * ReticleDistance), ReticleSpeed); // Top
      ReticleLines[1].Position = ReticleLines[1].Position.Lerp(position + new Vector2(speed * ReticleDistance, 0), ReticleSpeed); // Right
      ReticleLines[2].Position = ReticleLines[2].Position.Lerp(position + new Vector2(0, speed * ReticleDistance), ReticleSpeed); // Bottom
      ReticleLines[3].Position = ReticleLines[3].Position.Lerp(position + new Vector2(-speed * ReticleDistance, 0), ReticleSpeed); // Left

    }
  }
}
