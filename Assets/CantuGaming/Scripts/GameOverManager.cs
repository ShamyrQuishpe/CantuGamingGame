using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public bool IsGameOver { get; private set; }
    public event Action GameOverStarted;

    public void TriggerGameOver()
    {
        if (IsGameOver) return;

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
