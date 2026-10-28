using UnityEngine;

[RequireComponent(typeof(ScoreManager))]
public class ScoreDisplay : MonoBehaviour
{
    private ScoreManager scoreManager;
    private GUIStyle scoreStyle;

    private void Awake()
    {
        scoreManager = GetComponent<ScoreManager>();
    }

    private void OnGUI()
    {
        if (scoreManager == null) return;

        if (scoreStyle == null)
        {
            scoreStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 20,
                padding = new RectOffset(12, 12, 4, 4)
            };
        }

        GUI.Box(new Rect(12, 12, 190, 44), $"Puntos: {scoreManager.Score}", scoreStyle);
    }
}
