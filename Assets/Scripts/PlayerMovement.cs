using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody2D marioBody;
    public float maxSpeed = 20;
    public float upSpeed = 5;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    // button callback
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public GameObject enemies;
    // reset score
    public JumpOverGoombas jumpOverGoombas;
    // game over
    public GameObject gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        // set to 30fps
        marioSprite = GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // toggle state
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }

    // trigger is enabaled in goomba BoxCollider2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            // Game over condition
            ShowGameOverScreen();
        }
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        finalScoreText.text = "Score: " + jumpOverGoombas.score.ToString();
        Time.timeScale = 0.0f; // Pause the game 
    }

    // Button Callback
    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        gameOverScreen.SetActive(false);
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.position = new Vector3(-21.62145f, 4.82f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        // enemies.GetComponent<Transform>().localPosition = enemies.GetComponent<EnemyMovement>().startPosition;

        foreach (Transform eachChild in enemies.transform)
        {
            // different from transform.position (gloabl coordinates)
            // refers to the local position (0,0,0) wrt to its parents Enemies(as seen in EnemyMovement)
            // Note: Why doesnt this restart to default position?
            // Goomba needs to be relative to the enemies parent group
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        // reset score
        jumpOverGoombas.score = 0;

    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.velocity.magnitude < maxSpeed)
            {
                marioBody.AddForce(movement * speed);
            }
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            //stop
            marioBody.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }
}
