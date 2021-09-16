using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Ex.Models
{
    /// <summary>
    /// A class that stores the message entered by the user
    /// </summary>
    public class ChatMessage 
    {
        [Key]
        public Guid Id { get; set; }
        //Username for the user
        public string UserName { get; set; }
        //Message sent by the user
        public string Message { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
