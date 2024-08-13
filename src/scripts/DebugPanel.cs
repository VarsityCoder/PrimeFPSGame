using Godot;

namespace PrimeFPSGame.Scripts;

using System;
using System.Globalization;

public partial class DebugPanel : PanelContainer {

  private VBoxContainer _propertyContainer = new VBoxContainer();

  private string _framesPerSecond = "";

  private Label? _property;


  public override void _Ready() {
    Visible = false;
    _propertyContainer = GetNode<VBoxContainer>("MarginContainer/VBoxContainer");

    Global.DebugPanelGlobal = this;
    AddDebugProperty("FPS: ", _framesPerSecond);
  }

  public override void _Input(InputEvent @event) {
    if (@event.IsActionPressed("debug")) {
      Visible = !Visible;
    }
  }

  public override void _Process(double delta) {
    if (Visible) {
      _framesPerSecond = Math.Round(1.0 / delta, 2).ToString(CultureInfo.InvariantCulture);

      if (_property != null) {
        _property.Text = _property.Name + ": " + _framesPerSecond;
      }
    }
  }

  private void AddDebugProperty(string title, string value) {
    _property = new Label();
    _propertyContainer.AddChild(_property);
    _property.Name = title;
    _property.Text = _property.Name + value;
  }

  public void AddProperty(string title, string value, int order) {
    var target = (Label)_propertyContainer.FindChild(title, true, false);
    if (target == null) {
      target = new Label();
      _propertyContainer.AddChild(target);
      target.Name = title;
      target.Text = title + ": " + value;
    } else if (Visible) {
        target.Text = title + ": " + value;
        _propertyContainer.MoveChild(target, order);
    }
  }
}
