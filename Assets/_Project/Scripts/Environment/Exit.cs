using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public enum ExitType { Fire, Water }
    public ExitType type;

    private static bool fireReady = false;
    private static bool waterReady = false;

    private Collider2D exitCollider;
    private HashSet<Collider2D> playersInside = new HashSet<Collider2D>();

    private void OnEnable()
    {
        if (type == ExitType.Fire) fireReady = false;
        else waterReady = false;
        exitCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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

        if (fireReady && waterReady)
            LoadNextLevel();
    }

    private bool IsCorrectPlayer(Collider2D col)
    {
        string playerName = col.gameObject.name;
        return (type == ExitType.Fire && playerName.Contains("Fire")) ||
               (type == ExitType.Water && playerName.Contains("Water"));
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

        return overlapArea >= playerArea * 0.5f;
    }

    private bool IsGrounded(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb == null) return false;
        return Mathf.Abs(rb.linearVelocity.y) < 0.1f;
    }

    private void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
