using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class FollowSpriteSorting : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRendererToFollow;
    [SerializeField] private int sortOrderOffset;

    private Renderer _renderer;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.sortingLayerID = spriteRendererToFollow.sortingLayerID;
        _renderer.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
    }

    private void OnValidate()
    {
        if (!_renderer)
        {
            _renderer = GetComponent<Renderer>();
        }
    }

    private void LateUpdate()
    {
        _renderer.sortingLayerID = spriteRendererToFollow.sortingLayerID;
        _renderer.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
    }
}