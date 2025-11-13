using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpPower = 19f;
    //Wall Climb Settings
    public float climbSpeed = 5f;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;
    //Wall Climb Audio
    public AudioClip wallClimbClip;
    public AudioSource wallClimbSource;
    //walljump Audio
    public AudioClip walljumpclip;
    private AudioSource walljumpsource;
    //Wall Jump Settings
    public float wallJumpForce = 15f;
    public Vector2 wallJumpDirection = new Vector2(1, 1);
    public float wallJumpDuration = 0.2f;
    //Footstep Audio
    public AudioClip footstepClip;
    private AudioSource footstepSource;
    private Rigidbody2D rb;
    private Animator anim;

    private int direction = 1;
    private bool isJumping = false;
    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool isTouchingWall = false;
    private bool isWallJumping = false;
    private float wallJumpTimer;
    private float coyoteTimer;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimer;
    private float dashCooldownTimer;
    //layercheck
    public LayerMask groundLayer;
    


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.clip = footstepClip;
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        footstepSource.volume = 0.5f;

        wallClimbSource = gameObject.AddComponent<AudioSource>();
        wallClimbSource.clip = wallClimbClip;
        wallClimbSource.loop = true;
        wallClimbSource.playOnAwake = false;
        wallClimbSource.volume = 0.5f;

        walljumpsource = gameObject.AddComponent<AudioSource>();
        walljumpsource.clip = walljumpclip;
        walljumpsource.playOnAwake=false;
        walljumpsource.volume = 0.7f;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleWallClimb();
        HandleFootstepSound();
        UpdateAnimations();
        CheckWall();
        WallSlide();

        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
        }
        if (!isDashing && dashCooldownTimer <= 0f && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(PerformDash());
        }
        dashCooldownTimer -= Time.deltaTime;
    }
    IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        Vector2 dashDirection = new Vector2(direction, 0).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        anim.SetTrigger("Dash"); // Optional animation trigger

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & wallLayer) != 0)
            isTouchingWall = true;

        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            isTouchingWall = false;
            isGrounded = false;
        }
    }

    void CheckWall()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction * 0.5f, 0);
        Vector2 directionVec = Vector2.right * direction;

        RaycastHit2D wallHit = Physics2D.Raycast(origin, directionVec, wallCheckDistance, wallLayer);
        isTouchingWall = wallHit.collider != null;

        Debug.DrawRay(origin, directionVec * wallCheckDistance, Color.red);
    }

    void HandleMovement()
    {
        if (isClimbing || isWallJumping) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector3 moveVelocity = Vector3.zero;

        anim.SetBool("isRun", false);

        if (horizontalInput != 0)
        {
            direction = horizontalInput < 0 ? -1 : 1;
            moveVelocity = new Vector3(direction, 0, 0);
            transform.localScale = new Vector3(direction, 1, 1);

            anim.SetBool("isRun", true);
        }

        transform.position += moveVelocity * moveSpeed * Time.deltaTime;
        if (isDashing) return;
    }

    void HandleJump()
    {
        if (isClimbing || isWallJumping) return;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                isJumping = true;
            }
            else if (isTouchingWall)
            {
                WallJump();
                return;
            }
        }

        if (!isJumping) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isJumping = false;
    }

    void WallJump()
    {
        isWallJumping = true;
        wallJumpTimer = wallJumpDuration;

        Vector2 jumpDir = new Vector2(-direction * wallJumpDirection.x, wallJumpDirection.y).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(jumpDir * wallJumpForce, ForceMode2D.Impulse);

        transform.localScale = new Vector3(-direction, 1, 1);
        if (walljumpsource && wallClimbSource)
        {
            walljumpsource.Play(); //wall jump sound use case
        }
    }

    void WallSlide()
    {
        if (!isTouchingWall || isGrounded || isClimbing) return;
        //slowly down wall slide only
        float slideSpeed = -2f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, slideSpeed);
    }

    void HandleWallClimb()
    {
        if (!isTouchingWall || isWallJumping)
        {
            StopClimbing();
            return;
        }
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput == direction)
        {
            if (verticalInput != 0)
            {
                StartClimbing(verticalInput * climbSpeed);
            }
            else
            {
                StopClimbing();
                WallSlide();
            }
        }
        else
        {
            StopClimbing();
            WallSlide();
        }
    }
    void StartClimbing(float verticalSpeed)
    {
        if (!isClimbing)
            isClimbing = true;

        anim.SetBool("isClimbing", true);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalSpeed);
        rb.gravityScale = 0;

        if (!wallClimbSource.isPlaying)
        {
            wallClimbSource.Play();
        }
    }
    void StopClimbing()
    {
        if (isClimbing)
            isClimbing = false;

        anim.SetBool("isClimbing", false);
        rb.gravityScale = 5;

        if (wallClimbSource.isPlaying)
        {
            wallClimbSource.Stop();
        }
    }
    void HandleFootstepSound()
    {
        bool isRunning = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && isGrounded && !isClimbing;

        if (isRunning)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    void UpdateAnimations()
    {
        if (isClimbing)
        {
            anim.SetBool("isClimbing", true);
            anim.SetBool("isRun", false);
            anim.SetBool("IsRun", false);
            anim.SetFloat("VerticalSpeed", 0);
            return;
        }
        bool groundedForAnim = coyoteTimer > 0f;
        
        anim.SetBool("isClimbing", false);
        anim.SetBool("IsRun", Mathf.Abs(rb.linearVelocity.x) > 0.01f && groundedForAnim);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }
}
