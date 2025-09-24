namespace ProyectoPanaderia.ViewModels
{
    public class ItemCarritoVM
    {
        public int Id { get; set; }
        public int UsuarioId {  get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public string RolUsuario { get; set; }
    }
}
