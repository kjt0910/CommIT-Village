// SelectablePart.cs
using UnityEngine;

[DisallowMultipleComponent]
public class SelectablePart : MonoBehaviour
{
    [Tooltip("用于计算屏幕包围框的渲染器；留空则自动在子物体里寻找")]
    public Renderer targetRenderer;

    [Tooltip("边框与包围盒的额外留白（像素）")]
    public Vector2 padding = new Vector2(10, 10);

    void Reset()
    {
        if (!targetRenderer) targetRenderer = GetComponentInChildren<Renderer>();
    }
}
