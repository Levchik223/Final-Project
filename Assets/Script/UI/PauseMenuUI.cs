using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
   [SerializeField] private GameObject pauseMenu;
   [SerializeField] private GameObject pauseSettings;
   private bool _isPaused;

   private void Update()
   {
      if (Keyboard.current.escapeKey.wasPressedThisFrame)
      {
         if (pauseSettings.activeSelf)
         {
            CloseSettings();
            return;
         }
         if (_isPaused)
         {
            ResumeGame();
         }
         else
         {
            PauseGame();
         }
      }
   }

   public void ResumeGame()
   {
      pauseMenu.SetActive(false);
      Time.timeScale = 1f;
      _isPaused = false;
   }

   public void PauseGame()
   {
      pauseMenu.SetActive(true);
      Time.timeScale = 0f;
      _isPaused = true;
   }
   public void ExitToMainMenu()
   {
      Time.timeScale = 1f;
      SceneManager.LoadScene("Main Menu");
   }

   public void OpenSettings()
   {
      pauseMenu.SetActive(true);
      pauseSettings.SetActive(true);
   }

   public void CloseSettings()
   {
      pauseMenu.SetActive(true);
      pauseSettings.SetActive(false);
   }
}
