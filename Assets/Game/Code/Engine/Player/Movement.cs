using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Jape;
using UnityEngine;

using Input = Jape.Input;
using Math = Jape.Math;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Movement : Element
{
    public const float MoveSpeedRatio = 1;
    public const float MaxFallSpeed = 40;

    private const float JumpCooldownTime = 0.1f;
    private const float JumpTrajectoryTime = 1.2f;

    private const float JumpCrouchPenalty = 0.5f;
    private const float JumpAirPenalty = 0.66f;

    private const float GroundingProtectionTime = 0.15f;
    private const float UngroundingTime = 0.15f;

    public const float Friction = 0.4f;

    public float moveSpeed;
    public float jumpSpeed;
    public int jumpLimit = 1;

    [HideInInspector] 
    public int JumpsRemaining;

    public int dodgeLimit = 1;

    [HideInInspector] 
    public int DodgeRemaining;

    private float inputHorizontal;
    private float inputVertical;
    private bool? inputJump;
    private bool? inputDodge;

    [Space(16)]

    public Input input;

    [Space(16)]

    [SerializeField]
    private ContactFilter2D MovementFilter = default;

    [Space(16)]

    [SerializeField] 
    private bool DebugMovement = false;

    private new Player Player;

    private BoxCollider2D Collider;
    private Vector2 ColliderSize;
    private Vector2 ColliderOffset;
    private Bounds ColliderBounds;

    private Vector2 previousPosition;

    public State Moving { get; } = new State();
    public State Jumping { get; } = new State();
    public State Turning { get; } = new State();
    public State Crouching { get; } = new State();
    public State Sliding { get; } = new State();
    public State Grounded { get; } = new State();
    public State Falling { get; } = new State();
    public State Walled { get; } = new State();

    private Timer JumpCooldownTimer;
    private Timer JumpTrajectoryTimer;

    private Timer GroundingProtectionTimer;
    private Timer UngroundingTimer;

    private Activity dodge;

    private PhysicsMaterial2D physicsMaterial;

    // UPDATE //

    protected override void Init()
    {
        Player = GetComponent<Player>();

        Collider = Player.collider;
        ColliderSize = Collider.size;
        ColliderOffset = Collider.offset;
        ColliderBounds = Collider.bounds;

        JumpCooldownTimer = CreateTimer();
        JumpTrajectoryTimer = CreateTimer();
        GroundingProtectionTimer = CreateTimer();
        UngroundingTimer = CreateTimer();

        dodge = CreateActivity();

        dodge.Setup(DodgeSetup);
        dodge.Action(DodgeRoutine());
        dodge.Cleanup(DodgeCleanup);

        physicsMaterial = new PhysicsMaterial2D("Player")
        {
            friction = Friction,
        };

        Collider.sharedMaterial = physicsMaterial;
    }

    protected override void Enabled()
    {
        input.Enable();
    }

    protected override void Disabled()
    {
        input.Disable();
    }

    protected override void Tick()
    {
        if (DebugMovement) { DebugEvaluation(); }

        UpdateAnimation();

        GroundCheck();
        FallingCheck();

        Input();

        Slide();

        previousPosition = transform.position;
    }

    private void Input()
    {
        switch (Jape.Game.IsWeb)
        {
            case true:
                inputHorizontal = input.GetAction("HorizontalGL").AxisStream() ?? 0;
                inputVertical = input.GetAction("VerticalGL").AxisStream() ?? 0;
                break;

            case false:
                inputHorizontal = input.GetAction("Horizontal").AxisStream() ?? 0;
                inputVertical = input.GetAction("Vertical").AxisStream() ?? 0;
                break;
        }

        inputJump = input.GetAction("Jump").ButtonStream();
        inputDodge = input.GetAction("Dodge").ButtonStream();

        ChangeDirection(inputHorizontal);
        Move(inputHorizontal);
        Crouch(inputVertical);
        Jump(inputJump);
        Dodge(inputDodge);
    }

    private void Slide()
    {
        if (Grounded == true && 
            Crouching == true &&
            previousPosition.y > transform.position.y &&
            Player.rigidbody.velocity.x.Abs() > 0.1f &&
            Player.rigidbody.velocity.y < -0.1f)
        {
            if (Player.rigidbody.velocity.x > 0) { ChangeDirection(0, Direction.Horizontal.Right); }
            if (Player.rigidbody.velocity.x < 0) { ChangeDirection(0, Direction.Horizontal.Left); }

            if (Sliding == false)
            {
                Moving.Restrict(GetType());
                Turning.Restrict(GetType());
                Sliding.Set(true);

                Player.animator.SetBool("Slide", true);

                float velocityPower = Math.Rescale(Player.rigidbody.velocity.y, 0, -20, 0, 20);
                ApplyForce(velocityPower, (Vector3.down + transform.right).normalized);
            }

            ApplyForce(1.5f, Vector2.down);
        } 
        else
        {
            if (Sliding == true)
            {
                Sliding.Set(false);
                Player.animator.SetBool("Slide", false);
            }

            Moving.Unrestrict(GetType());
            Turning.Unrestrict(GetType());
        }
    }
    
    // MOVEMENT FUNCTIONS //

    private bool moving;
    public void Move(float InputX = default, bool UnitWalling = true)
    {
        float BaseSpeed = 4;

        if (!moving) { Moving.Set(false); }

        moving = false;

        if (Moving.IsRestricted()) { return; }

        Horizontal(BaseSpeed);

        void Horizontal(float Speed)
        {
            if (Mathf.Approximately(InputX, 0)) { return; }

            if (IsWalledRight(UnitWalling, true) && InputX > 0) { return; }
            if (IsWalledLeft(UnitWalling, true) && InputX < 0) { return; }

            if (Mathf.Abs(Player.rigidbody.velocity.x) > moveSpeed * MoveSpeedRatio) { return; }

            float MoveForce = Speed * moveSpeed;
            if (JumpTrajectoryTimer.IsProcessing()) { MoveForce *= (JumpTrajectoryTimer.TimeTotal - JumpTrajectoryTimer.TimeRemaining) / JumpTrajectoryTimer.TimeTotal; }
            if (Grounded == true && Crouching == true) { MoveForce /= 2; }

            Vector2 Force = new Vector2(InputX * MoveForce, 0);

            Player.rigidbody.AddForce(Force);

            moving = true;

            Moving.Set(true);
        }
    }

    public void Jump(bool? Input)
    {
        if (Grounded == true)
        {
            JumpsRemaining = jumpLimit;
            DodgeRemaining = dodgeLimit;
        }

        if (BecomeUngrounded) 
        { 
            BecomeUngrounded = false;
            JumpsRemaining -= 1;
        }

        if (!IsJumpPressed()) { return; }
        if (JumpsRemaining < 1) { return; }

        if (Jumping.IsRestricted()) { return; }
        if (JumpCooldownTimer.IsProcessing()) { return; }

        float StoppingForce = 0;
        if (Falling == true) {
            StoppingForce = Mathf.Abs(Player.rigidbody.velocity.y / MaxFallSpeed) / 1.5f;
            StoppingForce *= (Player.rigidbody.velocity.x * -1) * 1.5f;
        }

        float FallingCompensation = 1;
        if (Falling == true) {
            FallingCompensation = Mathf.Abs(Player.rigidbody.velocity.y / MaxFallSpeed) + 1;
            FallingCompensation *= 1.3f;
        }

        float JumpForce = 1.5f;
        JumpForce *= jumpSpeed;
        JumpForce *= FallingCompensation;
        JumpForce += Mathf.Abs(StoppingForce / 2);

        if (Crouching == true) { JumpForce *= JumpCrouchPenalty; }
        if (Grounded == false)
        {
            JumpForce *= JumpAirPenalty;
        } 
        else
        {
            Player.rigidbody.velocity = new Vector2(Player.rigidbody.velocity.x, Mathf.Clamp(Player.rigidbody.velocity.y, 0, float.MaxValue));
        }

        Vector2 Force = new Vector2(StoppingForce, JumpForce);

        JumpsRemaining -= 1;

        JumpCooldownTimer.ForceStart(JumpCooldownTime);
        JumpTrajectoryTimer.ForceStart(JumpTrajectoryTime);
        GroundingProtectionTimer.ForceStart(GroundingProtectionTime);

        PlayJump();
        Player.rigidbody.AddForce(Force, ForceMode2D.Impulse);

        CreateJob().Set(JumpHoldRoutine(Force)).Start();
    }

    private IEnumerable JumpHoldRoutine(Vector2 Force)
    {
        float time = 1.5f;
        Vector2 ForceMultiplier = Force * 3;

        Jumping.Set(true);

        Timer Timer = CreateTimer().Set(time).Start();
        while (Timer.IsProcessing()) 
        {
            if (!IsJumpPressed() && !IsJumpHeld())
            {
                Timer.Stop();
                Jumping.Set(false);
                yield break;
            }

            Player.rigidbody.AddForce(new Vector2(ForceMultiplier.x, ForceMultiplier.y * (Timer.TimeRemaining / (Timer.TimeTotal + (Timer.TimeTotal * 0.5f)))), ForceMode2D.Force);

            yield return Wait.Tick();
        }

        Jumping.Set(false);
    }

    private bool IsJumpPressed()
    {
        return inputJump == true;
    }

    private bool IsJumpHeld()
    {
        return inputJump == false;
    }

    public void Crouch(float Input, bool ForceStand = false)
    {
        if (ForceStand) { Stand(); return; }

        if (Crouching == true)
        {
            ApplyForce(0.25f, Vector2.down);
        }

        if (Crouching == false && Input >= 0) { return; }
        if (Crouching == true && Input < 0) { return; }

        if (Crouching == true && Input >= 0) { Stand(); return; }
        if (Crouching == false && Input < 0) { Crouch(); return; }

        void Crouch()
        {
            Crouching.Set(true);

            ColliderSize = Player.collider.size;
            ColliderOffset = Player.collider.offset;

            Collider.size = new Vector2(Collider.size.x, Collider.size.y / 2);
            Collider.offset = new Vector2(Collider.offset.x, Collider.offset.y - (Collider.size.y / 2));

            Player.animator.SetBool("Crouch", true);
        }

        void Stand()
        {
            Crouching.Set(false);

            Collider.size = ColliderSize;
            Collider.offset = ColliderOffset;

            Player.animator.SetBool("Crouch", false);
        }
    }

    public void Dodge(bool? input)
    {
        if (input != true)
        {
            return;
        }

        if (dodge.IsProcessing())
        {
            return;
        }

        if (DodgeRemaining <= 0)
        {
            return;
        }

        dodge.Start();
    }

    private void DodgeSetup()
    {
        transform.rotation = Quaternion.identity;
        Grounded.Restrict(GetType());
    }

    private void DodgeCleanup()
    {
        transform.rotation = Quaternion.identity;
        Grounded.Unrestrict(GetType());
    }

    private IEnumerable DodgeRoutine()
    {
        const int Ticks = 40;
        const float HorizontalForce = 15;
        const float VerticalForce = 5;

        Crouch(default, true);

        AnimationClip clip = null;
        switch (FacingDirection())
        {
            case Direction.Horizontal.Right:
                clip = Database.GetAsset<AnimationClip>("Dodge").Load<AnimationClip>();
                break;

            case Direction.Horizontal.Left:
                clip = Database.GetAsset<AnimationClip>("DodgeReverse").Load<AnimationClip>();
                break;
        }

        GameAnimation.PlayOverride(Player, clip, "Action", 1);

        float horizontal = GameMath.FloatDirection(Player.rigidbody.velocity.x) != FacingDirection() ? 0 : Player.rigidbody.velocity.x;
        float vertical = Mathf.Clamp(Player.rigidbody.velocity.y, 0, Mathf.Infinity);

        Player.rigidbody.velocity = new Vector2(horizontal, vertical);
        
        float force = Math.Rescale(Player.rigidbody.velocity.x.Abs(), 0, 20, HorizontalForce, HorizontalForce / 2);

        ApplyFacingForce(force);
        ApplyForce(VerticalForce, Vector2.up);

        if (Player.rigidbody.velocity.y <= 0 || Grounded == true) { ApplyForce(new Vector2(0, 7.5f)); }

        bool notGrounded = false;
        int time = Ticks; 
        while (time > 0)
        {
            if (Grounded == false) { notGrounded = true; }
            if (notGrounded && Grounded == true) { break; }
            time -= 1;
            yield return Wait.Tick();
        }

        DodgeRemaining -= 1;
    }

    public void ChangeDirection(float Input = 0, Direction.Horizontal ForceDirection = Direction.Horizontal.None)
    {
        // If Direction Argument Is Given, Force Direction
        if (ForceDirection != Direction.Horizontal.None)
        {
            switch (ForceDirection) {
                case Direction.Horizontal.Left:
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
                    return;

                case Direction.Horizontal.Right:
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    return;
            }

        // If No Argument Given, Set Direction Based On Input
        } else {
            if (Turning.IsRestricted()) { return; }

            if (Input < 0) {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
            }

            if (Input > 0) {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public Direction.Horizontal FacingDirection()
    {
        if (transform.lossyScale.x < 0) { return Direction.Horizontal.Left; }
        if (transform.lossyScale.x > 0) { return Direction.Horizontal.Right; }

        return Direction.Horizontal.None;
    }

    private void ChangeFriction(float friction)
    {
        physicsMaterial.friction = friction;

        Collider.enabled = false;
        Collider.enabled = true;
    }

    private void UpdateAnimation()
    {
        if (Grounded == true) { Player.animator.SetFloat("GroundSpeed", Player.rigidbody.velocity.x.Abs()); }
        else { Player.animator.SetFloat("GroundSpeed", 0); }
    }

    // COMMANDS //

    public void ApplyForce(Vector2 Force)
    {
        Player.rigidbody.AddForce(Force, ForceMode2D.Impulse);
    }

    public void ApplyForce(float Amount, Vector2 Direction)
    {
        Player.rigidbody.AddForce(Direction * Amount, ForceMode2D.Impulse);
    }

    public void ApplyFacingForce(float Amount, bool Forward = true)
    {
        Vector2 Force = Forward ? new Vector2(GameMath.DirectionFloat(FacingDirection()) * Amount, 0) : new Vector2((GameMath.DirectionFloat(FacingDirection()) * -1) * Amount, 0);

        Player.rigidbody.AddForce(Force, ForceMode2D.Impulse);
    }

    // MOVEMENT EVALUATIONS //

    private void DebugEvaluation()
    {
        GroundCheck(true);
        IsWalledLeft(false, true);
        IsWalledRight(false, true);
    }

    private bool BecomeUngrounded;

    public void GroundCheck(bool ShowDebug = default)
    {
        Vector2 AreaA = new Vector2(Collider.bounds.min.x + (ColliderBounds.size.x * 0.15f), Collider.bounds.min.y - 0.33f);
        Vector2 AreaB = new Vector2(Collider.bounds.max.x - (ColliderBounds.size.x * 0.15f), Collider.bounds.min.y - 0.03f);

        List<Collider2D> GroundCastColliders = new List<Collider2D>();

        int GroundCast = Physics2D.OverlapArea(AreaA, AreaB, MovementFilter, GroundCastColliders);

        if (this.Grounded.IsRestricted() || GroundingProtectionTimer.IsProcessing()) {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.green); }
            BecomeUngrounded = true;
            UngroundingTimer.Stop();
            Ungrounded();
            return;
        }

        if (UngroundingTimer.IsComplete())
        {
            BecomeUngrounded = true;
        }

        if (!UngroundingTimer.IsProcessing()) {
            if (GroundCast > 0) {
                // Grounded
                if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.red); }
                BecomeUngrounded = false;
                UngroundingTimer.ForceStart(UngroundingTime);
                Grounded();


            } else {
                // Ungrounded
                if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.green); }
                Ungrounded();
            }
        } else {
            if (GroundCast > 0) {
                // Grounded
                if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.red); }
                BecomeUngrounded = false;
                UngroundingTimer.Restart();
                Grounded();
            }

            // Becoming Ungrounded
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.yellow); }
            Grounded();
        }

        void Grounded()
        {
            if (this.Grounded == false)
            {
                PlayFootstep();
                ChangeFriction(Friction);
                this.Grounded.Set(true);
            }
            
        }

        void Ungrounded()
        {
            if (this.Grounded == true)
            {
                ChangeFriction(0);
                this.Grounded.Set(false);
            }
        }
    }

    public void FallingCheck()
    {
        if (Player.rigidbody != null) {
            if (Player.rigidbody.velocity.y < -0.1f && Grounded == false)
            {
                Falling.Set(true);
            }
            else 
            {
                Falling.Set(false);
            }
        }
    }

    public bool IsWalledLeft(bool ExcludeGrounded = default, bool ShowDebug = default)
    {
        bool walled = default;

        Vector2 AreaA = new Vector2(Collider.bounds.min.x - 0.03f, Collider.bounds.max.y - (Collider.bounds.size.y * 0.15f));
        Vector2 AreaB = new Vector2(Collider.bounds.min.x - 0.33f, Collider.bounds.min.y + (Collider.bounds.size.y * 0.15f));

        List<Collider2D> WallCastColliders = new List<Collider2D>();
        int WallCast = Physics2D.OverlapArea(AreaA, AreaB, MovementFilter, WallCastColliders);

        if (Walled.IsRestricted()) {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.red); }
            return true;
        }

        if (WallCast > 0)
        {
            walled = true;
        }

        if (walled && Grounded == false || walled && !ExcludeGrounded) {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.red); }
            return true;
        } else {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.green); }
            return false;
        }
    }

    public bool IsWalledRight(bool ExcludeGrounded = default, bool ShowDebug = default)
    {
        bool walled = default;

        Vector2 AreaA = new Vector2(Collider.bounds.max.x + 0.03f, Collider.bounds.max.y - (Collider.bounds.size.y * 0.15f));
        Vector2 AreaB = new Vector2(Collider.bounds.max.x + 0.33f, Collider.bounds.min.y + (Collider.bounds.size.y * 0.15f));

        List<Collider2D> WallCastColliders = new List<Collider2D>();
        int WallCast = Physics2D.OverlapArea(AreaA, AreaB, MovementFilter, WallCastColliders);

        if (Walled.IsRestricted()) {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.red); }
            return true;
        }

        if (WallCast > 0)
        {
            walled = true;
        }

        if (walled && Grounded == false || walled && !ExcludeGrounded) {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.red); }
            return true;
        } else {
            if (ShowDebug) { Debug.DrawLine(AreaA, AreaB, Color.green); }
            return false;
        }
    }

    public void PlayJump()
    {
        EntFuncAudio.Create(Database.GetAsset<SoundClip>("Jump", true).Load<SoundClip>(), Vector3.zero, transform);
    }

    public void PlayFootstep()
    {
        EntFuncAudio.Create(Database.GetAsset<SoundClip>("Footstep", true).Load<SoundClip>(), Vector3.zero, transform);
    }
}