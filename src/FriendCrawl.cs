﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RWCustom;
using UnityEngine;

namespace TheFriend;
// FriendCrawl code kindly given to me by Noir, thank you so much Noir!!! DO NOT use this code without his permission.
internal class FriendCrawl
{
    public static void Apply()
    {
        On.PlayerGraphics.Update += PlayerGraphics_Update;
        On.SlugcatHand.EngageInMovement += SlugcatHand_EngageInMovement;
    }

    public static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
        if (self.player.slugcatStats.name != Plugin.FriendName || Plugin.PoleCrawl() != true) return;

        var angle = Custom.AimFromOneVectorToAnother(self.player.bodyChunks[0].pos, self.player.bodyChunks[1].pos);
        var lastAngle = Custom.AimFromOneVectorToAnother(self.player.bodyChunks[0].lastPos, self.player.bodyChunks[1].lastPos);
        var a2 = Custom.PerpendicularVector(self.player.bodyChunks[1].pos, self.player.bodyChunks[0].pos);
        var a3 = Custom.PerpendicularVector(self.player.bodyChunks[0].pos, self.player.bodyChunks[1].pos);
        var flpDirNeg = self.player.flipDirection * -1;
        var dirVec = (self.player.bodyChunks[0].pos - self.player.bodyChunks[1].pos).normalized;

        //Adjusting draw positions slightly
        if (self.player.animation == Player.AnimationIndex.StandOnBeam && self.player.input[0].y < 1)
        {
            if (self.player.input[0].x != 0)
            {
                self.drawPositions[0, 0].y = Mathf.Lerp(self.drawPositions[0, 1].y + 5, self.drawPositions[0, 0].y + 5, 1f);
                self.drawPositions[1, 0].y = Mathf.Lerp(self.drawPositions[1, 1].y + 4, self.drawPositions[1, 0].y + 4, 1f);
                self.drawPositions[1, 0].x = Mathf.Lerp(self.drawPositions[1, 1].x + 2 * flpDirNeg, self.drawPositions[1, 0].x + 2 * flpDirNeg, 1f);

                self.head.pos.y = Mathf.Lerp(self.head.lastPos.y + 5f, self.head.pos.y + 5f, 1f);
                self.head.pos.x = Mathf.Lerp(self.head.lastPos.x + 1 * flpDirNeg, self.head.pos.x + 1 * flpDirNeg, 1f);
            }
            else
            {
                self.drawPositions[0, 0].y = Mathf.Lerp(self.drawPositions[0, 1].y + 2, self.drawPositions[0, 0].y + 2, 1f);
                self.drawPositions[1, 0].y = Mathf.Lerp(self.drawPositions[1, 1].y + 3, self.drawPositions[1, 0].y + 3, 1f);
                self.drawPositions[1, 0].x = Mathf.Lerp(self.drawPositions[1, 1].x + 1 * flpDirNeg, self.drawPositions[1, 0].x + 1 * flpDirNeg, 1f);

                self.head.pos.y = Mathf.Lerp(self.head.lastPos.y + 2.5f, self.head.pos.y + 2.5f, 1f);
                self.head.pos.x = Mathf.Lerp(self.head.lastPos.x + 1 * flpDirNeg, self.head.pos.x + 1 * flpDirNeg, 1f);
            }

            //Forcing bodychunks to rotate so the player is aligned horizontally

            switch (angle)
            {
                case > 0 and < 90: //Left Down->Middle
                    if (self.player.flipDirection != -1) break;
                    self.player.bodyChunks[0].vel += a2 * 1;
                    self.player.bodyChunks[1].vel += a3 * 1;
                    break;
                case > 90 and < 180: //Left Up->Middle
                    if (self.player.flipDirection != -1) break;
                    if (self.player.input[0].x != 0)
                    {
                        self.player.bodyChunks[0].vel -= a2 * self.player.bodyChunks[0].vel.y;
                        self.player.bodyChunks[1].vel -= a3 * self.player.bodyChunks[0].vel.y;
                    }
                    else
                    {
                        if (angle > 130)
                        {
                            self.player.bodyChunks[0].vel -= a2 * self.player.bodyChunks[0].vel.y * 0.75f;
                            self.player.bodyChunks[1].vel -= a3 * self.player.bodyChunks[0].vel.y * 0.75f;
                        }
                        else
                        {
                            self.player.bodyChunks[0].vel -= a2;
                            self.player.bodyChunks[1].vel -= a3;
                        }
                    }
                    break;

                case < 0 and > -90: //Right Down->Middle
                    if (self.player.flipDirection != 1) break;
                    self.player.bodyChunks[0].vel -= a2 * 1;
                    self.player.bodyChunks[1].vel -= a3 * 1;
                    break;
                case < -90 and > -180: //Right Up->Middle
                    if (self.player.flipDirection != 1) break;
                    if (self.player.input[0].x != 0)
                    {
                        self.player.bodyChunks[0].vel += a2 * self.player.bodyChunks[0].vel.y;
                        self.player.bodyChunks[1].vel += a3 * self.player.bodyChunks[0].vel.y;
                    }
                    else
                    {
                        if (angle < -130)
                        {
                            self.player.bodyChunks[0].vel += a2 * self.player.bodyChunks[0].vel.y * 0.75f;
                            self.player.bodyChunks[1].vel += a3 * self.player.bodyChunks[0].vel.y * 0.75f;
                        }
                        else
                        {
                            self.player.bodyChunks[0].vel += a2;
                            self.player.bodyChunks[1].vel += a3;
                        }
                    }
                    break;
            }

        }

    }
    public static bool SlugcatHand_EngageInMovement(On.SlugcatHand.orig_EngageInMovement orig, SlugcatHand self)
    {
        var player = (Player)self.owner.owner;

        if (player.slugcatStats.name != Plugin.FriendName || Plugin.PoleCrawl() != true) return orig(self);

        if (player.animation == Player.AnimationIndex.StandOnBeam && player.input[0].y < 1)
        {
            //Crawl anim code while on beams!
            self.mode = Limb.Mode.HuntAbsolutePosition;
            self.huntSpeed = 12f;
            self.quickness = 0.7f;
            if ((self.limbNumber == 0 || (Mathf.Abs((self.owner as PlayerGraphics).hands[0].pos.x - self.owner.owner.bodyChunks[0].pos.x) < 10f && (self.owner as PlayerGraphics).hands[0].reachedSnapPosition)) && !Custom.DistLess(self.owner.owner.bodyChunks[0].pos, self.absoluteHuntPos, 29f))
            {
                Vector2 absoluteHuntPos = self.absoluteHuntPos;
                self.FindGrip(self.owner.owner.room, self.connection.pos + new Vector2((float)(self.owner.owner as Player).flipDirection * 20f, 0f), self.connection.pos + new Vector2((float)(self.owner.owner as Player).flipDirection * 20f, 0f), 100f, new Vector2(self.owner.owner.bodyChunks[0].pos.x + (float)(self.owner.owner as Player).flipDirection * 28f, self.owner.owner.room.MiddleOfTile(self.owner.owner.bodyChunks[0].pos).y - 10f), 2, 1, false);
                if (self.absoluteHuntPos != absoluteHuntPos)
                {
                }
            }
            return false;
        }
        return orig(self);
    }
}
