using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int score;
    public int lives;

    public Text scoreText;
    public Text livesText;
    public GameObject gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        lives = 3;
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (lives == 0)
        {
            gameOverText.SetActive(true);
        }

        // update labels
        scoreText.text = "score: " + score;
        livesText.text = "lives: " + lives;
    }
}
