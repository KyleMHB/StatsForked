namespace Stats.Filters;

public interface IPresettableFilter
{
    string SerializeState();
    void DeserializeState(string state);
}
