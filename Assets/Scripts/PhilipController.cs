using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhilipController : MonoBehaviour
{
    private const float _groundCheckRadius = 0.2f;

    private Rigidbody2D Body;
    private Animator Animator;
    private Controls Controls;
    private bool _allowMovement = true;
    private bool _allowJump = true;
    private bool _allowCrouch = true;
    private bool _allowGlide = true;
    private bool _allowWallJump = true;
    private bool _sendAnimationInfo = true;
    private float _horizontalAxis = 0;
    private bool _grounded = false;
    private bool _crouching = false;
    private bool _swimming = false;
    private bool _latchedOnRight = false;
    private bool _latchedOnLeft = false;
    private bool _wallJumping = false;
    private bool _gliding = false;
    private float _glideDirection = 0;
    private bool _jumpHadRunningStart = false;

    public Transform FloorCheck;
    public Transform CielingCheck;
    public Transform RightWallCheck;
    public Transform LeftWallCheck;
    public LayerMask GroundLayers;
    public float MaxRunSpeed = 40;
    public float CrouchSpeedPercentage = 0.36f;
    public float SwimSpeedPercentage = 0.2f;
    public float JumpForce = 750;
    public float CrouchJumpForcePercentage = 0.5f;
    public bool AllowAirControl = true;
    public float UnderwaterDrag = 4;
    public float NormalDrag = 1;
    public float WallJumpForce = 500;
    public float GlideSpeed = 60;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        if (Animator == null)
        {
            Debug.LogError("Can't find an Animator component.  Animations won't work.");
            _sendAnimationInfo = false;
        }

        if (Body == null)
        {
            Debug.LogError("Can't find a Rigidbody2D on this component.  Movement won't work at all.");
            _allowMovement = false;
        }

        if (FloorCheck == null)
        {
            Debug.LogError("FloorCheck is null.  Jumping won't work.");
            _allowJump = false;
        }

        if (CielingCheck == null)
        {
            Debug.LogError("CielingCheck is null.  Crouching won't work.");
            _allowCrouch = false;
        }

        if (LeftWallCheck == null || RightWallCheck == null)
        {
            Debug.LogError("Either one of the Left or Right Wall Checks are null.  Gliding and Wall Jumping won't work.");
            _allowGlide = false;
            _allowWallJump = false;
        }

        Controls = new Controls();

        Controls.PlayerControls.Movement.performed += ctx =>
        {
            if (_allowMovement)
            {
                _horizontalAxis = ctx.ReadValue<Vector2>().x;
                Debug.Log(_horizontalAxis);
            }
        };

        Controls.PlayerControls.RunGlide.performed += ctx =>
        {
            if (_grounded)
            {
                SetRunState(ctx.ReadValueAsButton());
            }
            else
            {
                SetGlideState(ctx.ReadValueAsButton());
            }
        };

        Controls.PlayerControls.Jump.performed += ctx =>
        {
            if (_allowWallJump && (_latchedOnLeft || _latchedOnRight))
            {
                this.WallJump();
            }
            else
            {
                this.Jump(JumpForce);
            }
        };
    }

    private void SetRunState(bool runButtonDown)
    {
        // TODO
    }

    private void SetGlideState(bool glideButtonDown)
    {
        // Only update glide state if glide is possible
        if (_allowGlide)
        {
            if (!_gliding && glideButtonDown)
            {
                // Glides can only be performed if Philip had a running start
                // to his jump.
                if (_jumpHadRunningStart)
                {
                    // Glide can't be started if we are on land or swimming.
                    if (!_grounded && !_swimming)
                    {
                        // Can't initiate a glide when latched to a wall.
                        if (!_latchedOnLeft && !_latchedOnRight)
                        {
                            // We're gliding now.
                            _gliding = true;

                            // Direction of the glide depends on what the horizontal velocity
                            // of the player is.
                            if (Body.velocity.x >= 0.2f)
                            {
                                _glideDirection = 1;
                            }
                            else if (Body.velocity.x < -0.2f)
                            {
                                _glideDirection = -1;
                            }

                            // Disable Unity's gravity
                            Body.bodyType = RigidbodyType2D.Kinematic;

                            // Update animation state
                            if (_sendAnimationInfo)
                            {
                                Animator.SetBool("Gliding", true);
                            }
                        }
                    }
                }
            }
            else if (_gliding && !glideButtonDown)
            {
                _gliding = false;
                _jumpHadRunningStart = false;
                _glideDirection = 0;

                // Let Unity's physics kick in
                Body.bodyType = RigidbodyType2D.Dynamic;

                // Update animation state
                if (_sendAnimationInfo)
                {
                    Animator.SetBool("Gliding", false);
                }

            }
        }
    }

    private void WallJump()
    {
        // Walljumps can only occur if Philip had a running start going into
        // the jump.
        if (_jumpHadRunningStart)
        {
            if (_latchedOnRight)
            {
                // Wall jump to the left.
                this.Body.AddForce(new Vector2(
                        -WallJumpForce,
                        WallJumpForce
                    ));

                _wallJumping = true;

                // Tell the animator we're walljumping, and tell it what direction.
                if (_sendAnimationInfo)
                {
                    Animator.SetBool("WallJumping", true);
                    Animator.SetFloat("WalkSpeed", -1);
                }
            }
            else if (_latchedOnLeft)
            {
                // Wall jump to the left.
                this.Body.AddForce(new Vector2(
                        WallJumpForce,
                        WallJumpForce
                    ));

                _wallJumping = true;

                // Tell the animator we're walljumping, and tell it what direction.
                if (_sendAnimationInfo)
                {
                    Animator.SetBool("WallJumping", true);
                    Animator.SetFloat("WalkSpeed", 1);
                }
            }
        }
    }

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }

    public void Jump(float force)
    {
        if ((_grounded || _swimming) && _allowJump)
        {
            if (_crouching)
            {
                force *= CrouchJumpForcePercentage;
            }
            else
            {
                // Determine if the jump had a running start by checking if
                // the absolute horizontal velocity of the player is above
                // 5.
                //
                // Running starts allow Philip to perform walljumps and
                // glides.
                _jumpHadRunningStart = Mathf.Abs(Body.velocity.x) >= 5;
            }

            Body.AddForce(new Vector2(0, force));
        }
    }

    private void Update()
    {
        // Reset latch state
        _latchedOnLeft = false;
        _latchedOnRight = false;

        // Can we move?
        if (_allowMovement)
        {
            // Perform the ground check if we're allowed to jump.
            // If we're not allowed to jump then we're always grounded.
            if (_allowJump)
            {
                // If we aren't touching any ground objects then this'll stay false
                _grounded = false;

                // Determine what objects we're touching in the ground layers.
                var groundCollisions = Physics2D.OverlapCircleAll(this.FloorCheck.position, _groundCheckRadius, GroundLayers);

                // Go through each collision, if one of the objects isn't us then we're grounded.
                for (int i = 0; i < groundCollisions.Length; i++)
                {
                    if (groundCollisions[i].gameObject != this.gameObject)
                    {
                        _grounded = true;

                        // End a glide once we've landed
                        if (_gliding)
                        {
                            SetGlideState(false);
                        }

                        // Disable walljumping mode if we're in it.
                        if (_wallJumping)
                        {
                            _wallJumping = false;

                            // Tell the animator we're no longer wall-jumping
                            if (_sendAnimationInfo)
                            {
                                Animator.SetBool("WallJumping", false);
                            }
                        }
                    }
                }
            }
            else
            {
                _grounded = true;
            }

            // Set linear drag based on whether we are swimming or not
            if (_swimming)
            {
                Body.drag = UnderwaterDrag;
            }
            else
            {
                Body.drag = NormalDrag;

                // Wall jump latch
                if (!_grounded && _allowWallJump)
                {
                    // Check for right wall
                    var rightWallColliders = Physics2D.OverlapCircleAll(this.RightWallCheck.position, _groundCheckRadius, GroundLayers);

                    // Check for left walls.
                    var leftWallColliders = Physics2D.OverlapCircleAll(this.LeftWallCheck.position, _groundCheckRadius, GroundLayers);

                    for (int i = 0; i < leftWallColliders.Length; i++)
                    {
                        if (leftWallColliders[i].gameObject != this.gameObject)
                        {
                            // End glide if we've smacked our face on the wall :(
                            if (_gliding)
                            {
                                SetGlideState(false);
                            }

                            _latchedOnLeft = true;
                        }
                    }

                    for (int i = 0; i < rightWallColliders.Length; i++)
                    {
                        if (rightWallColliders[i].gameObject != this.gameObject)
                        {
                            // End glide if we've smacked our face on the wall :(
                            if (_gliding)
                            {
                                SetGlideState(false);
                            }

                            _latchedOnRight = true;
                        }
                    }
                }
            }
        }    
    }

    private void FixedUpdate()
    {
        // Can we move?
        if (_allowMovement)
        {
            if (_gliding)
            {
                // Calculate glide horizontal velocity
                float glideHorizontal = (GlideSpeed * _glideDirection) * Time.fixedDeltaTime;
                float glideDescent = (-UnderwaterDrag) * Time.fixedDeltaTime;

                // Set player velocity
                Body.velocity = new Vector2(glideHorizontal * 10, glideDescent * 10);
            }
            else if (_wallJumping)
            {
            }
            else
            {
                // Are we grounded?
                if (_grounded || ((AllowAirControl || _wallJumping) || _swimming))
                {
                    // Run speed.
                    float runSpeed = MaxRunSpeed;

                    // If we're crouching, then multiply by crouch speed.
                    if (_crouching)
                    {
                        runSpeed *= CrouchSpeedPercentage;
                    }

                    // Apply direction and time to the speed.
                    float direction = ((_horizontalAxis * runSpeed) * Time.fixedDeltaTime) * 10;

                    // Update the animator if we have one.
                    if (_sendAnimationInfo)
                    {
                        Animator.SetFloat("WalkSpeed", direction);
                    }

                    // Set the velocity of the rigid body.
                    Body.velocity = new Vector2(
                            direction,
                            Body.velocity.y
                        );
                }
            }
        }
    }

    public void StartSwimming()
    {
        if (_allowMovement)
        {
            // If we're gliding, IMMEDIATELY end the glide
            if (_gliding)
            {
                SetGlideState(false);
            }

            // Since water isn't considered ground, the game needs to do this
            // in order to unlock movement controls when walljumping.
            if (_wallJumping)
            {
                _wallJumping = false;

                // Tell the animator we're no longer wall-jumping
                if (_sendAnimationInfo)
                {
                    Animator.SetBool("WallJumping", false);
                }
            }
            _swimming = true;
        }
    }

    public void StopSwimming()
    {
        _swimming = false;
    }
}
