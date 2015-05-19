using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma
{
    class Probe
    {
        int[,] probecount;
        Random r = new Random();
        int z = 0;
        int c = 0;

        public Probe()
        {

        }

        public void probeC(int probe, int id, int issl)
        {
            for (int i=0; i < probecount.GetLength(0); i++)
            {
                if (z <= issl && probecount.GetLength(0)-z >= issl)
                {
                    c = r.Next(0,2);
                    z += c;
                    probecount[i,id] = c;
                }
                else if (z <= issl && probecount.GetLength(0)-z < issl)
                {
                    probecount[i,id] = 1;
                    z += 1;
                }
                else if(z == issl && probecount.GetLength(0)-z > 0)
                {
                    probecount[i,id] = 0;
                }
                else
                {
                    break;
                }
                
            }        
        }

        public int[,] probeCount
        {
            set
            {
                probecount = value;
            }
            get
            {
                return probecount;
            }
        }
    }
}
