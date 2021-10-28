using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using ReservaVehiculos.API.DTO;
using ReservaVehiculos.API.Entities;

namespace ReservaVehiculos.API.Services
{
    public interface IReservaService
    {
        Task<IEnumerable<Reserva>> ListarReservas();
        Task<Reserva> ObtenerReserva(string codigoReserva);

        Task<Reserva> ReservarVehiculo(ReservaRequest reserva);

        Task<Reserva> CancelarReserva(string codigoReserva);
    }
}