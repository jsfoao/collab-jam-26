using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[ExecuteAlways]
public class Stage : MonoBehaviour
{
    public List<StageProp> stageProps = new List<StageProp>();

    [SerializeField] private Camera stageCamera;
    public Camera StageCamera => stageCamera;

    [Header("Debug")]
    [SerializeField] public Color debugColor = Color.cyan;
    [SerializeField] public float debugFrustumHeight = 4f;
    [SerializeField] public float debugDistNear = 4f;
    [SerializeField] public float debugDistFar = 8f;

    void Awake()
    {
        stageCamera.enabled = false;

        if (!Application.isPlaying)
        {
            PopulateStageProps();
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }
        if (stageCamera)
        {
            StageManager stageManager = FindAnyObjectByType<StageManager>();
            if (stageManager)
            {            
                if (Selection.activeGameObject == gameObject)
                {
                    stageManager.lastSelectedStage = this; 
                }
                stageCamera.enabled = stageManager.lastSelectedStage == this;
            }
            else
            {
                stageCamera.enabled = false;
            }
        }
    }

    public virtual void OnEnter()
    {
        Debug.Log("Entered stage: " + gameObject.name);
    }

    public virtual void OnExit()
    {
        Debug.Log("Exited stage: " + gameObject.name);
    }

    void OnTransformChildrenChanged()
    {
        PopulateStageProps();
    }

    void OnValidate()
    {
        PopulateStageProps();
    }

    private void PopulateStageProps()
    {
        stageProps.Clear();
        StageProp[] props = GetComponentsInChildren<StageProp>();
        foreach (StageProp prop in props)
        {
            if (!prop)
            {
                continue;
            }
            stageProps.Add(prop);
        }
    }

    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = debugColor;
        style.fontSize = 14;
        style.alignment = TextAnchor.MiddleCenter;
        Vector3 position = GetGizmoLabelPosition();
        Handles.Label(position, name, style);

        float alpha = 0.1f;
        if (Selection.activeGameObject == gameObject)
        {
            alpha = 0.5f;
        }
        Gizmos.color = debugColor.WithAlpha(alpha);
        Gizmos.DrawSphere(GetGizmoLabelPosition(), 0.3f);

        float aspect = stageCamera.aspect;
        float fov = stageCamera.fieldOfView;
        float height = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        float width = height * aspect;
        Vector3 forward = stageCamera.transform.forward;
        Vector3 right = stageCamera.transform.right * width;

        float distNear = debugDistNear;
        float distFar = debugDistFar;
        // Draw mesh
        Mesh mesh = new Mesh();
        float frustumHeight = debugFrustumHeight;
        float frustumHalfHeight = frustumHeight * 0.5f;
        mesh.vertices = new Vector3[]
        {
            (forward - right).normalized * distNear + stageCamera.transform.position + Vector3.up * frustumHalfHeight,
            (forward - right).normalized * distFar + stageCamera.transform.position + Vector3.up * frustumHalfHeight,
            (forward - right).normalized * distNear + stageCamera.transform.position - Vector3.up * frustumHalfHeight,
            (forward - right).normalized * distFar + stageCamera.transform.position - Vector3.up * frustumHalfHeight,
            (forward + right).normalized * distNear + stageCamera.transform.position + Vector3.up * frustumHalfHeight,
            (forward + right).normalized * distFar + stageCamera.transform.position + Vector3.up * frustumHalfHeight,
            (forward + right).normalized * distNear + stageCamera.transform.position - Vector3.up * frustumHalfHeight,
            (forward + right).normalized * distFar + stageCamera.transform.position - Vector3.up * frustumHalfHeight
        };

        Gizmos.DrawLine(mesh.vertices[0], mesh.vertices[1]);
        Gizmos.DrawLine(mesh.vertices[2], mesh.vertices[3]);

        Gizmos.DrawLine(mesh.vertices[4], mesh.vertices[5]);
        Gizmos.DrawLine(mesh.vertices[6], mesh.vertices[7]);

        Gizmos.DrawLine(mesh.vertices[0], mesh.vertices[4]);
        Gizmos.DrawLine(mesh.vertices[0], mesh.vertices[2]);
        Gizmos.DrawLine(mesh.vertices[2], mesh.vertices[6]);
        Gizmos.DrawLine(mesh.vertices[6], mesh.vertices[4]);

        Gizmos.DrawLine(mesh.vertices[1], mesh.vertices[5]);
        Gizmos.DrawLine(mesh.vertices[1], mesh.vertices[3]);
        Gizmos.DrawLine(mesh.vertices[3], mesh.vertices[7]);
        Gizmos.DrawLine(mesh.vertices[7], mesh.vertices[5]);
    }

    void OnDrawGizmosSelected()
    {
        // Draw props
        Gizmos.color = Color.orange;

        foreach (StageProp prop in stageProps)
        {
            if (!prop)
            {
                continue;
            }
            Collider collider = prop.GetComponent<Collider>();
            if (!collider)
            {
                continue;
            }
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size + Vector3.one * 0.2f);
        }
    }

    public Vector3 GetGizmoLabelPosition()
    {
        float dist = debugDistNear;
        Vector3 position = stageCamera.transform.position + stageCamera.transform.forward * dist;
        position.y = stageCamera.transform.position.y + debugFrustumHeight * 0.5f;
        return position;
    }
}
