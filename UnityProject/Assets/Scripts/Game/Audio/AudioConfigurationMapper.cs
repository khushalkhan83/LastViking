using Core.Mapper;

namespace Game.Audio
{
    public class AudioConfigurationMapper : MapperBase<AudioID, AudioConfigurationMapper.AudioConfigurationData>
    {
        public class AudioConfigurationData
        {
            public AudioResourceID AudioResourceID { get; protected internal set; }
            public AudioSourceID AudioSourceID { get; protected internal set; }
        }

        private void Awake()
        {
            Map(AudioID.StoneHit01, AudioResourceID.StoneHit01, AudioSourceID.Surface);
            Map(AudioID.StoneHit02, AudioResourceID.StoneHit02, AudioSourceID.Surface);
            Map(AudioID.StoneHit03, AudioResourceID.StoneHit03, AudioSourceID.Surface);
            Map(AudioID.WoodHit01, AudioResourceID.WoodHit01, AudioSourceID.Surface);
            Map(AudioID.WoodHit02, AudioResourceID.WoodHit02, AudioSourceID.Surface);
            Map(AudioID.WoodHit03, AudioResourceID.WoodHit03, AudioSourceID.Surface);
            Map(AudioID.Chicken, AudioResourceID.Chicken, AudioSourceID.Animal);
            Map(AudioID.Boar, AudioResourceID.Boar, AudioSourceID.Surface);
            Map(AudioID.LootContainerDestroy, AudioResourceID.LootContainerDestroy, AudioSourceID.Surface);
            Map(AudioID.AnimalHit01, AudioResourceID.AnimalHit01, AudioSourceID.Surface);
            Map(AudioID.AnimalHit02, AudioResourceID.AnimalHit02, AudioSourceID.Surface);
            Map(AudioID.AnimalHit03, AudioResourceID.AnimalHit03, AudioSourceID.Surface);
            Map(AudioID.ShelterRepairing, AudioResourceID.ShelterRepairing, AudioSourceID.Surface);
            Map(AudioID.EnemyScream, AudioResourceID.EnemyScream, AudioSourceID.Enemy);
            Map(AudioID.EnemyHit, AudioResourceID.EnemyHit, AudioSourceID.Surface);
            Map(AudioID.PickUp, AudioResourceID.PickUp, AudioSourceID.UI);
            Map(AudioID.PickUpCoin, AudioResourceID.PickUpCoin, AudioSourceID.UI);
            Map(AudioID.BoarAttack, AudioResourceID.BoarAttack, AudioSourceID.Animal);
            Map(AudioID.BoarDeath, AudioResourceID.BoarDeath, AudioSourceID.Animal);
            Map(AudioID.ChickenDeath, AudioResourceID.ChickenDeath, AudioSourceID.Animal);
            Map(AudioID.BoarIdle, AudioResourceID.BoarIdle, AudioSourceID.Animal);
            Map(AudioID.ChickenIdle, AudioResourceID.ChickenIdle, AudioSourceID.Animal);
            Map(AudioID.BearRoar, AudioResourceID.BearRoar, AudioSourceID.AnimalBear);
            Map(AudioID.TurnOnFire, AudioResourceID.TurnOnFire, AudioSourceID.UI);
            Map(AudioID.TurnOffFire, AudioResourceID.TurnOffFire, AudioSourceID.UI);
            Map(AudioID.BearAttack, AudioResourceID.BearAttack, AudioSourceID.Animal);
            Map(AudioID.BearIdle, AudioResourceID.BearIdle, AudioSourceID.AnimalBear);
            Map(AudioID.BearDeath, AudioResourceID.BearDeath, AudioSourceID.AnimalBear);
            Map(AudioID.EnemyDeath, AudioResourceID.EnemyDeath, AudioSourceID.Enemy);
            Map(AudioID.EnemySteps, AudioResourceID.EnemySteps, AudioSourceID.Enemy);
            Map(AudioID.EnemyAgressive, AudioResourceID.EnemyAgressive, AudioSourceID.Enemy);
            Map(AudioID.EnemyIdle, AudioResourceID.EnemyIdle, AudioSourceID.Enemy);
            Map(AudioID.DropItem, AudioResourceID.DropItem, AudioSourceID.UI);
            Map(AudioID.Health, AudioResourceID.Health, AudioSourceID.UI);
            Map(AudioID.Food, AudioResourceID.Food, AudioSourceID.UI);
            Map(AudioID.Water, AudioResourceID.Water, AudioSourceID.UI);
            Map(AudioID.TreeFall, AudioResourceID.TreeFall, AudioSourceID.Surface);
            Map(AudioID.StoneDestroy, AudioResourceID.StoneDestroy, AudioSourceID.Surface);
            Map(AudioID.BoarInjured01, AudioResourceID.BoarInjured01, AudioSourceID.Animal);
            Map(AudioID.BoarInjured02, AudioResourceID.BoarInjured02, AudioSourceID.Animal);
            Map(AudioID.ZombieAwake, AudioResourceID.ZombieAwake, AudioSourceID.Enemy);
            Map(AudioID.BearRoarScream, AudioResourceID.BearRoarScream, AudioSourceID.AnimalBear);
            Map(AudioID.DoorOpen, AudioResourceID.DoorOpen, AudioSourceID.Surface);
            Map(AudioID.DoorClose, AudioResourceID.DoorClose, AudioSourceID.Surface);
            Map(AudioID.Burning, AudioResourceID.Burning, AudioSourceID.EnvironmentLoop);
            Map(AudioID.Loom, AudioResourceID.Loom, AudioSourceID.EnvironmentLoop);
            Map(AudioID.TurnOnLoom, AudioResourceID.TurnOnLoom, AudioSourceID.UI);
            Map(AudioID.TurnOffLoom, AudioResourceID.TurnOffLoom, AudioSourceID.UI);
            //Map(AudioID.BearSteps, AudioResourceID.BearSteps, AudioSourceID.Surface);
            Map(AudioID.EnemyPunch, AudioResourceID.EnemyPunch, AudioSourceID.Enemy);
            Map(AudioID.PlayerBreathing, AudioResourceID.PlayerBreathing, AudioSourceID.Surface);
            Map(AudioID.PlayerTakeHit01, AudioResourceID.PlayerTakeHit01, AudioSourceID.Surface);
            Map(AudioID.PlayerTakeHit02, AudioResourceID.PlayerTakeHit02, AudioSourceID.Surface);
            Map(AudioID.PlayerTakeHit03, AudioResourceID.PlayerTakeHit03, AudioSourceID.Surface);
            Map(AudioID.PlayerDeath, AudioResourceID.PlayerDeath, AudioSourceID.Surface);
            //Map(AudioID.EnvironmentDay, AudioResourceID.EnvironmentDay, AudioSourceID.Environment);
            //Map(AudioID.EnvironmentNight, AudioResourceID.EnvironmentNight, AudioSourceID.Environment);
            //Map(AudioID.EnvironmentMorning, AudioResourceID.EnvironmentMorning, AudioSourceID.Environment);
            //Map(AudioID.EnvironmentOcean, AudioResourceID.EnvironmentOcean, AudioSourceID.Environment);
            Map(AudioID.ChickenRun, AudioResourceID.ChickenRun, AudioSourceID.Animal);
            Map(AudioID.PlayerJump, AudioResourceID.PlayerJump, AudioSourceID.Surface);
            Map(AudioID.PlayerLanding, AudioResourceID.PlayerLanding, AudioSourceID.Surface);
            Map(AudioID.WeaponShootShotGun, AudioResourceID.WeaponShootShotGun, AudioSourceID.Surface);
            Map(AudioID.WeaponShootRifle, AudioResourceID.WeaponShootRifle, AudioSourceID.Surface);
            Map(AudioID.WeaponShootRevolver, AudioResourceID.WeaponShootRevolver, AudioSourceID.Surface);
            Map(AudioID.Miss, AudioResourceID.Miss, AudioSourceID.Surface);
            Map(AudioID.Broken, AudioResourceID.Broken, AudioSourceID.Surface);
            Map(AudioID.Draw, AudioResourceID.Draw, AudioSourceID.Surface);
            Map(AudioID.DrawSword, AudioResourceID.DrawSword, AudioSourceID.Surface);
            Map(AudioID.WindowOpen, AudioResourceID.WindowOpen, AudioSourceID.UI);
            Map(AudioID.Button, AudioResourceID.Button, AudioSourceID.UI);
            Map(AudioID.BoostButton, AudioResourceID.BoostButton, AudioSourceID.UI);
            Map(AudioID.CraftEnd, AudioResourceID.CraftEnd, AudioSourceID.UI);
            Map(AudioID.EnemySteps02, AudioResourceID.EnemySteps02, AudioSourceID.Enemy);
            Map(AudioID.EnemySteps03, AudioResourceID.EnemySteps03, AudioSourceID.Enemy);
            Map(AudioID.PlayerStepWater01, AudioResourceID.PlayerStepWater01, AudioSourceID.Surface);
            Map(AudioID.PlayerStepWater02, AudioResourceID.PlayerStepWater02, AudioSourceID.Surface);
            Map(AudioID.PlayerStepWater03, AudioResourceID.PlayerStepWater03, AudioSourceID.Surface);
            Map(AudioID.PlayerSwim01, AudioResourceID.PlayerSwim01, AudioSourceID.Surface);
            Map(AudioID.PlayerSwim02, AudioResourceID.PlayerSwim02, AudioSourceID.Surface);
            Map(AudioID.Count, AudioResourceID.Count, AudioSourceID.UI);
            Map(AudioID.CountFinal, AudioResourceID.CountFinal, AudioSourceID.UI);
            Map(AudioID.DeathView, AudioResourceID.DeathView, AudioSourceID.UI);
            Map(AudioID.Switch, AudioResourceID.Switch, AudioSourceID.UI);
            Map(AudioID.PlayerSwim03, AudioResourceID.PlayerSwim03, AudioSourceID.Surface);
            Map(AudioID.PlayerFootstep01, AudioResourceID.PlayerFootstep01, AudioSourceID.Surface);
            Map(AudioID.PlayerFootstep02, AudioResourceID.PlayerFootstep02, AudioSourceID.Surface);
            Map(AudioID.PlayerFootstep03, AudioResourceID.PlayerFootstep03, AudioSourceID.Surface);
            Map(AudioID.Draw02, AudioResourceID.Draw02, AudioSourceID.Surface);
            Map(AudioID.Whoosh02, AudioResourceID.Whoosh02, AudioSourceID.Surface);
            Map(AudioID.Blueprint, AudioResourceID.Blueprint, AudioSourceID.Surface);
            Map(AudioID.WolfAttack01, AudioResourceID.WolfAttack01, AudioSourceID.Animal);
            Map(AudioID.WolfAttack02, AudioResourceID.WolfAttack02, AudioSourceID.Animal);
            Map(AudioID.WolfIdle01, AudioResourceID.WolfIdle01, AudioSourceID.Animal);
            Map(AudioID.WolfIdle02, AudioResourceID.WolfIdle02, AudioSourceID.Animal);
            Map(AudioID.WolfInjured01, AudioResourceID.WolfInjured01, AudioSourceID.Animal);
            Map(AudioID.WolfInjured02, AudioResourceID.WolfInjured02, AudioSourceID.Animal);
            Map(AudioID.WolfAggr01, AudioResourceID.WolfAggr01, AudioSourceID.Animal);
            Map(AudioID.WolfAggr02, AudioResourceID.WolfAggr02, AudioSourceID.Animal);
            Map(AudioID.Explosion, AudioResourceID.Explosion, AudioSourceID.UI); // TODO: move to spesific source
            Map(AudioID.EnvironmentDay, AudioResourceID.EnvironmentDay, AudioSourceID.Environment);
            Map(AudioID.Ocean, AudioResourceID.Ocean, AudioSourceID.Environment);
            Map(AudioID.EnvironmentNight, AudioResourceID.EnvironmentNight, AudioSourceID.Environment);
            Map(AudioID.Construction, AudioResourceID.Construction, AudioSourceID.Surface);
            Map(AudioID.ObjectiveCompleate, AudioResourceID.ObjectiveCompleate, AudioSourceID.UI);
            Map(AudioID.WaveDefeate, AudioResourceID.ObjectiveCompleate, AudioSourceID.UI);
            Map(AudioID.WaveStart, AudioResourceID.EnemyScream, AudioSourceID.UI);
            Map(AudioID.Error, AudioResourceID.Error, AudioSourceID.UI);
        }

        public void Map(AudioID audioID, AudioResourceID audioResourceID, AudioSourceID audioSourceID)
        {
            Map(audioID, new AudioConfigurationData()
            {
                AudioSourceID = audioSourceID,
                AudioResourceID = audioResourceID
            });
        }
    }
}
