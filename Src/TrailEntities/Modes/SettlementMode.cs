using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public sealed class SettlementMode : GameMode, ISettlement
    {
        private readonly bool _canRest;
        private readonly IStore _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public SettlementMode(IGameSimulation game) : base(game)
        {
            _canRest = true;
            _store = new StoreMode(game);
        }

        public override ModeType Mode
        {
            get { return ModeType.Settlement; }
        }

        public bool CanRest
        {
            get { return _canRest; }
        }

        public IStore Store
        {
            get { return _store; }
        }

        public void GoToStore()
        {
            throw new NotImplementedException();
        }
    }
}
