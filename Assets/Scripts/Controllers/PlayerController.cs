using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : KinematicObject
{
    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Landed
    }

    #region Public fields
    /// <summary>
    /// Max horizontal speed of the player.
    /// </summary>
    public float maxSpeed = 7;
    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
    public float jumpTakeOffSpeed = 7;
    public Bounds Bounds => collider2d.bounds;

    public Collider2D collider2d;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    #endregion
    #region Private fields
    private JumpState jumpState = JumpState.Grounded;
    private bool stopJump;
    private bool jump;
    private Vector2 move;

    #endregion
    #region MonoBehaviour callbacks
    void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    #endregion
    #region Kinetic Overrides
    protected override void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
            jumpState = JumpState.PrepareToJump;
        else if (Input.GetButtonUp("Jump"))
            stopJump = true;

        UpdateJumpState();
        base.Update();
    }
    protected override void ComputeVelocity()
    {
        if (jump && IsGrounded)
        {
            velocity.y = jumpTakeOffSpeed * 1.5f;
            jump = false;
        }
        else if (stopJump)
        {
            stopJump = false;
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;

        animator.SetBool("grounded", IsGrounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }
    #endregion
    #region Private methods
    private void UpdateJumpState()
    {
        jump = false;
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                jumpState = JumpState.Jumping;
                jump = true;
                stopJump = false;
                break;
            case JumpState.Jumping:
                if (!IsGrounded)
                {
                    jumpState = JumpState.InFlight;
                }
                break;
            case JumpState.InFlight:
                if (IsGrounded)
                {
                    jumpState = JumpState.Landed;
                }
                break;
            case JumpState.Landed:
                jumpState = JumpState.Grounded;
                break;
        }
    }

    #endregion
}
