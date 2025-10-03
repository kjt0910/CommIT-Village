// PartUIBinder.cs
using UnityEngine;

public class PartUIBinder : MonoBehaviour
{
    [Header("��ͣ HoverUI ����")]
    public string hoverTitle;
    [TextArea] public string hoverSubtitle;
    public Sprite hoverIcon;

    [Header("���� TaskUI ���ݣ������ù�����壬��ֻ�� taskUIPanel��")]
    public string taskTitle;
    [TextArea] public string taskDescription;
    public Sprite taskIcon;

    [Header("��ѡ���˲�����ר�� TaskUI ���")]
    public GameObject taskUIPanel; // ��������ʹ�ù��� TaskUI
}
