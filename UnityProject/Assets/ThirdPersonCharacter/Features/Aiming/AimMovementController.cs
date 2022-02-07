using System.Collections;
using Cinemachine;
using Gamekit3D;
using UnityEngine;
using UnityEngine.Events;
using Game.Weapon.ProjectileLauncher.Interfaces;
using Game.ThirdPerson.Weapon.Core.Interfaces;
using Game.ThirdPerson.RangedCombat.Misc;

namespace Game.ThirdPerson.RangedCombat
{
    public class AimMovementController : MonoBehaviour
    {
        private const string k_AimLayerName = "Aim Uper Body Layer";
        private const string k_BowAttackTriggerName = "BowAttack";
        [SerializeField] private GameObject aimCamera;
        [SerializeField] private GameObject regularCamera;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject aimFollow;
        [SerializeField] private AimFillView aimUI;

        [SerializeField] private UnityEvent OnProjectileFired;
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private Animator aimRigAnimator;
        [SerializeField] private CinemachineExtension aimExtension;
        [SerializeField] private CameraSettings cameraSettings;

        private Quaternion nextRotation;
        private Vector3 nextPosition;

        private IProjectileLauncher launcher;
        private IWeaponInteractor weapon;

        public float rotationPower = 3;
        public float rotationLerp = 0.5f;
        public float speed = 1f;

        private bool aim;
        private int upperBodyLayerIndex;
        private float startShootPreparationTime;

        #region MonoBehaviour
        private void Awake()
        {
            upperBodyLayerIndex = animator.GetLayerIndex(k_AimLayerName);
            launcher = GetComponentInChildren<IProjectileLauncher>();
            weapon = GetComponentInChildren<IWeaponInteractor>();
        }

        private void OnEnable()
        {
            aim = true;
            StartShootPreparation();
            SetAimBasedOnDefaultCameraRoration();
            UpdateView();
            PlayPrepareShootAnimation();
            // aimFollow.transform.rotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            aim = false;
            UpdateView();
        }

        private void Update()
        {
            if(PlayerInput.Instance.AttackTap && CanFire())
            {
                Fire();
            }

            RotatePlayer(PlayerInput.Instance.CameraInput);
            UpdateAimFillUI();
        }
        #endregion

        private bool CanFire()
        {
            return Time.time - startShootPreparationTime > weapon.Cooldown;
        }

        private void RotatePlayer(Vector2 cameraInput)
        {
            #region Follow Transform Rotation

            //Rotate the Follow Target transform based on the input
            aimFollow.transform.rotation *= Quaternion.AngleAxis(cameraInput.x * rotationPower, Vector3.up);
            #endregion

            #region Vertical Rotation
                
            aimFollow.transform.rotation *= Quaternion.AngleAxis(-cameraInput.y * rotationPower, Vector3.right);

            var angles = aimFollow.transform.localEulerAngles;
            angles.z = 0;

            var angle = aimFollow.transform.localEulerAngles.x;

            //Clamp the Up/Down rotation
            if (angle > 180 && angle < 300)
            {
                angles.x = 300;
            }
            else if(angle < 180 && angle > 60)
            {
                angles.x = 60;
            }


            aimFollow.transform.localEulerAngles = angles;
            #endregion

            #region Player Rotation

            // nextRotation = Quaternion.Lerp(aimFollow.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

            // if (_move.x == 0 && _move.y == 0) 
            if (aim) 
            {   
                nextPosition = transform.position;

                // if (aimValue == 1)
                if (aim)
                {
                    //Set the player rotation based on the look transform
                    transform.rotation = Quaternion.Euler(0, aimFollow.transform.rotation.eulerAngles.y, 0);
                    //reset the y rotation of the look transform
                    aimFollow.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
                }

                return; 
            }
            // float moveSpeed = speed / 100f;
            // Vector3 position = (transform.forward * _move.y * moveSpeed) + (transform.right * _move.x * moveSpeed);
            // nextPosition = transform.position + position;        
            

            // //Set the player rotation based on the look transform
            // transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
            // //reset the y rotation of the look transform
            // followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
                
            #endregion
        }

        private void Fire()
        {
            if(!launcher.canShoot) return;
            
            var responce = launcher.Launch(new LaunchProjectileRequest(arrowPrefab,weapon.Damage));
            if(!responce.Success) return;

            OnProjectileFired?.Invoke();
            StartShootPreparation();
            UpdateView();
        }

        private void UpdateView()
        {
            aimCamera.SetActive(aim);
            regularCamera.SetActive(!aim);
            aimUI?.gameObject.SetActive(aim);
            aimRigAnimator.enabled = aim;

            StopAllCoroutines();
            StartCoroutine(UpperBodyLayerBlend());
            StartCoroutine(ShowReticle());
        }

        private void SetAimBasedOnDefaultCameraRoration()
        {
            Vector3 forward = Quaternion.Euler(0f, cameraSettings.Current.m_XAxis.Value, 0f) * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Quaternion targetRotation;

            targetRotation = Quaternion.LookRotation(forward);

            Vector3 resultingForward = targetRotation * Vector3.forward;

            aimFollow.transform.rotation = Quaternion.LookRotation(resultingForward);
        }

        private IEnumerator UpperBodyLayerBlend()
        {
            var endValue = aim ? 1: 0;
            var value = animator.GetLayerWeight(upperBodyLayerIndex);

            var step = aim ? 0.05f : -0.05f;

            
            while (value != endValue)
            {
                value += step;
                value = Mathf.Clamp01(value);
                animator.SetLayerWeight(upperBodyLayerIndex,value);
                yield return new WaitForSeconds(0.01f);
            } 
        }

        private IEnumerator ShowReticle()
        {
            yield return new WaitForSeconds(0.25f);
            aimUI?.gameObject.SetActive(aim);
        }

        private void UpdateAimFillUI()
        {
            aimUI?.SetFill((Time.time - startShootPreparationTime) / weapon.Cooldown);
        }

        private void StartShootPreparation()
        {
            startShootPreparationTime = Time.time;
            PlayPrepareShootAnimation();
        }

        private void PlayPrepareShootAnimation() => animator.SetTrigger(k_BowAttackTriggerName);
    }
}