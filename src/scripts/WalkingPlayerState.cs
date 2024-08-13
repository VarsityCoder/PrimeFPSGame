namespace PrimeFPSGame.Scripts;

using Godot;

public partial class WalkingPlayerState : State {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float TopAnimationSpeed = 2.2f;

  public override void Enter() {
    AnimationPlayer?.Play("Walking", -1.0f, 1.0f);
    Global.PlayerFpsController._startingSpeed = Global.PlayerFpsController.SpeedDefault;
  }

  public override void Update(float delta) {
    SetAnimationSpeed(Global.PlayerFpsController.Velocity.Length());
    if (Global.PlayerFpsController.Velocity.Length() == 0.0f) {
      EmitSignal(State.SignalName.Transition, "IdlePlayerState");
    }
  }

  private void SetAnimationSpeed(float speed) {
    var alpha = Mathf.Remap(speed, 0.0, Global.PlayerFpsController.SpeedDefault, 0.0, 1.0);
    if (AnimationPlayer != null) {
      AnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TopAnimationSpeed, (float)alpha);
    }
  }

  public override void _Input(InputEvent @event) {
    if (@event.IsActionPressed("sprint") && Global.PlayerFpsController.IsOnFloor()) {
      EmitSignal(State.SignalName.Transition, "SprintingPlayerState");
    }
  }

}
