using Input;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private AnimationCurve cameraRotationCurve;
    [SerializeField] Vector2 maxCameraRotation = new Vector2(2f, 5f);
    [SerializeField] private float cameraSpeed = 5f;

    private Player player;
    private Camera playerCamera;

    void Start()
    {
        player = GetComponent<Player>();
        playerCamera = player.Camera;
    }

    void Update()
    {
        Quaternion targetRotation;
        if (!enabled)
        {
            targetRotation = transform.rotation;
        }
        
        Vector2 mousePosition = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();

        float alphaX = cameraRotationCurve.Evaluate(mousePosition.y / Screen.height);
        float angleX = Mathf.Lerp(-maxCameraRotation.x, maxCameraRotation.x, alphaX);
        float alphaY = cameraRotationCurve.Evaluate(mousePosition.x / Screen.width);
        float angleY = Mathf.Lerp(-maxCameraRotation.y, maxCameraRotation.y, alphaY);
        Quaternion addRotation = Quaternion.Euler(-angleX, angleY, 0f);

        targetRotation = transform.rotation * addRotation;
        Quaternion newRotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, Time.deltaTime * cameraSpeed);
        playerCamera.transform.rotation = newRotation;
    }
}
