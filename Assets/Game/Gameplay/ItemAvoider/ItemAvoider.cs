using UnityEngine;

public class ItemAvoider : MonoBehaviour
{
    [SerializeField] private float avoidanceDistance = 0f;
    public float AvoidanceDistance => avoidanceDistance;
}
