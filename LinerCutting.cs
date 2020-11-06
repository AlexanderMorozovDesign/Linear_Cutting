using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuttingStock
{
    class LinerCutting
    {
        static void Main(string[] args)
        {
            Console.Write("Длина хлыста, мм: ");
            int whipLength = int.Parse(Console.ReadLine());

            Console.Write("Торцевой спил, мм: ");
            int endSawCut = int.Parse(Console.ReadLine());

            Console.Write("Ширина инструмента, мм: ");
            int toolWidth = int.Parse(Console.ReadLine());

            Console.Write("Безусловный отход, мм: ");
            int headlessRetreat = int.Parse(Console.ReadLine());

            Console.Write("Количество типоразмеров заготовок: ");
            int n = int.Parse(Console.ReadLine());

            List<int> amount = new List<int>(n);
            List<int> desiredLengths = new List<int>(n);

            for (int i = 1; i <= n; i++)
            {
                Console.Write(i+ " Длина: ");
                int len = int.Parse(Console.ReadLine());
                Console.Write("  Количество: ");
                int q = int.Parse(Console.ReadLine());
                
                desiredLengths.Add(len);
                amount.Add(q);
            }

            LinerCuttingClass linerCutting = new LinerCuttingClass(desiredLengths, amount, whipLength, endSawCut, toolWidth, headlessRetreat);

            List<List<int>> cuts = linerCutting.GetCuts();
            List<int> repeats = linerCutting.GetRepeats();
            List<int> retreat = linerCutting.GetRetreats();
            List<int> usingLength = linerCutting.GetUsingLength();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("№\tИсп. длина\tОстаток\tКол. повторов");
            n = repeats.Count();
            for (int i = 0; i < n; i++)
            {
                Console.Write(i + 1 + ".\t" + usingLength[i] + "\t\t" + retreat[i] + "\t" + repeats[i] + "\t\t");
                foreach (int cut in cuts[i])
                    Console.Write(cut + "\t");
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
