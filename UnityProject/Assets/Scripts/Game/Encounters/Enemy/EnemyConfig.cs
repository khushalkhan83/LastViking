namespace Encounters
{
    public class EnemyConfig
    {
        public readonly EnemyID id;
        public readonly bool attackPlayer;
        public readonly bool haveRightToAttackPlayer;
        public readonly bool afraidOfPlayer;
        public readonly bool canRegenHP;

        public EnemyConfig(EnemyID id, bool attackPlayer, bool haveRightToAttackPlayer, bool afraidOfPlayer, bool canRegenHP)
        {
            this.id = id;
            this.attackPlayer = attackPlayer;
            this.haveRightToAttackPlayer = haveRightToAttackPlayer;
            this.afraidOfPlayer = afraidOfPlayer;
            this.canRegenHP = canRegenHP;
        }
        public EnemyConfig(EnemyID id)
        {
            this.id = id;
            this.attackPlayer = false;
            this.haveRightToAttackPlayer = true;
            this.canRegenHP = true;
        }
    }
}