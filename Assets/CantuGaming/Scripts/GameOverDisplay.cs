using UnityEngine;

[RequireComponent(typeof(GameOverManager))]
public class GameOverDisplay : MonoBehaviour
{
    private GameOverManager gameOverManager;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;

    private void Awake()
    {
        gameOverManager = GetComponent<GameOverManager>();
    }

    private void OnGUI()
    {
        if (gameOverManager == null || !gameOverManager.IsGameOver) return;

        // Menor profundidad para quedar sobre el marcador sin ocultar la plaza.
        GUI.depth = -100;
        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.4f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previousColor;

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 40,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20
            };
        }

        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;
        float titleWidth = Mathf.Min(400f, Screen.width - 24f);
        float buttonWidth = Mathf.Min(180f, Screen.width - 24f);

        GUI.Label(new Rect(centerX - titleWidth * 0.5f, centerY - 75f, titleWidth, 55f), "GAME OVER", titleStyle);
        if (GUI.Button(new Rect(centerX - buttonWidth * 0.5f, centerY + 10f, buttonWidth, 48f), "Reintentar", buttonStyle))
            gameOverManager.Restart();
    }
}
