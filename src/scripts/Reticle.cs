using Godot;


namespace PrimeFPSGame.Scripts;

using System.CodeDom.Compiler;
using System.Numerics;
using Godot.Collections;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class Reticle : CenterContainer {

  [Export] private float _dotRadius = 3.0f;
  [Export] private Color _dotColor = Colors.White;
  [Export] private CharacterBody3D? _playerController;
  [Export] private float _reticleSpeed = 0.25f;
  [Export] private float _reticleDistance = 2.0f;
  [Export] private Array<Line2D>? _reticleLines;


  public override void _Ready() => QueueRedraw();

  public override void _Process(double delta) => AdjustReticleLines();

  public override void _Draw() {
    DrawCircle(Vector2.Zero, 4, Colors.DimGray);
    DrawCircle(Vector2.Zero, _dotRadius, _dotColor);
  }

  private void AdjustReticleLines() {
    if (_playerController != null && _reticleLines != null) {
      var velocity = _playerController.GetRealVelocity();
      var origin = Vector3.Zero;
      var position = Vector2.Zero;
      var speed = origin.DistanceTo(velocity);

      _reticleLines[0].Position = _reticleLines[0].Position.Lerp(position + new Vector2(0, -speed * _reticleDistance), _reticleSpeed); // Top
      _reticleLines[1].Position = _reticleLines[1].Position.Lerp(position + new Vector2(speed * _reticleDistance, 0), _reticleSpeed); // Right
      _reticleLines[2].Position = _reticleLines[2].Position.Lerp(position + new Vector2(0, speed * _reticleDistance), _reticleSpeed); // Bottom
      _reticleLines[3].Position = _reticleLines[3].Position.Lerp(position + new Vector2(-speed * _reticleDistance, 0), _reticleSpeed); // Left

    }
  }
}
