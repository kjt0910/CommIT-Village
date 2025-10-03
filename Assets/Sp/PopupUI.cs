using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// </summary>
public class PopupUI : MonoBehaviour
{
    [Header("��������")]
    public float duration = 0.3f;        // ��������ʱ��
    public float startScale = 0.7f;      // ����ʱ��ʼ����
    public Ease easeIn = Ease.OutBack;   // �򿪶�������
    public Ease easeOut = Ease.InBack;   // �رն�������

    [Header("��������")]
    [SerializeField] private Image mask; // ��͸�����֣����� Button��
    public float maskAlpha = 0.6f;       // �������͸����
    public float maskFadeTime = 0.2f;    // ���ֵ��뵭��ʱ��

    private Vector3 originalScale;       // ԭʼ����
    private CanvasGroup maskCanvasGroup; // ����͸���ȿ���

    private Tween scaleTween;            // ��ǰ���Ŷ���
    private Tween maskTween;             // ��ǰ���ֶ���

    void Awake()
    {
        originalScale = transform.localScale;
        gameObject.SetActive(false);

        if (mask != null)
        {
            // ����͸���ȿ���
            maskCanvasGroup = mask.GetComponent<CanvasGroup>();
            if (maskCanvasGroup == null)
                maskCanvasGroup = mask.gameObject.AddComponent<CanvasGroup>();

            maskCanvasGroup.alpha = 0;
            mask.gameObject.SetActive(false);

            // �������ʱ�رյ���
            Button maskBtn = mask.GetComponent<Button>();
            if (maskBtn != null)
                maskBtn.onClick.AddListener(PopClose);
        }
    }

    /// <summary>
    /// �� Popup
    /// </summary>
    public void PopOpen()
    {
        gameObject.SetActive(true);

        // ���ֵ���
        if (mask != null)
        {
            mask.gameObject.SetActive(true);
            maskTween?.Kill();
            maskCanvasGroup.alpha = 0;
            maskTween = maskCanvasGroup.DOFade(maskAlpha, maskFadeTime);
        }

        // �������Ŷ���
        scaleTween?.Kill();
        transform.localScale = originalScale * startScale;
        scaleTween = transform.DOScale(originalScale, duration).SetEase(easeIn);
    }

    /// <summary>
    /// �ر� Popup
    /// </summary>
    public void PopClose()
    {
        // ���ֵ���
        if (mask != null)
        {
            maskTween?.Kill();
            maskTween = maskCanvasGroup.DOFade(0, maskFadeTime)
                .OnComplete(() => mask.gameObject.SetActive(false));
        }

        // �������Ŷ���
        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale * startScale, duration)
            .SetEase(easeOut)
            .OnComplete(() => gameObject.SetActive(false));
    }

    void OnDestroy()
    {
        scaleTween?.Kill();
        maskTween?.Kill();
    }
}
