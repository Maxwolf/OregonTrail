namespace TrailCommon
{
    /// <summary>
    ///     Person entity that is one of the players, can be given a name, ailments, money. The money on the person contributes
    ///     to the parties total amount.
    /// </summary>
    public interface IPerson
    {
        RationLevel Ration { get; }
        RepairStatus Health { get; }
        uint DaysStarving { get; }
        Profession Profession { get; }
        string Name { get; }
        bool IsLeader { get; }
        void Eat(RationLevel amount);
        void Rest();
        void RepairVehicle();
        bool IsStarving();
        bool IsDead();
    }
}