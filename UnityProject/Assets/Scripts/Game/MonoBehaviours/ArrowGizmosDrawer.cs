using UnityEditor;
using UnityEngine;

public class ArrowGizmosDrawer : MonoBehaviour
{
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Handles.yAxisColor;
        var arrowStart = transform.position;
        var arrowEnd = arrowStart + transform.forward;
        var leftWing = - transform.forward - transform.right; 
        var rightWing =  - transform.forward + transform.right; 

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 1);
        Gizmos.DrawRay(arrowEnd, leftWing * 0.2f);
        Gizmos.DrawRay(arrowEnd, rightWing * 0.2f);
    }
#endif
}
