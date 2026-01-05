using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TMPPulseGlow : MonoBehaviour
{
    public Material baseMat; // возьми Hover материал (где включен Glow)

    [Header("Pulse")]
    [Range(0.05f, 10f)] public float speed = 2f;
    [Range(0f, 2f)] public float minGlow = 0.1f;
    [Range(0f, 2f)] public float maxGlow = 1.0f;

    private TMP_Text tmp;
    private Material runtimeMat;

    // В TMP material properties чаще всего glow power сидит в "_GlowPower"
    static readonly int GlowPower = Shader.PropertyToID("_GlowPower");

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        if (!tmp || !baseMat) return;

        // ВАЖНО: создаём инстанс, чтобы не менять материал у всех текстов
        runtimeMat = new Material(baseMat);
        tmp.fontMaterial = runtimeMat;
    }

    void Update()
    {
        if (!runtimeMat) return;

        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f; // 0..1
        float power = Mathf.Lerp(minGlow, maxGlow, t);

        // Если вдруг параметра нет — просто ничего не сломается
        if (runtimeMat.HasProperty(GlowPower))
            runtimeMat.SetFloat(GlowPower, power);
    }

    void OnDestroy()
    {
        if (runtimeMat) Destroy(runtimeMat);
    }
}
