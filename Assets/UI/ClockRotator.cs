using UnityEngine;
using DG.Tweening;

public class ClockRotator : MonoBehaviour
{
    [Header("����")]
    public int iconCount = 6;         // һ���ж��ٸ�icon
    public float rotateDuration = 0.5f; // ÿ����ת��ʱ��
    public Ease rotateEase = Ease.InOutSine;

    private int currentIndex = 0;
    private bool isStopped = false;

    void Start()
    {
        // �Զ���ʼ��ת
        RotateToNext();
    }

    void RotateToNext()
    {
        if (isStopped) return;

        currentIndex++;
        if (currentIndex >= iconCount) currentIndex = 0;

        float targetZ = -360f / iconCount * currentIndex;

        transform.DORotate(new Vector3(0, 0, targetZ), rotateDuration)
                 .SetEase(rotateEase)
                 .OnComplete(RotateToNext); // ������һ��
    }

    public void StopAt(int index)
    {
        isStopped = true;
        currentIndex = index;
        float targetZ = -360f / iconCount * currentIndex;

        transform.DORotate(new Vector3(0, 0, targetZ), rotateDuration)
                 .SetEase(Ease.OutBack); // ���һ�ο��Լӵ㵯��
    }
}

