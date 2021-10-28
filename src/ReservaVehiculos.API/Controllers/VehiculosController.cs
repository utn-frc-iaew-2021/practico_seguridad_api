using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReservaVehiculos.API.Services;
using ReservaVehiculos.API.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace ReservaVehiculos.API.Controllers
{
    [Route("[controller]")]
    [SwaggerResponse(400, "Bad Request", Type = typeof(ErrorResponse))]
    [SwaggerResponse(500, "Error no controlado", Type = typeof(ErrorResponse))]
    public class VehiculosController : ControllerBase
    {
        private readonly ILogger<VehiculosController> _logger;
        private readonly IConfiguration _config;
        private readonly VehiculoService _service;
        public VehiculosController(ILogger<VehiculosController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _service = new VehiculoService();
        }

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<Vehiculo>))]
        public async Task<IActionResult> Get()
        {
            var response = await _service.ConsultarVehiculos();
            return Ok(response);
        }

        [HttpGet("cotizar")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<VehiculoCotizacion>))]
        public async Task<IActionResult> Cotizar(int idCiudad, DateTime fechaHoraRetiro, DateTime fechaHoraDevolucion)
        {
            if(fechaHoraDevolucion.Year == 1)
            {
                throw new CustomException(400, "FechaDevolucion invalida.");
            }
            var response = await _service.CotizarVehiculos(idCiudad, fechaHoraRetiro, fechaHoraDevolucion);
            return Ok(response);
        }

        [HttpPost("bulk")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<VehiculoCotizacion>))]
        public async Task<IActionResult> BulkVehiculos([FromBody] List<VehiculoPorCiudad> vehiculos)
        {
            await _service.AgregarVehiculosPorCiudad(vehiculos);

            return Ok();
        }


    }
}
