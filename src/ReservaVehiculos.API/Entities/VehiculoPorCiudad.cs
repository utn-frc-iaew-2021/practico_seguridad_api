using System;
namespace ReservaVehiculos.API.Entities
{
    public class VehiculoPorCiudad
    {
        public string Id { get; set; }
        public int CantidadDisponible { get; set; }
        public Decimal PrecioPorDia { get; set; }
        public string CiudadId { get; set; }
        public string VehiculoId { get; set; }
    }
}