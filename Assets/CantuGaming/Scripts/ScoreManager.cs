using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }

    // Otras mecanicas pueden escuchar los cambios sin depender de la interfaz.
    public event Action<int> ScoreChanged;

    public void AddPoints(int points)
    {
        if (points <= 0) return;

        Score += points;
        ScoreChanged?.Invoke(Score);
    }
}
