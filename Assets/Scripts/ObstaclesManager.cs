using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
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
            RewardController rewardController = child.GetComponent<RewardController>();
            BlockSpawnController blockSpawnController = child.GetComponent<BlockSpawnController>();
            if (rewardController != null)
            {
                rewardController.GameRestart();
            }
            if (blockSpawnController != null)
            {
                blockSpawnController.GameRestart();
            }
        }
    }

    void Awake()
    {
        // other instructions
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }
}
