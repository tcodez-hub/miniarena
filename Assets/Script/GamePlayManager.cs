using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public GameObject[] objToEnableOnStart;
    public GameObject gameover;
    public TMP_Text winnerTMPText;
    public void StartGame()
    {
        foreach (var item in objToEnableOnStart)
        {
            item.SetActive(true);
        }
    }
    public void GameOver(string winnerText)
    {
        foreach (var item in objToEnableOnStart)
        {
            item.SetActive(false);
        }
        gameover.SetActive(true);
        winnerTMPText.text = winnerText + "WIN!!";
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
