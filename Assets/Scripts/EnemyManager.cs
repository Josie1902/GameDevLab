using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        foreach (Transform child in transform)
        {
            EnemyMovement goomba = child.GetComponent<EnemyMovement>();
            ZombieMovement zombie = child.GetComponent<ZombieMovement>();
            if (goomba != null)
            {
                goomba.GameRestart();
            }
            if (zombie != null)
            {
                zombie.GameRestart();
            }
        }
    }

    void Awake()
    {
        // other instructions
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

}
