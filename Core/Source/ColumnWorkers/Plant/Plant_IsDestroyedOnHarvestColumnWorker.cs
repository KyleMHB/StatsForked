namespace Stats
{
    public sealed class Plant_IsDestroyedOnHarvestColumnWorker : BooleanColumnWorker<ThingAlike>
    {
        public Plant_IsDestroyedOnHarvestColumnWorker(ColumnDef columndef) : base(columndef)
        {
        }
        protected override bool GetValue(ThingAlike thing)
        {
            return thing.Def.plant?.HarvestDestroys == true;
        }
    }
}
