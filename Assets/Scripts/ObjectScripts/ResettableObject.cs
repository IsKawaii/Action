using UnityEngine;

public class ResettableObject : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // 初期位置と回転を記録
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetToInitialState()
    {
        // 初期位置と回転を復元
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
