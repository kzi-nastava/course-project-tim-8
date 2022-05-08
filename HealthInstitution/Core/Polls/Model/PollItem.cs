﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Core.Polls.Model
{
    internal class PollItem
    {
        public String Name { get; set; }
        public List<Grade> grades { get; set; }

        public PollItem(string name, List<Grade> grades)
        {
            this.Name = name;
            this.grades = grades;
        }
    }
}
