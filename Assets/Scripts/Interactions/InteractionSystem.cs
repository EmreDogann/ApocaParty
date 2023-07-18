using System;
using System.Collections.Generic;
using Minion.States;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public class InteractionSystem : MonoBehaviour
    {
        private readonly List<IInteractionHandler> _interactionHandlers = new List<IInteractionHandler>();
        // Item1 -> Assignment mode active/inactive
        // Item2 -> Callback to use in assignment mode.
        private Tuple<bool, Action<InteractableBase>> _assignmentMode;

        private void Start()
        {
            _interactionHandlers.AddRange(GetComponents<IInteractionHandler>());
            _assignmentMode = new Tuple<bool, Action<InteractableBase>>(false, null);
        }

        private void OnEnable()
        {
            MinionAssignmentState.OnMinionInteract += ToggleAssignmentMode;
        }

        private void OnDisable()
        {
            MinionAssignmentState.OnMinionInteract -= ToggleAssignmentMode;
        }

        private void Update()
        {
            foreach (IInteractionHandler handler in _interactionHandlers)
            {
                // Don't check for interactions is the mouse is over a UI element.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                InteractableBase interactable = handler.CheckForInteraction(_assignmentMode.Item1);
                if (!handler.WasInteractedThisFrame())
                {
                    continue;
                }

                if (_assignmentMode.Item1 && _assignmentMode.Item2 != null)
                {
                    _assignmentMode.Item2(interactable);
                    _assignmentMode = new Tuple<bool, Action<InteractableBase>>(false, null);
                }
            }
        }

        private void ToggleAssignmentMode(Action<InteractableBase> callback)
        {
            if (!_assignmentMode.Item1)
            {
                _assignmentMode = new Tuple<bool, Action<InteractableBase>>(true, callback);
            }
        }
    }
}