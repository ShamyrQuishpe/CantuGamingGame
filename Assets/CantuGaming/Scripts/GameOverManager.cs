using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public enum MatchResult { None, Victory, Defeat }

    [Header("Partida")]
    [SerializeField, Min(1f)] private float matchDurationSeconds = 90f;
    [SerializeField] private StoneFollowAnchor stone;
    [SerializeField] private Transform playerAnchor;
    [SerializeField] private Transform enemyAnchor;

    public bool IsGameOver { get; private set; }
    public MatchResult Result { get; private set; }
    public float RemainingTime { get; private set; }
    public StoneFollowAnchor Stone => stone;
    public Transform PlayerAnchor => playerAnchor;
    public Transform EnemyAnchor => enemyAnchor;
    public event Action GameOverStarted;

    private void Awake()
    {
        RemainingTime = matchDurationSeconds;
    }

    private void Update()
    {
        AdvanceTime(Time.deltaTime);
    }

    private void AdvanceTime(float elapsedSeconds)
    {
        if (IsGameOver) return;

        RemainingTime = Mathf.Max(0f, RemainingTime - elapsedSeconds);
        if (RemainingTime > 0f) return;

        // La piedra es la única fuente de verdad sobre el resultado al terminar el tiempo.
        if (stone != null && playerAnchor != null && stone.Anchor == playerAnchor)
            EndMatch(MatchResult.Victory);
        else
        {
            if (stone == null || enemyAnchor == null || stone.Anchor != enemyAnchor)
                Debug.LogError("No se pudo identificar al poseedor de UltimaPiedra al terminar la partida.", this);
            EndMatch(MatchResult.Defeat);
        }
    }

    public void TriggerGameOver()
    {
        EndMatch(MatchResult.Defeat);
    }

    private void EndMatch(MatchResult result)
    {
        if (IsGameOver) return;

        Result = result;
        IsGameOver = true;
        Time.timeScale = 0f;
        GameOverStarted?.Invoke();
    }

    public void Restart()
    {
        if (!IsGameOver) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (IsGameOver) Time.timeScale = 1f;
    }
}
