using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{

    public GameObject menuFirst, difficultiesFirst;

    public void StartGame(int difficulty)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Difficulty.difficulty = (Difficulty.GameDifficulty)difficulty;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirst);
    }

    public void Difficulties()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(difficultiesFirst);
    }

    public void Back()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirst);
    }
}
