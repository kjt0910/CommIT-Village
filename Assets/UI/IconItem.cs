using UnityEngine;

[System.Serializable]
public class IconItem : MonoBehaviour
{
    public RectTransform icon;   // 这个按钮 / 图标
    public IconPopup popup;      // 绑定的 popup（可放在 icon 下作为子物体）

    public IconItem(RectTransform icon, IconPopup popup)
    {
        this.icon = icon;
        this.popup = popup;
    }
}

