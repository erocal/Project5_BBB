using UnityEngine;
using DG.Tweening;

public class EnergyShieldFollowHands : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform head;

    [Header("Position")]
    [SerializeField] private float forwardOffset = 0.45f;
    [SerializeField] private float upwardOffset = 0.05f;
    [SerializeField] private float positionSmooth = 12f;

    [Header("Controller Sway Position")]
    [SerializeField] private float horizontalSwayAmount = 0.08f;
    [SerializeField] private float verticalSwayAmount = 0.06f;
    [SerializeField] private float maxHorizontalInput = 0.35f;
    [SerializeField] private float maxVerticalInput = 0.25f;

    [Header("Controller Sway Rotation")]
    [SerializeField] private float horizontalRollAmount = 8f;
    [SerializeField] private float verticalPitchAmount = 6f;

    [Header("Controller Rotation Influence")]
    [SerializeField] private bool useControllerRotationInfluence = true;

    // 控制器平均 X 角度對盾牌 X 角度的影響倍率
    [SerializeField] private float controllerPitchInfluence = 1f;

    // 控制器平均 Y 角度對盾牌 Y 角度的影響倍率
    [SerializeField] private float controllerYawInfluence = 0.3f;

    // 控制器平均 Z 角度對盾牌 Z 角度的影響倍率
    [SerializeField] private float controllerRollInfluence = 0.5f;

    [SerializeField] private float maxControllerPitchAngle = 25f;
    [SerializeField] private float maxControllerYawAngle = 15f;
    [SerializeField] private float maxControllerRollAngle = 20f;

    [Header("DOTween Floating")]
    [SerializeField] private float floatAmplitude = 0.04f;
    [SerializeField] private float floatDuration = 0.8f;

    [Header("Rotation Clamp")]
    [SerializeField] private float minXAngle = -135f;
    [SerializeField] private float maxXAngle = -45f;
    [SerializeField] private float rotationSmooth = 12f;

    [Header("Model Rotation Offset")]
    [SerializeField] private Vector3 modelRotationOffset = new Vector3(-90f, 0f, -90f);

    private float floatOffset;
    private Tween floatTween;

    private void OnEnable()
    {
        floatTween = DOTween
            .To(
                () => floatOffset,
                value => floatOffset = value,
                floatAmplitude,
                floatDuration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void LateUpdate()
    {
        if (leftHand == null || rightHand == null || head == null)
            return;

        Vector3 handsCenter = (leftHand.position + rightHand.position) * 0.5f;

        Vector3 playerForward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;

        if (playerForward.sqrMagnitude < 0.001f)
            playerForward = transform.forward;

        Vector3 playerRight = Vector3.ProjectOnPlane(head.right, Vector3.up).normalized;

        if (playerRight.sqrMagnitude < 0.001f)
            playerRight = transform.right;

        Vector3 headToHands = handsCenter - head.position;

        float horizontalInput = Vector3.Dot(headToHands, playerRight);
        float verticalInput = Vector3.Dot(headToHands, Vector3.up);

        horizontalInput = Mathf.Clamp(horizontalInput / maxHorizontalInput, -1f, 1f);
        verticalInput = Mathf.Clamp(verticalInput / maxVerticalInput, -1f, 1f);

        Vector3 controllerSwayOffset =
            playerRight * horizontalInput * horizontalSwayAmount +
            Vector3.up * verticalInput * verticalSwayAmount;

        Vector3 targetPosition =
            handsCenter +
            playerForward * forwardOffset +
            Vector3.up * (upwardOffset + floatOffset) +
            controllerSwayOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * positionSmooth
        );

        Vector3 controllerRotationOffset = GetControllerRotationOffset();

        Quaternion targetRotation = GetShieldRotation(
            playerForward,
            horizontalInput,
            verticalInput,
            controllerRotationOffset
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmooth
        );
    }

    private Quaternion GetShieldRotation(
        Vector3 playerForward,
        float horizontalInput,
        float verticalInput,
        Vector3 controllerRotationOffset
    )
    {
        Quaternion lookRotation = Quaternion.LookRotation(playerForward, Vector3.up);

        Vector3 euler = lookRotation.eulerAngles;

        euler.x = NormalizeAngle(euler.x);

        float pitchSway = -verticalInput * verticalPitchAmount;
        float rollSway = -horizontalInput * horizontalRollAmount;

        float finalX = euler.x + pitchSway + controllerRotationOffset.x;
        finalX = Mathf.Clamp(finalX, minXAngle, maxXAngle);

        float finalY = euler.y + controllerRotationOffset.y;
        float finalZ = rollSway + controllerRotationOffset.z;

        Quaternion baseRotation = Quaternion.Euler(finalX, finalY, finalZ);
        Quaternion modelOffset = Quaternion.Euler(modelRotationOffset);

        return baseRotation * modelOffset;
    }

    private Vector3 GetControllerRotationOffset()
    {
        if (!useControllerRotationInfluence)
            return Vector3.zero;

        Quaternion centerRotation = Quaternion.Slerp(
            leftHand.rotation,
            rightHand.rotation,
            0.5f
        );

        Vector3 centerEuler = centerRotation.eulerAngles;

        centerEuler.x = NormalizeAngle(centerEuler.x);
        centerEuler.y = NormalizeAngle(centerEuler.y);
        centerEuler.z = NormalizeAngle(centerEuler.z);

        float pitch = Mathf.Clamp(
            centerEuler.x * controllerPitchInfluence,
            -maxControllerPitchAngle,
            maxControllerPitchAngle
        );

        float yaw = Mathf.Clamp(
            centerEuler.y * controllerYawInfluence,
            -maxControllerYawAngle,
            maxControllerYawAngle
        );

        float roll = Mathf.Clamp(
            centerEuler.z * controllerRollInfluence,
            -maxControllerRollAngle,
            maxControllerRollAngle
        );

        return new Vector3(pitch, yaw, roll);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private void OnDisable()
    {
        floatTween?.Kill();
    }
}