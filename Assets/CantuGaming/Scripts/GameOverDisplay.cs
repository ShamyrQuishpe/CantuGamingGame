using UnityEngine;

[RequireComponent(typeof(GameOverManager))]
public class GameOverDisplay : MonoBehaviour
{
    private const float FeedbackDuration = 2f;

    private GameOverManager gameOverManager;
    private GUIStyle timerStyle;
    private GUIStyle holderStyle;
    private GUIStyle feedbackStyle;
    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle buttonStyle;
    private string feedbackText;
    private Color feedbackColor;
    private float feedbackStartedAt;

    private void Awake()
    {
        gameOverManager = GetComponent<GameOverManager>();
    }

    private void OnEnable()
    {
        if (gameOverManager != null && gameOverManager.Stone != null)
            gameOverManager.Stone.StoneTransferred += OnStoneTransferred;
    }

    private void OnDisable()
    {
        if (gameOverManager != null && gameOverManager.Stone != null)
            gameOverManager.Stone.StoneTransferred -= OnStoneTransferred;
    }

    private void OnStoneTransferred(Transform newAnchor)
    {
        if (gameOverManager == null) return;

        if (newAnchor == gameOverManager.EnemyAnchor)
        {
            feedbackText = "¡Te robaron la piedra!";
            feedbackColor = new Color(0.95f, 0.35f, 0.2f, 1f);
        }
        else if (newAnchor == gameOverManager.PlayerAnchor)
        {
            feedbackText = "¡Piedra recuperada!";
            feedbackColor = new Color(0.35f, 0.85f, 0.45f, 1f);
        }
        else return;

        feedbackStartedAt = Time.unscaledTime;
    }

    private void OnGUI()
    {
        if (gameOverManager == null) return;
        PrepareStyles();

        GUI.depth = -50;
        DrawHud();
        if (!gameOverManager.IsGameOver) DrawTransferFeedback();
        else DrawResult();
    }

