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
    // TODO: restart screen to original posiiton
    public GameObject camera;
    // reset score
    public JumpOverGoombas jumpOverGoombas;
    // game over
    public GameObject gameOverScreen;
    // for animation
    public Animator marioAnimator;
    // Start is called before the first frame update

    // jumping sound
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void Start()
    {
        // set to 30fps
        marioSprite = GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();

        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);
    }

    // Update is called once per frame
    void Update()
    {
        // toggle state
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            // animator
            if (marioBody.velocity.x > 0.01f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            // animator
            if (marioBody.velocity.x < -0.01f)
                marioAnimator.SetTrigger("onSkid");
        }

        // animator
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.velocity.x));
    }

    // death scene
    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);
    void OnCollisionEnter2D(Collision2D col)
    {

        if ((col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Enemies") || col.gameObject.CompareTag("Obstacles")) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

        // layer mask bitwise according to Tags&Layers
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    // trigger is enabaled in goomba BoxCollider2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemies"))
        {
            Debug.Log("Collided with goomba!");
            // play death animation
            marioAnimator.Play("Mario_die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
            // Game over condition -> now triggered from animation
            // ShowGameOverScreen();
        }
    }

    void ShowGameOverScreen()
    {
        Time.timeScale = 0.0f; // Pause the game 
        gameOverScreen.SetActive(true);
        finalScoreText.text = "Score: " + jumpOverGoombas.score.ToString();
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
        camera.transform.position = new Vector3(-21.364f, 5.87f, -10.0f);
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
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        // reset score
        jumpOverGoombas.score = 0;

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
    }

    void FixedUpdate()
    {
        if (alive)
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
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
            }
        }
    }
}
