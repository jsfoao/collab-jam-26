using System.Collections;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Stage initialStage;
    
    [SerializeField] public float TransitionDuration = 1f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Stage currentStage;
    public Stage CurrentStage => currentStage;
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
        
        float duration = TransitionDuration;
        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;
            float curveAlpha = transitionCurve.Evaluate(alpha);

            // Update rotation
            player.transform.rotation = Quaternion.SlerpUnclamped(startRotation, endRotation, curveAlpha);

            // Update position
            player.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, curveAlpha);


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