using Godot;
using System;
namespace PrimeFPSGame.Scripts;

public partial class SprintingPlayerState : State {

  [Export] public AnimationPlayer? AnimationPlayer;
  [Export] public float TopAnimationSpeed = 1.6f;

  public override void Enter() {
    AnimationPlayer?.Play("Sprinting", 0.5f, 1.0f);
    Global.PlayerFpsController._startingSpeed = Global.PlayerFpsController.SpeedSprinting;
  }

  public override void Update(float delta) => SetAnimationSpeed(Global.PlayerFpsController.Velocity.Length());

  private void SetAnimationSpeed(float speed) {
    var alpha = Mathf.Remap(speed, 0.0, Global.PlayerFpsController.SpeedSprinting, 0.0, 1.0);
    if (AnimationPlayer != null) {
      AnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TopAnimationSpeed, (float)alpha);
    }
  }
  public override void _Input(InputEvent @event) {
    if (@event.IsActionReleased("sprint")) {
      EmitSignal(State.SignalName.Transition, "WalkingPlayerState");
    }
  }
}
