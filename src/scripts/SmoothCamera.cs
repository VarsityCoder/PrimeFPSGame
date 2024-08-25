using System;
using Godot;

namespace PrimeFPSGame.Scripts;
public partial class SmoothCamera : Camera3D {
  [Export] private float _speed = 44.0f;

  public override void _PhysicsProcess(double delta) {
    var weight = Math.Clamp(delta * _speed, 0.0, 1.0);
    GlobalTransform = GlobalTransform.InterpolateWith(GetParent<Node3D>().GlobalTransform, (float)weight);
    GlobalPosition = GetParent<Node3D>().GlobalPosition;
  }
}
