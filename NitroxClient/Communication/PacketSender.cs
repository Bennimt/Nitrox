﻿using NitroxClient.MonoBehaviours;
using NitroxModel.DataStructures.Util;
using NitroxModel.DataStructures.ServerModel;
using NitroxModel.Packets;
using System;
using UnityEngine;
using NitroxClient.GameLogic.Helper;
using static NitroxClient.GameLogic.Helper.TransientLocalObjectManager;

namespace NitroxClient.Communication
{
    public class PacketSender
    {
        public bool Active { get; set; }
        public String PlayerId { get; set; }

        private TcpClient client;
        
        public PacketSender(TcpClient client)
        {
            this.client = client;
            this.Active = false;
        }

        public void Authenticate()
        {
            Authenticate auth = new Authenticate(PlayerId);
            Send(auth);
        }

        public void UpdatePlayerLocation(Vector3 location, Quaternion rotation, Optional<VehicleModel> opVehicle, Optional<String> opSubGuid)
        {
            Movement movement;

            if (opVehicle.IsPresent())
            {
                VehicleModel vehicle = opVehicle.Get();
                movement = new VehicleMovement(PlayerId, ApiHelper.Vector3(location), vehicle.Rotation, vehicle.TechType, vehicle.Guid);
            }
            else
            {
                movement = new Movement(PlayerId, ApiHelper.Vector3(location), ApiHelper.Quaternion(rotation), opSubGuid);
            }

            Send(movement);
        }

        public void UpdateItemPosition(String guid, Vector3 location, Quaternion rotation)
        {
            ItemPosition itemPosition = new ItemPosition(PlayerId, guid, ApiHelper.Vector3(location), ApiHelper.Quaternion(rotation));            
            Send(itemPosition);
        }

        public void PickupItem(GameObject gameObject, String techType)
        {
            String guid = GuidHelper.GetGuid(gameObject);
            Vector3 itemPosition = gameObject.transform.position;

            PickupItem(itemPosition, guid, techType);
        }
        
        public void PickupItem(Vector3 itemPosition, String guid, String techType)
        {
            PickupItem pickupItem = new PickupItem(PlayerId, ApiHelper.Vector3(itemPosition), guid, techType);
            Send(pickupItem);
        }

        public void DropItem(GameObject gameObject, TechType techType, Vector3 dropPosition)
        {
            String guid = GuidHelper.GetGuid(gameObject);
            SyncedMultiplayerObject.ApplyTo(gameObject);
            
            Console.WriteLine("Dropping item with guid: " + guid);

            DroppedItem droppedItem = new DroppedItem(PlayerId, guid, ApiHelper.TechType(techType), ApiHelper.Vector3(dropPosition));
            Send(droppedItem);
        }

        public void PlaceBasePiece(ConstructableBase constructableBase, Base targetBase, TechType techType, Quaternion quaternion)
        {
            String guid = GuidHelper.GetGuid(constructableBase.gameObject);
            String parentBaseGuid = (targetBase == null) ? null : GuidHelper.GetGuid(targetBase.gameObject);
            Vector3 itemPosition = constructableBase.gameObject.transform.position;
            Transform camera = Camera.main.transform;

            PlaceBasePiece(guid, ApiHelper.TechType(techType), itemPosition, quaternion, camera, Optional<String>.OfNullable(parentBaseGuid));
        }

        public void PlaceBasePiece(String guid, String techType, Vector3 itemPosition, Quaternion quaternion, Transform camera, Optional<String> parentBaseGuid)
        {
            PlaceBasePiece placedBasePiece = new PlaceBasePiece(PlayerId, guid, ApiHelper.Vector3(itemPosition), ApiHelper.Quaternion(quaternion), ApiHelper.Transform(camera), techType, parentBaseGuid);
            Send(placedBasePiece);
            Console.WriteLine(placedBasePiece);
        }

