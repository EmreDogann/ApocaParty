using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public class InteractionSystem : MonoBehaviour
    {
        private readonly List<IInteractionHandler> _interactionHandlers = new List<IInteractionHandler>();

        private void Start()
        {
            _interactionHandlers.AddRange(GetComponents<IInteractionHandler>());
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            foreach (IInteractionHandler handler in _interactionHandlers)
            {
                // Don't check for interactions is the mouse is over a UI element.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                handler.CheckForInteraction();
            }
        }
    }
}