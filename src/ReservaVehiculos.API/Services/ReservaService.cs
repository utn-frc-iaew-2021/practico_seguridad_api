using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using ReservaVehiculos.API.Entities;
using ReservaVehiculos.API.DTO;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Linq;

using System.Threading.Tasks;

namespace ReservaVehiculos.API.Services
{
    public class ReservaService : IReservaService
    {
        private readonly DynamoDBContext _context;
        public ReservaService()
        {
            var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);

            _context = new DynamoDBContext(client);
        }
        public async Task<IEnumerable<Reserva>> ListarReservas()
        {
            var conditions = new List<ScanCondition>();

            var result = await _context.ScanAsync<Reserva>(conditions).GetRemainingAsync();

            return result;
        }

        public async Task<Reserva> ObtenerReserva(string codigoReserva)
        {
            var conditions = new List<ScanCondition>();

            var result = await _context.LoadAsync<Reserva>(codigoReserva);

            return result;
        }


        public async Task<Reserva> ReservarVehiculo(ReservaRequest reserva)
        {
            VehiculoCotizacion cotizacion = await _context.LoadAsync<VehiculoCotizacion>(reserva.IdCotizacion);

            var nvaReserva = new Reserva()
            {
                CodigoReserva = GenerarCodigoReserva(),
                Pasajero = reserva.Pasajero,
                FechaHoraRetiro = cotizacion.FechaHoraRetiro,
                FechaHoraDevolucion = cotizacion.FechaHoraDevolucion,
                FechaReserva = DateTime.Now,
                TotalReserva = cotizacion.PrecioTotal,
                UsuarioReserva = "Test",
                Ciudad = cotizacion.Ciudad,
                Vehiculo = cotizacion.Vehiculo,
                Estado = EstadoReservaEnum.Activa
            };
            await _context.SaveAsync<Reserva>(nvaReserva);

            return nvaReserva;
        }

         public async Task<Reserva> CancelarReserva(string codigoReserva)
        {
            var reserva = await _context.LoadAsync<Reserva>(codigoReserva);

            reserva.Estado = EstadoReservaEnum.Cancelada;
            reserva.FechaCancelacion = DateTime.Now;
            reserva.UsuarioCancelacion = "Test";
            await _context.SaveAsync<Reserva>(reserva);

            return reserva;
        }

        private static string GenerarCodigoReserva()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 5)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }


    }
}