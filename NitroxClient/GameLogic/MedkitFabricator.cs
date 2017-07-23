﻿using NitroxClient.Communication;
using NitroxClient.GameLogic.Helper;
using NitroxModel.DataStructures;
using NitroxModel.Packets;
using NitroxModel.Helper;
using System;

namespace NitroxClient.GameLogic
{
    public class MedkitFabricator
    {
        private PacketSender packetSender;

        public MedkitFabricator(PacketSender packetSender)
        {
            this.packetSender = packetSender;
        }

        public void Clicked(MedicalCabinet medicalCabinet)
        {
            Vector3 actionPosition = ApiHelper.Vector3(medicalCabinet.gameObject.transform.position);
            String guid = GuidHelper.GetGuid(medicalCabinet.gameObject);
            bool doorOpen = (bool)medicalCabinet.ReflectionGet("doorOpen");

            MedicalCabinetClicked cabinetClicked = new MedicalCabinetClicked(packetSender.PlayerId, guid, actionPosition, doorOpen, medicalCabinet.hasMedKit, medicalCabinet.timeSpawnMedKit);
            packetSender.Send(cabinetClicked);
        }
    }
}
