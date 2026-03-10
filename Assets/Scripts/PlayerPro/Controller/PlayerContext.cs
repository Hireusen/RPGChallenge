using UnityEngine;

public readonly struct PlayerContext
{
    // 어차피 수정 가능
    readonly public Rigidbody rb;
    readonly public Animator animator;
    readonly public Transform tr;
    readonly public Transform cameraTr;
    // 입력 값
    readonly public Vector2 inputMove;
    readonly public bool inputRun;
    readonly public bool inputJump;
    readonly public bool inputAttack;
    readonly public bool inputGuard;
    // 상태 값
    readonly public bool isGrounded;
    readonly public bool isFalling;
    readonly public bool isHit;
    readonly public bool isDropImpact;

    public PlayerContext
        (Rigidbody rb, Animator animator, Transform tr, Transform cameraTr,
        Vector2 inputMove, bool inputRun, bool inputJump, bool inputAttack, bool inputGuard,
        bool isGrounded, bool isFalling, bool isHit, bool isDropImpact)
    {
        this.rb = rb;
        this.animator = animator;
        this.cameraTr = cameraTr;
        this.tr = tr;
        this.inputMove = inputMove;
        this.inputRun = inputRun;
        this.inputJump = inputJump;
        this.inputAttack = inputAttack;
        this.inputGuard = inputGuard;
        this.isGrounded = isGrounded;
        this.isFalling = isFalling;
        this.isHit = isHit;
        this.isDropImpact = isDropImpact;
    }
}
