using StudentPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPortal.Models.Dtos.Requests;

public class CreateAccountDto
{
    public string StudentId { get; set; }
}