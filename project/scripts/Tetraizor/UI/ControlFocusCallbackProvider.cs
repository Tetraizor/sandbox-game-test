using Godot;

namespace Tetraizor.UI;

public partial class ControlFocusCallbackProvider : Control
{
    [Signal] public delegate void ClickedOutsideEventHandler();
    [Signal] public delegate void ClickedInsideEventHandler();

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
            if (mouseButtonEvent.Pressed)
                EmitSignal(SignalName.ClickedInside);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
            if (mouseButtonEvent.Pressed)
                EmitSignal(SignalName.ClickedOutside);
    }
}
