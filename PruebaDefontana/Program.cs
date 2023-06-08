using Microsoft.EntityFrameworkCore;
using PruebaDefontana.Modelos;

using (var context =  new PruebaContext())
{

    // Listado Base
    var ListaBase = context.Venta.Include(v => v.VentaDetalles).Where(x => x.Fecha >= DateTime.Now.AddDays(-30));

    // 2.1 El total de ventas de los últimos 30 días (monto total y Q de ventas).
    Console.WriteLine("Pregunta 1");
    Console.WriteLine("Cantidad de Ventas: " + ListaBase.Count());
    Console.WriteLine("Monto Total de Ventas: " + ListaBase.Sum(s => s.Total).ToString("0.00"));
    Console.WriteLine("\n");

    // 2.2 El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).
    var p2 = ListaBase.OrderByDescending(s => s.Total).FirstOrDefault();
    Console.WriteLine("Pregunta 2");
    Console.WriteLine("El monto de la venta más alta fue " + p2?.Total.ToString("0.00") + " y se registro el " + p2?.Fecha);
    Console.WriteLine("\n");

    // 2.3 Indicar cuál es el producto con mayor monto total de ventas.  
    var p3 = ListaBase.SelectMany(venta => venta.VentaDetalles, (venta, detalle) => new
                                    {
                                        NomProducto = detalle.IdProductoNavigation.Nombre,
                                        MontoVenta = detalle.TotalLinea
                                    })
                      .GroupBy(detalle => detalle.NomProducto)
                      .Select(grupo => new
                              {
                                  NomProducto = grupo.Key,
                                  MontoVentas = grupo.Sum(detalle => detalle.MontoVenta)
                              })
                      .OrderByDescending(grupo => grupo.MontoVentas)
                      .FirstOrDefault();
    Console.WriteLine("Pregunta 3");
    Console.WriteLine("El producto con mayor venta es: " + p3?.NomProducto);
    Console.WriteLine("\n");

    // 2.4 Indicar el local con mayor monto de ventas.
    var p4 = ListaBase.Select(venta => new
                                    {
                                        NomLocal = venta.IdLocalNavigation.Nombre,
                                        MontoVenta = venta.Total
                                    })

                      .GroupBy(venta => venta.NomLocal)
                      .Select(grupo => new
                                    {
                                        NomLocal = grupo.Key,
                                        MontoVentas = grupo.Sum(venta => venta.MontoVenta)
                                    })
                      .OrderByDescending(grupo => grupo.MontoVentas)
                      .FirstOrDefault();
    Console.WriteLine("Pregunta 4");
    Console.WriteLine("El local con mayor venta es: " + p4?.NomLocal);
    Console.WriteLine("\n");

    // 2.5 ¿Cuál es la marca con mayor margen de ganancias?
    var p5 = ListaBase.SelectMany(venta => venta.VentaDetalles, (venta, detalle) => new
                                            {
                                                NomMarca = detalle.IdProductoNavigation.IdMarcaNavigation.Nombre,
                                                MontoVenta = detalle.TotalLinea
                                            })
                      .GroupBy(detalle => detalle.NomMarca)
                      .Select(grupo => new
                                          {
                                              NomMarca = grupo.Key,
                                              MontoVentas = grupo.Sum(detalle => detalle.MontoVenta)
                                          })
                      .OrderByDescending(grupo => grupo.MontoVentas)
                      .FirstOrDefault();
    Console.WriteLine("Pregunta 5");
    Console.WriteLine("La marca con mayor margen de ganancias es: " + p5?.NomMarca);
    Console.WriteLine("\n");

    // 2.6 ¿Cómo obtendrías cuál es el producto que más se vende en cada local?
    var p6 = ListaBase.SelectMany(venta => venta.VentaDetalles, (venta, detalle) => new
                                            {
                                                NomLocal = venta.IdLocalNavigation.Nombre,
                                                NomProducto = detalle.IdProductoNavigation.Nombre,
                                                MontoVenta = detalle.TotalLinea
                                            })
                      .GroupBy(detalle => new { detalle.NomLocal, detalle.NomProducto })
                      .Select(grupo => new
                                            {
                                                NomLocal = grupo.Key.NomLocal,
                                                NomProducto = grupo.Key.NomProducto,
                                                MontoVentas = grupo.Sum(detalle => detalle.MontoVenta)
                                            })
                      .GroupBy(g => g.NomLocal)
                      .Select(g => new
                                            {
                                                NomLocal = g.Key,
                                                ProductoMayorVenta = g.OrderByDescending(p => p.MontoVentas).First()
                                            })
                      .ToList();
    Console.WriteLine("Pregunta 6");
    Console.WriteLine("La lista de producto que más se vende por cada local es:");
    Console.WriteLine($"{("Local").PadRight(15)}{("Producto").PadRight(30)}{("Monto de Ventas").PadRight(15)}");
    Console.WriteLine(new string('-', 60));

    foreach (var item in p6)
    {
        Console.WriteLine($"{item.NomLocal.PadRight(15)}{item.ProductoMayorVenta.NomProducto.PadRight(30)}{item.ProductoMayorVenta.MontoVentas.ToString("0.00").PadRight(15)}");
    }

}
