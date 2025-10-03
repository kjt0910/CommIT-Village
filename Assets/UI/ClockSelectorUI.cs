using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class ClockSelectorUI : MonoBehaviour
{
    
    public RectTransform pointer;          // 指针（会旋转的那个）
    public List<RectTransform> icons;      // 圈上的图标（静止）

    
    public bool autoArrangeInCircle = false;
    public float radius = 220f;
    public float startAngleDeg = 90f;      // 0°在右边；90°在上方，像表盘12点

   
    public float stepDuration = 0.35f;
    public Ease stepEase = Ease.InOutSine;
    public float stepInterval = 0.0f;      // >0 时自动每隔 interval 走一步；=0 仅靠 OnStepNext
    public bool autoStart = true;

    public float normalScale = 1.0f;
    public float selectedScale = 1.18f;
    public float scaleDuration = 0.18f;
    public Ease selectEase = Ease.OutBack;
    public Ease deselectEase = Ease.OutQuad;
    public bool punchOnFinal = true;
    public float punch = 0.08f;
    public float punchDur = 0.15f;

    public bool useUnscaledTime = false; // 暂停时也跑动画的话勾上
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

        // 初始所有 icon 缩放为 normal
        foreach (var icon in icons)
        {
            if (icon) icon.localScale = Vector3.one * normalScale;
        }
        // 初始高亮第0个
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

    // === 对外 API ===

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
    /// 停到指定 index（最后一下可更丝滑），停下后保持选中放大。
    /// </summary>
    public void StopAt(int index, float extraSpinTurns = 0f, float finalDur = 0.5f, Ease finalEase = Ease.OutCubic)
    {
        // 先停止连续驱动
        IsRunning = false;
        _accTimer = 0f;

        // 计算目标角（允许额外转几圈再落点）
        float targetAngle = IndexToPointerAngle(index);
        float currentZ = pointer.eulerAngles.z;

        // 把角度差处理到最近方向，再加整圈
        float delta = Mathf.DeltaAngle(currentZ, targetAngle);
        float spin = delta + 360f * (clockwise ? -extraSpinTurns : extraSpinTurns);

        _rotateTween?.Kill();
        _rotateTween = pointer.DORotate(new Vector3(0, 0, currentZ + spin), finalDur, RotateMode.Fast)
            .SetEase(finalEase)
            .SetUpdate(useUnscaledTime)
            .OnUpdate(() =>
            {
                // 根据指针角度更新“当前索引”的高亮（实时）
                int idx = AngleToNearestIndex(pointer.eulerAngles.z);
                if (idx != _lastIndex) HighlightIndex(idx, instant: false);
            })
            .OnComplete(() =>
            {
                CurrentIndex = index;
                HighlightIndex(CurrentIndex, instant: false, final: true);
            });
    }

    // === 内部 ===

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
                    // 如果没有时间间隔，就链式继续下一步
                    StepNext();
                }
            });
    }

    private void HighlightIndex(int index, bool instant, bool final = false)
    {
        // 取消上一个
        if (_lastIndex >= 0 && _lastIndex < icons.Count && icons[_lastIndex])
        {
            var last = icons[_lastIndex];
            last.DOKill(true);
            if (instant) last.localScale = Vector3.one * normalScale;
            else last.DOScale(normalScale, scaleDuration * 0.8f).SetEase(deselectEase).SetUpdate(useUnscaledTime);
        }

        // 选中新
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

    // 将索引映射到“指针角度”（指向该扇区中心）
    private float IndexToPointerAngle(int index)
    {
        int n = Mathf.Max(icons.Count, 1);
        float step = 360f / n;
        // 指针默认指向 startAngleDeg（index=0），顺时针旋转 = 角度减小
        float angle = startAngleDeg + (clockwise ? -index * step : index * step);
        return Normalize(angle);
    }

    // 根据指针当前角，算最接近的 index
    private int AngleToNearestIndex(float pointerZ)
    {
        int n = Mathf.Max(icons.Count, 1);
        float step = 360f / n;

        float diff = Mathf.DeltaAngle(startAngleDeg, pointerZ);
        float raw = clockwise ? -diff / step : diff / step;

        int idx = Mathf.RoundToInt(raw);
        idx = (idx % n + n) % n; // 规范化到 [0, n)
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

    // —— 下面是便捷按钮（可绑到 UI Button）——
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
