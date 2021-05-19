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
            // First registering your AssetBundle into the ResourcesAPI with a modPrefix that'll also be used for your prefab and icon paths
            // note that the string parameter of this GetManifestResourceStream call will change depending on
            // your namespace and file name
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BingusMod.ror2bingus")) 
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
               ItemTag.Utility,
                ItemTag.Damage
            };
          

            var itemDisplayRules = new ItemDisplayRule[1]; // keep this null if you don't want the item to show up on the survivor 3d model. You can also have multiple rules !
            itemDisplayRules[0].followerPrefab = BingusPrefab; // the prefab that will show up on the survivor
            itemDisplayRules[0].childName = "Head";
            itemDisplayRules[0].localScale = new Vector3(1f, 1f, 1f); // scale the model
            itemDisplayRules[0].localAngles = new Vector3(-90f, 0f, 0f); // rotate the model
            itemDisplayRules[0].localPos = new Vector3(0f, -0.25f, -0.34f); // position offset relative to the childName

            var bingus = new R2API.CustomItem(BingusItemDef, itemDisplayRules);

            bool index = ItemAPI.Add(bingus);

            On.RoR2.HealthComponent.FixedUpdate += (orig, self) =>
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
                            var monster = new TeamComponent();
                            try
                            {
                                if (TeamComponent.GetTeamMembers(TeamIndex.Monster).Count == 0) { orig(self); return; }
                                monster = TeamComponent.GetTeamMembers(TeamIndex.Monster)[random.Next(1, TeamComponent.GetTeamMembers(TeamIndex.Monster).Count)];
                                if (monster.body.gameObject.transform == null) { continue; }
                                if (monster.body.master.isBoss == true)
                                {
                                    monster = TeamComponent.GetTeamMembers(TeamIndex.Monster)[random.Next(1, TeamComponent.GetTeamMembers(TeamIndex.Monster).Count)];
                                }
                            }catch (System.ArgumentOutOfRangeException) { continue; }
                            
                            var baseAi = monster.body.master.GetComponent<RoR2.CharacterAI.BaseAI>();

                            monster.body.master.teamIndex = TeamIndex.Player;
                            monster.body.teamComponent.teamIndex = TeamIndex.Player;
                            baseAi.currentEnemy.Reset();
                            baseAi.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                        }
                    }
                    if (!self.isHealthLow && debounce == true)
                    {
                        debounce = false;
                    }
                    for (int i = 0; i < TeamComponent.GetTeamMembers(TeamIndex.Player).Count; i++)
                    {
                        var monster = TeamComponent.GetTeamMembers(TeamIndex.Player)[i];
                        if (monster.body.isPlayerControlled == true || body.gameObject.transform != null) { continue; }
                        var ai = monster.body.masterObject.GetComponent<RoR2.CharacterAI.BaseAI>();
                        if (ai.currentEnemy.characterBody.teamComponent.teamIndex == TeamIndex.Player)
                        {
                            ai.currentEnemy.Reset();
                            ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                        }
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
        static bool debounce = false;
        private static void Effects(HealthComponent self)
        {
            
        }
    }
}

    
