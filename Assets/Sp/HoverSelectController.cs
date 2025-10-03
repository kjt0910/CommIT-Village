// HoverSelectController.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HoverSelectController : MonoBehaviour
{
    [Header("�����ɵ��")]
    public Camera worldCam;
    public LayerMask selectableMask = ~0;
    public float rayMaxDistance = 1000f;

    [Header("���� HoverUI��һ�����ɣ�")]
    public RectTransform hoverUI;
    public TMP_Text hoverTitleText;
    public TMP_Text hoverSubtitleText;
    public Image hoverIconImage;

    [Header("���� TaskUI���ɿգ���������ר�����������ר����")]
    public GameObject sharedTaskUI;
    public TMP_Text taskTitleText;
    public TMP_Text taskDescText;
    public Image taskIconImage;

    [Header("����")]
    public string triggerName = "PlayMove";

    private SelectableOutline _hoverOutline;
    private Animator _hoverAnim;
    private PartUIBinder _hoverData;
    private GameObject _openedTaskPanel; // ��ǰ�򿪵�������壨ר���򹫹���

    void Start()
    {
        if (hoverUI) hoverUI.gameObject.SetActive(false);
        if (sharedTaskUI) sharedTaskUI.SetActive(false);
    }

    void Update()
    {
        bool overUI = EventSystem.current && EventSystem.current.IsPointerOverGameObject();

        if (!overUI) UpdateHover();

        // ��ͣ UI λ�ø���
        if (hoverUI && _hoverAnim)
        {
            Vector2 sp = worldCam.WorldToScreenPoint(_hoverAnim.transform.position);
            RectTransform canvas = hoverUI.GetComponentInParent<Canvas>().transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, sp, null, out var lp);
            hoverUI.anchoredPosition = lp;
        }

        // ����������� UI��ר�����ȣ�
        if (!overUI && Input.GetMouseButtonDown(0) && _hoverData)
        {
            OpenTaskUI(_hoverData);
        }
    }

    void UpdateHover()
    {
        Ray ray = worldCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, rayMaxDistance, selectableMask))
        {
            var outline = hit.collider.GetComponentInParent<SelectableOutline>();
            var anim = hit.collider.GetComponentInParent<Animator>();
            var data = hit.collider.GetComponentInParent<PartUIBinder>();

            if (outline != _hoverOutline)
            {
                if (_hoverOutline) _hoverOutline.SetOutlined(false);
                _hoverOutline = outline;
                _hoverAnim = anim;
                _hoverData = data;

                if (_hoverOutline) _hoverOutline.SetOutlined(true);
                RefreshHoverUI(data);
            }
        }
        else
        {
            if (_hoverOutline) _hoverOutline.SetOutlined(false);
            _hoverOutline = null; _hoverAnim = null; _hoverData = null;
            if (hoverUI) hoverUI.gameObject.SetActive(false);
        }
    }

    void RefreshHoverUI(PartUIBinder data)
    {
        if (!hoverUI) return;
        hoverUI.gameObject.SetActive(true);
        if (hoverTitleText) hoverTitleText.text = data ? data.hoverTitle : "";
        if (hoverSubtitleText) hoverSubtitleText.text = data ? data.hoverSubtitle : "";
        if (hoverIconImage)
        {
            hoverIconImage.enabled = data && data.hoverIcon;
            hoverIconImage.sprite = data ? data.hoverIcon : null;
        }
    }

    void OpenTaskUI(PartUIBinder data)
    {
        // �ص���һ��
        if (_openedTaskPanel) _openedTaskPanel.SetActive(false);

        if (data && data.taskUIPanel) // ��ר��
        {
            _openedTaskPanel = data.taskUIPanel;
            _openedTaskPanel.SetActive(true);
        }
        else if (sharedTaskUI)        // �ù���������İ�
        {
            if (taskTitleText) taskTitleText.text = data ? data.taskTitle : "";
            if (taskDescText) taskDescText.text = data ? data.taskDescription : "";
            if (taskIconImage)
            {
                taskIconImage.enabled = data && data.taskIcon;
                taskIconImage.sprite = data ? data.taskIcon : null;
            }
            _openedTaskPanel = sharedTaskUI;
            _openedTaskPanel.SetActive(true);
        }
    }

    // ������UI�ġ���ɡ���ť���ã����ŵ�ǰ��ͣ���󶯻����ر����
    public void OnTaskCompleted()
    {
        if (_hoverAnim && !string.IsNullOrEmpty(triggerName))
            _hoverAnim.SetTrigger(triggerName);

        if (_openedTaskPanel) _openedTaskPanel.SetActive(false);
        _openedTaskPanel = null;
    }
}

