using System;
using Amazon.DynamoDBv2.DataModel;

namespace ReservaVehiculos.API.Entities
{

    public class VehiculoCotizacion
    {
        public string Id { get; set; }
        public virtual Ciudad Ciudad { get; set; }
        public virtual Vehiculo Vehiculo { get; set; }

        public System.DateTime FechaHoraRetiro { get; set; }
        public System.DateTime FechaHoraDevolucion { get; set; }

        public string CantidadDias { get; set; }
        public Decimal PrecioPorDia { get; set; }

        public Decimal PrecioTotal { get; set; }

        public int CantidadVehiculosDisponibles { get; set; }

        public System.DateTime FechaCotizacion { get; set; }

        [DynamoDBProperty("ttl")]
        public Int32 TTL { get; set; }

    }
}
