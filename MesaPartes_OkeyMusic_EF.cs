using System;
using System.IO;

// ============================================================
// MesaPartes_OkeyMusic_EF.cs - EF semana 15 - CIIN1205P
// Mesa de partes de la tienda Okey Music.
// Punto de partida: la version final del EP (7 opciones).
// Mejoras EF 1 y 2: cambiar estado y anular documento (9 opciones).
// ============================================================
class Program
{
    const int MAX_EXP = 500;
    static string[] codigos = new string[MAX_EXP];
    static string[] dnis = new string[MAX_EXP];
    static string[] nombres = new string[MAX_EXP];
    static string[] asuntos = new string[MAX_EXP];
    static string[] tipos = new string[MAX_EXP];
    static DateTime[] fechas = new DateTime[MAX_EXP];
    static string[] estados = new string[MAX_EXP];
    static int totalExp = 0;
    static int contadorExp = 0;

    static readonly string archivoExp = "expedientes_okeymusic_EF.txt";

    static void Main(string[] args)
    {
        CargarDatosGuardados();
        int opcion;
        do
        {
            MostrarMenu();
            string entrada = Console.ReadLine();
            while (!int.TryParse(entrada, out opcion) || opcion < 1 || opcion > 9)
            {
                Console.Write("Dato no valido. Ingrese del 1 al 9: ");
                entrada = Console.ReadLine();
            }
            switch (opcion)
            {
                case 1: RegistrarExpediente(); break;
                case 2: MostrarExpedientes(); break;
                case 3: BuscarPorCodigo(); break;
                case 4: BuscarParcial(); break;
                case 5: OrdenarExpedientes(); break;
                case 6: VerTotal(); break;
                case 7: CambiarEstado(); break;
                case 8: AnularExpediente(); break;
                case 9: Console.WriteLine("Guardando archivo y saliendo..."); break;
                default: Console.WriteLine("Opcion no valida."); break;
            }
            if (opcion != 9 && !Console.IsInputRedirected)
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                try { Console.ReadKey(true); } catch { Console.ReadLine(); }
                Console.Clear();
            }
        } while (opcion != 9);
    }

    static void MostrarMenu()
    {
        Console.WriteLine("== MESA DE PARTES - OKEY MUSIC (EF v2) ==");
        Console.WriteLine("1. Registrar documento (genera EXP-0001...)");
        Console.WriteLine("2. Mostrar documentos");
        Console.WriteLine("3. Buscar por codigo EXP");
        Console.WriteLine("4. Buscar por cliente / asunto");
        Console.WriteLine("5. Ordenar documentos");
        Console.WriteLine("6. Ver total registrados");
        Console.WriteLine("7. Cambiar estado del documento");
        Console.WriteLine("8. Anular documento");
        Console.WriteLine("9. Salir y guardar");
        Console.WriteLine("========================================");
        Console.Write("Elija opcion: ");
    }

    static void RegistrarExpediente()
    {
        Console.WriteLine("\n--- REGISTRAR DOCUMENTO ---");
        if (totalExp >= MAX_EXP)
        {
            Console.WriteLine("Limite alcanzado (500).");
            return;
        }
        string dni = PedirDni();
        Console.Write("Cliente o proveedor: ");
        string nombre = Console.ReadLine().Trim();
        while (nombre.Length < 3)
        {
            Console.Write("Muy corto. Escriba nombre completo: ");
            nombre = Console.ReadLine().Trim();
        }
        Console.Write("Asunto (ej. garantia guitarra VOZZEX, pedido cuerdas): ");
        string asunto = Console.ReadLine().Trim();
        while (asunto.Length < 5)
        {
            Console.Write("Asunto muy corto (minimo 5 letras): ");
            asunto = Console.ReadLine().Trim();
        }
        string tipo = PedirTipo();

        contadorExp++;
        string codigo = "EXP-" + contadorExp.ToString("0000");

        codigos[totalExp] = codigo;
        dnis[totalExp] = dni;
        nombres[totalExp] = nombre;
        asuntos[totalExp] = asunto;
        tipos[totalExp] = tipo;
        fechas[totalExp] = DateTime.Now;
        estados[totalExp] = "Registrado";
        totalExp++;

        Guardar();
        Console.WriteLine("Documento " + codigo + " registrado: " + nombre + " | " + tipo + ".");
    }

    static string PedirDni()
    {
        Console.Write("DNI / RUC contacto (8 digitos DNI o 11 RUC): ");
        string d = Console.ReadLine().Trim();
        while (!EsDniRuc(d))
        {
            Console.Write("No valido. DNI 8 digitos o RUC 11 digitos: ");
            d = Console.ReadLine().Trim();
        }
        return d;
    }

    static bool EsDniRuc(string s)
    {
        if (s.Length != 8 && s.Length != 11) return false;
        foreach (char c in s)
        {
            if (c < '0' || c > '9') return false;
        }
        return true;
    }

    static string PedirTipo()
    {
        Console.WriteLine("Tipo: 1.Garantia  2.Reclamo  3.Pedido proveedor  4.Solicitud");
        Console.Write("Elija (1-4): ");
        string e = Console.ReadLine().Trim();
        int t;
        while (!int.TryParse(e, out t) || t < 1 || t > 4)
        {
            Console.Write("No valido (1-4): ");
            e = Console.ReadLine().Trim();
        }
        if (t == 1) return "Garantia";
        if (t == 2) return "Reclamo";
        if (t == 3) return "Pedido";
        return "Solicitud";
    }

    static void MostrarExpedientes()
    {
        if (totalExp == 0)
        {
            Console.WriteLine("\nAun no hay documentos.");
            return;
        }
        Console.WriteLine("\nCODIGO   | FECHA             | DOC      | CLIENTE/PROV.        | TIPO       | ASUNTO");
        Console.WriteLine(new string('-', 100));
        for (int i = 0; i < totalExp; i++)
        {
            string nom = nombres[i].Length > 20 ? nombres[i].Substring(0, 17) + "..." : nombres[i];
            string asu = asuntos[i].Length > 28 ? asuntos[i].Substring(0, 25) + "..." : asuntos[i];
            Console.WriteLine(string.Format("{0,-8} | {1,-17} | {2,-8} | {3,-20} | {4,-10} | {5}",
                codigos[i], fechas[i].ToString("yyyy-MM-dd HH:mm"), dnis[i], nom, tipos[i], asu));
        }
    }

    static void BuscarPorCodigo()
    {
        Console.WriteLine("\n--- BUSCAR POR CODIGO ---");
        Console.Write("Codigo (ej. EXP-0001): ");
        string b = Console.ReadLine().Trim().ToUpper();
        int pos = PosCodigo(b);
        if (pos == -1)
        {
            Console.WriteLine("No existe documento con ese codigo.");
            return;
        }
        Console.WriteLine("\nDocumento: " + codigos[pos]);
        Console.WriteLine("Fecha: " + fechas[pos].ToString("yyyy-MM-dd HH:mm"));
        Console.WriteLine("DNI/RUC: " + dnis[pos]);
        Console.WriteLine("Cliente/Proveedor: " + nombres[pos]);
        Console.WriteLine("Tipo: " + tipos[pos]);
        Console.WriteLine("Asunto: " + asuntos[pos]);
        Console.WriteLine("Estado: " + estados[pos]);
    }

    static int PosCodigo(string codigo)
    {
        for (int i = 0; i < totalExp; i++)
        {
            if (codigos[i] != null && codigos[i].Trim().ToUpper().Equals(codigo.Trim().ToUpper()))
            {
                return i;
            }
        }
        return -1;
    }

    static void BuscarParcial()
    {
        Console.WriteLine("\n--- BUSQUEDA PARCIAL ---");
        Console.Write("Parte del cliente o del asunto: ");
        string c = Console.ReadLine().Trim().ToLower();
        if (c.Length == 0)
        {
            Console.WriteLine("Escriba al menos una letra.");
            return;
        }
        int n = 0;
        for (int i = 0; i < totalExp; i++)
        {
            if (nombres[i].ToLower().Contains(c) || asuntos[i].ToLower().Contains(c))
            {
                Console.WriteLine("[" + codigos[i] + "] " + nombres[i] + " | " + tipos[i] + " | " + asuntos[i]);
                n++;
            }
        }
        if (n == 0) Console.WriteLine("Sin coincidencias para '" + c + "'.");
        else Console.WriteLine(" hallados: " + n);
    }

    static void OrdenarExpedientes()
    {
        if (totalExp < 2)
        {
            Console.WriteLine("Menos de 2 documentos, nada que ordenar.");
            return;
        }
        Console.WriteLine("\n--- ORDENAR ---");
        Console.WriteLine("1. Por codigo  2. Por fecha  3. Por cliente (A-Z)");
        Console.Write("Criterio: ");
        int opc;
        while (!int.TryParse(Console.ReadLine(), out opc) || opc < 1 || opc > 3)
        {
            Console.Write("No valido (1-3): ");
        }
        for (int i = 0; i < totalExp - 1; i++)
        {
            for (int j = 0; j < totalExp - 1 - i; j++)
            {
                bool cam = false;
                if (opc == 1 && string.Compare(codigos[j], codigos[j + 1], true) > 0) cam = true;
                else if (opc == 2 && fechas[j] > fechas[j + 1]) cam = true;
                else if (opc == 3 && string.Compare(nombres[j], nombres[j + 1], true) > 0) cam = true;
                if (cam) Cambiar(j, j + 1);
            }
        }
        Console.WriteLine("Ordenado. Revise con opcion 2.");
    }

    static void Cambiar(int a, int b)
    {
        string t; DateTime f;
        t = codigos[a]; codigos[a] = codigos[b]; codigos[b] = t;
        t = dnis[a]; dnis[a] = dnis[b]; dnis[b] = t;
        t = nombres[a]; nombres[a] = nombres[b]; nombres[b] = t;
        t = asuntos[a]; asuntos[a] = asuntos[b]; asuntos[b] = t;
        t = tipos[a]; tipos[a] = tipos[b]; tipos[b] = t;
        t = estados[a]; estados[a] = estados[b]; estados[b] = t;
        f = fechas[a]; fechas[a] = fechas[b]; fechas[b] = f;
    }

    static void VerTotal()
    {
        Console.WriteLine("\nTotal documentos: " + totalExp);
        if (totalExp > 0) Console.WriteLine("Ultimo: " + codigos[totalExp - 1]);
    }

    // ---- MEJORA EF 1: cambiar el estado del documento ----
    static void CambiarEstado()
    {
        Console.WriteLine("\n--- CAMBIAR ESTADO ---");
        Console.Write("Codigo (ej. EXP-0001): ");
        string b = Console.ReadLine().Trim().ToUpper();
        int pos = PosCodigo(b);
        if (pos == -1)
        {
            Console.WriteLine("No existe documento con ese codigo.");
            return;
        }
        Console.WriteLine("Estado actual de " + codigos[pos] + ": " + estados[pos]);
        estados[pos] = PedirEstado();
        Guardar();
        Console.WriteLine("Estado actualizado a: " + estados[pos] + ".");
    }

    static string PedirEstado()
    {
        Console.WriteLine("Nuevo estado: 1.En proceso  2.Atendido  3.Registrado");
        Console.Write("Elija (1-3): ");
        string e = Console.ReadLine().Trim();
        int t;
        while (!int.TryParse(e, out t) || t < 1 || t > 3)
        {
            Console.Write("No valido (1-3): ");
            e = Console.ReadLine().Trim();
        }
        if (t == 1) return "En proceso";
        if (t == 2) return "Atendido";
        return "Registrado";
    }

    // ---- MEJORA EF 2: anular (eliminar) un documento ----
    static void AnularExpediente()
    {
        Console.WriteLine("\n--- ANULAR DOCUMENTO ---");
        Console.Write("Codigo a anular (ej. EXP-0001): ");
        string b = Console.ReadLine().Trim().ToUpper();
        int pos = PosCodigo(b);
        if (pos == -1)
        {
            Console.WriteLine("No existe documento con ese codigo.");
            return;
        }
        Console.WriteLine("Se anulara: " + codigos[pos] + " | " + nombres[pos] + " | " + asuntos[pos]);
        Console.Write("Confirma? (S/N): ");
        string resp = Console.ReadLine().Trim().ToUpper();
        if (resp != "S")
        {
            Console.WriteLine("Operacion cancelada.");
            return;
        }
        Eliminar(pos);
        Guardar();
        Console.WriteLine("Documento " + b + " anulado y retirado de la lista.");
    }

    static void Eliminar(int pos)
    {
        for (int i = pos; i < totalExp - 1; i++)
        {
            codigos[i] = codigos[i + 1];
            dnis[i] = dnis[i + 1];
            nombres[i] = nombres[i + 1];
            asuntos[i] = asuntos[i + 1];
            tipos[i] = tipos[i + 1];
            fechas[i] = fechas[i + 1];
            estados[i] = estados[i + 1];
        }
        totalExp--;
        codigos[totalExp] = null; dnis[totalExp] = null;
        nombres[totalExp] = null; asuntos[totalExp] = null;
        tipos[totalExp] = null; estados[totalExp] = null;
    }

    static void Guardar()
    {
        using (StreamWriter sw = new StreamWriter(archivoExp, false))
        {
            for (int i = 0; i < totalExp; i++)
            {
                sw.WriteLine(string.Join("|",
                    codigos[i], dnis[i],
                    nombres[i].Replace("|", " "),
                    asuntos[i].Replace("|", " "),
                    tipos[i], fechas[i].ToString("O"), estados[i]));
            }
        }
    }

    static void CargarDatosGuardados()
    {
        if (!File.Exists(archivoExp)) return;
        foreach (string linea in File.ReadAllLines(archivoExp))
        {
            string[] d = linea.Split('|');
            DateTime f;
            if (d.Length == 7 && DateTime.TryParse(d[5], out f))
            {
                if (totalExp < MAX_EXP)
                {
                    codigos[totalExp] = d[0]; dnis[totalExp] = d[1];
                    nombres[totalExp] = d[2]; asuntos[totalExp] = d[3];
                    tipos[totalExp] = d[4]; fechas[totalExp] = f;
                    estados[totalExp] = d[6]; totalExp++;
                }
                if (d[0].StartsWith("EXP-"))
                {
                    int num;
                    if (int.TryParse(d[0].Substring(4), out num) && num > contadorExp)
                    {
                        contadorExp = num;
                    }
                }
            }
        }
    }
}
