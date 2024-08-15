using Godot;
using System;
using PrimeFPSGame.Scripts;

public partial class CrouchingPlayerState : PlayerMovementState
{
    [Export] public AnimationPlayer? AnimationPlayer;
    [Export] public float TopAnimationSpeed = 2.2f;
    [Export] public float SpeedDefault = 5.0f;
    [Export] public float Acceleration = 0.1f;
    [Export] public float Deceleration = 0.25f;
}
