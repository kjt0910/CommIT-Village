// PartSelector.cs
using UnityEngine;
using UnityEngine.EventSystems; // 防止点到 UI 时也触发

public class PartSelector : MonoBehaviour
{
    public Camera worldCamera;                 // 主相机
    public LayerMask selectableMask = ~0;      // 可点击的层
    public SelectionHighlighter highlighter;   // 上一步的脚本
    public GameObject partDetailPanel;         // 选中后要打开的 UI（可空）

    public float rayMaxDistance = 1000f;

    private SelectablePart _current;

    void Update()
    {
        // 忽略点击 UI 的情况
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

        // 选中后帧跟随（物体动/相机动时）
        if (_current)
            highlighter.ShowFor(_current);
    }

    void Select(SelectablePart part)
    {
        _current = part;
        highlighter.ShowFor(_current);

        if (partDetailPanel)
            partDetailPanel.SetActive(true); // 这里你再填充具体信息
    }

    public void Deselect()
    {
        _current = null;
        highlighter.Hide();
        if (partDetailPanel)
            partDetailPanel.SetActive(false);
    }
}
