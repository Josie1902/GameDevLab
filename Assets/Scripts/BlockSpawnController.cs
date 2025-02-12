using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject coin; // Assign the Coin object in Unity Inspector
    private Vector3 startPosition;
    private bool hit = false;
    public AudioSource coinAudio;

    void Start()
    {
        startPosition = coin.transform.position;
        coin.SetActive(false); // Hide the coin at the start
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hit)
        {
            hit = true;
            Debug.Log("Mario hits the block!");

            // Activate the coin
            coin.SetActive(true);
            coinAudio.PlayOneShot(coinAudio.clip);

            // Start the coin movement coroutine
            StartCoroutine(CoinMovements());
        }
    }

    IEnumerator CoinMovements()
    {
        float elapsedTime = 0f;
        float duration = 0.3f; // Adjust duration for smooth movement
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
