using UnityEngine;

public static class TransformExtension
{
    public static void SetParentWithTransform(this GameObject obj, Transform parent, Transform placeParent)
    {
        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(placeParent.position, placeParent.rotation);
    }
}
