using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuttingStock
{
    class LinerCuttingClass
    {
        List<int> desiredLengths;     //Длины необходимых заготовок Lengths of required blanks
        List<int> amount;             //Количество заготовок для каждой длины Number of blanks for each length
        int whipLength;               //Длина хлыста Whip length
        int endSawCut;                //Торцевой спил End saw cut      
        int toolWidth;                //Ширина инструмента Tool width
        int headlessRetreat;          //Безусловный отход Unconditional withdrawal

        List<List<int>> cuts;         //Длины заготовок для каждого хлыста Blank lengths for each whip
        List<int> repeats;            //Количество повторов каждого хлыста Number of repetitions of each whip
        List<int> retreats;           //Остаток на каждый хлыст Balance per whip
        List<int> usingLength;        //Использованной длины каждого хлыста Used length of each whip

        //Variables
        double[] schet = { 0, 0, 0, 0 };
        double[] schetDl = { 0, 0, 0, 0 };

        int cut, bez, torc, line, maxNumb, maxNumbMem;
        int x, j, i, y, zvr, hl, pop, verif;
        int maxRes, sum, sdv, pov;

        int[] l;
        int[] k;
        int[] z = new int[201];
        int[] zost = new int[201];
        int[,] lovr = new int[201, 201];
        int[,] kvr;
        int[] maxNumbVr = new int[201];
        int[] sumLine = new int[201];

        int[,] lvr;
        int[] q = new int[30001];
        int[] w = new int[30001];
        
        int[,] lo = new int[201, 201];
        int[] kol = new int[201];
        int[] p = new int[201];
        int[,] res;

        public List<List<int>> GetCuts()
        {
            return cuts;
        }

        public List<int> GetRepeats()
        {
            return repeats;
        }

        public List<int> GetRetreats()
        {
            return retreats;
        }

        public List<int> GetUsingLength()
        {
            return usingLength;
        }        

        public LinerCuttingClass(List<int> desiredLengths_, List<int> amount_, int whipLength_, int endSawCut_, int toolWidth_, int headlessRetreat_)
        {
            desiredLengths = new List<int>(desiredLengths_);
            amount = new List<int>(amount_);
            whipLength = whipLength_;
            endSawCut = endSawCut_;
            toolWidth = toolWidth_;
            headlessRetreat = headlessRetreat_;
            Calculate();
        }

        //Подрезание нулевых позиций Trimming Zero Positions
        void CutMass()
        {
            for (i = w[y]; i <= maxNumbVr[x] - 1; i++)
            {
                kvr[i, x] = kvr[i + 1, x];
                lvr[i, x] = lvr[i + 1, x];
            }
            kvr[maxNumbVr[x], x] = 0;
            lvr[maxNumbVr[x], x] = 0;
        }

        //BaseLine Базовый раскрой Basic cutting
        void BaseLine()
        {            
            zvr = line / l[1];
            if (zvr > k[1])
                zvr = k[1];

            for (j = 1; j <= zvr; j++)
            {
                for (i = 1; i <= j; i++)
                    lovr[i, j] = l[1];
                kvr[1, j] = k[1] - j;
                maxNumbVr[j] = maxNumb;
                sumLine[j] = j * l[1];
                sumLine[j + 1] = 0;
            }
            for (j = 1; j <= zvr; j++)
            {
                lvr[1, j] = l[1];
                for (i = 2; i <= maxNumb; i++)
                {
                    kvr[i, j] = k[i];
                    lvr[i, j] = l[i];
                }
            }
            if (kvr[1, zvr] == 0)
            {
                x = zvr;
                CutMass();
            }
        }

        //Algorithm for preparing an array for calculations
        void Optimiz()
        {
            int len = line - sumLine[x];
            int L;
            int dif;
            for (i = 1; i <= len; i++)
            {
                q[i] = 0;
                w[i] = 0;
                for (j = 1; j <= maxNumbVr[x]; j++)
                {
                    L = lvr[j, x];
                    dif = i - L;
                    if (dif == 0)
                    {
                        q[i] = L;
                        w[i] = j;
                    }
                    else
                        if (dif > 0 && q[i] < (q[dif] + L))
                        {
                                q[i] = q[dif] + L;
                                w[i] = j;
                        }
                }               
            }
        }

        //Forming a cut for the remainder
        void OstLine()
        {
            int kv;
            int ind = w[y];
            while (w[y] > 0)
            {
                kv = kvr[ind, x];
                if (kv > 0)
                {
                    kvr[ind, x] = kv - 1;
                    zost[x] = zost[x] + 1;
                    lovr[x, zost[x] + x] = lvr[ind, x];
                    sumLine[x] = sumLine[x] + lvr[ind, x];
                    y = y - lvr[ind, x];
                    ind = w[y];
                    continue;
                }
                else
                    if (maxNumbVr[x] > 1)
                    {
                        CutMass();
                        Optimiz();
                        ind = w[y];
                        continue;
                    }
                if (kv == 0 && maxNumbVr[x] == 1)
                {
                    y = 1;
                    return;
                }
            }
        }

        void Result()
        {
           cuts = new List<List<int>>();         
           repeats = new List<int>();          
           retreats = new List<int>();
           usingLength = new List<int>();  

            List<int> workpieces; 
            for (i = 1; i <= hl - 1; i++)
            {
                x = 0;
                sum = 0;
                kol[i] = 1;
                res[i + 1, 1] = i;
                res[i + 1, 4] = kol[i];

                repeats.Add(kol[i]); //Number of repetitions of each whip
                workpieces = new List<int>();
                for (j = 1; j <= z[i]; j++)
                {
                    if (z[i] > maxRes)
                        maxRes = z[i];
                    res[i + 1, j + 4] = lo[i, j] - cut;
                    sum = sum + lo[i, j];
                    schet[3] = schet[3] + 1;
                    schetDl[3] = schetDl[3] + lo[i, j] - cut;
                    workpieces.Add(res[i + 1, j + 4]); //Размер заготовки в хлысте Workpiece size in a whip
                    if (i > 1 && res[i + 1, j + 4] == res[i, j + 4])
                        x = x + 1;
                    else
                    {
                        x = 0;
                        pov = 0;
                    }
                }
                cuts.Add(workpieces);
                res[i + 1, 2] = sum + torc;
                res[i + 1, 3] = line + torc - res[i + 1, 2];
                usingLength.Add(res[i + 1, 2]);  //Использовано Used
                retreats.Add(res[i + 1, 3]);     //Остаток remainder

                if (x == z[i] && z[i] == z[i - 1])   
                {
                    kol[i - 1 - pov] = kol[i - 1 - pov] + 1;            //Увеличиваем количество повторов предыдущего хлыста Increasing the number of repetitions of the previous whip
                    repeats[repeats.Count - 2] = kol[i - 1 - pov];
                    pov = pov + 1;
                    usingLength.Remove(usingLength.Last());
                    retreats.Remove(retreats.Last());
                    repeats.Remove(repeats.Last());
                    cuts.Remove(cuts.Last());                           
                    sdv = sdv + 1;
                }
            }

            if (schet[1] == schet[2] && schet[1] == schet[3] && schetDl[1] == schetDl[2] && schetDl[1] == schetDl[3])
                return;
            else
            {
                throw new Exception("Ошибка! Возможен неверный раскрой");
                //return false;
            }
        }

        void Calculate()
        {
            cut = toolWidth;
            bez = headlessRetreat;
            torc = endSawCut;
            line = whipLength - torc;
            maxNumb = amount.Count;
            maxNumbMem = maxNumb;

            l = new int[maxNumb + 2];
            k = new int[maxNumb + 2];
            kvr = new int[maxNumb + 1,201];
            lvr = new int[maxNumb + 1,201];
            res = new int[maxNumb + 1, maxNumb + 1];

            //Считывание в массив Reading into an array
            j = 1;
            foreach (int len in desiredLengths)
            {
                l[j] = len + cut;
                if (l[j] + torc > line)
                    throw new Exception("Деталь " + j + " длиннее исходной заготовки");
                k[j] = amount[j - 1];
                schet[1] += k[j];
                schetDl[1] += (l[j] - cut) * k[j];
                j++;
            }

            //Сортировка по убыванию Sort descending
            int vl;
            int vk;
            for (i = 1; i <= maxNumb - 1; i++)
                for (j = i + 1; j <= maxNumb; j++)
                    if (l[j] > l[i])
                    {
                        vl = l[j];
                        vk = k[j];
                        l[j] = l[i];
                        k[j] = k[i];
                        l[i] = vl;
                        k[i] = vk;
                    }

            hl = 1;
            z[1] = 0;
            y = line;

            //Управляющий алгоритм Control algorithm
            while (k[1] > 0)
            {
                for (i = 1; i <= 100; i++)
                    zost[i] = 0;

                BaseLine();

                for (x = 1; x <= zvr; x++)
                {
                    y = line - sumLine[x];
                    Optimiz();
                    OstLine();
                }
                p[1] = zost[1] + 1;
                pop = 1;
                for (x = 2; x <= zvr; x++)
                {
                    verif = line - sumLine[x] - bez;
                    if (verif < 0)
                    {
                        p[x] = x + zost[x];
                        if (p[x] < p[x - 1])
                            pop = x;
                    }
                    else
                        if (sumLine[x] > sumLine[x - 1])
                            pop = x;
                }
                for (i = 1; i <= pop; i++)
                {
                    lo[hl, i] = l[1];
                    k[1] = k[1] - 1;
                    schet[2] = schet[2] + 1;
                    schetDl[2] = schetDl[2] + lo[hl, i] - cut;
                }
                for (j = 1; j <= zost[pop]; j++)
                {
                    lo[hl, j + pop] = lovr[pop, j + pop];
                    schet[2] = schet[2] + 1;
                    schetDl[2] = schetDl[2] + lo[hl, j + pop] - cut;
                }
                for (i = 1; i <= maxNumb; i++)
                {
                    k[i] = kvr[i, pop];
                    l[i] = lvr[i, pop];
                }
                z[hl] = pop + zost[pop];
                hl = hl + 1;
                if (hl == 200)
                {
                    Result();
                    return;
                }

                if (maxNumb > 1)
                    for (j = 1; j <= 3; j++)
                    {
                        if (k[1] == 0)
                        {
                            for (i = 1; i <= maxNumb; i++)
                            {
                                k[i] = k[i + 1];
                                l[i] = l[i + 1];                                
                            }
                            k[maxNumb] = 0;
                            l[maxNumb] = 0;
                        }
                        else
                            break;
                    }
            }//while (k[1]>0)   
            Result();
        }        

    }
}
