using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    // DownstairsBedroomScare
    void OnGettingUpComplete()
    {
        GhostEventManager.Instance.isSlowGettingUpFinished = true;
    }

    // DownstairsBedroomScare
    void OnRunningAndHittingComplete()
    {
        GhostEventManager.Instance.isRunAndHitFinished = true;
    }

    void OnCrawlBackComplete()
    {
        GhostEventManager.Instance.isCrawlBackFinished = true;
    }
}
