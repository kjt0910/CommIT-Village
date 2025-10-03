// SelectablePart.cs
using UnityEngine;

[DisallowMultipleComponent]
public class SelectablePart : MonoBehaviour
{
    [Tooltip("���ڼ�����Ļ��Χ�����Ⱦ�����������Զ�����������Ѱ��")]
    public Renderer targetRenderer;

    [Tooltip("�߿����Χ�еĶ������ף����أ�")]
    public Vector2 padding = new Vector2(10, 10);

    void Reset()
    {
        if (!targetRenderer) targetRenderer = GetComponentInChildren<Renderer>();
    }
}
