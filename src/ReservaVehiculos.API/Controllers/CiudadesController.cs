using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReservaVehiculos.API.Entities;
using ReservaVehiculos.API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ReservaVehiculos.API.Controllers {
    [Route ("[controller]")]
    [SwaggerResponse(400, "Bad Request", Type = typeof(ErrorResponse))]
    [SwaggerResponse(500, "Error no controlado", Type = typeof(ErrorResponse))]
    public class CiudadesController : ControllerBase {
        private readonly ILogger<CiudadesController> _logger;
        private readonly IConfiguration _config;
        private readonly PaisesService _service;
        private readonly VehiculoService _vehiculoService;
        public CiudadesController (ILogger<CiudadesController> logger, IConfiguration config) {
            _logger = logger;
            _config = config;
            _service = new PaisesService ();
            _vehiculoService = new VehiculoService ();
        }

        [HttpGet]
        [SwaggerResponse (200, Type = typeof (IEnumerable<Ciudad>))]
        public async Task<IActionResult> Get () {
            // System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // var client_name = currentUser.Claims.FirstOrDefault(x => x.Type == "https://example.com/client_name")?.Value;
            var response = await _service.ConsultarCiudades (null);
            return Ok (response);

        }

        [HttpGet ("{idCiudad}/vehiculos")]
        [SwaggerResponse (200, Type = typeof (List<VehiculoPorCiudad>))]
        public async Task<IActionResult> GetVehiculos (string idCiudad) {
            var response = await _vehiculoService.BuscarPorCiudad (idCiudad);
            return Ok (response);

        }

        [HttpPost ("{idCiudad}/vehiculos")]
        [SwaggerResponse (200, Type = typeof (List<VehiculoPorCiudad>))]
        public async Task<IActionResult> PostVehiculos (string idCiudad, [FromBody] VehiculoPorCiudad vehiculo) {
            var response = await _vehiculoService.AgregarACiudad (idCiudad, vehiculo);
            return Ok (response);

        }

        [HttpPost ("bulk")]
        public IActionResult BulkCiudades ([FromBody] List<Ciudad> ciudades) {
            _service.GrabarCiudades (ciudades);

            return Ok ();
        }

    }
}