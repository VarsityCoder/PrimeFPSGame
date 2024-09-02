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
	private void Reset()
	{
		if (_icon != null && _context != null)
		{
			_icon.Texture = null;
			_context.Text = "";
		}

	}

	private void _updateIcon(Texture2D image, bool isOverride)
	{
		if (_icon != null)
		{
			_icon.Texture = isOverride ? image : _defaultIcon;
		}

	}

	private void UpdateContext(string text)
	{
		if (_context != null)
		{
			_context.Text = text;
		}

	}
}
