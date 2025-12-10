using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace GameOver
{
    public class GameOverUi : MonoBehaviour
    {
        public TMP_Text textInfoWave;
        
        [SerializeField] WavesManager wavesManager;

        public GameObject gameOverCanva;

        private void Start()
        {
            GameEvents.onGameOver +=  GameOver;
        }

        private void OnDestroy()
        {
            GameEvents.onGameOver -=  GameOver;
        }

        public void GameOver()
        {
            Time.timeScale = 0;
            textInfoWave.text = "Defeated Wave :"+ wavesManager.GetCurrentWave.ToString();
            gameOverCanva.SetActive(true);
        }
    
        public void Retry()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("tittle");
        }

        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }
    }
}

