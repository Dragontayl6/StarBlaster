using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 2f;

    [SerializeField] int level2Threshold = 250;
    [SerializeField] int bossThreshold = 500;

    ScoreKeeper scoreKeeper;
    bool bossLevelLoaded = false;
    bool level2Loaded = false;

    void Awake()
    {
        scoreKeeper = FindFirstObjectByType<ScoreKeeper>();
        // If this LevelManager instance is created in Level2, mark it so
        level2Loaded = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level2";
        bossLevelLoaded = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "BossLevel";

        if (scoreKeeper != null)
        {
            scoreKeeper.OnScoreChanged += HandleScoreChanged;
            // handle current score in case thresholds were already met, but only if we're in a gameplay scene
            string current = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (current == "Level1" || current == "Level2")
            {
                HandleScoreChanged(scoreKeeper.GetScore());
            }
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Level1");
        scoreKeeper.ResetScore();
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    void OnDestroy()
    {
        if (scoreKeeper != null)
        {
            scoreKeeper.OnScoreChanged -= HandleScoreChanged;
        }
    }

    void HandleScoreChanged(int score)
    {
        // Only react to score changes when we're in gameplay scenes (Level1 or Level2).
        string current = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (current != "Level1" && current != "Level2") return;

        // If we haven't loaded Level2 yet, ensure it loads at level2Threshold first.
        if (!level2Loaded && score >= level2Threshold)
        {
            level2Loaded = true;
            SceneManager.LoadScene("Level2");
            return; // don't try to load boss in the same callback
        }

        // If Level2 has already been loaded, load BossLevel at bossThreshold.
        if (!bossLevelLoaded && level2Loaded && score >= bossThreshold)
        {
            bossLevelLoaded = true;
            SceneManager.LoadScene("BossLevel");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
