using UnityEngine;
using DG.Tweening;

/// <summary>
/// ���ڡ����������������ʾ�Ķ�����
/// ���ܣ�������˸����������������Լ����򿪵�¼����
/// </summary>
public class PressAnyKeyToLogin : MonoBehaviour
{
    [Header("����")]
    public PopupUI loginPopup;          // ��¼����
    public CanvasGroup group;           // ��������͸���ȣ������壩

    [Header("������˸����")]
    [Range(0f, 1f)] public float fadeMin = 0.35f;
    [Range(0f, 1f)] public float fadeMax = 1.0f;
    public float fadeDuration = 1.2f;

    public float scaleMin = 0.98f;
    public float scaleMax = 1.04f;
    public float scaleDuration = 1.6f;

    private bool triggered = false;
    private Tweener fadeTween;
    private Tweener scaleTween;

    void Awake()
    {
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        }

        group.alpha = fadeMax;
        transform.localScale = Vector3.one;
    }

    void OnEnable()
    {
        // ������˸
        fadeTween = group.DOFade(fadeMin, fadeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        scaleTween = transform.DOScale(scaleMax, scaleDuration)
            .From(scaleMin)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        if (triggered) return;

        if (Input.anyKeyDown ||
            Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))
        {
            TriggerLogin();
        }
    }

    private void TriggerLogin()
    {
        triggered = true;

        // ֹͣ��������
        fadeTween?.Kill();
        scaleTween?.Kill();

        // С��������
        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(0f, 0.2f));
        seq.Join(transform.DOScale(0.97f, 0.2f));
        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);

            if (loginPopup != null)
                loginPopup.PopOpen();
            else
                Debug.LogWarning("[PressAnyKeyToLogin] δ�� loginPopup��");
        });
    }

    void OnDisable()
    {
        fadeTween?.Kill();
        scaleTween?.Kill();
    }
}
