using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class MessagePageModel
    {
        public List<List<Local_Web_Server.Models.MessagesModel>> organizedMessages { get; set; }
    }
}