using UnityEngine;
using TMPro;
using DG.Tweening;

public class IconPopup : MonoBehaviour
{
    [Header("����")]
    public RectTransform panel;           // Popup ���
    public TMP_Text descriptionText;      // ˵������

    [Header("��������")]
    public float showScale = 1f;
    public float hideScale = 0.6f;
    public float duration = 0.25f;
    public Ease ease = Ease.OutBack;

    private Tween _tween;

    void Awake()
    {
        if (panel) panel.localScale = Vector3.one * hideScale;
        if (panel) panel.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (!panel) return;
        panel.gameObject.SetActive(true);
        if (descriptionText) descriptionText.text = message;

        _tween?.Kill();
        panel.localScale = Vector3.one * hideScale;
        _tween = panel.DOScale(showScale, duration).SetEase(ease);
    }

    public void Hide()
    {
        if (!panel) return;

        _tween?.Kill();
        _tween = panel.DOScale(hideScale, duration * 0.7f)
            .SetEase(Ease.InBack)
            .OnComplete(() => panel.gameObject.SetActive(false));
    }
}
