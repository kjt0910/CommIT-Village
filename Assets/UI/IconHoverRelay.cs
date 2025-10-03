using UnityEngine;
using UnityEngine.EventSystems;

public class IconHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public ClockSelectorPointer parent;  // �� DialRoot������ ClockSelectorPointer �����壩
    public int index;                    // �ð�ť�� icons �б��е�����

    // �������
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parent) parent.OnIconHoverBegin(index);
    }
    // ����Ƴ�
    public void OnPointerExit(PointerEventData eventData)
    {
        if (parent) parent.OnIconHoverEnd(index);
    }

    // ����/�ֱ�����ѡ�У�UI ������/Focus����
    public void OnSelect(BaseEventData eventData)
    {
        if (parent) parent.OnIconHoverBegin(index);
    }
    // ʧ��
    public void OnDeselect(BaseEventData eventData)
    {
        if (parent) parent.OnIconHoverEnd(index);
    }
}

