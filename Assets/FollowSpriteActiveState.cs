using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FollowSpriteActiveState : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteToFollow;
    private SpriteRenderer _thisSpriteRenderer;

    private void Awake()
    {
        _thisSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _thisSpriteRenderer.enabled = spriteToFollow.enabled;

        _thisSpriteRenderer.sortingLayerID = spriteToFollow.sortingLayerID;
        _thisSpriteRenderer.sortingOrder = spriteToFollow.sortingOrder;
    }
}