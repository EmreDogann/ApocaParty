using UnityEngine;

namespace GuestRequests.Jobs
{
    public class ShowOrHideSprite : Job
    {
        [SerializeField] private SpriteRenderer _requestSpriteRenderer;
        [SerializeField] private bool _show;

        public override void Enter()
        {
            base.Enter();
            _requestSpriteRenderer.enabled = _show;
        }

        public override float GetProgressPercentage()
        {
            return 1.0f;
        }

        public override float GetTotalDuration()
        {
            return 0.0f;
        }
    }
}