using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;

namespace TheFriend;
public static class PoacherClass
{
    // Technically this class is meant to hold variables used only for Poacher, but there's nothing barring me from using it with players in general. So i will. :)
    public class Poacher
    {
        // Poacher variables
        public int flicker;
        public int skullpos1;
        public int skullpos2;
        public int skullpos3;
        public Color customColor;
        public Color customColor2;
        public bool isPoacher;
        public bool IsSkullVisible;

        // General player variables
        public bool isRidingLizard;
        public float pointDir0;
        public float pointDir1;
        public int spearRotationTimer;
        public DragonRiding.AbstractDragonRider rideStick;
        public Creature dragonSteed;
        public int grabCounter;
        public int glanceDir;

        // Friend variables
        public bool longjump;
        public bool WantsUp;
        public bool HighJumped;


        public Poacher()
        {
            this.isPoacher = false;
            this.isRidingLizard = false;
        }
    }
    public static readonly ConditionalWeakTable<Player, Poacher> CWT = new();
    public static Poacher GetPoacher(this Player player) => CWT.GetValue(player, _ => new());
}
