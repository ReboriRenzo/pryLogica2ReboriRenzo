using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pryTrabajoPracticoLOGICA2
{

    public class Medicamento
    {
        public int Id { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Nombre { get; set; }
        public string Laboratorio { get; set; }
        public int StockMinimo { get; set; }
        public decimal Precio { get; set; }
    }

}
