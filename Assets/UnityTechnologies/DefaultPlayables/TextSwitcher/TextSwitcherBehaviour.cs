using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TextSwitcherBehaviour : PlayableBehaviour
{
    public bool keepTextOnFinish;
    public Color color = Color.white;
    public float fontSize = 14;
    public string text;

    public bool becameInactiveThisFrame;

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        becameInactiveThisFrame = true;
    }
}