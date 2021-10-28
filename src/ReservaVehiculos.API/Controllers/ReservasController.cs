using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReservaVehiculos.API.Services;
using ReservaVehiculos.API.Entities;
using ReservaVehiculos.API.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace ReservaVehiculos.API.Controllers
{
    [Route("[controller]")]
    [SwaggerResponse(400, "Bad Request", Type = typeof(ErrorResponse))]
    [SwaggerResponse(500, "Error no controlado", Type = typeof(ErrorResponse))]
    public class ReservasController : ControllerBase
    {
        private readonly ILogger<ReservasController> _logger;
        private readonly IConfiguration _config;
        private readonly IReservaService _service;
        public ReservasController(ILogger<ReservasController> logger, IConfiguration config, IReservaService service)
        {
            _logger = logger;
            _config = config;
            _service = service;
        }


        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<Reserva>))]
        public async Task<IActionResult> GetTodos()
        {
            var response = await _service.ListarReservas();

            return Ok(response);
        }


        [HttpGet("{codigoReserva}")]
        [SwaggerResponse(200, Type = typeof(Reserva))]
        public async Task<IActionResult> GetReserva(string codigoReserva)
        {
            var response = await _service.ObtenerReserva(codigoReserva);
            return Ok(response);

        }

        [HttpPost]
        [SwaggerResponse(201, Type = typeof(Reserva))]
        public async Task<IActionResult> Post([FromBody] ReservaRequest reserva)
        {
            var response = await _service.ReservarVehiculo(reserva);
            return Created("/reservas/" + response.CodigoReserva, response);
        }

        [HttpDelete("{codigoReserva}")]
        [SwaggerResponse(204)]
        public async Task<ActionResult> CancelarReserva(string codigoReserva)
        {
            await _service.CancelarReserva(codigoReserva);

            return NoContent();
        }
    }
}
