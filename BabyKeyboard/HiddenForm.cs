using System.Windows.Forms;

namespace BabyKeyboard
{
    public partial class HiddenForm : Form
    {
        public HiddenForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
    }
}
