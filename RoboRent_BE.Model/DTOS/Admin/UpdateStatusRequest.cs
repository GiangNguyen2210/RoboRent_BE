using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboRent_BE.Model.DTOS.Admin;

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
