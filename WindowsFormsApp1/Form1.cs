using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pacManComponent3.OnMatchWin += pacManComponent3_OnMatchWin;
            pacManComponent3.OnMatchLoose += pacManComponent3_OnMatchLoose;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("Form loaded");
        }

        private void pacManComponent3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Score:"+pacManComponent3.Score+" Lives"+pacManComponent3.Lives);
        }

        private void pacManComponent3_OnMatchWin()
        {
            listBox1.Items.Add("You win match");
        }

        private void pacManComponent3_OnMatchLoose()
        {
            listBox1.Items.Add("You loose match");
        }
    }
}
