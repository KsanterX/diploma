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
                if (issl > 0)
                {
                    c = r.Next(0,2);
                    issl -= c;

                    if (probe - i > issl)
                    {
                        probecount[i, id] = c;
                    }
                    else
                    {
                        probecount[i, id] = 1;
                        issl--;
                    }
                    
                }
                else
                {
                    probecount[i,id] = 0;
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
