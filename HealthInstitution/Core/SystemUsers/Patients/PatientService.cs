using HealthInstitution.Core.SystemUsers.Patients.Model;
using HealthInstitution.Core.SystemUsers.Patients.Repository;
using HealthInstitution.Core.SystemUsers.Users;
﻿using HealthInstitution.Core.MedicalRecords;
using HealthInstitution.Core.SystemUsers.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthInstitution.Core.Operations.Model;
using HealthInstitution.Core.Operations.Repository;
using HealthInstitution.Core.TrollCounters;
using HealthInstitution.Core.Notifications.Model;
using HealthInstitution.Core.Notifications.Repository;

namespace HealthInstitution.Core.SystemUsers.Patients
{
    public static class PatientService
    {
        static PatientRepository s_patientRepository = PatientRepository.GetInstance();
        public static void LoadNotifications()
        {
            AppointmentNotificationPatientRepository.GetInstance();
            AppointmentNotificationDoctorRepository.GetInstance();
        }
        public static List<Patient> GetAll()
        {
            return s_patientRepository.GetAll();
        }
        public static Patient GetByUsername(string username)
        {
            return s_patientRepository.GetByUsername(username);
        }
        public static void ChangeBlockedStatus(string username)
        {
            Patient patient = GetByUsername(username);
            User user = UserService.GetByUsername(username);
            s_patientRepository.ChangeBlockedStatus(patient);
            UserService.ChangeBlockedStatus(user);
            //dodati u usera
        }
        public static void Add(UserDTO userDTO, MedicalRecords.Model.MedicalRecordDTO medicalRecordDTO)
        {
            Patient patient = new Patient(userDTO);
            medicalRecordDTO.Patient = patient;
            MedicalRecordService.Add(medicalRecordDTO);
            UserService.Add(userDTO);
            TrollCounterService.Add(userDTO.Username);
            s_patientRepository.Add(patient);
        }
        public static void Update(UserDTO userDTO)
        {
            Patient patient = new Patient(userDTO);
            s_patientRepository.Update(patient);
            UserService.Update(userDTO);
        }
        public static void Delete(string username)
        {
            s_patientRepository.Delete(username);
            TrollCounterService.Delete(username);
            UserService.Delete(username);
        }
        public static void DeleteNotifications(Patient patient)
        {
            s_patientRepository.DeleteNotifications(patient);
        }
        public static List<AppointmentNotification> GetActiveAppointmentNotification(Patient patient)
        {
            List<AppointmentNotification> appointmentNotifications=new List<AppointmentNotification>();
            foreach (AppointmentNotification appointmentNotification in patient.Notifications)
                if (appointmentNotification.ActiveForPatient)
                    appointmentNotifications.Add(appointmentNotification);
            return appointmentNotifications;
        }
    } 
}
