using Godot;

namespace Tetraizor.UI;

public partial class WorldGeneratorUIManager : Control
{

    [ExportSubgroup("UI References")]
    [Export] private Button _collapseButton;
    [Export] private TextureRect _collapsedButtonIcon;
    [Export] private ControlFocusCallbackProvider _uiClickManager;

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
        _collapseButton.Pressed += ToggleCollapse;

        GetViewport().GuiFocusChanged += (focusedElement) =>
        {
            _focusedElement = focusedElement;
            _isInteractingWithUI = focusedElement != null;
        };

        _uiClickManager.ClickedOutside += () =>
        {
            if (_focusedElement != null)
            {
                _focusedElement.ReleaseFocus();
            }

            _isInteractingWithUI = false;
        };

        _uiClickManager.ClickedInside += () =>
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
            _collapsedButtonIcon.FlipV = true;
        }
        else
        {
            _collapsedButtonIcon.FlipV = false;
        }
    }
}
