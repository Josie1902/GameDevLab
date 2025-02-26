using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHoldController : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer parentSprite;
    private SpriteRenderer holdSprite;

    void Start()
    {
        GameObject body = GameObject.Find("Mario");
        transform.SetParent(body.transform);
        transform.localPosition = new Vector3(0.03f, -0.018f, 0);
    }

    void Update()
    {
    }
}
