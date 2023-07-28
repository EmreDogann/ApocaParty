using Audio;
using MyBox;
using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "Actor/New Actor", order = 0)]
    public class ActorSO : ScriptableObject
    {
        public new string name;
        [OverrideLabel("Dialogue Avatar")] public Sprite sprite;
        public AudioSO voice;
    }
}