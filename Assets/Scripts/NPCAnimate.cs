using UnityEngine;

public class SimpleIdleAnimation : MonoBehaviour
{
    public Sprite[] idleSprites; // 把两帧放在这里
    public float switchTime = 0.5f; // 每0.5秒切换一次

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= switchTime)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % idleSprites.Length;
            spriteRenderer.sprite = idleSprites[currentIndex];
        }
    }
}
