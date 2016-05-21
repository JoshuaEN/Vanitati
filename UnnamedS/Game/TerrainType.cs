using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TerrainType : BaseType
    {
        public virtual IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>(0);
        public virtual bool CanCapture { get; } = false;
        public virtual bool CanBePillage { get; } = false; 
        public virtual int MaxCapturePoints { get; } = 20;
        public virtual int MaxHealth { get; } = 100;

        public virtual bool CanSupply { get; }
        public virtual bool CanRepair { get; }

        public virtual bool IsVictoryPoint { get; }

        public virtual IReadOnlyDictionary<SupplyType, int> ResuppliesPerTurn { get; } = new Dictionary<SupplyType, int>(0);
        public virtual IReadOnlyDictionary<MovementType, int> RepairsPerTurn { get; } = new Dictionary<MovementType, int>(0);

        public abstract TerrainHeight Height { get; }
        public abstract TerrainClassifications Classification { get; }
        public abstract TerrainDifficulty Difficultly { get; }

        public virtual double Toughness
        {
            get
            {
                switch (Difficultly)
                {
                    case TerrainDifficulty.Treacherous:
                        return 5;
                    case TerrainDifficulty.Rough:
                        return 2;
                    case TerrainDifficulty.Natural:
                        return 1;
                    case TerrainDifficulty.Tamed:
                        return 0.5;
                    case TerrainDifficulty.Industrialized:
                        return 3;
                    case TerrainDifficulty.Paved:
                        return -1;

                    default:
                        throw new NotSupportedException($"Unknown terrain of ${this}");
                }
            }
        }

        public virtual int ConcealmentModifier
        {
            get
            {
                int fromDifficulty = 0;

                switch(Difficultly)
                {
                    case TerrainDifficulty.Treacherous:
                        fromDifficulty = 15;
                        break;
                    case TerrainDifficulty.Rough:
                        fromDifficulty = 10;
                        break;
                    case TerrainDifficulty.Industrialized:
                        fromDifficulty = 5;
                        break;
                    case TerrainDifficulty.Tamed:
                    case TerrainDifficulty.Natural:
                        fromDifficulty = 0;
                        break;
                    case TerrainDifficulty.Paved:
                        fromDifficulty = -10;
                        break;
                    default:
                        throw new NotSupportedException($"Unknown terrain difficulty of ${Difficultly}");
                }

                int fromHeight = 0;

                //switch(Height)
                //{
                //    case TerrainHeight.Elevated:
                //        fromHeight = 4;
                //        break;
                //    case TerrainHeight.Normal:
                //        fromHeight = 0;
                //        break;
                //    case TerrainHeight.Depressed:
                //        fromHeight = 6;
                //        break;
                //    default:
                //        throw new NotSupportedException($"Unknown terrain height of ${Height}");
                //}

                return fromDifficulty + fromHeight;
            }
        }

        public virtual int AccuracyModifier
        {
            get
            {
                int fromDifficulty = 0;

                //switch(Difficultly)
                //{
                //    case TerrainDifficulty.Treacherous:
                //        fromDifficulty = 0;
                //        break;
                //    case TerrainDifficulty.Rough:
                //        fromDifficulty = 0;
                //        break;
                //    case TerrainDifficulty.Natural:
                //    case TerrainDifficulty.Tamed:
                //        fromDifficulty = -5;
                //        break;
                //    case TerrainDifficulty.Industrialized:
                //        fromDifficulty = -5;
                //        break;
                //    case TerrainDifficulty.Paved:
                //        fromDifficulty = 0;
                //        break;
                //    default:
                //        throw new NotSupportedException($"Unknown terrain difficulty of ${Difficultly}");
                //}

                int fromHeight = 0;

                //switch (Height)
                //{
                //    case TerrainHeight.Elevated:
                //        fromHeight = 10;
                //        break;
                //    case TerrainHeight.Normal:
                //        fromHeight = 0;
                //        break;
                //    case TerrainHeight.Depressed:
                //        fromHeight = -10;
                //        break;
                //    default:
                //        throw new NotSupportedException($"Unknown terrain height of ${Height}");
                //}

                return fromDifficulty + fromHeight;
            }
        }

        public virtual int DigInCap
        {
            get
            {
                switch (Difficultly)
                {
                    case TerrainDifficulty.Treacherous:
                        return 5;
                    case TerrainDifficulty.Rough:
                        return 4;
                    case TerrainDifficulty.Tamed:
                    case TerrainDifficulty.Natural:
                        return 1;
                    case TerrainDifficulty.Industrialized:
                        return 2;
                    case TerrainDifficulty.Paved:
                        return 0;

                    default:
                        throw new NotSupportedException($"Unknown terrain of ${this}");
                }
            }
        }

        protected TerrainType(string key) : base("terrain_" + key) { }

        public static IReadOnlyDictionary<string, TerrainType> TYPES { get; }
        static TerrainType()
        {
            TYPES = BuildTypeListing<TerrainType>("UnnamedStrategyGame.Game.TerrainTypes");
        }

        public override string ToString()
        {
            return Key;
        }

        public enum TerrainHeight { Depressed, Normal, Elevated }

        [Flags]
        public enum TerrainClassifications { Land = 1, River = 2, Sea = 4 }

        public enum TerrainDifficulty { Paved, Industrialized, Tamed, Natural, Rough, Treacherous }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(MaxCapturePoints >= 0);
            Contract.Invariant(null != Actions);
            Contract.Invariant(MaxHealth >= 0);
            Contract.Invariant(RepairsPerTurn != null);
            Contract.Invariant(null != ResuppliesPerTurn);
            Contract.Invariant(CanRepair == true || RepairsPerTurn.Count == 0);
            Contract.Invariant(CanSupply == true || ResuppliesPerTurn.Count == 0);
        }
    }
}
