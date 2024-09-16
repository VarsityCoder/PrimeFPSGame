using Godot;
using Godot.Collections;

namespace PrimeFPSGame.Scripts.Portal;

public partial class InvisibleTrigger : Area3D
{
    [Export] private bool _activate = true;
    [Export] private Array<Node> activables = new Array<Node>();
    

    private void OnBodyEntered(Node3D body)
    {
        if (_activate && body.IsInGroup("player"))
        {
            foreach (var activable in activables)
            {
                if (true)
                {
                    
                }
                
            }
            
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (!_activate && body.IsInGroup("player"))
        {
            foreach (var activable in activables)
            {
                if (true)
                {
                  
                }
            }
        }
    }
}
