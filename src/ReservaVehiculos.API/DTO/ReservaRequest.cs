using System;
using ReservaVehiculos.API.Entities;

namespace ReservaVehiculos.API.DTO
{
    public class ReservaRequest
    {

        public string IdCotizacion { get; set; }
        public Entities.Pasajero Pasajero { get; set; }
    }
}
