using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fundamentos_EP
{
    public class Expediente
    {
        public int Codigo;
        public DateTime Fecha;
        public string Remitente;
        public string Asunto;
        public int Tipo;
        public string Area;
        public int PlazoDias;
        public int Estado = 1;   // 1 Recibido, 2 En atención, 3 Archivado

        public DateTime FechaLimite
        {
            get { return Fecha.AddDays(PlazoDias); }
        }
    }

    // ---------- LÓGICA DEL ALGORITMO ----------
    public static class GestorExpedientes
    {
        public const int Capacidad = 100;

        // tipo -> (descripción, área responsable, plazo en días)
        public static readonly Dictionary<int, string[]> Tipos = new Dictionary<int, string[]>
        {
            { 1, new[] { "Solicitud de cliente", "Ventas y Marketing", "5" } },
            { 2, new[] { "Contrato de servicios", "Administración y Finanzas", "10" } },
            { 3, new[] { "Factura o comprobante de pago", "Administración y Finanzas", "3" } },
            { 4, new[] { "Memorando, oficio o carta", "Gerencia General", "2" } },
            { 5, new[] { "Expediente del personal", "Recursos Humanos", "7" } },
            { 6, new[] { "Certificado o permiso", "Operaciones", "7" } },
        };

        public static readonly string[] Estados = { "", "Recibido", "En atención", "Archivado" };

        // Código único correlativo: AAAA + secuencia de 3 dígitos (p. ej. 2026001).
        public static int GenerarCodigo(int anio, List<Expediente> lista)
        {
            return anio * 1000 + lista.Count + 1;
        }

        // Regla de derivación tipo -> área (acceso directo, O(1)).
        public static string DerivarArea(int tipo)
        {
            return Tipos[tipo][1];
        }

        // Registra al final de la lista, que queda ordenada por código.
        public static Expediente RegistrarDocumento(List<Expediente> lista, int anio,
            DateTime fecha, string remitente, string asunto, int tipo)
        {
            if (!Tipos.ContainsKey(tipo))
                throw new ArgumentException("Tipo de documento no válido (1 a 6).");
            if (string.IsNullOrWhiteSpace(remitente) || string.IsNullOrWhiteSpace(asunto))
                throw new ArgumentException("Remitente y asunto son obligatorios.");
            if (lista.Count >= Capacidad)
                throw new InvalidOperationException("Capacidad máxima de expedientes alcanzada.");

            Expediente exp = new Expediente();
            exp.Codigo = GenerarCodigo(anio, lista);
            exp.Fecha = fecha;
            exp.Remitente = remitente.Trim();
            exp.Asunto = asunto.Trim();
            exp.Tipo = tipo;
            exp.Area = DerivarArea(tipo);
            exp.PlazoDias = int.Parse(Tipos[tipo][2]);
            lista.Add(exp);
            return exp;
        }

        // Búsqueda binaria por código, O(log n). Retorna el índice o -1 si no existe.
        public static int BuscarExpediente(List<Expediente> lista, int codigo)
        {
            int inf = 0, sup = lista.Count - 1;
            while (inf <= sup)
            {
                int medio = (inf + sup) / 2;
                if (lista[medio].Codigo == codigo) return medio;
                if (lista[medio].Codigo < codigo) inf = medio + 1;
                else sup = medio - 1;
            }
            return -1;
        }

        // Ordenamiento por inserción (estable) por fecha; no modifica la lista original.
        public static List<Expediente> OrdenarPorFecha(List<Expediente> lista)
        {
            List<Expediente> orden = new List<Expediente>(lista);
            for (int i = 1; i < orden.Count; i++)
            {
                Expediente clave = orden[i];
                int j = i - 1;
                while (j >= 0 && orden[j].Fecha > clave.Fecha)
                {
                    orden[j + 1] = orden[j];
                    j--;
                }
                orden[j + 1] = clave;
            }
            return orden;
        }
    }

    // ---------- PRUEBAS SIMPLES ----------
    public static class Pruebas
    {
        static int ok = 0, fallos = 0;

        static void Verificar(bool condicion, string nombre)
        {
            if (condicion) ok++; else fallos++;
            Console.WriteLine((condicion ? "ok     " : "FALLÓ  ") + nombre);
        }

        static List<Expediente> Ejemplo()
        {
            List<Expediente> l = new List<Expediente>();
            GestorExpedientes.RegistrarDocumento(l, 2026, new DateTime(2026, 10, 5), "Colegio San José", "Solicitud de cotización", 1);
            GestorExpedientes.RegistrarDocumento(l, 2026, new DateTime(2026, 10, 2), "Proveedor Andes", "Factura N° 001-245", 3);
            GestorExpedientes.RegistrarDocumento(l, 2026, new DateTime(2026, 10, 9), "Gerencia General", "Memorando de reunión", 4);
            GestorExpedientes.RegistrarDocumento(l, 2026, new DateTime(2026, 10, 2), "Municipalidad", "Oficio de permiso", 6);
            return l;
        }

        public static int Ejecutar()
        {
            List<Expediente> l = Ejemplo();
            Verificar(l[0].Codigo == 2026001 && l[3].Codigo == 2026004, "códigos únicos y correlativos");
            Verificar(GestorExpedientes.DerivarArea(3) == "Administración y Finanzas", "derivación por tipo");

            bool lanzo = false;
            try { GestorExpedientes.RegistrarDocumento(new List<Expediente>(), 2026, DateTime.Today, "X", "Y", 9); }
            catch (ArgumentException) { lanzo = true; }
            Verificar(lanzo, "tipo inválido lanza excepción");

            bool todos = true;
            for (int i = 0; i < l.Count; i++)
                if (GestorExpedientes.BuscarExpediente(l, l[i].Codigo) != i) todos = false;
            Verificar(todos, "búsqueda encuentra cada código");
            Verificar(GestorExpedientes.BuscarExpediente(l, 2026999) == -1, "código inexistente retorna -1");
            Verificar(GestorExpedientes.BuscarExpediente(new List<Expediente>(), 2026001) == -1, "lista vacía retorna -1");

            List<Expediente> o = GestorExpedientes.OrdenarPorFecha(l);
            Verificar(o[0].Codigo == 2026002 && o[1].Codigo == 2026004 &&
                      o[2].Codigo == 2026001 && o[3].Codigo == 2026003, "orden por fecha estable");
            Verificar(l[0].Codigo == 2026001, "el ordenamiento no modifica la lista original");

            Console.WriteLine("\n" + ok + " pruebas correctas, " + fallos + " fallidas.");
            return fallos;
        }
    }

    // ---------- PROGRAMA PRINCIPAL ----------
    public static class Program
    {
        static string Fmt(DateTime d)
        {
            return d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        static void Mostrar(Expediente e)
        {
            Console.WriteLine(e.Codigo + " | " + Fmt(e.Fecha) + " | " + e.Remitente + " | " + e.Asunto +
                " | " + e.Area + " | límite " + Fmt(e.FechaLimite) + " | " + GestorExpedientes.Estados[e.Estado]);
        }

        static void ListarPorFecha(List<Expediente> lista)
        {
            if (lista.Count == 0) Console.WriteLine("No hay expedientes registrados.");
            foreach (Expediente e in GestorExpedientes.OrdenarPorFecha(lista)) Mostrar(e);
        }

        static void Demo()
        {
            List<Expediente> lista = new List<Expediente>();
            Console.WriteLine("== Registro ==");
            Mostrar(GestorExpedientes.RegistrarDocumento(lista, 2026, new DateTime(2026, 10, 5), "Colegio San José", "Solicitud de cotización", 1));
            Mostrar(GestorExpedientes.RegistrarDocumento(lista, 2026, new DateTime(2026, 10, 2), "Proveedor Andes", "Factura N° 001-245", 3));
            Mostrar(GestorExpedientes.RegistrarDocumento(lista, 2026, new DateTime(2026, 10, 9), "Gerencia General", "Memorando de reunión", 4));
            Mostrar(GestorExpedientes.RegistrarDocumento(lista, 2026, new DateTime(2026, 10, 2), "Municipalidad", "Oficio de permiso", 6));

            Console.WriteLine("\n== Búsqueda del código 2026003 ==");
            int pos = GestorExpedientes.BuscarExpediente(lista, 2026003);
            if (pos >= 0) Mostrar(lista[pos]); else Console.WriteLine("No encontrado");

            Console.WriteLine("\n== Búsqueda del código 2026999 ==");
            Console.WriteLine(GestorExpedientes.BuscarExpediente(lista, 2026999) < 0 ? "No encontrado" : "Encontrado");

            Console.WriteLine("\n== Listado ordenado por fecha ==");
            ListarPorFecha(lista);
        }

        static void Menu()
        {
            List<Expediente> lista = new List<Expediente>();
            Console.Write("Año de registro (AAAA): ");
            int año = int.Parse(Console.ReadLine());
            while (true)
            {
                Console.WriteLine("\n1. Registrar  2. Buscar  3. Listar por fecha  4. Cambiar estado  0. Salir");
                Console.Write("Opción: ");
                string op = Console.ReadLine();
                if (op == null) break;
                op = op.Trim();
                try
                {
                    if (op == "1")
                    {
                        foreach (KeyValuePair<int, string[]> t in GestorExpedientes.Tipos)
                            Console.WriteLine("  " + t.Key + ". " + t.Value[0]);
                        Console.Write("Fecha (dd/mm/aaaa): ");
                        DateTime fecha;
                        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out fecha))
                            throw new ArgumentException("Fecha no válida.");
                        Console.Write("Remitente: "); string rem = Console.ReadLine();
                        Console.Write("Asunto: "); string asu = Console.ReadLine();
                        Console.Write("Tipo: "); int tipo = int.Parse(Console.ReadLine());
                        Mostrar(GestorExpedientes.RegistrarDocumento(lista, año, fecha, rem, asu, tipo));
                    }
                    else if (op == "2" || op == "4")
                    {
                        Console.Write("Código: ");
                        int pos = GestorExpedientes.BuscarExpediente(lista, int.Parse(Console.ReadLine()));
                        if (pos < 0) Console.WriteLine("Expediente no encontrado.");
                        else if (op == "2") Mostrar(lista[pos]);
                        else
                        {
                            Console.Write("Nuevo estado (1 Recibido, 2 En atención, 3 Archivado): ");
                            int nuevo = int.Parse(Console.ReadLine());
                            if (nuevo < 1 || nuevo > 3) throw new ArgumentException("Estado no válido.");
                            lista[pos].Estado = nuevo;
                            Mostrar(lista[pos]);
                        }
                    }
                    else if (op == "3") ListarPorFecha(lista);
                    else if (op == "0") break;
                    else Console.WriteLine("Opción no válida.");
                }
                catch (Exception err)
                {
                    if (err is ArgumentException || err is InvalidOperationException || err is FormatException)
                        Console.WriteLine("Error: " + err.Message);
                    else throw;
                }
            }
        }

        public static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (Array.IndexOf(args, "--pruebas") >= 0) return Pruebas.Ejecutar();
            if (Array.IndexOf(args, "--demo") >= 0) Demo(); else Menu();
            return 0;
        }
    }
}
