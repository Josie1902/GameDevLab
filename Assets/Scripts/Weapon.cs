using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public Transform gunHoldPoint;
    public SpriteRenderer gunHoldPointSprite;
    public float bulletSpeed = 10f;
    public GameObject prefabBullet;
    public TextMeshPro bulletText;
    private AudioSource bulletAudioSource;
    public AudioSource engageAudioSource;

    private bool isPickedUp = false;
    private Vector2 bulletDirection;
    private Transform bulletRotation;
    private Animator weaponAnimatior;
    public int maxBullets = 5;
    private int bulletsFired = 0;
    public Vector3 startPosition;


    // Start is called before the first frame update
    void Start()
    {
        bulletRotation = prefabBullet.GetComponent<Transform>();
        bulletAudioSource = prefabBullet.GetComponent<AudioSource>();

        weaponAnimatior = GetComponent<Animator>();

        if (bulletText != null)
        {
            bulletText.text = $"{maxBullets} Bullets";
            bulletText.transform.position = transform.position + new Vector3(0.213f, 0.120f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {
            transform.position = gunHoldPoint.position;
            transform.rotation = gunHoldPoint.rotation;
        }
    }

    public void PickupGun()
    {
        if (!isPickedUp)
        {
            Debug.Log("Gun Picked up!");
            weaponAnimatior.enabled = false;
            bulletText.gameObject.SetActive(false);

            engageAudioSource.PlayOneShot(engageAudioSource.clip);

            transform.SetParent(gunHoldPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            GetComponent<BoxCollider2D>().enabled = false;

            isPickedUp = true;
        }
    }

    public void ClickToFire()
    {
        if (bulletsFired >= maxBullets)
        {
            Debug.Log("Out of bullets!");
            return;
        }
        if (isPickedUp)
        {
            Vector3 bulletPosition = gunHoldPoint.position + new Vector3(0.08f, 0f, 0f);
            GameObject bullet = Instantiate(prefabBullet, bulletPosition, bulletRotation.rotation);

            Vector3 marioLocalScale = gunHoldPoint.parent.localScale;

            bulletDirection = marioLocalScale.x < 0 ? -gunHoldPoint.right : gunHoldPoint.right;
            bulletAudioSource.PlayOneShot(bulletAudioSource.clip);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = bulletDirection * bulletSpeed;

            bulletsFired++;

            Destroy(bullet, 1f);
        }
    }

    public void GameRestart()
    {
        transform.position = startPosition;
        weaponAnimatior.enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        isPickedUp = false;
        bulletsFired = 0;
    }

    void Awake()
    {
        // other instructions
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }
}
