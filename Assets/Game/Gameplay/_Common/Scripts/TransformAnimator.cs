using System.Collections;
using UnityEngine;

public class TransformAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform root;
    [SerializeField] private Transform target;


    [Header("Animator")]
    [SerializeField] private float duration = 1f;
    [SerializeField, Range(0f, 1f)] private float alpha = 0f;

    [Header("Position")]
    [SerializeField] private bool updatePosition = true;
    [SerializeField] private AnimationCurve positionAnimCurve;
    
    [Header("Rotation")]
    [SerializeField] private bool updateRotation = false;
    [SerializeField] private AnimationCurve rotationAnimCurve;

    [Header("Scale")]
    [SerializeField] private bool updateScale = false;
    [SerializeField] private AnimationCurve scaleAnimCurve;

    public bool Animating => animating;
    public bool Open => open;
    
    private bool animating = false;
    private bool open = false;

    void Start()
    {
        alpha = 0;
        open = false;
    }

    public void StartAnim()
    {
        if (animating)
        {
            return;
        }
        if (open)
        {
            StartCoroutine(AnimCoroutine(true));
        }
        else
        {
            StartCoroutine(AnimCoroutine(false));
        }
    }

    public void StartForwardAnim()
    {
        if (animating)
        {
            return;
        }
        StartCoroutine(AnimCoroutine(false));
    }

    public void StartReverseAnim()
    {
        if (animating)
        {
            return;
        }
        StartCoroutine(AnimCoroutine(true));
    }

    private IEnumerator AnimCoroutine(bool reverse)
    {
        float time = 0f;
        animating = true;

        if (!reverse)
        {
            open = true;
        }
        while (time < duration)
        {
            time += Time.deltaTime;
            alpha = time / duration;
            if (reverse)
            {
                alpha = 1f - alpha;
            }
            UpdatePosition(alpha);
            UpdateRotation(alpha);
            UpdateScale(alpha);
            yield return null;
        }
        OnFinishAnim(reverse);
    }

    private void OnFinishAnim(bool reverse)
    {
        animating = false;
        if (reverse)
        {
            alpha = 0f;
            open = false;
            UpdatePosition(0f);
            UpdateRotation(0f);
            UpdateScale(0f);
        }
        else
        {
            alpha = 1f;
            UpdatePosition(1f);
            UpdateRotation(1f);
            UpdateScale(1f);
        }
    }

    void UpdatePosition(float alpha)
    {
        if (!updatePosition)
        {
            return;
        }
        if (!root)
        {
            return;
        }
        if (!target)
        {
            return;
        }
        float curveAlpha = positionAnimCurve.Evaluate(alpha);
        root.transform.position = Vector3.Lerp(transform.position, target.position, curveAlpha);
    }

    void UpdateRotation(float alpha)
    {
        if (!updateRotation)
        {
            return;
        }
        if (!root)
        {
            return;
        }
        if (!target)
        {
            return;
        }
        float curveAlpha = rotationAnimCurve.Evaluate(alpha);
        root.transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, curveAlpha);
    }

    void UpdateScale(float alpha)
    {
        if (!updateScale)
        {
            return;
        }
        if (!root)
        {
            return;
        }
        if (!target)
        {
            return;
        }
        float curveAlpha = scaleAnimCurve.Evaluate(alpha);
        root.transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, curveAlpha);
    }

    void OnDrawGizmos()
    {
        if (!root || !target)
        {
            return;
        }

        if (updatePosition)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(startPos, endPos);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(root.position, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startPos, 0.05f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(endPos, 0.05f);
        }

        if (updateRotation)
        {
            const float size = 0.5f;

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, size);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + root.forward * size);
            Gizmos.DrawSphere(transform.position + root.forward * size, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * size);
            Gizmos.DrawSphere(transform.position + transform.forward * size, 0.05f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + target.forward * size);
            Gizmos.DrawSphere(transform.position + target.forward * size, 0.05f);
        }


        UpdatePosition(alpha);
        UpdateRotation(alpha);
        UpdateScale(alpha);
    }
}
