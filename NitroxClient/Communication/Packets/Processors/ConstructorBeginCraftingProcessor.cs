﻿using NitroxClient.Communication.Packets.Processors.Base;
using NitroxClient.GameLogic.Helper;
using NitroxModel.DataStructures.Util;
using NitroxModel.Helper;
using NitroxModel.Packets;
using System;
using System.Reflection;
using UnityEngine;

namespace NitroxClient.Communication.Packets.Processors
{
    public class ConstructorBeginCraftingProcessor : GenericPacketProcessor<ConstructorBeginCrafting>
    {
        public override void Process(ConstructorBeginCrafting packet)
        {
            Optional<GameObject> opGameObject = GuidHelper.GetObjectFrom(packet.Guid);

            if(opGameObject.IsEmpty())
            {
                Console.WriteLine("Trying to build " + packet.TechType + " with unmanaged constructor - ignoring.");
                return;
            }

            GameObject gameObject = opGameObject.Get();
            Crafter crafter = gameObject.GetComponentInChildren<Crafter>(true);

            if(crafter == null)
            {
                Console.WriteLine("Trying to build " + packet.TechType + " but we did not have a corresponding constructorInput - how did that happen?");
                return;
            }

            Optional<TechType> opTechType = ApiHelper.TechType(packet.TechType);

            if(opTechType.IsEmpty())
            {
                Console.WriteLine("Trying to build unknown tech type: " + packet.TechType + " - ignoring.");
                return;
            }

            MethodInfo onCraftingBegin = typeof(Crafter).GetMethod("OnCraftingBegin", BindingFlags.NonPublic | BindingFlags.Instance);
            Validate.NotNull(onCraftingBegin);
            onCraftingBegin.Invoke(crafter, new object[] { opTechType.Get(), packet.Duration }); //TODO: take into account latency for duration            
        }
    }
}
