using UnityEngine;

public class SimpleIdleAnimation : MonoBehaviour
{
    public Sprite[] idleSprites; // ����֡��������
    public float switchTime = 0.5f; // ÿ0.5���л�һ��

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
