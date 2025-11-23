using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAvailabilityServer.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public int floor { get; set; }
        public bool occupied { get; set; }
    }
}
