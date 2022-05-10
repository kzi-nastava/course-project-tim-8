﻿using HealthInstitution.Core.Equipments.Model;
using HealthInstitution.Core.Equipments.Repository;
using HealthInstitution.Core.Renovations.Model;
using HealthInstitution.Core.Renovations.Repository;
using HealthInstitution.Core.Rooms.Model;
using HealthInstitution.Core.Rooms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Core.Renovations.Functionality
{
    public class RenovationChecker
    {
        private static RenovationRepository s_renovationRepository = RenovationRepository.GetInstance();
        private static RoomRepository s_roomRepository = RoomRepository.GetInstance();
        private static EquipmentRepository s_equipmentRepository = EquipmentRepository.GetInstance();

        public static void UpdateByRenovation()
        {
            foreach (Renovation renovation in s_renovationRepository.Renovations)
            {

                if (renovation.GetType() == typeof(Renovation))
                {
                    if (renovation.StartDate == DateTime.Today)
                    {
                        StartRenovation(renovation.Room);
                    }

                    if (renovation.EndDate == DateTime.Today.AddDays(-1))
                    {
                        EndRenovation(renovation.Room);
                    }
                }
                else if (renovation.GetType() == typeof(RoomMerger))
                {
                    RoomMerger roomMerger = (RoomMerger)renovation;

                    if (roomMerger.StartDate == DateTime.Today)
                    {
                        StartMerge(roomMerger.Room, roomMerger.RoomForMerge, roomMerger.MergedRoom);
                    }

                    if (roomMerger.EndDate == DateTime.Today.AddDays(-1))
                    {
                        EndMerge(roomMerger.Room, roomMerger.RoomForMerge, roomMerger.MergedRoom);
                    }
                }
                else
                {
                    RoomSeparation roomSeparation = (RoomSeparation)renovation;
                    //todo
                }
            }
        }

        public static void StartRenovation(Room room)
        {
            s_roomRepository.Update(room.Id, room.Type, room.Number, true);
        }

        public static void EndRenovation(Room room)
        {
            s_roomRepository.Update(room.Id, room.Type, room.Number, false);
        }

        public static void StartMerge(Room firstRoom, Room secondRoom, Room mergedRoom)
        {           
            s_roomRepository.Update(firstRoom.Id, firstRoom.Type, firstRoom.Number, true);
            s_roomRepository.Update(secondRoom.Id, secondRoom.Type, secondRoom.Number, true);
        }

        public static void EndMerge(Room firstRoom, Room secondRoom, Room mergedRoom)
        {
            foreach (Equipment equipment in firstRoom.AvailableEquipment)
            {
                mergedRoom.AvailableEquipment.Add(equipment);
            }
            firstRoom.AvailableEquipment.Clear();

            foreach (Equipment equipment in secondRoom.AvailableEquipment)
            {
                int index = mergedRoom.AvailableEquipment.FindIndex(eq => eq.Name == equipment.Name && eq.Type == equipment.Type);
                if (index >= 0)
                {
                    mergedRoom.AvailableEquipment[index].Quantity += equipment.Quantity;
                    s_equipmentRepository.Delete(equipment.Id);
                }
                mergedRoom.AvailableEquipment.Add(equipment);
            }
            secondRoom.AvailableEquipment.Clear();

            s_roomRepository.Update(mergedRoom.Id, mergedRoom.Type, mergedRoom.Number, false, true);
            s_roomRepository.Update(firstRoom.Id, firstRoom.Type, firstRoom.Number, false, false);
            s_roomRepository.Update(secondRoom.Id, secondRoom.Type, secondRoom.Number, false, false);
        }


    }
}
