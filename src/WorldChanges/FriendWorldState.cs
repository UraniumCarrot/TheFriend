using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using System.Globalization;
using TheFriend.Creatures;

namespace TheFriend.WorldChanges;

public class FriendWorldState
{
    public static bool FaminePlayer(RainWorldGame game) // Applies actual room changes 
    {
        if (game?.StoryCharacter == Plugin.FriendName || game?.StoryCharacter == Plugin.DragonName && game != null) { SolaceWorldstate = true; return true; }
        else { SolaceWorldstate = false; return false; }
    }
    public static bool SolaceWorldstate;
    public static bool FamineName(SlugcatStats.Name name) // For use in menus and region properties 
    {
        if (name == Plugin.FriendName || name == Plugin.DragonName) { SolaceName = true; return true; }
        else { SolaceName = false; return false; }
    }
    public static bool SolaceName;

    public static void Apply()
    {
        On.Region.ctor += Region_ctor;
        On.Room.SlugcatGamemodeUniqueRoomSettings += Room_SlugcatGamemodeUniqueRoomSettings;
        On.RoomSettings.Load += RoomSettings_Load;
        On.Region.GetRegionFullName += Region_GetRegionFullName;
        On.Region.GetProperRegionAcronym += Region_GetProperRegionAcronym;
        On.WorldLoader.ctor_RainWorldGame_Name_bool_string_Region_SetupValues += WorldLoader_ctor_RainWorldGame_Name_bool_string_Region_SetupValues;

        On.AbstractCreature.setCustomFlags += AbstractCreature_setCustomFlags;
        On.ScavengerAbstractAI.InitGearUp += ScavengerAbstractAI_InitGearUp;
        On.CreatureCommunities.InfluenceLikeOfPlayer += CreatureCommunities_InfluenceLikeOfPlayer;
    }

    public static void CreatureCommunities_InfluenceLikeOfPlayer(On.CreatureCommunities.orig_InfluenceLikeOfPlayer orig, CreatureCommunities self, CreatureCommunities.CommunityID commID, int region, int playerNumber, float influence, float interRegionBleed, float interCommunityBleed)
    {
        try
        {
            if (SolaceWorldstate &&
                commID == CreatureCommunities.CommunityID.Lizards &&
                self.session.game.GetStorySession.saveState.cycleNumber == 0 &&
                self.session.game.StoryCharacter == Plugin.FriendName &&
                !self.session.game.IsArenaSession &&
                Plugin.FriendRepLock())
            {
                return;
            }
            if (!Plugin.LocalLizRep())
            {
                orig(self, commID, region, playerNumber, influence, interRegionBleed, interCommunityBleed);
                return;
            }
            if (commID == CreatureCommunities.CommunityID.Lizards && (SolaceWorldstate || Plugin.LocalLizRepAll()))
                interCommunityBleed = 1f;

            orig(self, commID, region, playerNumber, influence, interRegionBleed, interCommunityBleed);
        }
        catch(Exception e) { Debug.Log("Solace: Something bad happened! CreatureCommunities.InfluenceLikeOfPlayer broke!" + e); }
    }

    public static void WorldLoader_ctor_RainWorldGame_Name_bool_string_Region_SetupValues(On.WorldLoader.orig_ctor_RainWorldGame_Name_bool_string_Region_SetupValues orig, WorldLoader self, RainWorldGame game, SlugcatStats.Name playerCharacter, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues)
    {
        FaminePlayer(game);
        if (game != null) FamineWorld.HasFamines(game);
        if (SolaceWorldstate) playerCharacter = Plugin.FriendName;
        orig(self,game,playerCharacter,singleRoomWorld,worldName,region,setupValues);
    }

    public static string Region_GetProperRegionAcronym(On.Region.orig_GetProperRegionAcronym orig, SlugcatStats.Name character, string baseAcronym)
    {
        if (SolaceWorldstate)
        {
            if (baseAcronym == "DS") baseAcronym = "UG";
            if (baseAcronym == "SS") baseAcronym = "RM";
        }
        return orig(character, baseAcronym);
    } // Swaps in Undergrowth and the Rot
    public static void Room_SlugcatGamemodeUniqueRoomSettings(On.Room.orig_SlugcatGamemodeUniqueRoomSettings orig, Room self, RainWorldGame game)
    {
        orig(self, game);
        if (game.IsStorySession && SolaceWorldstate && self.world.region.name != "UG" && self.world.region.name != "SS" && self.world.region.name != "RM" && self.world.region.name != "SB")
        {
            self.roomSettings.wetTerrain = false;
            self.roomSettings.CeilingDrips = 0f;
            self.roomSettings.WaveAmplitude = 0.0001f;
            self.roomSettings.WaveLength = 1f;
            self.roomSettings.WaveSpeed = 0.51f;
            self.roomSettings.SecondWaveAmplitude = 0.0001f;
            self.roomSettings.SecondWaveLength = 1f;
            self.roomSettings.DangerType = MoreSlugcatsEnums.RoomRainDangerType.Blizzard;
        }
        if (game.IsStorySession && SolaceWorldstate && (self.world.region.name == "UG" || self.world.region.name == "SS" || self.world.region.name == "RM" || self.world.region.name == "SB"))
        {
            self.roomSettings.wetTerrain = false;
            self.roomSettings.CeilingDrips = 0f;
        }
    } // Default room settings
    public static bool RoomSettings_Load(On.RoomSettings.orig_Load orig, RoomSettings self, SlugcatStats.Name playerChar) // Complete room settings hijack
    {
        if (SolaceWorldstate)
        {
            playerChar = Plugin.FriendName;
            var settings = new string[] { playerChar.value, "saint", "rivulet" };
            for (int i = 0; i < settings.Length; i++)
            {
                string path = WorldLoader.FindRoomFile(self.name, false, "_settings-" + settings[i] + ".txt");
                if (File.Exists(path))
                {
                    self.filePath = path;
                    break;
                }
            }
        }
        return orig(self, playerChar);
    }

