using UnityEngine;

[System.Serializable]
public class IconItem : MonoBehaviour
{
    public RectTransform icon;   // �����ť / ͼ��
    public IconPopup popup;      // �󶨵� popup���ɷ��� icon ����Ϊ�����壩

    public IconItem(RectTransform icon, IconPopup popup)
    {
        this.icon = icon;
        this.popup = popup;
    }
}

