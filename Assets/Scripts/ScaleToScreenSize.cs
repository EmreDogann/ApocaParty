using UnityEngine;

public class ScaleToScreenSize : MonoBehaviour
{
    private SpriteRenderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        Camera main = Camera.main;
        _renderer.size = new Vector2(main.scaledPixelWidth, main.scaledPixelHeight);
    }
}
