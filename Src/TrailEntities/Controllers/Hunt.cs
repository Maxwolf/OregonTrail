using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    public abstract class Hunt : IHunt
    {
        public GameMode ModeType
        {
            get { return GameMode.Hunt; }
        }

        public void TickMode()
        {
            throw new System.NotImplementedException();
        }

        public IVehicle Vehicle
        {
            get { throw new System.NotImplementedException(); }
        }

        public void UseBullets(uint amount)
        {
            throw new System.NotImplementedException();
        }

        public void AddFood(uint amount)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new System.NotImplementedException();
        }
    }
}