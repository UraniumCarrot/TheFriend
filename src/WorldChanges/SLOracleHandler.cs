using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheFriend.WorldChanges;

public class SLOracleHandler
{
    public static void Apply()
    {
        On.SLOracleBehavior.UnconciousUpdate += SLOracleBehavior_UnconciousUpdate;
        On.SLOrcacleState.ForceResetState += SLOrcacleState_ForceResetState;
        On.SLOracleBehavior.Update += SLOracleBehavior_Update;
        On.RainWorldGame.IsMoonActive += RainWorldGame_IsMoonActive;
        On.RainWorldGame.MoonHasRobe += RainWorldGame_MoonHasRobe;
        On.RainWorldGame.IsMoonHeartActive += RainWorldGame_IsMoonHeartActive;
    }

    // Simple Moon fixes
    public static void SLOracleBehavior_UnconciousUpdate(On.SLOracleBehavior.orig_UnconciousUpdate orig, SLOracleBehavior self)
    {
        orig(self);
        if (self.oracle.room.game.IsStorySession && FriendWorldState.SolaceWorldstate)
        {
            self.oracle.SetLocalGravity(1f);
            if (self.oracle.room.world.rainCycle.brokenAntiGrav.on)
            {
                self.oracle.room.world.rainCycle.brokenAntiGrav.counter = -1;
                self.oracle.room.world.rainCycle.brokenAntiGrav.to = 0f;
            }
            self.oracle.arm.isActive = false;
            self.moonActive = false;
        }
    }
    public static bool RainWorldGame_IsMoonActive(On.RainWorldGame.orig_IsMoonActive orig, RainWorldGame self)
    {
        orig(self);
        if (FriendWorldState.SolaceWorldstate && self.GetStorySession.saveState.miscWorldSaveData.SLOracleState.neuronsLeft > 0) return true;
        return orig(self);
    }
    public static bool RainWorldGame_IsMoonHeartActive(On.RainWorldGame.orig_IsMoonHeartActive orig, RainWorldGame self)
    {
        if (FriendWorldState.SolaceWorldstate) return true;
        return orig(self);
    }
    public static void SLOrcacleState_ForceResetState(On.SLOrcacleState.orig_ForceResetState orig, SLOrcacleState self, SlugcatStats.Name saveStateNumber)
    {
        orig(self, saveStateNumber);
        if (FriendWorldState.SolaceWorldstate) self.neuronsLeft = 7;
    }
    public static bool RainWorldGame_MoonHasRobe(On.RainWorldGame.orig_MoonHasRobe orig, RainWorldGame self)
    {
        if (FriendWorldState.SolaceWorldstate) return true;
        return orig(self);
    }

    // Moon behaviors

    public static void SLOracleBehavior_Update(On.SLOracleBehavior.orig_Update orig, SLOracleBehavior self, bool eu)
    {
        orig(self, eu);
        if (FriendWorldState.SolaceWorldstate && !(self.hasNoticedPlayer))
        {
            self.movementBehavior = SLOracleBehavior.MovementBehavior.Idle;
        }
    }
    /*public class SLOracleFriendBehaviors
    {
        public static SLOracleBehavior.MovementBehavior GiveMark = new SLOracleBehavior.MovementBehavior("GiveMark", register: true);
    }
    public static void SLOracleBehavior_Update(On.SLOracleBehavior.orig_Update orig, SLOracleBehavior self, bool eu)
    {
        if (self.hasNoticedPlayer && self.moonActive && self.player.slugcatStats.name == Plugin.FriendName)
        {
            self.movementBehavior = SLOracleFriendBehaviors.GiveMark;
        }
        if (self.movementBehavior == SLOracleFriendBehaviors.GiveMark)
        {
            self.movementBehavior = SLOracleBehavior.MovementBehavior.ShowMedia;
        } // Only used for Friend
        else orig(self, eu);
    }*/


    // Moon dialogue
}
