using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineClickSelector : MonoBehaviour
{
    public Camera worldCam;
    public LayerMask clickableMask = ~0;
    public float rayMaxDistance = 1000f;
    public string triggerName = "PlayMove";   // 统一的 Animator Trigger

    private SelectableOutline _current;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // 如果点击在 UI 上就忽略（需要场景里有 EventSystem）
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = worldCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, clickableMask))
        {
            var sel = hit.collider.GetComponentInParent<SelectableOutline>();
            if (sel)
            {
                // 切换描边
                if (_current && _current != sel) _current.SetOutlined(false);
                _current = sel;
                _current.SetOutlined(true);

                // 触发对应 Animator
                var anim = sel.GetComponentInParent<Animator>();
                if (anim && !string.IsNullOrEmpty(triggerName))
                    anim.SetTrigger(triggerName);
            }
        }
        else
        {
            // 点空白处时取消选中
            if (_current) _current.SetOutlined(false);
            _current = null;
        }
    }
}

