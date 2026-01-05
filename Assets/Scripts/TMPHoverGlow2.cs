using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPHoverGlow2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material normalMat;
    public Material hoverMat;

    private TMP_Text tmp;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        if (tmp != null && normalMat != null)
            tmp.fontMaterial = normalMat;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmp != null && hoverMat != null)
            tmp.fontMaterial = hoverMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmp != null && normalMat != null)
            tmp.fontMaterial = normalMat;
    }
}
