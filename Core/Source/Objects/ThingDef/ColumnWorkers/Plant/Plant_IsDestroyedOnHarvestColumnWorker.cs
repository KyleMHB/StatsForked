using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant
{
    public sealed class Plant_IsDestroyedOnHarvestColumnWorker : BooleanColumnWorker<VirtualThing>
    {
        public Plant_IsDestroyedOnHarvestColumnWorker(ColumnDef columndef) : base(columndef)
        {
        }
        protected override bool GetCellValue(VirtualThing thing)
        {
            return thing.Def.plant?.HarvestDestroys == true;
        }
    }
}
