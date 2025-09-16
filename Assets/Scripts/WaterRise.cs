using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaterRise : MonoBehaviour
{
    public Transform player;
    public float riseSpeed = 0.5f;
    public Button restartButton;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        if (transform.position.y > player.position.y + 0.1f)
        {
            Time.timeScale = 0f;
            restartButton.gameObject.SetActive(true);
        }
    }
    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameField");
    }
}
