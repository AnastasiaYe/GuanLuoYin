using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssignSprite : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    public Image img;

    void Update()
    {
        if (img.color == new Color(1f, 1f, 1f, 1f))
        {
            img.sprite = sprite1;
        }
        else if (img.color == new Color(0.99f, 0.99f, 0.99f, 1f))
        {
            img.sprite = sprite2;
        }
        else if (img.color == new Color(0.98f, 0.98f, 0.98f, 1f))
        {
            img.sprite = sprite3;
        }
        else if (img.color == new Color(0.97f, 0.97f, 0.97f, 1f))
        {
            img.sprite = sprite4;
        }
    }
}
