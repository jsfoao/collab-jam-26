using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    public Camera Camera => camera;

    [SerializeField] private Stage initialStage;

    [SerializeField] private float transitionDuration = 1f;

    private Stage currentStage;

    void Start()
    {
        if (initialStage)
        {
            EnterStage(initialStage, true);
        }
    }

    public void EnterStage(Stage newStage, bool immediate = false)
    {
        if (currentStage)
        {
            ExitStage();
        }
        currentStage = newStage;
        if (!currentStage)
        {
            return;
        }

        // Setup stage camera
        Camera stageCamera = currentStage.StageCamera;
        stageCamera.enabled = true;
        stageCamera.tag = "MainCamera";

        foreach (StageProp prop in currentStage.stageProps)
        {
            if (prop)
            {
                prop.enabled = true;
            }
        }

        currentStage.OnEnter();

        if (!immediate)
        {
            StartCameraTransition();
        }
    }

    private void StartCameraTransition()
    {
        StartCoroutine(AnimateCameraTransition());
    }

    private IEnumerator AnimateCameraTransition()
    {
        Quaternion startRotation = camera.transform.rotation;
        Quaternion endRotation = currentStage.StageCamera.transform.rotation;

        Vector3 startPosition = camera.transform.position;
        Vector3 endPosition = currentStage.StageCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;

            // Update rotation
            camera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            // Update position
            camera.transform.position = Vector3.Lerp(startPosition, endPosition, t);


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        FinishCameraTransition();
    }

    private void FinishCameraTransition()
    {
        camera.transform.rotation = currentStage.StageCamera.transform.rotation;
        camera.transform.position = currentStage.StageCamera.transform.position;
    }

    public void ExitStage()
    {
        if (!currentStage)
        {
            return;
        }

        foreach (StageProp prop in currentStage.stageProps)
        {
            if (prop)
            {
                prop.enabled = false;
            }
        }

        // Disable current stage camera
        Camera stageCamera = currentStage.StageCamera;
        stageCamera.enabled = false;
        stageCamera.tag = "Untagged";

        currentStage.OnExit();
        currentStage = null;
    }
}