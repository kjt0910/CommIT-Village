using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineClickSelector : MonoBehaviour
{
    public Camera worldCam;
    public LayerMask clickableMask = ~0;
    public float rayMaxDistance = 1000f;
    public string triggerName = "PlayMove";   // ͳһ�� Animator Trigger

    private SelectableOutline _current;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // �������� UI �Ͼͺ��ԣ���Ҫ�������� EventSystem��
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = worldCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, clickableMask))
        {
            var sel = hit.collider.GetComponentInParent<SelectableOutline>();
            if (sel)
            {
                // �л����
                if (_current && _current != sel) _current.SetOutlined(false);
                _current = sel;
                _current.SetOutlined(true);

                // ������Ӧ Animator
                var anim = sel.GetComponentInParent<Animator>();
                if (anim && !string.IsNullOrEmpty(triggerName))
                    anim.SetTrigger(triggerName);
            }
        }
        else
        {
            // ��հ״�ʱȡ��ѡ��
            if (_current) _current.SetOutlined(false);
            _current = null;
        }
    }
}

