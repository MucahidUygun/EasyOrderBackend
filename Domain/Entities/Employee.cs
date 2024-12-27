using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Employee:User
{
    public int Sallary { get; set; }
    public string Position { get; set; }
}
