using UnityEngine;

public class ScaleToScreenSize : MonoBehaviour
{
    private Vector2 _lastScreenSize;

    private void Awake()
    {
        Resize();
    }

    // private void Update()
    // {
    //     Vector2 screenSize = new Vector2(Screen.width, Screen.height);
    //
    //     if (_lastScreenSize != screenSize)
    //     {
    //         _lastScreenSize = screenSize;
    //         Resize();
    //     }
    // }

    private void Resize()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(worldScreenWidth / spriteRenderer.sprite.bounds.size.x,
            worldScreenHeight / spriteRenderer.sprite.bounds.size.y, 1);
    }
}