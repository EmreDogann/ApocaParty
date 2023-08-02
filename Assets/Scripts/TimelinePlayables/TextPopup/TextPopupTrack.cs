using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.TextPopup
{
    [TrackColor(0.45928f, 0f, 0.745283f)]
    [TrackClipType(typeof(TextPopupClip))]
    [TrackBindingType(typeof(TextMeshProUGUI))]
    public class TextPopupTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TextPopupMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            const string kText = "m_text";
            const string kLocalScale = "m_LocalScale";

            TextMeshProUGUI trackBinding = director.GetGenericBinding(this) as TextMeshProUGUI;
            if (trackBinding == null)
            {
                return;
            }

            GameObject trackBindingGo = trackBinding.gameObject;

            driver.AddFromName<TextMeshProUGUI>(trackBindingGo, kText);

            driver.AddFromName<Transform>(trackBindingGo, kLocalScale + ".x");
            driver.AddFromName<Transform>(trackBindingGo, kLocalScale + ".y");
            driver.AddFromName<Transform>(trackBindingGo, kLocalScale + ".z");
        }
    }
}