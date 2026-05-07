using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Stage : MonoBehaviour
{
    [SerializeField] public StageProp[] stageProps;

    private Camera stageCamera;
    public Camera StageCamera => stageCamera;

    void Awake()
    {
        stageCamera = GetComponent<Camera>();
        stageCamera.enabled = false;
    }

    public virtual void OnEnter()
    {
        Debug.Log("Entered stage: " + gameObject.name);
    }

    public virtual void OnExit()
    {
        Debug.Log("Exited stage: " + gameObject.name);
    }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
            
            Gizmos.color = Color.blue;
            foreach (StageProp prop in stageProps)
            {
                if (prop)
                {
                    Gizmos.DrawWireSphere(prop.transform.position, 1f);
                }
            }
        }
}
