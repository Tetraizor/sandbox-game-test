using Godot;
using System;
using Tetraizor.Player;

public partial class DebugUI : Control
{
    [ExportGroup("References")]
    [ExportSubgroup("Property References")]
    [Export] private PlayerController _player;

    [ExportSubgroup("Control References")]
    [Export] private Label _labelDebugDisplay;

    public override void _Process(double delta)
    {


        _labelDebugDisplay.Text =
                                $"Position: {_player.Position.ToString("F2")}\n" +
                                $"Speed: {_player.Velocity.ToString("F2")}\n" +
                                $"IsWalking: {_player.IsWalking}\n" +
                                $"IsRising: {_player.IsRising}\n" +
                                $"IsGrounded: {_player.IsGrounded}";
    }
}
