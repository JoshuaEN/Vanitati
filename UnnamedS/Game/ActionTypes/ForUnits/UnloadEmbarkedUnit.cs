using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public class UnloadEmbarkedUnit : UnitAction
    {
        public override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(Unit),
            typeof(Location)
        };

        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        private UnloadEmbarkedUnit() : base("unload_embarked_unit") { }
        public static UnloadEmbarkedUnit Instance { get; } = new UnloadEmbarkedUnit();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var parsedContext = new TargetContextTwoArgs<UnitContext, Unit, Location, ActionTriggers>(state, context, TargetContextBase.Load.Source | TargetContextBase.Load.Target);

            var sourceTile = parsedContext.Source.GetTile(state);
            var unit = parsedContext.FirstTargetValue;
            var location = parsedContext.SecondTargetValue;


            return SourceChecks(state, sourceTile, unit, location) && TargetTileChecks(state, sourceTile, state.GetTile(location));

        }

        private bool SourceChecks(IReadOnlyBattleGameState state, Tile sourceTile, Unit unit, Location location)
        {
            if (sourceTile == null || sourceTile.Unit == null)
                return false;

            if (sourceTile.Unit.EmbarkedUnits.FirstOrDefault(u => u.UnitID == unit.UnitID) == null)
                return false;

            return true;
        }

        private bool TargetTileChecks(IReadOnlyBattleGameState state, Tile sourceTile, Tile targetTile)
        {
            if (state.LocationsAroundPoint(sourceTile.Location, 1, 1).Contains(targetTile.Location) == false)
                return false;

            if (targetTile.Unit != null)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var parsedContext = new TargetContextTwoArgs<UnitContext, Unit, Location, ActionTriggers>(state, context, TargetContextBase.Load.Source | TargetContextBase.Load.Target);

            var sourceTile = parsedContext.Source.GetTile(state);
            var unit = parsedContext.FirstTargetValue;
            var location = parsedContext.SecondTargetValue;

            var unitParams = unit.GetWriteableProperties();
            unitParams["Actions"] = 0;
            unitParams["Movement"] = 0;
            unitParams["Embarked"] = false;
            unitParams["Location"] = location;

            var newEmbarkedUnitList = sourceTile.Unit.EmbarkedUnits.ToList();

            newEmbarkedUnitList.RemoveAll(u => u.UnitID == unit.UnitID);

            return new List<StateChange>()
            {
                new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    {"EmbarkedUnits", newEmbarkedUnitList }
                }, sourceTile.Location),
                new StateChanges.UnitStateChange(unit.UnitID, unitParams, location, StateChanges.UnitStateChange.Cause.Added)
            };
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            return new List<Modifier>(0);
        }

        protected override bool RangeBasedValidTargetCanPerform(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return TargetTileChecks(state, sourceTile, targetTile);
        }

        public override IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var parsedContext = new TargetContextTwoArgs<UnitContext, Unit, Location, ActionTriggers>(state, context, TargetContextBase.Load.Source);

            var sourceTile = parsedContext.Source.GetTile(state);
            var unit = parsedContext.FirstTargetValue;

            if(unit == null)
            {
                return sourceTile.Unit.EmbarkedUnits.AsReadOnly();
            }

            var dic = new Dictionary<Location, ActionChain>();

            foreach(var location in state.LocationsAroundPoint(sourceTile.Location, 1, 1))
            {
                var targetTile = state.GetTile(location);

                if (targetTile == null || TargetTileChecks(state, sourceTile, targetTile) == false)
                    continue;

                dic.Add(location, new ActionChain(new ActionChain.Link(this, new UnitContext(sourceTile.Location), new GenericContext(unit, location))));
            }

            return dic;
        }
    }
}
