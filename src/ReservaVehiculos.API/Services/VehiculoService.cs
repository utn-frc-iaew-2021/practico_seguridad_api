using System.Runtime.CompilerServices;
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
using ReservaVehiculos.API.Entities;

namespace ReservaVehiculos.API.Services
{
    public class VehiculoService
    {

        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;

        public VehiculoService()
        {
            _client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            _context = new DynamoDBContext(_client);
        }

        public async Task<IEnumerable<Vehiculo>> ConsultarVehiculos()
        {
            var conditions = new List<ScanCondition>();

            var result = await _context.ScanAsync<Vehiculo>(conditions).GetRemainingAsync();
            // if (!paises.Any())
            //     throw new CustomException(61, "No se encontraron paises.");

            return result;
        }

        public async Task<List<VehiculoCotizacion>> CotizarVehiculos(int idCiudad, DateTime fechaHoraRetiro, DateTime fechaHoraDevolucion)
        {
            var listaCotizacion = new List<VehiculoCotizacion>();
            Ciudad ciudad = await _context.LoadAsync<Ciudad>(idCiudad.ToString());

            if (ciudad == null)
                throw new CustomException(400, "La ciudad no se encuentra registrada.");

            var conditions = new List<ScanCondition>();

            var result = await _context.ScanAsync<Reserva>(conditions).GetRemainingAsync();

            //Obtenemos la cantidad de vehiculos reservados por Ciudad
            var vehiculsoReservados = (from r in result
                                       where (r.FechaHoraRetiro >= fechaHoraRetiro && r.FechaHoraRetiro <= fechaHoraDevolucion) ||
(r.FechaHoraDevolucion >= fechaHoraRetiro && r.FechaHoraDevolucion <= fechaHoraDevolucion) &&
r.Estado == EstadoReservaEnum.Activa
                                       group r by r.Ciudad.Id into grp
                                       select new { Key = grp.Key.ToString(), Count = grp.Count() });

            //Buscamos los vehiculos disponibles por ciudad.

            var condCiudad = new List<ScanCondition>();
            condCiudad.Add(new ScanCondition("CiudadId", ScanOperator.Equal, idCiudad.ToString()));
            var vehiculosPorCiudad = await _context.ScanAsync<VehiculoPorCiudad>(condCiudad).GetRemainingAsync();

            var diffFecha = fechaHoraDevolucion.Subtract(fechaHoraRetiro);

            foreach (var item in vehiculosPorCiudad)
            {
                var resultado = vehiculsoReservados.FirstOrDefault(p => p.Key == item.VehiculoId);
                if (resultado != null)
                {
                    item.CantidadDisponible -= vehiculsoReservados.First(p => p.Key == item.VehiculoId).Count;
                }
                if (item.CantidadDisponible > 0)
                {
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.AddMinutes(10).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    var cotizacion = new VehiculoCotizacion()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Ciudad = ciudad,
                        Vehiculo = await _context.LoadAsync<Vehiculo>(item.VehiculoId),
                        FechaHoraRetiro = fechaHoraRetiro,
                        FechaHoraDevolucion = fechaHoraDevolucion,
                        PrecioPorDia = item.PrecioPorDia,
                        PrecioTotal = item.PrecioPorDia * (decimal)diffFecha.TotalDays,
                        CantidadDias = diffFecha.TotalDays.ToString(),
                        CantidadVehiculosDisponibles = item.CantidadDisponible,
                        FechaCotizacion = DateTime.Now,
                        TTL = unixTimestamp
                    };
                    listaCotizacion.Add(cotizacion);

                    var task = _context.SaveAsync<VehiculoCotizacion>(cotizacion);
                }
            }

            return listaCotizacion;
        }

        public async void GrabarVehiculos(List<Vehiculo> vehiculos)
        {
            foreach (var item in vehiculos)
            {
                await _context.SaveAsync<Vehiculo>(item);
            }

            return;
        }

        public async Task<List<VehiculoPorCiudad>> BuscarPorCiudad(string idCiudad)
        {
            //Buscamos los vehiculos disponibles por ciudad.
            var condCiudad = new List<ScanCondition>();
            condCiudad.Add(new ScanCondition("CiudadId", ScanOperator.Equal, idCiudad));
            var vehiculosPorCiudad = await _context.ScanAsync<VehiculoPorCiudad>(condCiudad).GetRemainingAsync();
            return vehiculosPorCiudad;
        }

        public async Task<VehiculoPorCiudad> AgregarACiudad(string idCiudad, VehiculoPorCiudad vehiculo)
        {
            vehiculo.Id = Guid.NewGuid().ToString();
            // vehiculo.Ciudad = await _context.LoadAsync<Ciudad> (idCiudad);
            await _context.SaveAsync<VehiculoPorCiudad>(vehiculo);

            return vehiculo;
        }

        public async Task AgregarVehiculosPorCiudad(List<VehiculoPorCiudad> VehiculosPorCiudad)
        {
            foreach (var item in VehiculosPorCiudad)
            {

                item.Id = Guid.NewGuid().ToString();
                await _context.SaveAsync<VehiculoPorCiudad>(item);
            }


        }

    }
}