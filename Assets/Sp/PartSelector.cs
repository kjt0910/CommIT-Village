// PartSelector.cs
using UnityEngine;
using UnityEngine.EventSystems; // ��ֹ�㵽 UI ʱҲ����

public class PartSelector : MonoBehaviour
{
    [Header("����")]
    public Camera worldCamera;                 // �����
    public LayerMask selectableMask = ~0;      // �ɵ���Ĳ�
    public SelectionHighlighter highlighter;   // ��һ���Ľű�
    public GameObject partDetailPanel;         // ѡ�к�Ҫ�򿪵� UI���ɿգ�

    [Header("���߲���")]
    public float rayMaxDistance = 1000f;

    private SelectablePart _current;

    void Update()
    {
        // ���Ե�� UI �����
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, selectableMask))
            {
                var part = hit.collider.GetComponentInParent<SelectablePart>();
                if (part) Select(part);
                else Deselect();
            }
            else
            {
                Deselect();
            }
        }

        // ѡ�к�֡���棨���嶯/�����ʱ��
        if (_current)
            highlighter.ShowFor(_current);
    }

    void Select(SelectablePart part)
    {
        _current = part;
        highlighter.ShowFor(_current);

        if (partDetailPanel)
            partDetailPanel.SetActive(true); // ����������������Ϣ
    }

    public void Deselect()
    {
        _current = null;
        highlighter.Hide();
        if (partDetailPanel)
            partDetailPanel.SetActive(false);
    }
}
