using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody2D marioBody;
    public float maxSpeed = 20;
    public float upSpeed = 5;
    private bool onGroundState = true;
    private bool faceRightState = true;

    // game over
    // for animation
    public Animator marioAnimator;
    // Start is called before the first frame update

    // jumping sound
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public AudioClip marioWin;

    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;

    private bool moving = false;
    private bool jumpedState = false;
    GameManager gameManager;
    private Collider2D marioCollider;

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void Start()
    {
        // set to 30fps
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioCollider = GetComponent<Collider2D>();
        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        // SceneManager.activeSceneChanged += SetStartingPosition;
    }

    public void SetStartingPosition(Scene current, Scene next)
    {
        if (next.name == "World1-2")
        {
            // change the position accordingly in your World-1-2 case
            this.transform.position = new Vector3(-23.361f, 4.82f, 0);
        }
    }

    void Awake()
    {
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }


    // Update is called once per frame

    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.velocity.x));
    }

    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.velocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }

    public bool isFlip()
    {
        return faceRightState;
    }

    void FlipMarioSprite(int value)
    {
        Vector3 scale = transform.localScale;

        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            scale.x = -Mathf.Abs(scale.x);
            if (marioBody.velocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            scale.x = Mathf.Abs(scale.x);
            if (marioBody.velocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
        transform.localScale = scale;
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
            Collider2D goombaCollider = other.GetComponent<Collider2D>();

            // **Check if Mario is landing from above**
            bool isAbove = marioCollider.bounds.min.y >= goombaCollider.bounds.max.y - 0.1f;
            bool isAlignedHorizontally = marioCollider.bounds.center.x >= goombaCollider.bounds.min.x &&
                                         marioCollider.bounds.center.x <= goombaCollider.bounds.max.x;

            if (isAbove && isAlignedHorizontally)
            {
                // Call Goomba's Stomp function
                other.GetComponent<EnemyMovement>().Stomp();
                Debug.Log("Goomba is stomped!");
            }
            else
            {
                // Mario dies if hit from the side or bottom
                marioAnimator.Play("Mario_die");
                marioAudio.PlayOneShot(marioDeath);
                alive = false;
                Debug.Log("Mario collided with goomba!");
            }
        }
        if (other.gameObject.CompareTag("Weapon"))
        {
            other.GetComponent<Weapon>().PickupGun();
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            // Mario dies if hit from the side or bottom
            marioAnimator.Play("Mario_die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
            Debug.Log("Mario collided with goomba!");
        }
        if (other.gameObject.CompareTag("Flag"))
        {
            Debug.Log("You Survived!");
            marioAudio.PlayOneShot(marioWin);
            Time.timeScale = 0.0f;
            gameManager.GameWon();
        }
    }


    void GameOverScene()
    {
        Time.timeScale = 0.0f;
        gameManager.GameOver();
    }

    public void GameRestart()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

        // reset position
        marioBody.transform.position = new Vector3(-21.62145f, 4.82f, 0.0f);
        // reset sprite direction
        faceRightState = true;

        FlipMarioSprite(1);

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;
    }

}
