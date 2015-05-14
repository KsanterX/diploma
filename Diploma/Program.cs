using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma
{
    static class Program
    {
        //Form access
        static MainForm MyForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MyForm = new MainForm();
            Application.Run(MyForm);


            
        }

        

        
    }
}
