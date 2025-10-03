using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BottomKeypadController : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform panel;         
    public CanvasGroup mask;            
    public TMP_InputField[] inputs; 

    [Header("Anim")]
    public float height = 380f;    
    public float duration = 0.25f;
    public Ease ease = Ease.OutCubic;

    private bool _visible;
    private TMP_InputField _current;
    private Tween _moveTween, _maskTween;

    void Awake()
    {
       
        var anchored = panel.anchoredPosition;
        anchored.y = -height;
        panel.anchoredPosition = anchored;

        if (mask)
        {
            mask.alpha = 0;
            mask.gameObject.SetActive(false);
            var btn = mask.GetComponent<Button>();
            if (btn) btn.onClick.AddListener(Hide);
        }

        
        foreach (var input in inputs)
        {
            if (!input) continue;
            input.onSelect.AddListener(_ => ShowFor(input));
            input.onDeselect.AddListener(_ => {  });
        }
    }

    public void ShowFor(TMP_InputField target)
    {
        _current = target;
        Show();
    }

    public void Show()
    {
        if (_visible) return;
        _visible = true;

        mask?.gameObject.SetActive(true);
        _maskTween?.Kill();
        _maskTween = mask?.DOFade(1f, duration * 0.8f);

        _moveTween?.Kill();
        _moveTween = panel.DOAnchorPosY(0f, duration).SetEase(ease);

       
        if (_current) _current.ActivateInputField();
    }

    public void Hide()
    {
        if (!_visible) return;
        _visible = false;

        _maskTween?.Kill();
        if (mask)
            _maskTween = mask.DOFade(0f, duration * 0.6f)
                .OnComplete(() => mask.gameObject.SetActive(false));

        _moveTween?.Kill();
        _moveTween = panel.DOAnchorPosY(-height, duration).SetEase(ease);
    }

    public void Append(string s)
    {
        if (!_current) return;
        int caret = _current.caretPosition;
        var t = _current.text ?? "";
        caret = Mathf.Clamp(caret, 0, t.Length);
        _current.text = t.Insert(caret, s);
        _current.caretPosition = caret + s.Length;
        _current.selectionStringAnchorPosition = _current.caretPosition;
        _current.selectionStringFocusPosition = _current.caretPosition;
        _current.ActivateInputField();
    }

    public void Backspace()
    {
        if (!_current) return;
        var t = _current.text ?? "";
        int caret = _current.caretPosition;
        if (caret > 0)
        {
            _current.text = t.Remove(caret - 1, 1);
            _current.caretPosition = caret - 1;
        }
        _current.ActivateInputField();
    }

    public void ClearAll() { if (_current) _current.text = ""; }

    public void FocusNext()
    {
        if (!_current || inputs == null || inputs.Length == 0) return;
        int idx = System.Array.IndexOf(inputs, _current);
        int next = (idx + 1 < inputs.Length) ? idx + 1 : idx;
        _current = inputs[next];
        _current.Select();
        _current.ActivateInputField();
    }
}

