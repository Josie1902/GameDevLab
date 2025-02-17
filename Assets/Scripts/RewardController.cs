using System.Collections;
using UnityEngine;

public class RewardController : MonoBehaviour
{
    public GameObject coin;
    public Sprite usedBlockSprite;

    private Vector3 startPosition;
    private Vector3 boxStartPosition;

    private bool hit = false;
    public GameObject reward;
    private Rigidbody2D box;
    private SpringJoint2D springJoint;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public AudioSource coinAudio;

    void Start()
    {
        box = reward.GetComponent<Rigidbody2D>();
        springJoint = reward.GetComponent<SpringJoint2D>();
        animator = reward.GetComponent<Animator>();
        spriteRenderer = reward.GetComponent<SpriteRenderer>();
        startPosition = coin.transform.position;
        boxStartPosition = box.transform.position;
        coin.SetActive(false); // Hide the coin at the start
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hit)
        {
            hit = true;
            Debug.Log("Mario hits the block!");

            coin.SetActive(true);
            coinAudio.PlayOneShot(coinAudio.clip);

            // Start the coin movement coroutine
            StartCoroutine(CoinMovements());

            // Start disabling the block once it stops moving
            StartCoroutine(DisableQuestionBox());
        }
    }

    public void GameRestart()
    {
        // reset animation
        animator.enabled = true;
        animator.Play("Reward_blinking");
        box.bodyType = RigidbodyType2D.Dynamic;
        springJoint.enabled = true;
        box.transform.position = boxStartPosition;
        coin.transform.position = startPosition;
        hit = false;
    }

    IEnumerator DisableQuestionBox()
    {
        // Wait until the block stops moving
        yield return new WaitForSeconds(0.5f);
        animator.enabled = false;
        spriteRenderer.sprite = usedBlockSprite;
        box.bodyType = RigidbodyType2D.Static;
        springJoint.enabled = false;
    }

    IEnumerator CoinMovements()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        Vector3 peakPosition = startPosition + Vector3.up * 0.7f;

        // Move coin upwards
        while (elapsedTime < duration)
        {
            coin.transform.position = Vector3.Lerp(startPosition, peakPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // Move coin back down
        while (elapsedTime < duration)
        {
            coin.transform.position = Vector3.Lerp(peakPosition, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Hide the coin again after animation
        coin.SetActive(false);
    }
}
