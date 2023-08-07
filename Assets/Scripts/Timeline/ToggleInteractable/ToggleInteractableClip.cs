using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.ToggleInteractable
{
    [Serializable]
    public class InteractableData
    {
        public ExposedReference<Collider2D> collider2DReference;
        public bool activeState;
        [HideInInspector] public Collider2D collider2D;
        [HideInInspector] public bool initialActiveState;
    }

    [Serializable]
    public class ToggleInteractableClip : PlayableAsset, ITimelineClipAsset, IPropertyPreview
    {
        [HideInInspector] public ToggleInteractableBehaviour template = new ToggleInteractableBehaviour();
        [SerializeField] private List<InteractableData> interactableDatas = new List<InteractableData>();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ToggleInteractableBehaviour>.Create(graph, template);
            ToggleInteractableBehaviour clone = playable.GetBehaviour();

            foreach (InteractableData interactableData in interactableDatas)
            {
                Collider2D newCollider2D =
                    interactableData.collider2DReference.Resolve(graph.GetResolver());

                if (newCollider2D == null)
                {
                    interactableData.collider2D = null;
                    continue;
                }

                if (newCollider2D != interactableData.collider2D)
                {
                    interactableData.collider2D = newCollider2D;
                    interactableData.initialActiveState = interactableData.collider2D.enabled;
                }
            }

            clone.interactableDatas = interactableDatas;
            return playable;
        }

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            foreach (InteractableData interactableData in interactableDatas)
            {
                if (interactableData.collider2D != null)
                {
                    driver.AddFromName<Collider2D>(interactableData.collider2D.gameObject, "m_Enabled");
                }
            }
        }
    }
}