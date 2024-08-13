using Godot;

namespace PrimeFPSGame.Scripts;


[GlobalClass]
public partial class Global : Node {
  public static DebugPanel DebugPanelGlobal = new DebugPanel();
  public static PlayerFpsController PlayerFpsController = new PlayerFpsController();

}
