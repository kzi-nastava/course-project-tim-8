﻿using HealthInstitution.Core.Examinations.Model;
using HealthInstitution.Core.Examinations.Repository;
using HealthInstitution.Core.Notifications.Model;
using HealthInstitution.Core.Operations.Model;
using HealthInstitution.Core.SystemUsers.Doctors.Model;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HealthInstitution.Core.SystemUsers.Doctors.Repository;

internal class DoctorRepository
{
    private String _fileName;
    public List<Doctor> Doctors { get; set; }
    public Dictionary<String, Doctor> DoctorsByUsername { get; set; }

    private JsonSerializerOptions _options = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    private DoctorRepository(String fileName)
    {
        this._fileName = fileName;
        this.Doctors = new List<Doctor>();
        this.DoctorsByUsername = new Dictionary<string, Doctor>();
        this.LoadFromFile();
    }

    private static DoctorRepository s_instance = null;

    public static DoctorRepository GetInstance()
    {
        if (s_instance == null)
        {
            s_instance = new DoctorRepository(@"..\..\..\Data\JSON\doctors.json");
        }
        return s_instance;
    }

    private Doctor Parse(JToken? doctor)
    {
        String username = (String)doctor["username"];
        String password = (String)doctor["password"];
        String name = (String)doctor["name"];
        String surname = (String)doctor["surname"];
        SpecialtyType specialtyType;
        Enum.TryParse(doctor["specialty"].ToString(), out specialtyType);
        return new Doctor(username, password, name, surname, specialtyType);
    }

    public void LoadFromFile()
    {
        var allDoctors = JArray.Parse(File.ReadAllText(this._fileName));
        foreach (var doctor in allDoctors)
        {
            Doctor loadedDoctor = Parse(doctor);
            this.Doctors.Add(loadedDoctor);
            this.DoctorsByUsername.Add(loadedDoctor.Username, loadedDoctor);
        }
    }

    private List<dynamic> PrepareForSerialization()
    {
        List<dynamic> reducedDoctors = new List<dynamic>();
        foreach (Doctor doctor in this.Doctors)
        {
            reducedDoctors.Add(new
            {
                username = doctor.Username,
                password = doctor.Password,
                name = doctor.Name,
                surname = doctor.Surname,
                specialty = doctor.Specialty
            });
        }
        return reducedDoctors;
    }

    public void Save()
    {
        var allDoctors = JsonSerializer.Serialize(PrepareForSerialization(), _options);
        File.WriteAllText(this._fileName, allDoctors);
    }

    public List<Doctor> GetAll()
    {
        return this.Doctors;
    }

    public Doctor GetById(String username)
    {
        if (this.DoctorsByUsername.ContainsKey(username))
            return this.DoctorsByUsername[username];
        return null;
    }

    public void DeleteExamination(Examination examination)
    {
        examination.Doctor.Examinations.Remove(examination);
        Save();
    }

    public void DeleteOperation(Operation operation)
    {
        operation.Doctor.Operations.Remove(operation);
        Save();
    }

    public List<Examination> GetScheduledExaminations(Doctor doctor)
    {
        var scheduledExaminations = new List<Examination>();
        foreach (var examination in doctor.Examinations)
        {
            if (examination.Status == ExaminationStatus.Scheduled)
                scheduledExaminations.Add(examination);
        }
        return scheduledExaminations;
    }

    public List<Operation> GetScheduledOperations(Doctor doctor)
    {
        var scheduledOperations = new List<Operation>();
        foreach (var operation in doctor.Operations)
        {
            if (operation.Status == ExaminationStatus.Scheduled)
                scheduledOperations.Add(operation);
        }
        return scheduledOperations;
    }

    public List<Examination> GetExaminationsInThreeDays(List<Examination> examinations)
    {
        var upcomingExaminations = new List<Examination>();
        DateTime today = DateTime.Now;
        DateTime dateForThreeDays = today.AddDays(3);
        foreach (Examination examination in examinations)
        {
            if (examination.Appointment <= dateForThreeDays && examination.Appointment >= today)
                upcomingExaminations.Add(examination);
        }
        return upcomingExaminations;
    }

    public List<Operation> GetOperationsInThreeDays(List<Operation> operations)
    {
        var upcomingOperations = new List<Operation>();
        DateTime today = DateTime.Now;
        DateTime dateForThreeDays = today.AddDays(3);
        foreach (Operation operation in upcomingOperations)
        {
            if (operation.Appointment <= dateForThreeDays && operation.Appointment >= today)
                upcomingOperations.Add(operation);
        }
        return upcomingOperations;
    }

    public List<Examination> GetExaminationsByDate(List<Examination> examinations, DateTime date)
    {
        var examinationsForDate = new List<Examination>();
        foreach (Examination examination in examinations)
        {
            if (examination.Appointment.Date == date)
                examinationsForDate.Add(examination);
        }
        return examinationsForDate;
    }

    public List<Operation> GetOperationsByDate(List<Operation> operations, DateTime date)
    {
        var operationsForDate = new List<Operation>();
        foreach (Operation operation in operations)
        {
            if (operation.Appointment.Date == date)
                operationsForDate.Add(operation);
        }
        return operationsForDate;
    }

    public void DeleteNotification(Doctor doctor, AppointmentNotification notification)
    {
        doctor.Notifications.Remove(notification);
        Save();
    }

    public List<Doctor> GetSearchName(string keyword)
    {
        keyword = keyword.Trim();
        List<Doctor> found = new List<Doctor>();

        foreach (Doctor doctor in DoctorsByUsername.Values)
        {
            if (doctor.Name.ToLower().Contains(keyword)) found.Add(doctor);
        }

        return found;
    }

    public List<Doctor> GetSearchSurname(string keyword)
    {
        keyword = keyword.Trim();
        List<Doctor> found = new List<Doctor>();

        foreach (Doctor doctor in DoctorsByUsername.Values)
        {
            if (doctor.Surname.ToLower().Contains(keyword)) found.Add(doctor);
        }

        return found;
    }

    public List<Doctor> GetSearchSpeciality(string keyword)
    {
        keyword = keyword.Trim();
        List<Doctor> found = new List<Doctor>();

        foreach (Doctor doctor in DoctorsByUsername.Values)
        {
            if (doctor.Specialty.ToString().ToLower().Contains(keyword)) found.Add(doctor);
        }

        return found;
    }
}