using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventIntTool : MonoBehaviour
{
    void OnTrigger()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.IncreaseScore();
        }
    }
}
