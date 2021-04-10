using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{

    public bool isPaused = false;
    public GameObject pauseMenu;
    public GameObject pauseMenuFirst;

    private void Start()
    {
        ContinueGame();
    }

    void Update()
    {
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("joystick button 9"))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ContinueGame();
            }
        }
        
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
    }

    public void ContinueGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
