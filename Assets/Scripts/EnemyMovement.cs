using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Button Callback
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // Start is called before the first frame update
    private float originalX;
    private float maxOffset = 2.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;
    public AudioSource audioSource;
    private Animator goombaAnimator;
    private Collider2D goombaCollider;
    public GameObject goomba;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
        ComputeVelocity();
        goombaAnimator = GetComponent<Animator>();
        goombaCollider = GetComponent<Collider2D>();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    public void GameRestart()
    {
        goomba.SetActive(true);
        goombaCollider.enabled = true;
        goombaAnimator.SetTrigger("gameRestart");
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
    }

    // trigger is enabaled in goomba BoxCollider2D
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
    }

    public void Stomp()
    {
        goombaAnimator.SetTrigger("stomped");
        StartCoroutine(DeactivateAfterStompAnimation());
    }

    private IEnumerator DeactivateAfterStompAnimation()
    {
        // Wait for the animation to finish (assumes animation length is 1 second)
        yield return new WaitForSeconds(0.06f);

        // Now deactivate after animation
        goombaCollider.enabled = false;
        goomba.SetActive(false);
    }


    void PlayStompedSound()
    {
        // play jump sound
        audioSource.PlayOneShot(audioSource.clip);
    }
    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            // if goomba is not far awya from its starting position, 
            // move to designated direction
            Movegoomba();
        }
        else
        {
            // cahnge direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }
}
