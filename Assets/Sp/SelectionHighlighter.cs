// SelectionHighlighter.cs
using UnityEngine;
using UnityEngine.UI;

public class SelectionHighlighter : MonoBehaviour
{
    public Canvas selectionCanvas;      // SelectionCanvas
    public RectTransform frame;         // SelectionFrame (Image 的 RectTransform)
    public Camera worldCamera;          // 看 3D 物体的相机（一般是主相机）

    public void ShowFor(SelectablePart part)
    {
        if (!part || !part.targetRenderer) { Hide(); return; }

        // 1) 取包围盒 8 个角点
        var b = part.targetRenderer.bounds;
        Vector3[] corners =
        {
            new(b.min.x, b.min.y, b.min.z),
            new(b.max.x, b.min.y, b.min.z),
            new(b.min.x, b.max.y, b.min.z),
            new(b.max.x, b.max.y, b.min.z),
            new(b.min.x, b.min.y, b.max.z),
            new(b.max.x, b.min.y, b.max.z),
            new(b.min.x, b.max.y, b.max.z),
            new(b.max.x, b.max.y, b.max.z),
        };

        // 2) 转换到屏幕坐标，求 min/max
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
        float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 sp = worldCamera.WorldToScreenPoint(corners[i]);
            // 如果在相机后面，直接隐藏（可自行改成裁剪）
            if (sp.z < 0f) { Hide(); return; }

            minX = Mathf.Min(minX, sp.x);
            minY = Mathf.Min(minY, sp.y);
            maxX = Mathf.Max(maxX, sp.x);
            maxY = Mathf.Max(maxY, sp.y);
        }

        // 3) 加 padding（像素）
        minX -= part.padding.x;
        minY -= part.padding.y;
        maxX += part.padding.x;
        maxY += part.padding.y;

        // 4) 屏幕像素 -> Canvas 本地坐标
        RectTransform canvasRT = selectionCanvas.transform as RectTransform;
        Vector2 bl, tr;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, new Vector2(minX, minY), null, out bl);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, new Vector2(maxX, maxY), null, out tr);

        // 5) 设置框的位置与尺寸
        frame.gameObject.SetActive(true);
        frame.anchoredPosition = (bl + tr) * 0.5f;
        frame.sizeDelta = (tr - bl);
    }

    public void Hide()
    {
        if (frame) frame.gameObject.SetActive(false);
    }
}

