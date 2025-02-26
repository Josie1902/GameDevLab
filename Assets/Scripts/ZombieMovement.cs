using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    // Button Callback
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // Start is called before the first frame update
    private bool killed = false;

    public AudioSource deathAudioSource;
    private AudioSource shootAudioSource;

    public Animator zombieAnimator;
    private Collider2D zombieCollider;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    void Start()
    {
        zombieCollider = GetComponent<Collider2D>();
        shootAudioSource = projectilePrefab.GetComponent<AudioSource>();
        StartCoroutine(PlayShootAnimation());
    }

    public void GameRestart()
    {
        killed = false;
        zombieCollider.enabled = true;
        zombieAnimator.SetTrigger("gameRestart");
        transform.localPosition = startPosition;
    }

    private IEnumerator PlayShootAnimation()
    {
        while (true && !killed)
        {
            yield return new WaitForSeconds(5f);

            zombieAnimator.SetTrigger("Shoot");

            yield return new WaitForSeconds(1f);

            zombieAnimator.SetTrigger("Walk");
        }
    }

    public void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        shootAudioSource.PlayOneShot(shootAudioSource.clip);
        rb.velocity = new Vector2(-5f, 0);

        Destroy(projectile, 2f);
    }


    // trigger is enabaled in zombie BoxCollider2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            zombieAnimator.SetTrigger("killed");
            StopAllCoroutines();
            killed = true;
        }
    }

    public void DisableZombie()
    {
        GetComponent<Collider2D>().enabled = false;
        Vector3 newPosition = transform.position;
        newPosition.y -= 0.1f;
        transform.position = newPosition;
    }

    void PlayDeadSound()
    {
        // play jump sound
        deathAudioSource.PlayOneShot(deathAudioSource.clip);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
