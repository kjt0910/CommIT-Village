using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// </summary>
public class PopupUI : MonoBehaviour
{
    [Header("动画设置")]
    public float duration = 0.3f;        // 弹窗动画时间
    public float startScale = 0.7f;      // 弹出时初始缩放
    public Ease easeIn = Ease.OutBack;   // 打开动画曲线
    public Ease easeOut = Ease.InBack;   // 关闭动画曲线

    [Header("遮罩设置")]
    [SerializeField] private Image mask; // 半透明遮罩（需是 Button）
    public float maskAlpha = 0.6f;       // 遮罩最大透明度
    public float maskFadeTime = 0.2f;    // 遮罩淡入淡出时间

    private Vector3 originalScale;       // 原始缩放
    private CanvasGroup maskCanvasGroup; // 遮罩透明度控制

    private Tween scaleTween;            // 当前缩放动画
    private Tween maskTween;             // 当前遮罩动画

    void Awake()
    {
        originalScale = transform.localScale;
        gameObject.SetActive(false);

        if (mask != null)
        {
            // 遮罩透明度控制
            maskCanvasGroup = mask.GetComponent<CanvasGroup>();
            if (maskCanvasGroup == null)
                maskCanvasGroup = mask.gameObject.AddComponent<CanvasGroup>();

            maskCanvasGroup.alpha = 0;
            mask.gameObject.SetActive(false);

            // 点击遮罩时关闭弹窗
            Button maskBtn = mask.GetComponent<Button>();
            if (maskBtn != null)
                maskBtn.onClick.AddListener(PopClose);
        }
    }

    /// <summary>
    /// 打开 Popup
    /// </summary>
    public void PopOpen()
    {
        gameObject.SetActive(true);

        // 遮罩淡入
        if (mask != null)
        {
            mask.gameObject.SetActive(true);
            maskTween?.Kill();
            maskCanvasGroup.alpha = 0;
            maskTween = maskCanvasGroup.DOFade(maskAlpha, maskFadeTime);
        }

        // 弹窗缩放动画
        scaleTween?.Kill();
        transform.localScale = originalScale * startScale;
        scaleTween = transform.DOScale(originalScale, duration).SetEase(easeIn);
    }

    /// <summary>
    /// 关闭 Popup
    /// </summary>
    public void PopClose()
    {
        // 遮罩淡出
        if (mask != null)
        {
            maskTween?.Kill();
            maskTween = maskCanvasGroup.DOFade(0, maskFadeTime)
                .OnComplete(() => mask.gameObject.SetActive(false));
        }

        // 弹窗缩放动画
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
