namespace Interface
{
    public interface IMonsterRegistry
    {
        void Register(MonsterActor monster);
        void UnRegister(MonsterActor monster);
    }
}