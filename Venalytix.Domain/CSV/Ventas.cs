using Venalytix.Domain.Base;

public class Ventas : CSVBASE
{
    public int IdVentas { get; set; }
    public int IdCliente { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total => Cantidad * Precio;
}

