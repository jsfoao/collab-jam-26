using Unity.VisualScripting;
using UnityEditor;
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
        stageManager.EnterStage(stage, false);
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    void OnDrawGizmos()
    {
        if (stage)
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider)
            {
                Color gizmoColor = Color.cyan.WithAlpha(0.1f);
                StageProp stageProp = GetComponent<StageProp>();
                if (stageProp)
                {
                    if (!stageProp.enabled)
                    {
                        gizmoColor = Color.red.WithAlpha(0.1f);
                    }
                }

                Gizmos.color = gizmoColor;

                Matrix4x4 prevMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                Gizmos.matrix = prevMatrix;
                
                string text = stage ? stage.name : "No Stage";
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 10;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(transform.position, text, style);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        if (stage)
        {
            Gizmos.color = Color.red.WithAlpha(0.8f);
            Vector3 position = stage.GetGizmoLabelPosition();

            Gizmos.DrawLine(transform.position, position);
            Gizmos.DrawSphere(position, 0.1f);
        }
    }
}
