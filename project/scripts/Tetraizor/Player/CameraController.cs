using Godot;
using System;

namespace Tetraizor.Player;

public partial class CameraController : Camera2D
{
    [Export] private Node2D _target;

    [Export] private float _zoomIncreaseFactor = 20.0f;

    [Export] private float _maxZoom = 4f;
    [Export] private float _minZoom = .1f;

    public override void _PhysicsProcess(double delta)
    {
        Position = _target.Position;
    }

    public override void _Process(double dDelta)
    {
        if (Input.IsActionJustReleased("zoom_in")) ChangeZoom(true);
        if (Input.IsActionJustReleased("zoom_out")) ChangeZoom(false);
    }

    private void ChangeZoom(bool increase)
    {
        // Makes the zoom value normalized between 0 and 1 respected to _minZoom and _maxZoom.
        float normalizedZoom = Math.Clamp((Zoom.X - _minZoom) / (_maxZoom - _minZoom), 0, 1);

        // Processes it with a logarithmic function to make it more sensitive at the start and less sensitive at the end.
        // More _zoomIncreaseFactor, more the curve is closer to a linear one. 
        float sensitivity = Math.Clamp((float)Math.Log((normalizedZoom * (_zoomIncreaseFactor - 1)) + 1, _zoomIncreaseFactor), 0, 1);

        float zoomSensitivity = (increase ? 1 : -1) * (sensitivity + 5);

        Zoom = Math.Clamp(Zoom.X + (zoomSensitivity * (float)GetProcessDeltaTime()), _minZoom, _maxZoom) * Vector2.One;
    }
}
