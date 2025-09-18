using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WaterRise : MonoBehaviour
{
    public Transform player;
    public float riseSpeed = 0.5f;
    public Button restartButton;
    public TextMeshProUGUI timerText;
    public GameObject restart;
    private float timer = 180f;

    private Vector3 startPos;


    void Start()
    {
        Time.timeScale = 1f;
        startPos = transform.position;
        restartButton.onClick.AddListener(RestartGame);
    }
    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        if (transform.position.y > player.position.y - 1.5f)
        {
            Time.timeScale = 0f;
            restartButton.gameObject.SetActive(true);
        }
        timer -= (float)Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timer);
        if (timer <= 0)
            TimeOut();

    }
    void RestartGame()
    {
        SceneManager.LoadScene("GameField");
    }
    void TimeOut()
    {
        restart.SetActive(true);
        Time.timeScale = 0f;
    }
}
