using UnityEngine;
using UnityEngine.UI;

public class BackgroundFlicker : MonoBehaviour
{
    [SerializeField] private Sprite frame1;
    [SerializeField] private Sprite frame2;
    [SerializeField] private float flickerSpeed = 0.5f;

    private Image backgroundImage;
    private float timer = 0f;
    private bool useFrame1 = true;

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        backgroundImage.sprite = frame1;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flickerSpeed)
        {
            useFrame1 = !useFrame1;
            backgroundImage.sprite = useFrame1 ? frame1 : frame2;
            timer = 0f;
        }
    }
}