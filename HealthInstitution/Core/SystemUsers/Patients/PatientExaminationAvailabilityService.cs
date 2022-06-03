﻿using HealthInstitution.Core.Examinations.Model;
using HealthInstitution.Core.Examinations.Repository;
using HealthInstitution.Core.Operations.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Core.SystemUsers.Patients
{
    internal class PatientExaminationAvailabilityService
    {
        private void CheckIfPatientHasExaminations(ExaminationDTO examinationDTO)
        {
            var patient = examinationDTO.MedicalRecord.Patient;
            DateTime appointment = examinationDTO.Appointment;
            var patientExaminations = ExaminationRepository.GetInstance().GetPatientExaminations(patient);

            foreach (var examination in patientExaminations)
            {
                if (examination.Appointment == appointment)
                {
                    throw new Exception("That patient is not available");
                }
            }
        }

        private void CheckIfPatientHasOperations(ExaminationDTO examinationDTO)
        {
            var patient = examinationDTO.MedicalRecord.Patient;
            DateTime appointment = examinationDTO.Appointment;
            var patientOperations = OperationRepository.GetInstance().GetPatientOperations(patient);

            foreach (var operation in patientOperations)
            {
                if ((appointment < operation.Appointment.AddMinutes(operation.Duration)) && (appointment.AddMinutes(15) > operation.Appointment))
                {
                    throw new Exception("That patient is not available");
                }
            }
        }

        private void CheckIfPatientIsAvailable(ExaminationDTO examinationDTO)
        {
            CheckIfPatientHasExaminations(examinationDTO);
            CheckIfPatientHasOperations(examinationDTO);
        }
    }
}