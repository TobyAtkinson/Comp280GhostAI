using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacmanController : MonoBehaviour
{
    // Constants for the players scoring
    public const float POWER_PILL_TIMER = 10;
    public const int PILL_SCORE = 10;
    public const int POWERPILL_SCORE = 50;
    public const int GHOST_SCORE = 100;

    // Private variables for storing players information
    private Rigidbody body;
    private GameController game;
    private Text invulnText;
    private GameObject powerPillGFX;
    private float currentspeed;
    private GameObject[] ghosts;
    private Vector3 startingPosition;
    private GameObject victoryUI;

    // Variables shown in editor to allow customization
    [Header("Customizable speed variables")]
    [Tooltip("How fast the player moves normally")]
    [SerializeField] private float normalSpeed = 4f;
    [Tooltip("How fast the playter moves while boosted by a power pill")]
    [SerializeField] private float poweredSpeed = 6f;

    [Header("Invuln Timer shows how long left the player has when powered")]
    public float invulnTimer;

    private void Awake()
    {
        // Finding and storing the game controller, rigidbody, powerPill Text and GXF aswell as the ghosts
        body = GetComponent<Rigidbody>();
        game = GameObject.Find("GameController").GetComponent<GameController>();
        invulnText = GameObject.Find("PowerTimeUI").GetComponent<Text>();
        powerPillGFX = GameObject.Find("PowerPillGFX");
        ghosts = GameObject.FindGameObjectsWithTag("ghost");
        // Finding the victory UI and disabling it untill the player completes the game
        victoryUI = GameObject.Find("victoryUI");
        if(victoryUI != null)
        {
            victoryUI.SetActive(false);
        }
        startingPosition = this.transform.position;
    }

    void Start()
    {
        // Resets the player so they start normally
        invulnTimer = 0;
        powerPillGFX.SetActive(false);
        currentspeed = normalSpeed;
    }

    void Update()
    {
        // If the player is powered decrease the time left each second aswell as update the UI
        if (invulnTimer > 0)
        {
            invulnTimer -= Time.deltaTime;
            invulnText.text = "Power Time: " +  Mathf.Round(invulnTimer);
        }
        // Pressing P reloads the level
        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        // Pressing ESC closes the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        // Runs movement in Fixed Update so it goes smoothly on each frame
        Movement();
    }

    void Movement()
    {
        // Uses Horizontal and Vertical Input keys to allow player to change velocity
        float xSpeed = Input.GetAxis("Horizontal");
        float ySpeed = Input.GetAxis("Vertical");
        Vector3 vel = new Vector3(xSpeed, 0, ySpeed) * currentspeed;

        // This results in the player moving
        body.AddForce(vel);
    }

    // Method which returns true if the player is powered
    bool isPowered()
    {
        return invulnTimer > 0;
    }

    void OnCollisionEnter(Collision col)
    {
        // If player has collided with ghost
        if (col.gameObject.tag == "ghost")
        {
            // If Player is powered ghost is killed
            if (isPowered())
            {
                game.score += GHOST_SCORE;
                col.gameObject.GetComponent<GhostAI>().KilledByPlayer();
            }
            // Else Player is not powered so they lose a life
            else
            {
                game.lives--;
                // If player has run out of lives they die
                if(game.lives <= 0)
                {
                    // Disable their visual and feeze the game so they appear dead
                    this.GetComponent<MeshRenderer>().enabled = false;
                    Time.timeScale = 0f;
                }
                // Else they respawn at the centre and the ghosts reset
                else
                {
                    for (int i = 0; i < ghosts.Length; i++)
                    {
                        ghosts[i].GetComponent<GhostAI>().TeleportToRespawnPoint();
                    }
                    this.transform.position = startingPosition;
                } 
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // If player has touched pill
        if (col.gameObject.tag == "pill")
        {
            // Add score
            game.score += PILL_SCORE;
            // Remove pill from list of patrol points so ghosts no longer patrol there
            for (int i = 0; i < ghosts.Length; i++)
            {
                ghosts[i].GetComponent<GhostAI>().PillCollected(col.gameObject);
            }
            // Destroy pill
            Destroy(col.gameObject);

            // Checking to see if every pill has been collected
            // If there are no pills left end the game
            if (ghosts[0].GetComponent<GhostAI>().AnyPillsLeft() == false)
            {
                if (victoryUI != null)
                {
                    victoryUI.SetActive(true);
                }
                Time.timeScale = 0f;
            }
            // Else do nothing, the game carries on
        }
        // Else if the player has touched a powerpill
        else if (col.gameObject.tag == "powerpill")
        {
            // If they are not currently powered start flashing and speed player
            if (!isPowered())
            {
                StartCoroutine(FlashDelay());
                currentspeed = poweredSpeed;
            }
            // Add score, set timer and destroy power pill
            game.score += POWERPILL_SCORE;
            invulnTimer = POWER_PILL_TIMER;
            Destroy(col.gameObject);
        }
    }

    // Loop that makes player flash when powered
    IEnumerator FlashDelay()
    {
        // Toggle flash graphics every quarter of a second
        powerPillGFX.SetActive(!powerPillGFX.activeInHierarchy);
        yield return new WaitForSeconds(0.25f);

        // If player is still powered continue loop
        if(isPowered())
        {
            StartCoroutine(FlashDelay());
        }
        // Else power has run out
        else
        {
            // Stop flash then reset UI and speed
            powerPillGFX.SetActive(false);
            invulnText.text = "";
            currentspeed = normalSpeed;
            
        }
    }
}