    private void PrepareStyles()
    {
        if (timerStyle != null) return;

        timerStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        holderStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        feedbackStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        feedbackStyle.normal.textColor = Color.white;
        titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.white;
        subtitleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };
        subtitleStyle.normal.textColor = Color.white;
        buttonStyle = new GUIStyle(GUI.skin.button);
    }

    private void DrawHud()
    {
        bool narrow = Screen.width < 400;
        float boxWidth = Mathf.Max(0f, Mathf.Min(190f, (Screen.width - 36f) * 0.5f));
        timerStyle.fontSize = narrow ? 16 : 20;
        holderStyle.fontSize = narrow ? 17 : 20;

        int secondsLeft = Mathf.CeilToInt(gameOverManager.RemainingTime);
        timerStyle.normal.textColor = secondsLeft <= 10 ? new Color(1f, 0.45f, 0.35f) : Color.white;
        string timeText = $"Tiempo: {secondsLeft / 60:00}:{secondsLeft % 60:00}";
        GUI.Box(new Rect(Screen.width - 12f - boxWidth, 12f, boxWidth, 44f), timeText, timerStyle);

        string holderText = "Piedra: sin poseedor";
        Color holderColor = Color.white;
        if (gameOverManager.Stone != null && gameOverManager.Stone.Anchor == gameOverManager.PlayerAnchor)
        {
            holderText = "Piedra: Cantuña";
            holderColor = new Color(1f, 0.88f, 0.45f);
        }
        else if (gameOverManager.Stone != null && gameOverManager.Stone.Anchor == gameOverManager.EnemyAnchor)
        {
            holderText = "Piedra: Diablo";
            holderColor = new Color(1f, 0.5f, 0.4f);
        }

        holderStyle.normal.textColor = holderColor;
        float holderWidth = Mathf.Max(0f, Mathf.Min(400f, Screen.width - 24f));
        GUI.Box(new Rect((Screen.width - holderWidth) * 0.5f, 64f, holderWidth, 40f), holderText, holderStyle);
    }

    private void DrawTransferFeedback()
    {
        if (string.IsNullOrEmpty(feedbackText)) return;
        float progress = (Time.unscaledTime - feedbackStartedAt) / FeedbackDuration;
        if (progress >= 1f) return;

        float opacity = 1f - Mathf.Clamp01(progress);
        float width = Mathf.Max(0f, Mathf.Min(400f, Screen.width - 24f));
        Rect banner = new Rect((Screen.width - width) * 0.5f, 112f, width, 50f);
        DrawSolid(banner, new Color(feedbackColor.r, feedbackColor.g, feedbackColor.b, 0.55f * opacity));
        feedbackStyle.fontSize = Screen.width < 400 ? 18 : 22;
        Color previous = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, opacity);
        GUI.Label(banner, feedbackText, feedbackStyle);
        GUI.color = previous;

        DrawStonePulse(progress, opacity);
    }

    private void DrawStonePulse(float progress, float opacity)
    {
        if (gameOverManager.Stone == null || Camera.main == null) return;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(gameOverManager.Stone.transform.position);
        if (screenPoint.z <= 0f) return;

        float size = 42f + 38f * progress;
        float x = screenPoint.x - size * 0.5f;
        float y = Screen.height - screenPoint.y - size * 0.5f;
        Rect frame = new Rect(x, y, size, size);
        DrawFrame(frame, new Color(feedbackColor.r, feedbackColor.g, feedbackColor.b, 0.8f * opacity), 3f);
    }

    private void DrawResult()
    {
        GUI.depth = -100;
        DrawSolid(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.5f));

        bool victory = gameOverManager.Result == GameOverManager.MatchResult.Victory;
        Color accent = victory ? new Color(0.35f, 0.85f, 0.45f) : new Color(0.95f, 0.35f, 0.2f);
        float panelWidth = Mathf.Max(0f, Mathf.Min(420f, Screen.width - 32f));
        float panelHeight = Mathf.Max(0f, Mathf.Min(230f, Screen.height - 24f));
        Rect panel = new Rect((Screen.width - panelWidth) * 0.5f,
            (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
        DrawSolid(panel, new Color(0.09f, 0.1f, 0.13f, 0.92f));
        DrawFrame(panel, accent, 3f);

        titleStyle.fontSize = Screen.width < 400 ? 30 : 40;
        subtitleStyle.fontSize = Screen.width < 400 ? 15 : 18;
        buttonStyle.fontSize = Screen.width < 400 ? 17 : 20;
        string title = victory ? "¡VICTORIA!" : "GAME OVER";
        string subtitle = victory ? "Cantuña conservó la última piedra." : "El diablo se quedó con la última piedra.";
        GUI.Label(new Rect(panel.x + 12f, panel.y + 24f, panel.width - 24f, 56f), title, titleStyle);
        GUI.Label(new Rect(panel.x + 18f, panel.y + 86f, panel.width - 36f, 55f), subtitle, subtitleStyle);

        float buttonWidth = Mathf.Min(190f, panel.width - 36f);
        if (GUI.Button(new Rect(panel.center.x - buttonWidth * 0.5f,
                panel.yMax - 68f, buttonWidth, 48f), "Reintentar", buttonStyle))
            gameOverManager.Restart();
    }

    private static void DrawFrame(Rect frame, Color color, float thickness)
    {
        DrawSolid(new Rect(frame.x, frame.y, frame.width, thickness), color);
        DrawSolid(new Rect(frame.x, frame.yMax - thickness, frame.width, thickness), color);
        DrawSolid(new Rect(frame.x, frame.y, thickness, frame.height), color);
        DrawSolid(new Rect(frame.xMax - thickness, frame.y, thickness, frame.height), color);
    }

    private static void DrawSolid(Rect area, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(area, Texture2D.whiteTexture);
        GUI.color = previous;
    }
}
