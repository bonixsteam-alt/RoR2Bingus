/*
 this is made with the custom item template because i'm lazy
*/

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
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
        internal static ManualLogSource Logger;

        private const string ModPrefix = "@BingusMod:";

        internal static void Init(ManualLogSource Logger)
        {
            Assets.Logger = Logger;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BingusMod.ror2bingus")) // this model is nightmare fuel
            {
                var bundle = AssetBundle.LoadFromStream(stream);

                BingusPrefab = bundle.LoadAsset<GameObject>("Assets/Import/bingus/bingus.prefab");
                BingusIcon = bundle.LoadAsset<Sprite>("Assets/Import/bingus_icon/bingus_icon.png");
            }

            CreateBingusItem();

            AddLanguageTokens();
        }

        private static void CreateBingusItem()
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

            if (!ItemAPI.Add(bingus))
            {
                BingusItemDef = null;
                Assets.Logger.LogError("Unable to add bingus item");
            }
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

    
