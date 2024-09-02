using Godot;
namespace PrimeFPSGame.Scripts;

public partial class ContextComponent : CenterContainer
{
	[Export] private TextureRect? _icon;
	[Export] private Label? _context;
	[Export] private Texture2D? _defaultIcon;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Global.UiContext = this;
		Reset();	
	}
	public void Reset()
	{
		GD.Print("In Reset function");
		if (_icon != null && _context != null)
		{
			_icon.Texture = null;
			_context.Text = "";
		}

	}

	public void UpdateIcon(Texture2D image, bool isOverride)
	{
		GD.Print("In UpdateIcon function");
		if (_icon != null)
		{
			_icon.Texture = isOverride ? image : _defaultIcon;
		}

	}

	public void UpdateContent(string text)
	{
		GD.Print("In UpdateContent function");
		if (_context != null)
		{
			_context.Text = text;
		}

	}
}
