using System.Collections.Generic;
using ReservaVehiculos.API.Entities;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

using System.Threading.Tasks;

namespace ReservaVehiculos.API.Services
{
    public class PaisesService
    {
        private readonly DynamoDBContext _context;
        public PaisesService()
        {
            var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);

            _context = new DynamoDBContext(client);
        }
        public async Task<IEnumerable<Pais>> ConsultarPaises()
        {
            var conditions = new List<ScanCondition>();
            var paises = await _context.ScanAsync<Pais>(conditions).GetRemainingAsync();
            return paises;
        }

        public async Task<IEnumerable<Ciudad>> ConsultarCiudades(string idPais)
        {
            var conditions = new List<ScanCondition>();
            if (!string.IsNullOrEmpty(idPais))
            {
                conditions.Add(new ScanCondition("PaisId", ScanOperator.Equal, idPais));

            }
            var ciudades = await _context.ScanAsync<Ciudad>(conditions).GetRemainingAsync();

            return ciudades;
        }

        public async void GrabarCiudades(List<Ciudad> ciudades)
        {
            foreach (var ciudad in ciudades)
            {
                await _context.SaveAsync<Ciudad>(ciudad);
            }

            return;
        }

         public async void GrabarPaises(List<Pais> paises)
        {
            foreach (var item in paises)
            {
                await _context.SaveAsync<Pais>(item);
            }

            return;
        }
    }
}