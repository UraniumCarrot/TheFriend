﻿using System.Diagnostics.CodeAnalysis;

namespace TheFriend;
public static class CreatureTemplateType
{
    [AllowNull] public static CreatureTemplate.Type MotherLizard = new(nameof(MotherLizard), true);
    [AllowNull] public static CreatureTemplate.Type YoungLizard = new(nameof(YoungLizard), true);
    [AllowNull] public static CreatureTemplate.Type SnowSpider = new(nameof(SnowSpider), true);

    public static void UnregisterValues()
    {
        if (MotherLizard != null)
        {
            MotherLizard.Unregister();
            MotherLizard = null;
        }
        if (YoungLizard != null)
        {
            YoungLizard.Unregister();
            YoungLizard = null;
        }
        if (SnowSpider != null)
        {
            SnowSpider.Unregister();
            SnowSpider = null;
        }
    }
}

public static class AbstractObjectType
{
    [AllowNull] public static AbstractPhysicalObject.AbstractObjectType LittleCracker = new(nameof(LittleCracker), true);
    [AllowNull] public static AbstractPhysicalObject.AbstractObjectType Boulder = new(nameof(Boulder), true);
    [AllowNull] public static AbstractPhysicalObject.AbstractObjectType BoomMine = new(nameof(BoomMine), true);
    public static void UnregisterValues()
    {
        if (LittleCracker != null)
        {
            LittleCracker.Unregister();
            LittleCracker = null;
        }
        if (Boulder != null)
        {
            Boulder.Unregister();
            Boulder = null;
        }
        if (BoomMine != null)
        {
            BoomMine.Unregister();
            BoomMine = null;
        }
    }
}

public static class SandboxUnlockID
{
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID MotherLizard = new(nameof(MotherLizard), true);
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID YoungLizard = new(nameof(YoungLizard), true);
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID SnowSpider = new(nameof(SnowSpider), true);
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID UnlockLittleCracker = new(nameof(UnlockLittleCracker), true);
    //[AllowNull] public static MultiplayerUnlocks.SandboxUnlockID UnlockBoulder = new(nameof(UnlockBoulder), true);
    public static void UnregisterValues()
    {
        if (MotherLizard != null)
        {
            MotherLizard.Unregister();
            MotherLizard = null;
        }
        if (YoungLizard != null)
        {
            YoungLizard.Unregister();
            YoungLizard = null;
        }
        if (SnowSpider != null)
        {
            SnowSpider.Unregister();
            SnowSpider = null;
        }
        if (UnlockLittleCracker != null)
        {
            UnlockLittleCracker.Unregister();
            UnlockLittleCracker = null;
        }
        //if (UnlockBoulder != null)
        //{
        //    UnlockBoulder.Unregister();
        //    UnlockBoulder = null;
        //}
    }
}