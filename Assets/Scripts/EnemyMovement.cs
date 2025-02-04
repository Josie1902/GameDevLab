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

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }
    // trigger is enabaled in goomba BoxCollider2D
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
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
