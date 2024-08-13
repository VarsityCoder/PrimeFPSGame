using Godot;

namespace PrimeFPSGame.Scripts;

public partial class Crosshair : Control
{
  public override void _Draw() {
    DrawCircle(Vector2.Zero, 4, Colors.DimGray);
    DrawCircle(Vector2.Zero, 3, Colors.White);

    DrawLine(new Vector2(16,0), new Vector2(24,0), Colors.White, 2);
    DrawLine(new Vector2(-16,0), new Vector2(-24,0), Colors.White, 2);
    DrawLine(new Vector2(0,16), new Vector2(0,24), Colors.White, 2);
    DrawLine(new Vector2(0,-16), new Vector2(0,-24), Colors.White, 2);
  }
}
