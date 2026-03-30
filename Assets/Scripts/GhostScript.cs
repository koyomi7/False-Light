using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public void OnAnimationComplete(string flagName)
    {
        var manager = GhostEventManager.Instance;
        var field = manager.GetType().GetField(flagName);

        if (field != null) field.SetValue(manager, true);
    }
}
