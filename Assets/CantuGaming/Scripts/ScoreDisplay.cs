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
                padding = new RectOffset(12, 12, 4, 4)
            };
        }

        float boxWidth = Mathf.Max(0f, Mathf.Min(190f, (Screen.width - 36f) * 0.5f));
        scoreStyle.fontSize = Screen.width < 400 ? 16 : 20;
        GUI.Box(new Rect(12f, 12f, boxWidth, 44f), $"Puntos: {scoreManager.Score}", scoreStyle);
    }
}
