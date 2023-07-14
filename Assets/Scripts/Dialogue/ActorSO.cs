using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "Dialogue/New Actor", order = 0)]
    public class ActorSO : ScriptableObject
    {
        public new string name;
        public Sprite sprite;
    }
}