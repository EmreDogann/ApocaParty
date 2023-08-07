using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Canvas))]
public class FollowSpriteSortingUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRendererToFollow;
    [SerializeField] private int sortOrderOffset;
    [SerializeField] private bool runOnUpdate;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (spriteRendererToFollow)
        {
            _canvas.sortingLayerID = spriteRendererToFollow.sortingLayerID;
            _canvas.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
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
    }

    private void Update()
    {
        if (runOnUpdate)
        {
            _canvas.sortingLayerID = spriteRendererToFollow.sortingLayerID;
            _canvas.sortingOrder = spriteRendererToFollow.sortingOrder + sortOrderOffset;
        }
    }
}