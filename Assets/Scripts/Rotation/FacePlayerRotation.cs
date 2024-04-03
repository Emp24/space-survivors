
using UnityEngine;
[CreateAssetMenu]
public class FacePlayerRotation : RotationStrategy
{
    // A function that rotates an object towards a target position using the given target vector and objectTransform.
    public override void Rotation(Transform target, Transform objectTransform)
    {
        float angle = Mathf.Atan2(target.position.y, target.position.x) * Mathf.Rad2Deg;
        objectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }
}
