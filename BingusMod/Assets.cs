/*
 this is made with the custom item template because i'm lazy
*/

using System.Reflection;
using R2API;
using RoR2;
using UnityEngine;

namespace BingusMod
{
    internal static class Assets
    {
        internal static GameObject BingusPrefab;
        internal static Sprite BingusIcon;

        internal static ItemDef BingusItemDef;

        private const string ModPrefix = "@BingusMod:";

        internal static void Init()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BingusMod.ror2bingus")) // this model is nightmare fuel
            {
                var bundle = AssetBundle.LoadFromStream(stream);

                BingusPrefab = bundle.LoadAsset<GameObject>("Assets/Import/bingus/bingus.prefab");
                BingusIcon = bundle.LoadAsset<Sprite>("Assets/Import/bingus_icon/bingus_icon.png");
            }

            BingusAsRedTierItem();

            AddLanguageTokens();
        }

        private static void BingusAsRedTierItem()
        {
            BingusItemDef = ScriptableObject.CreateInstance<ItemDef>();
            var bid = BingusItemDef;
            bid.name = "BINGUS";
            bid.tier = ItemTier.Tier3;
            bid.pickupModelPrefab = BingusPrefab;
            bid.pickupIconSprite = BingusIcon;
            bid.nameToken = "Bingus";
            bid.pickupToken = "Entering low health state charms enemies";
            bid.descriptionToken = "Entering low health state charms 2 enemies (+1 per stack), why? because they are stupid and think bingus squish is cute";
            bid.loreToken = "do not let its look fool you, it's a class-534 threat.";
            bid.tags = new[]
            {
                ItemTag.Utility
            };


            var itemDisplayRules = new ItemDisplayRule[1];
            itemDisplayRules[0].followerPrefab = BingusPrefab;
            itemDisplayRules[0].childName = "Head"; // this is probably very stupid
            itemDisplayRules[0].localScale = new Vector3(1f, 1f, 1f);
            itemDisplayRules[0].localAngles = new Vector3(-90f, 0f, 0f);
            // SinnamonFell i don't know how you did it but somehow it didnt rotate the model and scaled it up massively
            // it looks like the model in the really early stages
            itemDisplayRules[0].localPos = new Vector3(0f, -0.25f, -0.34f);

            var bingus = new R2API.CustomItem(BingusItemDef, itemDisplayRules);

            bool index = ItemAPI.Add(bingus);
            bool debounce = false;

            On.RoR2.CharacterMaster.OnBodyDamaged += (orig, self, damageReport) => // i think i found a better way
            {

            };

            On.RoR2.HealthComponent.FixedUpdate += (orig, self) => // everyone who has just saw this line is probably screaming at me now because it's probably catastrophically bad
            {
                if (self.Equals(null) || self.body.isPlayerControlled == false) { orig(self); return; }

                var body = self.body;
                if (body.inventory.GetItemCount(BingusItemDef) > 0)
                {
                    if (self.isHealthLow && debounce == false)
                    {
                        System.Random random = new System.Random();
                        debounce = true;
                        for (int i = 0; i <= body.inventory.GetItemCount(BingusItemDef); i++)
                        {
                            var monster = new TeamComponent(); // placeholder
                            try
                            {
                                if (TeamComponent.GetTeamMembers(TeamIndex.Monster).Count == 0) { orig(self); return; }
                                monster = TeamComponent.GetTeamMembers(TeamIndex.Monster)[random.Next(1, TeamComponent.GetTeamMembers(TeamIndex.Monster).Count)];
                                if (monster.body.Equals(null)) { continue; }
                                if (monster.body.master.isBoss == true)
                                {
                                    monster = TeamComponent.GetTeamMembers(TeamIndex.Monster)[random.Next(1, TeamComponent.GetTeamMembers(TeamIndex.Monster).Count)];
                                }
                            }catch (System.ArgumentOutOfRangeException) { continue; }
                            
                            var baseAi = monster.body.master.GetComponent<RoR2.CharacterAI.BaseAI>();

                            monster.body.master.teamIndex = TeamIndex.Player;
                            monster.body.teamComponent.teamIndex = TeamIndex.Player;
                            for(int j=0; j<TeamComponent.GetTeamMembers(TeamIndex.Player).Count; j++)
                            {
                                var ally = TeamComponent.GetTeamMembers(TeamIndex.Player)[j];
                                var ai = ally.body.master.GetComponent<RoR2.CharacterAI.BaseAI>();
                                if (ally.body.isPlayerControlled == true || ai.currentEnemy.characterBody.teamComponent.teamIndex==TeamIndex.Player) { Chat.AddMessage("Skipped"); continue; }
                                ai.currentEnemy.Reset();
                                ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                            }
                            baseAi.currentEnemy.Reset();
                            baseAi.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                        }
                    }
                    if (!self.isHealthLow && debounce == true)
                    {
                        debounce = false;
                    }
                }
                orig(self);
            };
        }

        private static void AddLanguageTokens()
        {
            LanguageAPI.Add("BINGUS_NAME", "Bingus");
            LanguageAPI.Add("BINGUS_PICKUP", "Entering low health state charms enemies");
            LanguageAPI.Add("BINGUS_DESC", "Entering low health state charms 2 enemies (+1 per stack), why? because they are stupid and think bingus squish is cute");
            LanguageAPI.Add("BINGUS_LORE", "do not let its look fool you, it's a class-534 threat.");
        }
    }
}

    
