using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-299)]
public class VRController : MonoBehaviour
{

    public static VRController Instance { get; private set; }

    [Header("XR Input")]
    [Tooltip("建議拖 XRI LeftHand Interaction / Activate，通常是 Trigger。")]
    [SerializeField] private InputActionReference leftHandActivateAction;

    [Tooltip("建議拖 XRI RightHand Interaction / Activate，通常是 Trigger。")]
    [SerializeField] private InputActionReference rightHandActivateAction;

    public InputActionReference LeftHandActivateAction => leftHandActivateAction;
    public InputActionReference RightHandActivateAction => rightHandActivateAction;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 換場景不銷毀
        DontDestroyOnLoad(gameObject);

    }

    
}