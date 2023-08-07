using UnityEngine;

[ExecuteAlways]
public class ScaleToScreenSize : MonoBehaviour
{
    [SerializeField] private bool runOnAwake;
    [SerializeField] private bool runOnUpdate;

    private Vector2 _lastScreenSize;

    private void Awake()
    {
        if (runOnAwake)
        {
            Resize();
        }
    }

    private void Update()
    {
        if (!runOnUpdate)
        {
            return;
        }

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        if (_lastScreenSize != screenSize)
        {
            _lastScreenSize = screenSize;
            Resize();
        }
    }

    private void Resize()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(worldScreenWidth / spriteRenderer.sprite.bounds.size.x,
            worldScreenHeight / spriteRenderer.sprite.bounds.size.y, 1);
    }
}