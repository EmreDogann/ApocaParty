using Needs;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class ChangeSprite : Job
    {
        [SerializeField] private SpriteRenderer requestSpriteRenderer;
        [SerializeField] private Sprite icon;

        public override void Enter(IRequestOwner owner, ref NeedMetrics metrics)
        {
            base.Enter(owner, ref metrics);
            requestSpriteRenderer.sprite = icon;
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            return 1.0f;
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return 0.0f;
        }
    }
}