using UnityEngine;

namespace GuestRequests.Jobs
{
    public class ChangeSprite : Job
    {
        [SerializeField] private SpriteRenderer requestSpriteRenderer;
        [SerializeField] private Sprite icon;

        public override void Enter()
        {
            base.Enter();
            requestSpriteRenderer.sprite = icon;
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