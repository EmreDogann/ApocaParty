using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.1394896f, 0.4411765f, 0.3413077f)]
[TrackClipType(typeof(TextSwitcherClip))]
[TrackBindingType(typeof(TextMeshProUGUI))]
public class TextSwitcherTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TextSwitcherMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        TextMeshProUGUI trackBinding = director.GetGenericBinding(this) as TextMeshProUGUI;
        if (trackBinding == null)
        {
            return;
        }

        SerializedObject serializedObject = new SerializedObject(trackBinding);
        SerializedProperty iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.hasVisibleChildren)
            {
                continue;
            }

            driver.AddFromName<TextMeshProUGUI>(trackBinding.gameObject, iterator.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}