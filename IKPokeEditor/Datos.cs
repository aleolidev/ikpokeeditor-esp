using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKPokeEditor
{
    public partial class Datos : Form
    {
        private int pkmnAmount;

        public int PkmnAmount
        {
            get { return pkmnAmount; }
            set { pkmnAmount = value; }
        }

        public Datos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pkmnAmount = Int32.Parse(numericUpDown1.Value.ToString());
        }
    }
}
