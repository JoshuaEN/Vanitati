using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForMovementType))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class MovementType : BaseType
    {

        public abstract IReadOnlyList<TerrainType.TerrainClassifications> TraversableClassifications { get; }
        public abstract IReadOnlyList<TerrainType.TerrainDifficulty> TraversableDifficulties { get; }

        protected MovementType(string key) : base("movement_" + key) { }

        public abstract int GetMovementCost(TerrainType terrainType);

        public virtual bool CanTraverse(TerrainType terrainType)
        {
            bool passClassification = false;
            foreach(var classification in TraversableClassifications)
            {
                if (terrainType.Classification.HasFlag(classification))
                {
                    passClassification = true;
                    break;
                }
                    
            }

            if (passClassification == false)
                return false;

            return TraversableDifficulties.Count == 0 || TraversableDifficulties.Contains(terrainType.Difficultly);
        }

        public static IReadOnlyDictionary<string, MovementType> TYPES { get; }

        static MovementType()
        {
            TYPES = BuildTypeListing<MovementType>("UnnamedStrategyGame.Game.MovementTypes");
        }

        public static void LoadMovementTypeListings()
        {
            if (LAND_INFANTRY_TYPES != null)
                throw new NotSupportedException("Already done");


            LAND_MOVEMENT_TYPES = new List<MovementType>()
            {
                MovementTypes.Boots.Instance,
                MovementTypes.Treads.Instance,
                MovementTypes.Wheels.Instance,
                MovementTypes.HalfTrack.Instance
            };

            LAND_INFANTRY_TYPES = new List<MovementType>()
            {
                MovementTypes.Boots.Instance
            };

            LAND_VEHICLE_MOVEMENT_TYPES = new List<MovementType>()
            {
                MovementTypes.Treads.Instance,
                MovementTypes.Wheels.Instance,
                MovementTypes.HalfTrack.Instance
            };

            AIR_VEHICLE_MOVEMENT_TYPES = new List<MovementType>()
            {
                MovementTypes.Propeller.Instance
            };

        }

        public static IReadOnlyList<MovementType> LAND_MOVEMENT_TYPES { get; private set; } 
        public static IReadOnlyList<MovementType> LAND_INFANTRY_TYPES { get; private set; } 
        public static IReadOnlyList<MovementType> LAND_VEHICLE_MOVEMENT_TYPES { get; private set; }
        public static IReadOnlyList<MovementType> AIR_VEHICLE_MOVEMENT_TYPES { get; private set; }
    }

    [ContractClassFor(typeof(MovementType))]
    internal abstract class ContractClassForMovementType : MovementType
    {
        private ContractClassForMovementType() : base(null) { }

        public override int GetMovementCost(TerrainType terrainType)
        {
            Contract.Requires<ArgumentNullException>(null != terrainType);
            Contract.Ensures(Contract.Result<int>() >= 0);

            throw new NotImplementedException();
        }
    }
}
