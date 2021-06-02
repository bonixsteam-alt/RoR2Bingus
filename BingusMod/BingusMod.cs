using System;
using System.Linq;
using System.Collections.Generic;

using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;

using RoR2;


namespace BingusMod
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class CustomItem : BaseUnityPlugin
    {
        private const string ModVer = "1.0.4";
        private const string ModName = "Bingus Mod";
        public const string ModGuid = "com.bonix.bingusmod";

        internal new static ManualLogSource Logger;

        private static Random random = new Random();
        private static bool debounce = false;
        public void Awake()
        {
            Logger = base.Logger;

            Assets.Init(Logger);


            On.RoR2.HealthComponent.FixedUpdate += OnHealthFixedUpdate;
        }

        public static void OnHealthFixedUpdate(On.RoR2.HealthComponent.orig_FixedUpdate orig, HealthComponent self)
        {
            if (!self.Equals(null) && self.body.isPlayerControlled)
            {
                int bingus_count = self.body.inventory.GetItemCount(Assets.BingusItemDef);
                if (bingus_count > 0)
                {
                    DoBingus(bingus_count, self);
                }
            }

            orig(self);
        }

        public static void DoBingus(int bingus_count, HealthComponent self)
        {
            if (self.isHealthLow)
            {
                // we've already executed - skip...
                if (debounce)
                {
                    return;
                }

                debounce = true;
                for (int i = 0; i <= bingus_count; i++)
                {
                    var monsters = TeamComponent.GetTeamMembers(TeamIndex.Monster);
                    if (monsters.Count == 0) // nothing to charm
                    {
                        return;
                    }

                    // copy our monsters list into a randomly sorted order
                    // it's O(n), but it's the best we can really do here...
                    var monsters_copy = new List<TeamComponent>(monsters).OrderBy(x => random.Next()).ToList();

                    // grab our random monster that's not a boss
                    TeamComponent monster = null;
                    foreach (var m in monsters_copy)
                    {
                        if (!m.body.master.isBoss)
                        {
                            Logger.Log(LogLevel.Debug, "Selected monster: " + m.body.master.name);
                            monster = m;
                            break;
                        }
                    }

                    // We were unable to find non-boss a monster to charm, lets just exit
                    // and try again later.
                    if (monster == null)
                    {
                        Logger.Log(LogLevel.Warning, "Unable to find a suitable mob to bingus.");
                        return;
                    }

                    // Assign to player team
                    monster.body.master.teamIndex = TeamIndex.Player;
                    monster.body.teamComponent.teamIndex = TeamIndex.Player;

                    // Reset aggro
                    var baseAi = monster.body.master.GetComponent<RoR2.CharacterAI.BaseAI>();
                    baseAi.currentEnemy.Reset();
                    baseAi.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                }

                // reset drone aggro if needed
                var players = TeamComponent.GetTeamMembers(TeamIndex.Player);
                foreach (var player in players)
                {
                    if (!player.body.isPlayerControlled)
                    {
                        var ai = player.body.masterObject.GetComponent<RoR2.CharacterAI.BaseAI>();
                        if (ai.currentEnemy.characterBody.teamComponent.teamIndex == TeamIndex.Player)
                        {
                            ai.currentEnemy.Reset();
                            ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                        }
                    }
                }
            }
            else
            {
                debounce = false;
            }
        }
    }
}
