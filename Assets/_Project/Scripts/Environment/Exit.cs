using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public enum ExitType { Fire, Water }
    public ExitType type;

    public Animator animator;
    [SerializeField] private float animSpeed = 1f;
    private float animTime = 0f;

    private static bool fireReady = false;
    private static bool waterReady = false;

    private Collider2D exitCollider;
    private HashSet<Collider2D> playersInside = new HashSet<Collider2D>();

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        exitCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (type == ExitType.Fire) fireReady = false;
        else waterReady = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playersInside.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playersInside.Remove(collision);
    }

    private void Update()
    {
        bool ready = false;
        foreach (var col in playersInside)
        {
            if (col == null) continue;
            if (!IsCorrectPlayer(col)) continue;
            if (!IsHalfOverlapping(col)) continue;
            if (!IsGrounded(col)) continue;
            ready = true;
            break;
        }

        if (type == ExitType.Fire) fireReady = ready;
        else waterReady = ready;

        if (animator != null)
        {
            float target = ready ? 1f : 0f;
            animTime = Mathf.MoveTowards(animTime, target, Time.deltaTime * animSpeed);
            int stateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            animator.Play(stateHash, 0, animTime);
            animator.speed = 0f;
        }

        if (fireReady && waterReady)
            GameManager.Instance.NextStage();
    }

    private bool IsCorrectPlayer(Collider2D col)
    {
        Move move = col.GetComponentInParent<Move>();
        if (move == null) return false;
        return (type == ExitType.Fire  &&  move.fire) ||
               (type == ExitType.Water && !move.fire);
    }

    private bool IsHalfOverlapping(Collider2D col)
    {
        Bounds p = col.bounds;
        Bounds e = exitCollider.bounds;

        float overlapX = Mathf.Min(p.max.x, e.max.x) - Mathf.Max(p.min.x, e.min.x);
        float overlapY = Mathf.Min(p.max.y, e.max.y) - Mathf.Max(p.min.y, e.min.y);

        if (overlapX <= 0 || overlapY <= 0) return false;

        float overlapArea = overlapX * overlapY;
        float playerArea = p.size.x * p.size.y;

        return overlapArea >= playerArea * 0.1f;
    }

    private bool IsGrounded(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponentInParent<Rigidbody2D>();
        if (rb == null) return false;
        return Mathf.Abs(rb.linearVelocity.y) < 0.5f;
    }


}
