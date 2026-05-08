using Input;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField] public float stiffness = 100f;
    [SerializeField] public float damping = 10f;
    [SerializeField] public float grabDistance = 2f;
    
    private Item currentItem;
    public Item CurrentItem => currentItem;

    public Player Player { get; private set; }

    private Vector3 targetPosition;

    void Awake()
    {
        Player = GetComponent<Player>();
    }

    void Update()
    {
        if (!currentItem)
        {
            return;
        }

        // Move item with damped spring force
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (!rb)
        {
            return;
        }

        // Update target position
        Vector3 mousePosition = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Collider collider = currentItem.GetComponent<Collider>();
        float radius = Mathf.Max(collider.bounds.extents.x, collider.bounds.extents.y, collider.bounds.extents.z);
        Vector3 targetPos = ray.GetPoint(grabDistance);

        RaycastHit[] hits = Physics.SphereCastAll(ray.origin, radius, ray.direction, Mathf.Infinity, LayerMask.GetMask("Default"));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == collider)
            {
                continue;
            }
            Link link = hit.collider.GetComponent<Link>();
            if (!link)
            {
                continue;
            }
            targetPos = ray.origin + ray.direction * hit.distance - ray.direction * radius;
        }
        SetTargetPosition(targetPos);

        // Update spring force
        Vector3 direction = targetPosition - currentItem.transform.position;
        Vector3 force = direction * stiffness - rb.linearVelocity * damping;
        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public bool Grab(Item item)
    {
        if (!item)
        {
            return false;
        }
        if (!item.CanGrab(this))
        {
            return false;
        }
        item.OnGrab(this);
        currentItem = item;
        return true;
    }

    public void Drop()
    {
        if (currentItem != null)
        {
            currentItem.OnDrop(this);
            currentItem = null;
        }
    }

    public bool TryLink()
    {
        if (!currentItem)
        {
            return false;
        }
        Link link = currentItem.GetComponent<Link>();
        if (!link)
        {
            return false;
        }

        Link otherLink = null;
        Vector3 mousePosition = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Default"));
        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"Hit {hit.collider.gameObject.name}");
            Link hitLink = hit.collider.GetComponent<Link>();
            if (!hitLink)
            {
                continue;
            }
            if (hitLink == link)
            {
                continue;
            }
            otherLink = hitLink;
            break;
        }
        if (!otherLink)
        {
            return false;
        }
        return link.LinkTo(otherLink);
    }

    public bool IsGrabbing()
    {
        return currentItem != null;
    }
}