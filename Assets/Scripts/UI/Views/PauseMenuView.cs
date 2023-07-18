using Events;
using UnityEngine;

namespace UI.Views
{
    public class PauseMenuView : View
    {
        [SerializeField] private BoolEventChannelSO OnGamePauseEvent;

        internal override void Open(bool beingSwapped)
        {
            transform.parent.gameObject.SetActive(true);
            if (!beingSwapped)
            {
                OnGamePauseEvent.Raise(true);
            }

            base.Open(beingSwapped);
        }

        internal override void Close()
        {
            if (UIManager.Instance.IsOnlyView())
            {
                OnGamePauseEvent.Raise(false);
            }

            base.Close();
        }
    }
}