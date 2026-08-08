using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
  [SerializeField] private GameObject gameOverMenu;
  
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip gameOverSound;
  [SerializeField] private GameMusicManager musicManager;

  private void Start()
  {
    gameOverMenu.SetActive(false);
    Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
  }
  private void Player_OnPlayerDeath(object sender, System.EventArgs e)
  {
    StartCoroutine(ShowGameOverRoutine());
  }

  private IEnumerator ShowGameOverRoutine()
  {
    yield return new WaitForSeconds(1f);
    musicManager.StopMusic();
    gameOverMenu.SetActive(true);
    audioSource.PlayOneShot(gameOverSound);
    Time.timeScale = 0f;
  }

  public void Continue()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void ExitToMenu()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene("Main Menu");
  }

  private void OnDestroy()
  {
    if (Player.Instance != null)
    {
      Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
    }
  }
}
