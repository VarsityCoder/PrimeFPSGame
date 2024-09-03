using Godot;
namespace PrimeFPSGame.Scripts;

public partial class MessageBus : Node
{
 //TODO implement this in the future not working right now
 [Signal]
 public delegate void InteractionFocusedEventHandler();

 [Signal]
 public delegate void InteractionUnfocusedEventHandler();

}
