using System;
using System.Windows.Forms;

namespace BabyKeyboard
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create a full-screen form for each screen
            foreach (var screen in Screen.AllScreens)
            {
                var form = new FullScreenForm(screen.Bounds);
                FormController.RegisterForm(form);
                form.Show();
            }

            // Start the application with a hidden form to keep the message loop running
            Application.Run(new HiddenForm());
        }
    }
}
