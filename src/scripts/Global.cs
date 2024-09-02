using Godot;

namespace PrimeFPSGame.Scripts;


[GlobalClass]
public partial class Global : Node
{
  public static DebugPanel? DebugPanelGlobal;
  public static PlayerFpsController? PlayerFpsController;
  public static ContextComponent? UiContext;

}