    public static void Region_ctor(On.Region.orig_ctor orig, Region self, string name, int firstRoomIndex, int regionNumber, SlugcatStats.Name storyIndex) // Adjusts region parameters
    {
        FamineName(storyIndex);
        if (SolaceName) storyIndex = Plugin.FriendName;
        orig(self, name, firstRoomIndex, regionNumber, storyIndex);
        if (SolaceName)
        {
            Debug.Log("Applying regional changes...");
            var regionParams = self.regionParams;
            regionParams.earlyCycleChance = 0f;
            regionParams.earlyCycleFloodChance = 0f;
            if (regionParams.earlyCycleChance == 0f && regionParams.earlyCycleFloodChance == 0f) Debug.Log("Precycle chance destroyed for famine characters!");
            if (self.name != "UG" || self.name != "SB")
            {
                regionParams.batDepleteCyclesMax = 0;
                regionParams.batDepleteCyclesMin = 0;
                regionParams.batsPerActiveSwarmRoom = 0;
                regionParams.batsPerInactiveSwarmRoom = 0;
                regionParams.batDepleteCyclesMaxIfLessThanFiveLeft = 0;
                regionParams.batDepleteCyclesMaxIfLessThanTwoLeft = 0;
                regionParams.slugPupSpawnChance = 0.05f;
                if (regionParams.batDepleteCyclesMax == 0f && regionParams.batDepleteCyclesMin == 0 && regionParams.batsPerActiveSwarmRoom == 0) Debug.Log("Regional changes applied to " + self.name + "!");
            }
        }
    }
    public static string Region_GetRegionFullName(On.Region.orig_GetRegionFullName orig, string regionAcro, SlugcatStats.Name slugcatIndex)
    {
        FamineName(slugcatIndex);
        if (SolaceName) slugcatIndex = Plugin.FriendName;
        return orig(regionAcro, slugcatIndex);
    } // Changes region's name in character select screen

    public static void ScavengerAbstractAI_InitGearUp(On.ScavengerAbstractAI.orig_InitGearUp orig, ScavengerAbstractAI self)
    {
        orig(self);
        bool random = (Random.value > 0.7) ? true : false;
        if (self.world.game.IsStorySession && SolaceWorldstate && random && self.world.region.name != "SH" && self.world.region.name != "SB")
        {
            AbstractPhysicalObject obj = new AbstractPhysicalObject(self.world, AbstractPhysicalObject.AbstractObjectType.Lantern, null, self.parent.pos, self.world.game.GetNewID());
            self.world.GetAbstractRoom(self.parent.pos).AddEntity(obj);
            new AbstractPhysicalObject.CreatureGripStick(self.parent, obj, 1, carry: true);
        }
    } // Makes scavengers spawn with lanterns
    public static void AbstractCreature_setCustomFlags(On.AbstractCreature.orig_setCustomFlags orig, AbstractCreature self)
    {
        orig(self);
        if (SolaceWorldstate)
        {
            var type = self.creatureTemplate;
            if (type.BlizzardAdapted || self.creatureTemplate.type == CreatureTemplateType.MotherLizard) self.HypothermiaImmune = true;
            if (type.type == CreatureTemplate.Type.Centipede || type.type == CreatureTemplateType.MotherLizard || type.type == CreatureTemplateType.YoungLizard) self.ignoreCycle = false;
            else self.ignoreCycle = true;
            if (type.type == CreatureTemplate.Type.BigSpider && self.world.region.name != "SB" && self.world.region.name != "UG") self.Winterized = true;
            if (type.type == CreatureTemplateType.YoungLizard) self.Winterized = true;
        }
    } // Stops creatures from running when blizzard starts (or should, anyway)
}
