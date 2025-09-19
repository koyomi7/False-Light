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

    // DownstairsBedroomScare
    void OnCrawlBackComplete()
    {
        GhostEventManager.Instance.isCrawlBackFinished = true;
    }

    // DownstairsLivingRoomScare
    void OnFallingComplete()
    {
        GhostEventManager.Instance.isFallingFinished = true;
    }

    // DownstairsLivingRoomScare
    void OnVanishComplete()
    {
        GhostEventManager.Instance.isVanishFinished = true;
    }

    // DownstairsHallwayScare
    void OnWalkingBackComplete()
    {
        GhostEventManager.Instance.isWalkingBackFinished = true;
    }
}
