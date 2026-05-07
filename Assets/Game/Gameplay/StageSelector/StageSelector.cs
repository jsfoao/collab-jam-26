using UnityEngine;

public class StageSelector : MonoBehaviour, IInteractable
{
    [SerializeField] private Stage stage;
    public Stage Stage => stage;

     void Start()
    {
        Interactable interactable = GetComponent<Interactable>();
        interactable.contexts.Add(this);
    }

    // --- IInteractable ---

    public bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        if (!stage)
        {
            return false;
        }
        return true;
    }

    public void OnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    public void OnInteract(PlayerInteractionHandler interactionHandler)
    {
        StageManager stageManager = GameManager.Instance.StageManager;
        stageManager.EnterStage(stage);
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }
}
