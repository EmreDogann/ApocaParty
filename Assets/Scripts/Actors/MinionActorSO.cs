using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(fileName = "New Minion Actor", menuName = "Actor/New Minion Actor", order = 1)]
    public class MinionActorSO : ActorSO
    {
        public Sprite defaultIcon;
        public Sprite kitchenIcon;
        public Sprite musicIcon;
        public Sprite eventIcon;
    }
}