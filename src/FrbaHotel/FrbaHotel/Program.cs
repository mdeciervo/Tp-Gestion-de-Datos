using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FrbaHotel
{   
    
    static class Program
    {
        public static Home inicial;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            inicial = new Home();
            Application.Run(inicial);

        }
    }
}
