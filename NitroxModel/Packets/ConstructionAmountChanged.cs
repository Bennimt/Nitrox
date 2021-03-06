﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NitroxModel.DataStructures;
using NitroxModel.DataStructures.Util;

namespace NitroxModel.Packets
{
    [Serializable]
    public class ConstructionAmountChanged : PlayerActionPacket
    {
        public String Guid { get; private set; }
        public float ConstructionAmount { get; private set; }
        
        public ConstructionAmountChanged(String playerId, Vector3 itemPosition, String guid, float constructionAmount) : base(playerId, itemPosition)
        {
            this.Guid = guid;
            this.ConstructionAmount = constructionAmount;
        }

        public override string ToString()
        {
            return "[ConstructionAmountChanged( - playerId: " + PlayerId + " Guid:" + Guid + " ConstructionAmount: " + ConstructionAmount + "]";
        }
    }
}
