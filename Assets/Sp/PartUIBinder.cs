// PartUIBinder.cs
using UnityEngine;

public class PartUIBinder : MonoBehaviour
{
    [Header("悬停 HoverUI 内容")]
    public string hoverTitle;
    [TextArea] public string hoverSubtitle;
    public Sprite hoverIcon;

    [Header("任务 TaskUI 内容（若不用公共面板，可只填 taskUIPanel）")]
    public string taskTitle;
    [TextArea] public string taskDescription;
    public Sprite taskIcon;

    [Header("可选：此部件的专属 TaskUI 面板")]
    public GameObject taskUIPanel; // 若留空则使用公共 TaskUI
}
