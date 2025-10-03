using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class ClockSelectorUI : MonoBehaviour
{
    [Header("����")]
    public RectTransform pointer;          // ָ�루����ת���Ǹ���
    public List<RectTransform> icons;      // Ȧ�ϵ�ͼ�꣨��ֹ��

    [Header("���֣���ѡ�Զ��Ų���")]
    public bool autoArrangeInCircle = false;
    public float radius = 220f;
    public float startAngleDeg = 90f;      // 0�����ұߣ�90�����Ϸ��������12��

    [Header("��ת����")]
    public float stepDuration = 0.35f;
    public Ease stepEase = Ease.InOutSine;
    public float stepInterval = 0.0f;      // >0 ʱ�Զ�ÿ�� interval ��һ����=0 ���� OnStepNext
    public bool autoStart = true;

    [Header("ѡ�ж���")]
    public float normalScale = 1.0f;
    public float selectedScale = 1.18f;
    public float scaleDuration = 0.18f;
    public Ease selectEase = Ease.OutBack;
    public Ease deselectEase = Ease.OutQuad;
    public bool punchOnFinal = true;
    public float punch = 0.08f;
    public float punchDur = 0.15f;

    [Header("����")]
    public bool useUnscaledTime = false; // ��ͣʱҲ�ܶ����Ļ�����
    public bool clockwise = true;

    public int CurrentIndex { get; private set; } = 0;
    public bool IsRunning { get; private set; }

    private int _lastIndex = -1;
    private float _accTimer = 0f;
    private Tween _rotateTween;

    void Reset()
    {
        pointer = GetComponentInChildren<RectTransform>();
    }

    void Awake()
    {
        DOTween.Init(false, true, LogBehaviour.Default);
        DOTween.defaultUpdateType = useUnscaledTime ? UpdateType.Late : UpdateType.Normal;
        DOTween.defaultTimeScaleIndependent = useUnscaledTime;

        // ��ʼ���� icon ����Ϊ normal
        foreach (var icon in icons)
        {
            if (icon) icon.localScale = Vector3.one * normalScale;
        }
        // ��ʼ������0��
        HighlightIndex(0, instant: true);

        if (autoArrangeInCircle) ArrangeIconsInCircle();
    }

    void Start()
    {
        if (autoStart) StartRun();
    }

    void Update()
    {
        if (!IsRunning || stepInterval <= 0f) return;
        _accTimer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (_accTimer >= stepInterval)
        {
            _accTimer = 0f;
            StepNext();
        }
    }

    // === ���� API ===

    public void StartRun()
    {
        if (icons == null || icons.Count == 0 || pointer == null) return;
        IsRunning = true;
        _accTimer = 0f;
    }

    public void StopRun()
    {
        IsRunning = false;
        _rotateTween?.Kill();
    }

    public void StepNext()
    {
        if (!IsRunning) return;
        int next = (CurrentIndex + (clockwise ? 1 : -1) + icons.Count) % icons.Count;
        RotateToIndex(next, stepDuration, stepEase, loopAfter: true);
    }

    /// <summary>
    /// ͣ��ָ�� index�����һ�¿ɸ�˿������ͣ�º󱣳�ѡ�зŴ�
    /// </summary>
    public void StopAt(int index, float extraSpinTurns = 0f, float finalDur = 0.5f, Ease finalEase = Ease.OutCubic)
    {
        // ��ֹͣ��������
        IsRunning = false;
        _accTimer = 0f;

        // ����Ŀ��ǣ��������ת��Ȧ����㣩
        float targetAngle = IndexToPointerAngle(index);
        float currentZ = pointer.eulerAngles.z;

        // �ѽǶȲ����������ټ���Ȧ
        float delta = Mathf.DeltaAngle(currentZ, targetAngle);
        float spin = delta + 360f * (clockwise ? -extraSpinTurns : extraSpinTurns);

        _rotateTween?.Kill();
        _rotateTween = pointer.DORotate(new Vector3(0, 0, currentZ + spin), finalDur, RotateMode.Fast)
            .SetEase(finalEase)
            .SetUpdate(useUnscaledTime)
            .OnUpdate(() =>
            {
                // ����ָ��Ƕȸ��¡���ǰ�������ĸ�����ʵʱ��
                int idx = AngleToNearestIndex(pointer.eulerAngles.z);
                if (idx != _lastIndex) HighlightIndex(idx, instant: false);
            })
            .OnComplete(() =>
            {
                CurrentIndex = index;
                HighlightIndex(CurrentIndex, instant: false, final: true);
            });
    }

    // === �ڲ� ===

    private void RotateToIndex(int nextIndex, float duration, Ease ease, bool loopAfter)
    {
        float targetAngle = IndexToPointerAngle(nextIndex);
        float currentZ = pointer.eulerAngles.z;
        float delta = Mathf.DeltaAngle(currentZ, targetAngle);

        _rotateTween?.Kill();
        _rotateTween = pointer.DORotate(new Vector3(0, 0, currentZ + delta), duration, RotateMode.Fast)
            .SetEase(ease)
            .SetUpdate(useUnscaledTime)
            .OnUpdate(() =>
            {
                int idx = AngleToNearestIndex(pointer.eulerAngles.z);
                if (idx != _lastIndex) HighlightIndex(idx, instant: false);
            })
            .OnComplete(() =>
            {
                CurrentIndex = nextIndex;
                if (loopAfter && IsRunning && stepInterval <= 0f)
                {
                    // ���û��ʱ����������ʽ������һ��
                    StepNext();
                }
            });
    }

    private void HighlightIndex(int index, bool instant, bool final = false)
    {
        // ȡ����һ��
        if (_lastIndex >= 0 && _lastIndex < icons.Count && icons[_lastIndex])
        {
            var last = icons[_lastIndex];
            last.DOKill(true);
            if (instant) last.localScale = Vector3.one * normalScale;
            else last.DOScale(normalScale, scaleDuration * 0.8f).SetEase(deselectEase).SetUpdate(useUnscaledTime);
        }

        // ѡ����
        if (index >= 0 && index < icons.Count && icons[index])
        {
            var cur = icons[index];
            cur.DOKill(true);
            if (instant) cur.localScale = Vector3.one * selectedScale;
            else cur.DOScale(selectedScale, scaleDuration).SetEase(selectEase).SetUpdate(useUnscaledTime)
                   .OnComplete(() => {
                       if (final && punchOnFinal)
                           cur.DOPunchScale(Vector3.one * punch, punchDur, 8, 0.6f).SetUpdate(useUnscaledTime);
                   });
        }

        _lastIndex = index;
    }

    // ������ӳ�䵽��ָ��Ƕȡ���ָ����������ģ�
    private float IndexToPointerAngle(int index)
    {
        int n = Mathf.Max(icons.Count, 1);
        float step = 360f / n;
        // ָ��Ĭ��ָ�� startAngleDeg��index=0����˳ʱ����ת = �Ƕȼ�С
        float angle = startAngleDeg + (clockwise ? -index * step : index * step);
        return Normalize(angle);
    }

    // ����ָ�뵱ǰ�ǣ�����ӽ��� index
    private int AngleToNearestIndex(float pointerZ)
    {
        int n = Mathf.Max(icons.Count, 1);
        float step = 360f / n;

        float diff = Mathf.DeltaAngle(startAngleDeg, pointerZ);
        float raw = clockwise ? -diff / step : diff / step;

        int idx = Mathf.RoundToInt(raw);
        idx = (idx % n + n) % n; // �淶���� [0, n)
        return idx;
    }

    private float Normalize(float deg)
    {
        deg %= 360f;
        if (deg < 0f) deg += 360f;
        return deg;
    }

    private void ArrangeIconsInCircle()
    {
        int n = icons.Count;
        if (n == 0) return;
        float step = 360f / n;
        for (int i = 0; i < n; i++)
        {
            var rt = icons[i];
            if (!rt) continue;
            float ang = Mathf.Deg2Rad * (startAngleDeg + (clockwise ? -i * step : i * step));
            Vector2 pos = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
            rt.anchoredPosition = pos;
            rt.localScale = Vector3.one * (i == 0 ? selectedScale : normalScale);
        }
        _lastIndex = 0;
        CurrentIndex = 0;
    }

    // ���� �����Ǳ�ݰ�ť���ɰ� UI Button������
    public void UI_Start() => StartRun();
    public void UI_Stop() => StopRun();
    public void UI_StepNext() => StepNext();
    public void UI_StopAtRandom()
    {
        if (icons == null || icons.Count == 0) return;
        int idx = UnityEngine.Random.Range(0, icons.Count);
        StopAt(idx, extraSpinTurns: 1.5f, finalDur: 1.1f, finalEase: Ease.OutCubic);
    }
}
