using UnityEngine;
using DG.Tweening;

public class ClockRotator : MonoBehaviour
{
    [Header("设置")]
    public int iconCount = 6;         // 一共有多少个icon
    public float rotateDuration = 0.5f; // 每次旋转的时间
    public Ease rotateEase = Ease.InOutSine;

    private int currentIndex = 0;
    private bool isStopped = false;

    void Start()
    {
        // 自动开始旋转
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
                 .OnComplete(RotateToNext); // 继续下一个
    }

    public void StopAt(int index)
    {
        isStopped = true;
        currentIndex = index;
        float targetZ = -360f / iconCount * currentIndex;

        transform.DORotate(new Vector3(0, 0, targetZ), rotateDuration)
                 .SetEase(Ease.OutBack); // 最后一段可以加点弹性
    }
}

