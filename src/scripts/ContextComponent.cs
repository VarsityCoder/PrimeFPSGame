using Godot;
namespace PrimeFPSGame.Scripts;

public partial class ContextComponent : CenterContainer
{
	[Export] private TextureRect? _icon;
	[Export] private Label? _context;
	[Export] private Texture2D? _defaultIcon;

	private MessageBus _messageBus = new MessageBus(); 
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Global.UiContext = this;
		//TODO get messagebus working
		// _messageBus.Connect("InteractionFocused", new Callable(this, "Update"));
		// _messageBus.Connect("InteractionUnfocused", new Callable(this, "Reset"));
		Reset();
	}
	public void Reset()
	{
		if (_icon != null && _context != null)
		{
			_icon.Texture = null;
			_context.Text = "";
		}

	}

	public void UpdateIcon(Texture2D image, bool isOverride)
	{
		if (_icon != null)
		{
			_icon.Texture = isOverride ? image : _defaultIcon;
		}
	
	}
	
	public void UpdateContent(string text)
	{
		if (_context != null)
		{
			_context.Text = text;
		}
	
	}

	public void Update(string text, Texture2D image, bool isOverride)
	{
		if (_context != null)
		{
			_context.Text = text;
		}
		if (_icon != null)
		{
			if (isOverride)
			{
				_icon.Texture = image;
			}
			else
			{
				_icon.Texture = _defaultIcon;
			}
		}
	}
}
