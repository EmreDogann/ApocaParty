using MyBox;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Canvas))]
public class FollowSpriteSortingUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRendererToFollow;
    [SerializeField] private int sortOrderOffset;
    [SerializeField] private bool runOnUpdate;

    [OverrideLabel("Is Active")] [SerializeField] private bool _isFollowing;
    [SpriteLayer] [SerializeField] private int disabledSortLayer;
    [SerializeField] private int disabledSortOrder;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (spriteRendererToFollow)
        {
            _canvas.sortingLayerID = spriteRendererToFollow.sortingLayerID;
            _canvas.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
        }

        if (!_isFollowing)
        {
            _canvas.sortingLayerID = disabledSortLayer;
            _canvas.sortingOrder = disabledSortOrder;
        }
    }

    private void OnValidate()
    {
        if (!_canvas)
        {
            _canvas = GetComponent<Canvas>();
        }

        if (spriteRendererToFollow)
        {
            _canvas.sortingLayerID = spriteRendererToFollow.sortingLayerID;
            _canvas.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
        }

        if (!_isFollowing)
        {
            _canvas.sortingLayerID = disabledSortLayer;
            _canvas.sortingOrder = disabledSortOrder;
        }
    }

    private void Update()
    {
        if (runOnUpdate && _isFollowing)
        {
            _canvas.sortingLayerID = spriteRendererToFollow.sortingLayerID;
            _canvas.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
        }
    }

    public void SetFollowing(bool isFollowing)
    {
        _isFollowing = isFollowing;

        if (!_isFollowing)
        {
            _canvas.sortingLayerID = disabledSortLayer;
            _canvas.sortingOrder = disabledSortOrder;
        }
    }
}