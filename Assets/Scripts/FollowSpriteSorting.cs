using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FollowSpriteSorting : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRendererToFollow;
    [SerializeField] private int sortOrderOffset;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sortingLayerID = spriteRendererToFollow.sortingLayerID;
        _spriteRenderer.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
    }

    private void Update()
    {
        _spriteRenderer.sortingLayerID = spriteRendererToFollow.sortingLayerID;
        _spriteRenderer.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
    }
}