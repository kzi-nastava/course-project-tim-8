﻿using HealthInstitution.Core.Notifications.Repository;
using HealthInstitution.Core.RestRequestNotifications.Model;
using HealthInstitution.Core.SystemUsers.Doctors.Model;
using HealthInstitution.Core.SystemUsers.Doctors.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthInstitution.Core.RestRequestNotifications.Repository
{
    public class RestRequestNotificationDoctorRepository
    {
        private String _fileName;
        private RestRequestNotificationDoctorRepository(String fileName)
        {
            this._fileName = fileName;
            this.LoadFromFile();
        }

        private static RestRequestNotificationDoctorRepository s_instance = null;

        public static RestRequestNotificationDoctorRepository GetInstance()
        {
            {
                if (s_instance == null)
                {
                    s_instance = new RestRequestNotificationDoctorRepository(@"..\..\..\Data\JSON\restRequestNotificationDoctor.json");
                }
                return s_instance;
            }
        }

        public void LoadFromFile()
        {
            var doctorsByUsername = DoctorRepository.GetInstance().DoctorsByUsername;
            var notificationsById = RestRequestNotificationRepository.GetInstance().NotificationsById;
            var doctorUseranamesNotificationIds = JArray.Parse(File.ReadAllText(this._fileName));
            foreach (var pair in doctorUseranamesNotificationIds)
            {
                int id = (int)pair["id"];
                String username = (String)pair["username"];
                Doctor doctor = doctorsByUsername[username];
                RestRequestNotification notification = notificationsById[id];
                doctor.RestRequestNotifications.Add(notification);
                notification.RestRequest.Doctor = doctor;
            }
        }

        public void Save()
        {
            List<dynamic> doctorUseranamesNotificationIds = new List<dynamic>();
            var notifications = RestRequestNotificationRepository.GetInstance().Notifications;
            foreach (var notification in notifications)
            {
                Doctor doctor = notification.RestRequest.Doctor;
                if (notification.Active)
                    doctorUseranamesNotificationIds.Add(new { id = notification.Id, username = doctor.Username });
            }
            var allPairs = JsonSerializer.Serialize(doctorUseranamesNotificationIds);
            File.WriteAllText(this._fileName, allPairs);
        }
    }
}
