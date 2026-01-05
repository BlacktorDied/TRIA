using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyVisual : MonoBehaviour
{
    public KeyCode key;
    public Image background;
    public TMP_Text label;

    public Color normalColor = new Color(0.12f, 0.12f, 0.12f, 1f);
    public Color pressedColor = new Color(0f, 1f, 0.8f, 1f);

    void Update()
    {
        if (!background) return;
        background.color = (key != KeyCode.None && Input.GetKey(key)) ? pressedColor : normalColor;
    }
}
