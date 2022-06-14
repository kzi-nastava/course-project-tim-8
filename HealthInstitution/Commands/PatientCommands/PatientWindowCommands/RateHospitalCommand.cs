﻿using HealthInstitution.Core;
using HealthInstitution.GUI.PatientView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Commands.PatientCommands.PatientWindowCommands;

public class RateHospitalCommand : CommandBase
{
    public override void Execute(object? parameter)
    {
        new PatientHospitalPollDialog().ShowDialog();
    }
}