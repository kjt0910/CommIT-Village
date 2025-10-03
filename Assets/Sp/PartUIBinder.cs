// PartUIBinder.cs
using UnityEngine;

public class PartUIBinder : MonoBehaviour
{
    [Header("HoverUI")]
    public string hoverTitle;
    [TextArea] public string hoverSubtitle;
    public Sprite hoverIcon;

    [Header("TaskUI")]
    public string taskTitle;
    [TextArea] public string taskDescription;
    public Sprite taskIcon;

    [Header(" TaskUI pale")]
    public GameObject taskUIPanel; 
}
