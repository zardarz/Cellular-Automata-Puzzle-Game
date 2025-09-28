using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int CurrnetLevel = 0;


    public void GoToLevel(string name) {
        SceneManager.LoadScene(name);
    }

    public void GoToNextLevel() {
        CurrnetLevel += 1;
        string sceneName = "Level " + CurrnetLevel;
        GoToLevel(sceneName);
    }

    public void Quit() {
        Application.Quit();
    }

    public void Reset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}