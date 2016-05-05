using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;
using UnnamedStrategyGame.Game.ActionTypes.ForUnits;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitType : BaseType
    {
        protected static readonly IReadOnlyList<UnitAction> DEFAULT_ACTIONS = new List<UnitAction>()
        {
            ClearRepeatedActionManually.Instance,
            ReplenishUnitTurnResources.Instance,
            TriggerRepeatedActionAutomatically.Instance,
            ClearRepeatedActionAutomatically.Instance
        };

        public abstract MovementType MovementType { get; }
        public abstract IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; }
        public abstract IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; }
        public abstract IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; }
        public abstract IReadOnlyList<UnitAction> Actions { get; }
        public abstract int MaxMovement { get; }
        public virtual int MaxActions { get; } = 1;
        public virtual int MaxHealth { get; } = 10;
        public abstract double MaxArmor { get; }
        public abstract int Concealment { get; }
        public abstract int BuildCost { get; }

        public virtual bool EffectedByTerrainModifiers { get; } = true;

        public static IReadOnlyDictionary<string, UnitType> TYPES { get; }

        static UnitType()
        {
            TYPES = BuildTypeListing<UnitType>("UnnamedStrategyGame.Game.UnitTypes");
        }

        protected UnitType(string key) : base("unit_" + key) { }

        public sealed class ArmorProtectionFrom
        {
            public const double
                Nothing = 0,

                SmallArms = 2, LightMachineGuns = 3, HeavyMachineGuns = 6,

                AutoCannon = 12,

                SmallCaliberTankGuns = 16, LargeCaliberTankGuns = 25, AntiTankRockets = 30,

                Bombs = 40,
                
                LandArtillery = 40, LandRocketArtillery = 45,

                SmallCaliberNavalArtillery = 60, LargeCaliberNavalArtillery = 80;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(MovementType != null);
            Contract.Invariant(SupplyLimits != null);
            Contract.Invariant(MovementSupplyUsage != null);
            Contract.Invariant(TurnSupplyUsage != null);
            Contract.Invariant(Actions != null);
            Contract.Invariant(MaxMovement > -1);
            Contract.Invariant(MaxHealth > -1);
            Contract.Invariant(MaxArmor >= 0);
            Contract.Invariant(MaxActions > -1);
            Contract.Invariant(BuildCost > -1);
        }

    }
}
