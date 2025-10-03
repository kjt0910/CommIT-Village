using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// �Ƽ����������Ч��������ǩ����߷��⡢�»���ɨ�⡢�������š�����ɫ
/// �÷�������ÿ�� TMP_InputField �ĸ�������
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class FancyInputFieldFX : MonoBehaviour
{
    [Header("Refs")]
    public TMP_InputField input;
    public RectTransform floatingLabel;   // �ڲ��ġ�User / Password����ǩ��Text/TMP��
    public Image glowOutline;             // ���ܷ�����ߣ�PNG/SVG��Image��
    public Image underline;               // �»��� Image������ɨ�⣩
    public RectTransform rootToPunch;     // ��ý���ʱ��΢���ŵ�������һ������Լ���

    [Header("Colors")]
    public Color glowIdle = new Color(0.1f, 0.7f, 1f, 0.35f);
    public Color glowFocus = new Color(0.3f, 0.9f, 1f, 0.85f);
    public Color caretColor = new Color(0.2f, 0.9f, 1f, 1f);

    [Header("Label motion")]
    public Vector2 labelFocusedOffset = new Vector2(0, 22); // �۽�/����ʱ����
    public float labelScaleFocused = 0.82f;
    public float labelAnim = 0.16f;

    [Header("Underline scan")]
    public float scanWidth = 0.2f;   // ����/�����Ҫ֧�� Maskable��һ�� Image OK��
    public float scanDur = 0.9f;

    [Header("Punch")]
    public float punchScale = 0.035f;
    public float punchDur = 0.12f;

    private Vector2 _labelInitPos;
    private Vector3 _labelInitScale;
    private Color _caretOrig;
    private Tween _scanTween;

    void Reset()
    {
        input = GetComponent<TMP_InputField>();
        if (!rootToPunch) rootToPunch = (RectTransform)transform;
    }

    void Awake()
    {
        if (!input) input = GetComponent<TMP_InputField>();
        if (!rootToPunch) rootToPunch = (RectTransform)transform;
        if (floatingLabel) { _labelInitPos = floatingLabel.anchoredPosition; _labelInitScale = floatingLabel.localScale; }

        _caretOrig = input.caretColor;
        input.onSelect.AddListener(OnFocus);
        input.onDeselect.AddListener(OnBlur);
        input.onValueChanged.AddListener(OnValueChanged);

        ApplyStateInstant();
    }

    void OnDestroy()
    {
        input.onSelect.RemoveListener(OnFocus);
        input.onDeselect.RemoveListener(OnBlur);
        input.onValueChanged.RemoveListener(OnValueChanged);
        _scanTween?.Kill();
    }

    void OnEnable() => ApplyStateInstant();

    void OnFocus(string _)
    {
        // glow + caret
        if (glowOutline) glowOutline.DOColor(glowFocus, 0.12f);
        input.caretColor = caretColor;

        // label ����/��С
        MoveLabel(focusedOrHasText: true);

        // �»���ɨ��
        StartScan();

        // ��΢��һ��
        if (rootToPunch) rootToPunch.DOPunchScale(Vector3.one * punchScale, punchDur, 8, 0.8f);
    }

    void OnBlur(string _)
    {
        bool hasText = !string.IsNullOrEmpty(input.text);
        if (glowOutline) glowOutline.DOColor(glowIdle, 0.18f);
        input.caretColor = _caretOrig;

        // ������ʱ��ǩ����
        MoveLabel(focusedOrHasText: hasText);

        // ֹͣɨ��
        _scanTween?.Kill();
    }

    void OnValueChanged(string _)
    {
        bool hasText = !string.IsNullOrEmpty(input.text) || input.isFocused;
        MoveLabel(focusedOrHasText: hasText);
    }

    void ApplyStateInstant()
    {
        bool hasText = !string.IsNullOrEmpty(input.text);
        if (glowOutline) glowOutline.color = hasText || input.isFocused ? glowFocus : glowIdle;
        input.caretColor = input.isFocused ? caretColor : _caretOrig;
        SetLabelState(hasText || input.isFocused, instant: true);
    }

    void MoveLabel(bool focusedOrHasText)
    {
        SetLabelState(focusedOrHasText, instant: false);
    }

    void SetLabelState(bool up, bool instant)
    {
        if (!floatingLabel) return;
        Vector2 targetPos = up ? _labelInitPos + labelFocusedOffset : _labelInitPos;
        Vector3 targetScale = up ? Vector3.one * labelScaleFocused : _labelInitScale;
        if (instant)
        {
            floatingLabel.anchoredPosition = targetPos;
            floatingLabel.localScale = targetScale;
        }
        else
        {
            floatingLabel.DOAnchorPos(targetPos, labelAnim).SetEase(Ease.OutQuad);
            floatingLabel.DOScale(targetScale, labelAnim).SetEase(Ease.OutQuad);
        }
    }

    void StartScan()
    {
        if (!underline) return;
        _scanTween?.Kill();

        // ʹ�� Image.fillAmount ������������ʽ��ɨ��
        underline.type = Image.Type.Filled;
        underline.fillMethod = Image.FillMethod.Horizontal;
        underline.fillOrigin = (int)Image.OriginHorizontal.Left;
        underline.fillAmount = 0f;

        _scanTween = DOTween.Sequence()
            .Append(DOTween.To(() => underline.fillAmount, v => underline.fillAmount = v, scanWidth, scanDur * 0.35f))
            .Append(DOTween.To(() => underline.fillAmount, v => underline.fillAmount = v, 1f, scanDur * 0.65f))
            .SetLoops(-1, LoopType.Restart);
    }
}

