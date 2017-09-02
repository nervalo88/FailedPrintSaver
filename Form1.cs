using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FailedPrintSaver
{
    public partial class Form1 : Form
    {
        GcodeModifier gcode = new GcodeModifier();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;

        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                if (File.Exists(filePaths[0]))
                {
                    //MessageBox.Show(filePaths[0]);
                    gcode.setFilename(filePaths[0]);
                    updateStatusLabel();
                }
            }
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Gcode Files (.gcode)|*.gcode|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gcode.setFilename(openFileDialog1.FileName);
                updateStatusLabel();
            }

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float val = float.Parse(textBox1.Text, CultureInfo.InvariantCulture.NumberFormat);
                gcode.findAvailableZCoordInFile(val);
                updateStatusLabel();
            }
            catch
            {

            }
        }

        private void updateStatusLabel()
        {
            if (gcode.isFileLoaded())
            {
                statusLabel.Text = string.Format("Will trim {0} @ Z{1}", gcode.getFileName(), gcode.getZfailcoord().ToString("n2"));
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            gcode.generateFile();
        }


    }
}
