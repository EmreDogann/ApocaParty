using UnityEngine;

public class ParticlesPlayOnActive : MonoBehaviour
{
    [SerializeField] private bool alsoControlChildren;
    [SerializeField] private ParticleSystem _particleSystem;

    private void OnEnable()
    {
        if (_particleSystem)
        {
            _particleSystem.Play(alsoControlChildren);
        }
    }

    private void OnDisable()
    {
        if (_particleSystem)
        {
            _particleSystem.Stop(alsoControlChildren);
        }
    }
}