using Godot;

namespace PrimeFPSGame.Scripts.Portal;

public partial class Portal : Area3D
{
    //private static bool _teleporting = false;
    [Export] private int _verticalViewportResolution = 512;
    private int id = -1;
    private Tween? _tween;


    public override void _Ready()
    {
        SetupViewportSize();
    }

    private void SetupViewportSize()
    {
        var viewportSize = GetViewport().GetWindow().Size;
        var aspectRatio = (float)viewportSize.X  / viewportSize.Y;
    }

    private void OnBodyEntered(Node3D body)
    {
        
    }

    private void OnBodyExited(Node3D body)
    {
        
    }
}
