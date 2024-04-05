
using UnityEngine;
[CreateAssetMenu]
public class FacePlayerRotation : RotationStrategy
{
    // A function that rotates an object towards a target position using the given target vector and objectTransform.
    public override void Rotation(Transform target, Transform objectTransform)
    {

        Vector2 targetVector = new Vector2(target.position.x, target.position.y);
        targetVector.x = targetVector.x - objectTransform.position.x;
        targetVector.y = targetVector.y - objectTransform.position.y;
        float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;
        objectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

    }
}
