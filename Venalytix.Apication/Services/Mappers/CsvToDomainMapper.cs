using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.Models.ModelsCSV;
using Venalytix.Domain.CSV;

namespace Venalytix.Apication.Services.Mappers
{
    public static class CsvToDomainMapper
    {
        //Mapear los clientes del CSV raw a la entidad Cliente del dominio
        public static List<Clientes> MapClientes(List<CustomerCsv> rawCustomers)
        {
            if (rawCustomers == null || !rawCustomers.Any())
                return new List<Clientes>();

            return rawCustomers.Select(c => new Clientes
            {
                IdCLiente = c.CustomerID,
                Nombre = $"{c.FirstName} {c.LastName}".Trim(),
                Email = c.Email,
                Region = c.Country,
                FechaRegistro = null // se puede usar más adelante si tu CSV o API la incluye
            }).ToList();
        }

        //Mapear productos
        public static List<Productos> MapProductos(List<ProductCsv> rawProducts)
        {
            if (rawProducts == null || !rawProducts.Any())
                return new List<Productos>();

            return rawProducts.Select(p => new Productos
            {
                IdProductos = p.ProductID,
                Nombre = p.ProductName, 
                Categoria = p.Category,
                Precio = p.Price,
                Activo = true
            }).ToList();
        }

        //Mapear ventas combinando orders + order_details
        public static List<Ventas> MapVentas(List<OrderCsv> orders, List<OrderDetailCsv> details)
        {
            if (orders == null || details == null)
                return new List<Ventas>();

            var ventas = new List<Ventas>();
            var ordersById = orders.ToDictionary(o => o.OrderID, o => o);

            foreach (var d in details)
            {
                if (!ordersById.TryGetValue(d.OrderID, out var order))
                    continue;

                ventas.Add(new Ventas
                {
                    IdVentas = d.OrderID,                   
                    IdCliente = order.CustomerID,
                    IdProducto = d.ProductID,
                    Cantidad = d.Quantity,
                    Precio = d.TotalPrice / d.Quantity,      
                    Fecha = order.OrderDate
                });
            }

            return ventas;
        }
    }
}
