// SelectablePart.cs
using UnityEngine;

[DisallowMultipleComponent]
public class SelectablePart : MonoBehaviour
{
    public Renderer targetRenderer;

    public Vector2 padding = new Vector2(10, 10);

    void Reset()
    {
        if (!targetRenderer) targetRenderer = GetComponentInChildren<Renderer>();
    }
}
