using Input;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField] public float stiffness = 100f;
    [SerializeField] public float damping = 10f;
    [SerializeField] public float grabDistance = 2f;
    [SerializeField] public LayerMask avoidanceLayerMask;
    [SerializeField] public LayerMask linkLayerMask;
    private Item currentItem;
    public Item CurrentItem => currentItem;

    public Player Player { get; private set; }

    public Vector3 TargetPosition => targetPosition;
    private Vector3 targetPosition;

    void Awake()
    {
        Player = GetComponent<Player>();
    }

    void Update()
    {
        // Update target position
        Vector3 mousePosition = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Collider collider = null;
        float radius = 1f;
        if (currentItem)
        {
            collider = currentItem.Collider;
            radius = Mathf.Max(collider.bounds.extents.x, collider.bounds.extents.y, collider.bounds.extents.z);
        }
        
        Vector3 targetPos = ray.GetPoint(grabDistance);

        // Avoid obstacles
        RaycastHit[] hits = Physics.SphereCastAll(ray.origin, radius, ray.direction, Mathf.Infinity, avoidanceLayerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider && hit.collider == collider)
            {
                continue;
            }
            ItemAvoider itemAvoider = hit.collider.GetComponent<ItemAvoider>();
            if (!itemAvoider)
            {
                continue;
            }
            targetPos = ray.origin + ray.direction * hit.distance - ray.direction * (radius + itemAvoider.AvoidanceDistance);
        }

        SetTargetPosition(targetPos);

        // Move item with damped spring force
        if (!currentItem)
        {
            return;
        }
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (!rb)
        {
            return;
        }

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
        if (currentItem == item)
        {
            return false;
        }
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
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, linkLayerMask);
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