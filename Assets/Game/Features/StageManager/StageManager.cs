using System.Collections;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Stage initialStage;
    [SerializeField] private float transitionDuration = 1f;

    private Stage currentStage;
    [NonSerialized] public Stage lastSelectedStage;

    public bool IsTransitioning { get; private set; }

    void Start()
    {
        // Enter initial stage immediately if set
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

        currentStage.OnEnter();
        
        if (immediate)
        {
            FinishPlayerTransition();
            return;
        }

        StartPlayerTransition();
    }

    private void StartPlayerTransition()
    {
        if (IsTransitioning)
        {
            return;
        }

        IsTransitioning = true;
        StartCoroutine(AnimatePlayerTransition());
    }

    private IEnumerator AnimatePlayerTransition()
    {
        Player player = GameManager.Instance.Player;
        Vector3 startPosition = player.transform.position;
        Quaternion startRotation = player.transform.rotation;

        Camera stageCamera = currentStage.StageCamera;
        Quaternion endRotation = stageCamera.transform.rotation;
        Vector3 endPosition = stageCamera.transform.position;
        
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;

            // Update rotation
            player.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            // Update position
            player.transform.position = Vector3.Lerp(startPosition, endPosition, t);


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        FinishPlayerTransition();
    }

    private void FinishPlayerTransition()
    {
        Camera stageCamera = currentStage.StageCamera;
        Player player = GameManager.Instance.Player;
        player.transform.rotation = stageCamera.transform.rotation;
        player.transform.position = stageCamera.transform.position;
        IsTransitioning = false;

        SetPropsEnabled(true);
    }

    public void ExitStage()
    {
        if (!currentStage)
        {
            return;
        }

        SetPropsEnabled(false);

        currentStage.OnExit();
        currentStage = null;
    }

    private void SetPropsEnabled(bool enabled)
    {
        if (!currentStage)
        {
            return;
        }

        foreach (StageProp prop in currentStage.stageProps)
        {
            if (prop)
            {
                prop.enabled = enabled;
            }
        }
    }
}