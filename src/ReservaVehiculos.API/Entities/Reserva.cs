using System;

namespace ReservaVehiculos.API.Entities
{

    public class Reserva
    {
        public string CodigoReserva { get; set; }
        public System.DateTime FechaReserva { get; set; }
        public string UsuarioReserva { get; set; }
        public decimal TotalReserva { get; set; }
        public Pasajero Pasajero { get; set; }
        public System.DateTime FechaHoraRetiro { get; set; }
        public System.DateTime FechaHoraDevolucion { get; set; }
        public EstadoReservaEnum Estado { get; set; }
        public Nullable<System.DateTime> FechaCancelacion { get; set; }
        public string UsuarioCancelacion { get; set; }

        public virtual Ciudad Ciudad { get; set; }

        public virtual Vehiculo Vehiculo { get; set; }
    }
}
