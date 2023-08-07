using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
public class ScaleColliderToUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransformToMatch;
    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_rectTransformToMatch)
        {
            _boxCollider2D.size = new Vector2(
                _rectTransformToMatch.sizeDelta.x,
                _rectTransformToMatch.sizeDelta.y
            );
        }
    }
}