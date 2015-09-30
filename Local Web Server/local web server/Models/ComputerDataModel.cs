using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class ComputerData
    {
        public string IPAddress { get; set; }
        public string Mac { get; set; }
        public string HostName { get; set; }
    }
}