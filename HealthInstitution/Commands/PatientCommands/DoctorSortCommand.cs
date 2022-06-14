﻿using HealthInstitution.Core;
using HealthInstitution.Core.Examinations;
using HealthInstitution.GUI.PatientViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Commands.PatientCommands;

public class DoctorSortCommand : CommandBase
{
    private MedicalRecordViewViewModel _medicalRecordViewModel;

    public DoctorSortCommand(MedicalRecordViewViewModel medicalRecordViewModel)
    {
        _medicalRecordViewModel = medicalRecordViewModel;
    }

    public override void Execute(object? parameter)
    {
        _medicalRecordViewModel.Examinations = ExaminationService.OrderByDoctor(_medicalRecordViewModel.Examinations);
        _medicalRecordViewModel.PutIntoGrid();
    }
}