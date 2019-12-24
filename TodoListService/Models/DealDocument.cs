using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GREATRoomAPI.Models
{
    public class DealDocument
    {
        public string Title { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string Url { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
