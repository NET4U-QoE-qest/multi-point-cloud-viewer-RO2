using UnityEngine;

[ExecuteInEditMode]
public class SJM_DistanceEditorHelper : MonoBehaviour
{
#if (UNITY_EDITOR)


    public bool forceDistance = false;

    public float rightTargetDistance = 10.5f;
    public float centerTargetDistance = 6.5f;
    public float leftTargetDistance = 4f;

    public bool forceSideDistance = false;
    public float sideDistance = 1f;

    public Transform origin;

    public Transform rightTarget;
    public Transform centerTarget;
    public Transform leftTarget;
    //float distanceT1;
    //float distanceT2;
    //float distanceT3;
    public bool debugRay = true;

    Vector3 headingRightTarget;
    Vector3 headingCenterTarget;
    Vector3 headingLeftTarget;

    void Update()
    {
        DistanceCalc();

        if (forceDistance) ForceDistance();

    }

    void DistanceCalc()
    {
        headingRightTarget = rightTarget.position - origin.position;
        headingCenterTarget = centerTarget.position - origin.position;
        headingLeftTarget = leftTarget.position - origin.position;

        //distanceT1 = Vector3.Distance(origin.transform.position, rightTarget.transform.position);
        //distanceT1 = headingRightTarget.magnitude;
        //distanceT2 = headingCenterTarget.magnitude;
        //distanceT3 = headingLeftTarget.magnitude;
    }

    void ForceDistance()
    {

        if (!forceSideDistance)
        {
            rightTarget.position = new Vector3(rightTarget.position.x, rightTarget.position.y, origin.position.z + headingRightTarget.normalized.z * rightTargetDistance);
            centerTarget.position = new Vector3(centerTarget.position.x, centerTarget.position.y, origin.position.z + headingCenterTarget.normalized.z * centerTargetDistance);
            leftTarget.position = new Vector3(leftTarget.position.x, leftTarget.position.y, origin.position.z + headingLeftTarget.normalized.z * leftTargetDistance);
        }
        else
        {
            rightTarget.position = new Vector3(-sideDistance, rightTarget.position.y, origin.position.z + headingRightTarget.normalized.z * rightTargetDistance);
            centerTarget.position = new Vector3(centerTarget.position.x, centerTarget.position.y, origin.position.z + headingCenterTarget.normalized.z * centerTargetDistance);
            leftTarget.position = new Vector3(sideDistance, leftTarget.position.y, origin.position.z + headingLeftTarget.normalized.z * leftTargetDistance);
        }

    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (!debugRay) return;

        if (origin && rightTarget) Gizmos.DrawLine(origin.position, rightTarget.position); Gizmos.DrawLine(centerTarget.position, rightTarget.position);
        if (origin && centerTarget) Gizmos.DrawLine(origin.position, centerTarget.position);
        if (origin && leftTarget) Gizmos.DrawLine(origin.position, leftTarget.position); Gizmos.DrawLine(centerTarget.position, leftTarget.position);
    }
#endif
}
