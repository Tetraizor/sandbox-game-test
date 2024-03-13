using System;
using System.ComponentModel.DataAnnotations;
using Godot;

namespace Tetraizor.ProceduralGeneration;

public partial class WorldGenerator : Node2D
{
    // Generator State
    private int _currentGenStep = 0;

    // Temporary Map Data
    private Color[,] _map;

    [ExportGroup("Main Parameters")]
    [Export] private int _seed = 0;
    [Export] private Vector2I _mapSize = new Vector2I(256, 64);

    [ExportGroup("Sea Level Outline")]
    [Export] private int _seaLevel = 48;
    [Export] private int _turbulenceRange = 4;

    // Other References
    private Font _debugDrawFont = ResourceLoader.Load<Font>("res://res/fonts/PixelArtFont.ttf");
    private Random _mainRandomGenerator;

    public override void _Ready()
    {
        _map = new Color[_mapSize.X, _mapSize.Y];
        _mainRandomGenerator = new Random(_seed);
    }

    public void GenerateNextStep()
    {
        switch (_currentGenStep)
        {
            case 0:
                GenerateGradientLayer();
                break;
            case 1:
                ApplySurfaceCutout();
                break;
            case 2:
                ApplySurfaceTurbulence();
                break;
            case 3:
                GenerateTestLayer();
                break;
            default:
                GD.Print("Generation complete!");
                break;
        }

        _currentGenStep++;
    }

    private void GenerateGradientLayer()
    {
        GD.Print("Generating gradient layer...");

        for (int y = 0; y < _mapSize.Y; y++)
        {
            for (int x = 0; x < _mapSize.X; x++)
            {
                float value = (float)y / _mapSize.Y;
                _map[x, y] = new Color(value, value, value, 1);
            }
        }

        QueueRedraw();
    }

    private void ApplySurfaceCutout()
    {
        GD.Print("Applying surface cutout...");

        float threshold = (_mapSize.Y - _seaLevel) / (float)_mapSize.Y;

        for (int y = 0; y < _mapSize.Y; y++)
        {
            for (int x = 0; x < _mapSize.X; x++)
            {
                _map[x, y] = _map[x, y].R > threshold ? Colors.Black : Colors.White;

            }
        }

        QueueRedraw();
    }

    private void ApplySurfaceTurbulence()
    {
        GD.Print("Applying turbulence...");

        Color[,] tempMap = (Color[,])_map.Clone();

        for (int x = 0; x < _mapSize.X; x++)
        {
            for (int y = _turbulenceRange; y < _mapSize.Y - _turbulenceRange; y++)
            {
                tempMap[x, y] = (_map[x, y + _mainRandomGenerator.Next(-_turbulenceRange, _turbulenceRange + 1)].R == 0) ? Colors.Black : Colors.White;
            }
        }

        _map = (Color[,])tempMap.Clone();

        QueueRedraw();
    }

    private void ApplySimplexNoise()
    {
        GD.Print("Applying simplex noise...");

        SimplexNoise.Noise.Seed = _seed;
        SimplexNoise.Noise.CalcPixel1D(-_mapSize.X, 0.1f);

        Color[,] tempMap = (Color[,])_map.Clone();

        for (int x = 0; x < _mapSize.X; x++)
        {
            for (int y = _turbulenceRange; y < _mapSize.Y - _turbulenceRange; y++)
            {
                tempMap[x, y] = (_map[x, y + _mainRandomGenerator.Next(-_turbulenceRange, _turbulenceRange + 1)].R == 0) ? Colors.Black : Colors.White;
            }
        }

        _map = (Color[,])tempMap.Clone();

        QueueRedraw();
    }

    private void GenerateTestLayer()
    {
        GD.Print("Generating test layer...");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Enter)
            {
                GenerateNextStep();
            }
        }
    }

    public override void _Draw()
    {
        base._Draw();
        GD.Print("Initiate redraw...");

        for (int y = 0; y < _mapSize.Y; y++)
        {
            for (int x = 0; x < _mapSize.X; x++)
            {
                DrawRect(new Rect2(x * 32, y * 32, 32, 32), _map[x, y], true);
                // DrawString(_debugDrawFont, new Vector2(x * 32, (y + 1) * 32), x + ", " + y, HorizontalAlignment.Left, -1, 16, Colors.Black);
            }
        }
    }
}