using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
    {
    private static object hechos;

    static void Main()
        {

            var reglas = new List<(string Id, int Prio, string[] Ant, string[] Cons)>
            {
                ("R1", 65, new[] { "BOQUILLA_CALIENTE", "FILAMENTO_NO_FLUYE" }, new[] { "POSIBLE_ATASCO" }),
                ("R2", 85, new[] { "POSIBLE_ATASCO", "MOTOR_EXTRUSOR_GIRA", "SENSOR_FILAMENTO_OK" }, new[] { "OBSTRUCCION_BOQUILLA" }),
                ("R3", 95, new[] { "OBSTRUCCION_BOQUILLA" }, new[] { "PAUSAR_Y_PURGAR" }),
                ("R4", 75, new[] { "POSIBLE_ATASCO", "-MOTOR_EXTRUSOR_GIRA" }, new[] { "REVISAR_ENGRANAJES" })
            };


                var casos = new[]
                {
                new[] { "BOQUILLA_CALIENTE", "FILAMENTO_NO_FLUYE", "MOTOR_EXTRUSOR_GIRA", "SENSOR_FILAMENTO_OK" },
                new[] { "BOQUILLA_CALIENTE", "FILAMENTO_NO_FLUYE" },
                new[] { "BOQUILLA_CALIENTE", "MOTOR_EXTRUSOR_GIRA" }
            };


            for (int i = 0; i < casos.Length; i++)
            {
                Console.WriteLine($"\n CASO {i + 1}: {{ {string.Join(", ", casos[i])} }} ");

                var echos = new HashSet<string>(casos[i], StringComparer.OrdinalIgnoreCase);    
                var disparadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var secuencia = new List<string>();
                int paso = 1;

                while (true)
                {
                    var candidata = reglas
                        .Where(r => !disparadas.Contains(r.Id) && r.Ant.All(cond =>
                        {
                            bool negada = cond.StartsWith("-");
                            string varNombre = negada ? cond.Substring(1) : cond;
                            return negada ? !hechos.Contains(varNombre) : hechos.Contains(varNombre);
                        }))
                        .OrderByDescending(r => r.Prio)
                        .FirstOrDefault();

                    if (candidata.Id == null)
                    {
                        Console.WriteLine($"[Paso {paso}] Punto fijo alcanzado.");
                        break;
                    }

                    disparadas.Add(candidata.Id);
                    secuencia.Add(candidata.Id);
                    foreach (var c in candidata.Cons) hechos.Add(c);

                    Console.WriteLine($"[Paso {paso++}] Dispara {candidata.Id} (P:{candidata.Prio}) -> Deduce: {string.Join(", ", candidata.Cons)}");
                }

                string resumen = secuencia.Count > 0 ? $"Encadena {string.Join(" -> ", secuencia)}" : "Sin disparos";
                Console.WriteLine($"Resumen: [{resumen}]");
                Console.WriteLine($"Hechos Finales: {{ {string.Join(", ", hechos)} }}");
            }
        }
    }
