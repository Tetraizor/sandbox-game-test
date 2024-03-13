using System;
using Godot;

using Tetraizor.Data;
using Tetraizor.UI;

namespace Tetraizor.Player;
public partial class PlayerController : CharacterBody2D
{
    #region Properties
    [ExportSubgroup("Controller Parameters")]
    [Export] private float _speed = 256;
    [Export] private float _runMultiplier = 1.5f;

    [Export] private float _maxFallSpeed = 1024;
    [Export] private float _gravity = 2048;

    [Export] private float _jumpThrust = 700;

    [ExportGroup("References")]
    [ExportSubgroup("Node References")]
    [Export] private AnimationTree _animationTree;
    [Export] private Sprite2D _renderer;

    [ExportSubgroup("Particle References")]
    [Export] private GpuParticles2D _walkParticles;
    [Export] private GpuParticles2D _jumpParticles;
    [Export] private GpuParticles2D _landParticles;

    // Controls
    private float _horizontalAxis = 0;
    private bool _isJumpButtonPressed;

    // State
    private ReactiveProperty<bool> _isWalking = new ReactiveProperty<bool>(false);
    public bool IsWalking => _isWalking.Value;

    private ReactiveProperty<bool> _isRising = new ReactiveProperty<bool>(false);
    public bool IsRising => _isRising.Value;

    private ReactiveProperty<bool> _isGrounded = new ReactiveProperty<bool>(true);
    public bool IsGrounded => _isGrounded.Value;

    #endregion

    #region Godot Lifecycle Methods

    public override void _Ready()
    {
        if (_renderer == null)
            _renderer = GetNode<Sprite2D>("Sprite2D");

        _isGrounded.OnValueChanged += (oldValue, newValue) =>
        {
            _walkParticles.Emitting = newValue && _isWalking.Value;

            if (!newValue)
            {
                _jumpParticles.Restart();
                _jumpParticles.Emitting = true;
            }
            else
            {
                _landParticles.Restart();
                _landParticles.Emitting = true;
            }
        };

        _isWalking.OnValueChanged += (oldValue, newValue) =>
        {
            _walkParticles.Emitting = newValue && _isGrounded.Value;
        };
    }

    public override void _PhysicsProcess(double dDelta)
    {
        MoveAndSlide();
    }

    public override void _Process(double dDelta)
    {
        float delta = (float)dDelta;

        if (WorldGeneratorUIManager.IsInteractingWithUI)
            ResetInput();
        else
            HandleInput();

        HandleWalk(delta);
        HandleStates();
        HandleJumping();
    }
    #endregion

    #region Input Methods

    private void HandleInput()
    {
        _isJumpButtonPressed = Input.IsActionJustPressed("jump");
        _horizontalAxis = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
    }

    private void ResetInput()
    {
        _horizontalAxis = 0;
        _isJumpButtonPressed = false;
    }

    #endregion

    #region Locomotion Methods

    private void HandleJumping()
    {
        if (_isJumpButtonPressed)
        {
            if (IsOnFloor())
            {
                Velocity = new Vector2(Velocity.X, -_jumpThrust);

                var stateMachine = _animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
                stateMachine.Travel("jump", true);
            }
        }
    }

    private void HandleStates()
    {
        _isRising.Value = Velocity.Y < 0;
        _isGrounded.Value = IsOnFloor();
    }

    private void HandleWalk(float delta)
    {
        _isWalking.Value = Math.Abs(_horizontalAxis) > 0;

        Velocity = new Vector2(
            _horizontalAxis * _speed,
            IsOnFloor() ? Velocity.Y : Mathf.MoveToward(Velocity.Y, _maxFallSpeed, _gravity * delta)
        );

        if (_isWalking.Value) _renderer.FlipH = _horizontalAxis < 0;
    }

    #endregion
}
