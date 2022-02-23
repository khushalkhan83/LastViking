using System.Collections;
using Game.AI;
using Game.Audio;
using Game.Models;
using Game.Weapon.ProjectileLauncher.Interfaces;
using UltimateSurvival;
using UnityEngine;


namespace Game.Weapon.ProjectileLauncher.Implementation
{
    public class ProjectileLauncher : MonoBehaviour, IProjectileLauncher
    {
        [SerializeField] private Transform fireTransform;
        [SerializeField] private Transform aimFollow;
        [SerializeField] private AudioID _audioBroken;

        private GameObject arrowPrefab;
        private float damage;
        public bool canShoot {get; private set;} = true;
        private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;
        private AudioSystem AudioSystem => AudioSystem.Instance;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public LaunchProjectileResponse Launch(LaunchProjectileRequest request)
        {
            if(canShoot == false) return Success(false);

            arrowPrefab = request.prefab;
            damage = request.damage;
            StartCoroutine(Fire());

            return Success(true);

            LaunchProjectileResponse Success(bool value) => new LaunchProjectileResponse(value);
        }

        private IEnumerator Fire()
        {
            canShoot = false;
            Debug.Log("Can`t shoot");

            GameObject projectile = Instantiate(arrowPrefab);
            
            float x = Screen.width * 0.5f;
            float y = Screen.height * 0.5f;

            Ray crosshair = mainCamera.ScreenPointToRay(new Vector3(x, y, 0));

            Vector3 aimPoint;
            RaycastHit hit;
            if (Physics.Raycast(crosshair, out hit, 100))
            {
                aimPoint = hit.point;
            }
            else
            {
                aimPoint = crosshair.origin + crosshair.direction;
            }

            Ray beam = new Ray(aimFollow.position, aimPoint - aimFollow.position);

            projectile.transform.forward = beam.direction;
            projectile.transform.position = fireTransform.position + fireTransform.forward;

            if(projectile.TryGetComponent<ProjectileDamager>(out var damager))
            {
                damager.damage = damage;
                damager.from = PlayerEventHandler.gameObject;
            }
            try { projectile.GetComponent<ArrowProjectile>().Fire();} catch { }

            yield return new WaitForSeconds(0.2f);
            canShoot = true;
            Debug.Log("Can shoot");
            DecreaseDurability();
        }

        protected virtual void DecreaseDurability()
        {
            if(HotBarModel.EquipCell != null && HotBarModel.EquipCell.IsHasItem)
            {
                var equipItem = HotBarModel.EquipCell.Item;
                if (equipItem != null && equipItem.TryGetProperty("Durability", out var durability))
                {
                    durability.Float.Current--;
                    if (durability.Float.Current <= 0)
                    {
                        AudioSystem.PlayOnce(_audioBroken, transform.position);
                    }
                    equipItem.SetProperty(durability);
                }
            }
        }
    }
}