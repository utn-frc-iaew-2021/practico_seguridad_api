namespace ReservaVehiculos.API.Entities
{
    public class Vehiculo
    {
        public string Id { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Puntaje { get; set; }
        public string TieneDireccion { get; set; }
        public string TieneAireAcon { get; set; }
        public string CantidadPuertas { get; set; }
        public string TipoCambio { get; set; }
    }
}
