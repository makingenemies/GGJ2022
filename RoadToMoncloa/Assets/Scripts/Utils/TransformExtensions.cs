using UnityEngine;

public static class TransformExtensions
{
    public static void Move(this Transform transform, float xDistance, float yDistance, float zDistance)
    {
        transform.position = transform.position + new Vector3(xDistance, yDistance, zDistance);
    }
}