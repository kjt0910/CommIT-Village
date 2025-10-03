// SelectionHighlighter.cs
using UnityEngine;
using UnityEngine.UI;

public class SelectionHighlighter : MonoBehaviour
{
    [Header("����")]
    public Canvas selectionCanvas;      // SelectionCanvas
    public RectTransform frame;         // SelectionFrame (Image �� RectTransform)
    public Camera worldCamera;          // �� 3D ����������һ�����������

    public void ShowFor(SelectablePart part)
    {
        if (!part || !part.targetRenderer) { Hide(); return; }

        // 1) ȡ��Χ�� 8 ���ǵ�
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

        // 2) ת������Ļ���꣬�� min/max
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
        float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 sp = worldCamera.WorldToScreenPoint(corners[i]);
            // �����������棬ֱ�����أ������иĳɲü���
            if (sp.z < 0f) { Hide(); return; }

            minX = Mathf.Min(minX, sp.x);
            minY = Mathf.Min(minY, sp.y);
            maxX = Mathf.Max(maxX, sp.x);
            maxY = Mathf.Max(maxY, sp.y);
        }

        // 3) �� padding�����أ�
        minX -= part.padding.x;
        minY -= part.padding.y;
        maxX += part.padding.x;
        maxY += part.padding.y;

        // 4) ��Ļ���� -> Canvas ��������
        RectTransform canvasRT = selectionCanvas.transform as RectTransform;
        Vector2 bl, tr;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, new Vector2(minX, minY), null, out bl);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, new Vector2(maxX, maxY), null, out tr);

        // 5) ���ÿ��λ����ߴ�
        frame.gameObject.SetActive(true);
        frame.anchoredPosition = (bl + tr) * 0.5f;
        frame.sizeDelta = (tr - bl);
    }

    public void Hide()
    {
        if (frame) frame.gameObject.SetActive(false);
    }
}

