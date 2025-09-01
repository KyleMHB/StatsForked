namespace Stats
{
    public sealed class Plant_IsDestroyedOnHarvestColumnWorker : BooleanColumnWorker<AbstractThing>
    {
        public Plant_IsDestroyedOnHarvestColumnWorker(ColumnDef columndef) : base(columndef)
        {
        }
        protected override bool GetValue(AbstractThing thing)
        {
            return thing.Def.plant?.HarvestDestroys == true;
        }
    }
}