        public void PlaceFurniture(GameObject gameObject, TechType techType, Vector3 itemPosition, Quaternion quaternion)
        {
            String guid = GuidHelper.GetGuid(gameObject);
            String subGuid = GuidHelper.GetGuid(Player.main.GetCurrentSub().gameObject);
            Transform camera = Camera.main.transform;

            PlaceFurniture(guid, subGuid, ApiHelper.TechType(techType), itemPosition, quaternion, camera);
        }

        public void PlaceFurniture(String guid, String subGuid, String techType, Vector3 itemPosition, Quaternion quaternion, Transform camera)
        {
            PlaceFurniture placedFurniture = new PlaceFurniture(PlayerId, guid, subGuid, ApiHelper.Vector3(itemPosition), ApiHelper.Quaternion(quaternion), ApiHelper.Transform(camera), techType);
            Send(placedFurniture);
        }
                
        public void ChangeConstructionAmount(GameObject gameObject, float amount)
        {     
            Vector3 itemPosition = gameObject.transform.position;
            String guid = GuidHelper.GetGuid(gameObject);

            ConstructionAmountChanged amountChanged = new ConstructionAmountChanged(PlayerId, ApiHelper.Vector3(itemPosition), guid, amount);
            Send(amountChanged);            
        }

        public void ChangeConstructionAmount(String guid, Vector3 itemPosition, float amount)
        {
            if (amount < 1f) // Construction complete event handled by function below
            {
                ConstructionAmountChanged amountChanged = new ConstructionAmountChanged(PlayerId, ApiHelper.Vector3(itemPosition), guid, amount);
                Send(amountChanged);
            }
        }

        public void ConstructionComplete(GameObject gameObject)
        {
            Optional<String> newlyConstructedBaseGuid = Optional<String>.Empty();
            Optional<object> opConstructedBase = TransientLocalObjectManager.Get(TransientObjectType.BASE_GHOST_NEWLY_CONSTRUCTED_BASE_GAMEOBJECT);

            if (opConstructedBase.IsPresent())
            {
                GameObject constructedBase = (GameObject)opConstructedBase.Get();
                newlyConstructedBaseGuid = Optional<String>.Of(GuidHelper.GetGuid(constructedBase));
            }

            Vector3 itemPosition = gameObject.transform.position;
            String guid = GuidHelper.GetGuid(gameObject);

            ConstructionCompleted constructionCompleted = new ConstructionCompleted(PlayerId, ApiHelper.Vector3(itemPosition), guid, newlyConstructedBaseGuid);
            Send(constructionCompleted);
        }

        public void ConstructorBeginCrafting(GameObject constructor, TechType techType, float duration)
        {
            String constructorGuid = GuidHelper.GetGuid(constructor);

            Console.WriteLine("Building item from constructor with uuid: " + constructorGuid);

            Optional<object> opConstructedObject = TransientLocalObjectManager.Get(TransientObjectType.CONSTRUCTOR_INPUT_CRAFTED_GAMEOBJECT);

            if (opConstructedObject.IsPresent())
            {
                GameObject constructedObject = (GameObject)opConstructedObject.Get();
                String constructedObjectGuid = GuidHelper.GetGuid(constructedObject);
                ConstructorBeginCrafting beginCrafting = new ConstructorBeginCrafting(PlayerId, constructorGuid, constructedObjectGuid, ApiHelper.TechType(techType), duration);
                Send(beginCrafting);
            }
            else
            {
                Console.WriteLine("Could not send packet because there wasn't a corresponding constructed object!");
            }
        }

        public void SendChatMessage(String text)
        {
            ChatMessage message = new ChatMessage(PlayerId, text);
            Send(message);
        }

        public void AnimationChange(AnimChangeType type, AnimChangeState state)
        {
            AnimationChangeEvent animEvent = new AnimationChangeEvent(PlayerId, (int)type, (int)state);
            Send(animEvent);
        }

        public void Send(Packet packet)
        {
            if (Active)
            {
                try
                {
                    client.Send(packet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending packet " + packet, ex);
                }
            }
        }
    }
}
