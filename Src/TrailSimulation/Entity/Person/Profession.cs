using System.ComponentModel;

namespace TrailSimulation.Entity
{
    public enum Profession
    {
        [Description("Be a banker from Boston")]
        Banker = 1,

        [Description("Be a carpenter from Ohio")]
        Carpenter = 2,

        [Description("Be a farmer from Illinois")]
        Farmer = 3
    }
}