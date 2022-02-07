using UnityEngine;
using UltimateSurvival.InputSystem;
using NaughtyAttributes;

public class ImpactReceiver : MonoBehaviour
{
    #region Data
    #pragma warning disable 0649
    [SerializeField] private bool useDebugButton = false;
    [ShowIf("useDebugButton")] [SerializeField] private KeyCode debugButton = KeyCode.I;
    [SerializeField] private Vector3 dir = Vector3.left;
    [SerializeField] private float force = 1;
    [Button] void AddImpact() => AddImpact(dir,force);
    
    float mass = 3.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    private CharacterController character;
    private bool impactInProgress;

    #pragma warning restore 0649
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        character = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2F)
        {
            character.Move(impact * Time.deltaTime);
        }
        // consumes the impact energy each cycle:

        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        // if(impactInProgress)
        // {
        //     character.enabled = false;
        // }

        if(Input.GetKeyDown(debugButton) && useDebugButton)
        {
            AddImpact();
        }
    }

    #endregion

    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    public void Reset() => impact = Vector3.zero;
}