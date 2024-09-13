using Godot;
using Godot.Collections;

namespace PrimeFPSGame.Scripts.Portal;

public partial class InvisibleTrigger : Area3D
{
    [Export] private bool _activate = true;
    [Export] private Array<Node> activables = new Array<Node>();
    
}
