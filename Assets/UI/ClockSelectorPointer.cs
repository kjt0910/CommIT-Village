using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ClockSelectorPointer : MonoBehaviour
{
    
    public RectTransform pointer;            // 旋转的指针（pivot=0.5,0）
    public RectTransform pointerTip;         // 指针尖端（在 Pointer 的最前端）
    public RectTransform center;             // 表盘中心（通常=本物体）
    public List<RectTransform> icons = new List<RectTransform>();

    
    public bool autoArrangeCircle = true;    // 勾上：运行时自动排布
    public float radius = 220f;              // 半径
    public float startAngleDeg = 90f;        // 0°在右边；90°在上（12点）
    public bool clockwise = true;            // 顺/逆时针布置
    public bool normalizeAnchors = true;     // 排布前归一化 anchor/pivot

    
    public float stepDegrees = 60f;          // 每步转多少度
    public float stepDuration = 0.35f;
    public Ease stepEase = Ease.InOutSine;
    public float stepInterval = 0.0f;        // >0 自动每隔 interval 走一步
    public bool autoStart = true;
    public bool useUnscaledTime = false;

    
    public List<IconPopup> iconPopups = new List<IconPopup>(); // 与 icons 一一对应

    
    public List<IconItem> items = new List<IconItem>();

    
    public float normalScale = 1.0f;
    public float selectedScale = 1.2f;
    public float scaleDuration = 0.18f;
    public Ease selectEase = Ease.OutBack;
    public Ease deselectEase = Ease.OutQuad;
    public bool punchOnFinal = true;
    public float punch = 0.08f;
    public float punchDur = 0.15f;

    
    public bool alignPointerOnHover = true;  // 悬停时让指针对齐该按钮
    public float hoverAlignDur = 0.2f;
    public Ease hoverAlignEase = Ease.OutCubic;

    public int CurrentIndex { get; private set; } = -1;
    public bool IsRunning { get; private set; }

    private float _accTimer = 0f;
    private Tween _rotateTween;

    private int _manualHighlightIndex = -1; // 手动高亮模式

    void Awake()
    {
        if (!center) center = (RectTransform)transform;

        DOTween.defaultUpdateType = useUnscaledTime ? UpdateType.Late : UpdateType.Normal;
        DOTween.defaultTimeScaleIndependent = useUnscaledTime;

        if (autoArrangeCircle) ArrangeIconsInCircle();

        foreach (var rt in icons)
            if (rt) rt.localScale = Vector3.one * normalScale;

        UpdateSelectionFromPointer(force: true);
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

    // ======= 圆形排布 =======
    [ContextMenu("Arrange Icons In Circle (Now)")]
    public void ArrangeIconsInCircle()
    {
        int n = icons.Count;
        if (n == 0) return;

        float step = 360f / n;
        for (int i = 0; i < n; i++)
        {
            var rt = icons[i];
            if (!rt) continue;

            if (normalizeAnchors)
            {
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
            }

            float ang = startAngleDeg + (clockwise ? -i * step : i * step);
            float rad = ang * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            rt.anchoredPosition = pos;
            rt.localRotation = Quaternion.identity;

            var relay = rt.GetComponent<IconHoverRelay>();
            if (relay)
            {
                relay.parent = this;
                relay.index = i;
            }

            // 如果 icon 上有 IconPopup，自动填充到 iconPopups
            var ip = rt.GetComponentInChildren<IconPopup>(true);
            if (ip)
            {
                if (iconPopups.Count <= i) iconPopups.Add(ip);
                else iconPopups[i] = ip;
            }
        }

        CurrentIndex = 0;
    }

    // ======= 控制 API =======
    public void StartRun()
    {
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
        float delta = (clockwise ? -stepDegrees : stepDegrees);

        _rotateTween?.Kill();
        _rotateTween = pointer
            .DORotate(new Vector3(0, 0, pointer.eulerAngles.z + delta), stepDuration, RotateMode.Fast)
            .SetEase(stepEase)
            .SetUpdate(useUnscaledTime)
            .OnUpdate(() => UpdateSelectionFromPointer(force: false));
    }

    public void StopAt(int index, float extraTurns = 1.2f, float finalDur = 0.8f, Ease finalEase = Ease.OutCubic)
    {
        if (icons == null || icons.Count == 0) return;
        index = Mathf.Clamp(index, 0, icons.Count - 1);
        IsRunning = false;

        Vector2 dir = DirFromCenterTo(icons[index]);
        float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float currentZ = pointer.eulerAngles.z;

        float delta = Mathf.DeltaAngle(currentZ, targetZ);
        float spin = delta + (clockwise ? -360f * extraTurns : 360f * extraTurns);

        _rotateTween?.Kill();
        _rotateTween = pointer
            .DORotate(new Vector3(0, 0, currentZ + spin), finalDur, RotateMode.Fast)
            .SetEase(finalEase)
            .SetUpdate(useUnscaledTime)
            .OnUpdate(() => UpdateSelectionFromPointer(force: false))
            .OnComplete(() =>
            {
                UpdateSelectionFromPointer(force: true, final: true);
            });
    }

    public void StopAtRandom()
    {
        if (icons == null || icons.Count == 0) return;
        StopAt(Random.Range(0, icons.Count), 1.5f, 1.0f, Ease.OutCubic);
    }

    // ======= 悬停事件 =======
    public void OnIconHoverBegin(int index)
    {
        index = Mathf.Clamp(index, 0, items.Count - 1);
        _manualHighlightIndex = index;
        StopRun();

        ApplyHighlight(index, final: false, instant: false);

        if (alignPointerOnHover)
        {
            Vector2 dir = DirFromCenterTo(items[index].icon);
            float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            _rotateTween?.Kill();
            _rotateTween = pointer
                .DORotate(new Vector3(0, 0, targetZ), hoverAlignDur, RotateMode.Fast)
                .SetEase(hoverAlignEase)
                .SetUpdate(useUnscaledTime)
                .OnUpdate(() => UpdateSelectionFromPointer(force: true));
        }

        // === 弹出该 icon 对应的 popup ===
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].popup)
            {
                if (i == index) items[i].popup.Show("");
                else items[i].popup.Hide();
            }
        }
    }


    public void OnIconHoverEnd(int index)
    {
        if (_manualHighlightIndex == index)
        {
            _manualHighlightIndex = -1;
            UpdateSelectionFromPointer(force: true);
            StartRun();

            // === 隐藏该 popup ===
            if (index < iconPopups.Count && iconPopups[index])
                iconPopups[index].Hide();
        }
    }

    // ======= 选中判定 =======
    private void UpdateSelectionFromPointer(bool force, bool final = false)
    {
        if (_manualHighlightIndex >= 0)
        {
            if (force) ApplyHighlight(_manualHighlightIndex, final: false, instant: false);
            return;
        }

        int nearest = GetNearestIconByPointerTip();
        if (nearest == -1) return;
        if (!force && nearest == CurrentIndex) return;

        ApplyHighlight(nearest, final, instant: false);
    }

    private void ApplyHighlight(int index, bool final, bool instant)
    {
        if (index < 0 || index >= icons.Count) return;

        if (CurrentIndex >= 0 && CurrentIndex < icons.Count && icons[CurrentIndex])
        {
            var last = icons[CurrentIndex];
            last.DOKill(true);
            if (instant)
                last.localScale = Vector3.one * normalScale;
            else
                last.DOScale(normalScale, scaleDuration * 0.8f)
                    .SetEase(deselectEase)
                    .SetUpdate(useUnscaledTime);
        }

        var cur = icons[index];
        if (cur)
        {
            cur.DOKill(true);
            if (instant)
                cur.localScale = Vector3.one * selectedScale;
            else
                cur.DOScale(selectedScale, scaleDuration)
                   .SetEase(selectEase)
                   .SetUpdate(useUnscaledTime)
                   .OnComplete(() =>
                   {
                       if (final && punchOnFinal)
                           cur.DOPunchScale(Vector3.one * punch, punchDur, 8, 0.6f)
                              .SetUpdate(useUnscaledTime);
                   });
        }

        CurrentIndex = index;
    }

    private int GetNearestIconByPointerTip()
    {
        if (!pointerTip || icons == null || icons.Count == 0) return -1;
        Vector2 c = WorldPos(center);
        Vector2 tip = WorldPos(pointerTip);
        Vector2 dirTip = (tip - c).normalized;
        if (dirTip.sqrMagnitude < 1e-6f) return -1;

        int best = -1;
        float bestDot = -2f;
        for (int i = 0; i < icons.Count; i++)
        {
            var rt = icons[i];
            if (!rt) continue;
            Vector2 dir = (WorldPos(rt) - c).normalized;
            float d = Vector2.Dot(dirTip, dir);
            if (d > bestDot)
            {
                bestDot = d;
                best = i;
            }
        }
        return best;
    }

    private Vector2 DirFromCenterTo(RectTransform rt)
    {
        Vector2 c = WorldPos(center);
        Vector2 p = WorldPos(rt);
        return (p - c).normalized;
    }

    private Vector2 WorldPos(RectTransform rt)
    {
        return (Vector2)rt.TransformPoint(rt.rect.center);
    }

    // ==== UI 绑定便捷函数 ====
    public void UI_Start() => StartRun();
    public void UI_Stop() => StopRun();
    public void UI_StepNext() => StepNext();
    public void UI_StopAtRandom() => StopAtRandom();
}

