using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class InteractionSystem : MonoBehaviour
    {
        private readonly List<IInteractionHandler> _interactionHandlers = new List<IInteractionHandler>();
        // Item1 -> Assignment mode active/inactive
        // Item2 -> Callback to use in assignment mode.
        private static Tuple<bool, Action<InteractableBase>> _assignmentMode;

        private void Start()
        {
            _interactionHandlers.AddRange(GetComponents<IInteractionHandler>());
            _assignmentMode = new Tuple<bool, Action<InteractableBase>>(false, null);
        }

        private void Update()
        {
            foreach (IInteractionHandler handler in _interactionHandlers)
            {
                InteractableBase interactable = handler.CheckForInteraction();
                if (!handler.WasInteractedThisFrame())
                {
                    continue;
                }

                if (_assignmentMode.Item1 && _assignmentMode.Item2 != null)
                {
                    _assignmentMode.Item2(interactable);
                }
            }
        }

        public static void EnableAssignmentMode(Action<InteractableBase> callback)
        {
            if (!_assignmentMode.Item1)
            {
                _assignmentMode = new Tuple<bool, Action<InteractableBase>>(true, callback);
            }
        }

        public static void DisableAssignmentMode()
        {
            _assignmentMode = new Tuple<bool, Action<InteractableBase>>(false, null);
        }
    }
}