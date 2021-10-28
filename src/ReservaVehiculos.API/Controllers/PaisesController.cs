using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReservaVehiculos.API.Services;
using ReservaVehiculos.API.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace ReservaVehiculos.API.Controllers
{
    
    [Route("[controller]")]
    [SwaggerResponse(400, "Bad Request", Type = typeof(ErrorResponse))]
    [SwaggerResponse(500, "Error no controlado", Type = typeof(ErrorResponse))]
    public class PaisesController : ControllerBase
    {
        private readonly ILogger<PaisesController> _logger;
        private readonly IConfiguration _config;
        private readonly PaisesService _service;
        public PaisesController(ILogger<PaisesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _service = new PaisesService();
        }

        [HttpGet]
        [SwaggerResponse (200, Type = typeof (IEnumerable<Pais>))]
        [Authorize(Policy = "read:paises")]
        public async Task<IActionResult> Get()
        {
            var response = await _service.ConsultarPaises();
            return Ok(response);

        }

        [HttpGet("{idPais}/ciudades")]
        [SwaggerResponse (200, Type = typeof (IEnumerable<Ciudad>))]
        public async Task<IActionResult> GetCiudades(string idPais)
        {
            var response = await _service.ConsultarCiudades(idPais);
            return Ok(response);

        }

        [HttpPost("bulk")]
        public IActionResult BulkPaises([FromBody] List<Pais> paises)
        {
            _service.GrabarPaises(paises);

            return Ok();
        }

    }
}
