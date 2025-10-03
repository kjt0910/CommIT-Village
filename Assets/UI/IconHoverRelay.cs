using UnityEngine;
using UnityEngine.EventSystems;

public class IconHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public ClockSelectorPointer parent;  // 拖 DialRoot（挂有 ClockSelectorPointer 的物体）
    public int index;                    // 该按钮在 icons 列表中的索引

    // 鼠标移入
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parent) parent.OnIconHoverBegin(index);
    }
    // 鼠标移出
    public void OnPointerExit(PointerEventData eventData)
    {
        if (parent) parent.OnIconHoverEnd(index);
    }

    // 键盘/手柄导航选中（UI “高亮/Focus”）
    public void OnSelect(BaseEventData eventData)
    {
        if (parent) parent.OnIconHoverBegin(index);
    }
    // 失焦
    public void OnDeselect(BaseEventData eventData)
    {
        if (parent) parent.OnIconHoverEnd(index);
    }
}

