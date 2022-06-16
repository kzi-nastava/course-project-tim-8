﻿using HealthInstitution.Core;
using HealthInstitution.Core.SystemUsers.Patients.Model;
using HealthInstitution.GUI.PatientView;
using HealthInstitution.ViewModels.GUIViewModels.PatientViewViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Commands.PatientCommands.PatientWindowCommands;

public class PickDoctorCommand : CommandBase
{
    private Patient _loggedPatient;

    public PickDoctorCommand(Patient loggedPatient)
    {
        _loggedPatient = loggedPatient;
    }

    public override void Execute(object? parameter)
    {
        new DoctorPickExamination()
        {
            DataContext = new DoctorPickViewModel(_loggedPatient)
        }.ShowDialog();
    }
}