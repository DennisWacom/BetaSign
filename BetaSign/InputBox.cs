using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaSign
{
    public partial class InputBox : Form
    {
        private bool continueNext = false;

        
        public static string show(string msg, bool showPassword, IWin32Window parent)
        {
            InputBox inputBox = new InputBox(msg, showPassword);
            inputBox.ShowDialog(parent);

            while (inputBox.continueNext == false)
            {

            }
            inputBox.Close();

            return inputBox.getInputText();
        }

        public InputBox(string msg, bool showPassword)
        {
            InitializeComponent();

            label1.Text = msg;
            textBox1.UseSystemPasswordChar = showPassword;

        }

        public string getInputText()
        {
            if (textBox1.Text != null && textBox1.Text != "")
            {
                return textBox1.Text;
            }
            return null;
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            continueNext = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            continueNext = true;
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

    }
}
