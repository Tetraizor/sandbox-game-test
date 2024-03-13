using Godot;

namespace Tetraizor.UI;

public partial class WorldGeneratorUIManager : Control
{
    [ExportSubgroup("Main Parameters")]
    [Export] private bool _startFromCollapsed = true;

    [ExportSubgroup("UI References")]
    [Export] private Button _buttonCollapseHeader;
    [Export] private TextureRect _iconCollapsedButton;
    [Export] private ControlFocusCallbackProvider _panelClickCallbackProvider;

    [Export] private Control _panelCollapsible;
    private Vector2 _defaultPanelSize;

    // States
    private static bool _isInteractingWithUI = false;
    public static bool IsInteractingWithUI => _isInteractingWithUI;

    private bool _isCollapsed = false;

    // Private References
    private Control _focusedElement;
    public Control FocusedElement => _focusedElement;

    // Godot Lifecycle Methods
    public override void _Ready()
    {
        _buttonCollapseHeader.Pressed += ToggleCollapse;

        _defaultPanelSize = _panelClickCallbackProvider.Size;
        if (_startFromCollapsed)
        {
            _panelCollapsible.Size = _buttonCollapseHeader.Size;
            _isCollapsed = true;
        }

        GetViewport().GuiFocusChanged += (focusedElement) =>
        {
            _focusedElement = focusedElement;
            _isInteractingWithUI = focusedElement != null;
        };

        _panelClickCallbackProvider.ClickedOutside += () =>
        {
            if (_focusedElement != null)
            {
                _focusedElement.ReleaseFocus();
            }

            _isInteractingWithUI = false;
        };

        _panelClickCallbackProvider.ClickedInside += () =>
        {
            _isInteractingWithUI = true;
        };
    }

    // UI Controls
    public void ToggleCollapse()
    {
        _isCollapsed = !_isCollapsed;

        if (_isCollapsed)
        {
            _iconCollapsedButton.FlipV = true;

            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(_panelCollapsible, "size", _buttonCollapseHeader.Size, .2f);
            tween.Play();

            GD.Print("Start from " + _panelCollapsible.Size + " to " + _buttonCollapseHeader.Size);
        }
        else
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(_panelCollapsible, "size", _defaultPanelSize, .2f);
            tween.Play();
            _iconCollapsedButton.FlipV = false;

            GD.Print("Start from " + _panelCollapsible.Size + " to " + _defaultPanelSize);
        }
    }
}
