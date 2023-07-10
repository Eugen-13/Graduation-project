using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Models
{
    public class MapModel
    {
        public int Id { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int IsPlace { get; set; }
        public int PlaceId { get; set; } = -1;
        public string Text { get; set; } = null;    

    }
}
