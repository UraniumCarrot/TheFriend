﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCustom;
using DevInterface;
using MoreSlugcats;
using Fisobs.Core;
using Fisobs.Creatures;
using Fisobs.Sandbox;
using UnityEngine;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using Fisobs.Properties;

namespace TheFriend.Creatures;

public class SnowSpider : BigSpider
{
    public SnowSpider(AbstractCreature acrit) : base(acrit,acrit.world)
    {
        var state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(acrit.ID.RandomSeed);
        yellowCol = Color.Lerp(new Color(0.3f, 0.9f, 1f), new Color(0.6f, 0.55f, 1f), UnityEngine.Random.value);
        UnityEngine.Random.state = state;
    }
    public override void Update(bool eu)
    {
        base.Update(eu);
        try
        {
            if (this != null && this.room != null && !dead)
            {
                foreach (IProvideWarmth heat in room.blizzardHeatSources)
                {
                    if (heat != null)
                    {
                        float heatpos = Vector2.Distance(base.firstChunk.pos, heat.Position());
                        if (heat.loadedRoom == room && heatpos < heat.range)
                        {
                            float heateffect = Mathf.InverseLerp(heat.range, heat.range * 0.2f, heatpos);
                            State.health -= Mathf.Lerp(heat.warmth * heateffect, 0f, HypothermiaExposure);
                        }
                    }
                }
                State.health -= 0.0014705883f;
                State.health = Mathf.Min(1f, State.health + (base.Submersion >= 0.1f ? 0 : HypothermiaExposure * 0.008f));
            }
            if (graphicsModule != null) (graphicsModule as SnowSpiderGraphics).bodyThickness = SnowSpiderGraphics.originalBodyThickness + State.health;
        }
        catch (Exception) { Debug.Log("Solace: Harmless exception occurred in SnowSpider.Update"); }
    }
    public override Color ShortCutColor()
    {
        return yellowCol;
    }
    public override void InitiateGraphicsModule()
    {
        graphicsModule ??= new SnowSpiderGraphics(this);
        graphicsModule.Reset();
    }
}
