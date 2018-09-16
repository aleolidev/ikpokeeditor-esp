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
    public partial class NombrePokemon : Form
    {
        private string pkmnString;

        public string PkmnName
        {
            get { return pkmnString; }
            set { pkmnString = value; }
        }

        public NombrePokemon()
        {
            InitializeComponent();
        }

        private void AcceptName_Click(object sender, EventArgs e)
        {
            pkmnString = PokemonName.Text;
        }

        private void NombrePokemon_Load(object sender, EventArgs e)
        {

        }
    }
}
