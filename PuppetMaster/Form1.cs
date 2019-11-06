using MSDAD.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSDAD.PuppetMaster
{    
    public partial class Form1 : Form
    {
        private const string PCS_URL = "tcp://localhost:1001/PCS";

        bool text_changed = false;        
        
        public Form1()
        {            
            InitializeComponent();                   
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.InputBox.Text = "";

            text_changed = false;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string text;

            PCSInterface pcsInterface = (PCSInterface)Activator.GetObject(typeof(PCSInterface), PCS_URL);
            if (!this.InputBox.Text.Equals("") && text_changed == true)
            {
                text = this.InputBox.Text;
                Console.WriteLine(text);                
                pcsInterface.Send(text);
            }

            text_changed = false;
        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            text_changed = true;
        }

       
    }
}
