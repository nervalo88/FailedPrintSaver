using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FailedPrintSaver
{
    class GcodeModifier
    {
        private string filename;
        private float Zfailcoord = -1;

        private byte[] prefix;

        private long lineIdToStartFrom = -1;
        public GcodeModifier()
        {
            if (File.Exists("start.gcode"))
            {
                prefix = File.ReadAllBytes("start.gcode");
                //System.Windows.Forms.MessageBox.Show(Encoding.ASCII.GetString(prefix), "Result", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }else
            {
                System.Windows.Forms.MessageBox.Show(String.Format("file.gcode Not found, no prefix will be added"), "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                prefix = null;
            }
        }

        public void generateFile()
        {
            if(this.filename == ""|| this.Zfailcoord < 0 || this.lineIdToStartFrom < 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format("File generation failed, have you loaded a file ?"), "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            string outputFileName = Path.GetDirectoryName(filename)+"\\"+ String.Format("{0}Z{1}{2}",Path.GetFileNameWithoutExtension(this.filename), this.Zfailcoord.ToString("n2"),Path.GetExtension(this.filename));
            File.WriteAllBytes(outputFileName, prefix);
            using(var reader = new StreamReader(this.filename))
            {
                using (var writer = new StreamWriter(outputFileName,true))
                {
                    writer.WriteLine(string.Format("\nG1 Z{0}",Zfailcoord.ToString("n2")));
                    long n = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (n >= lineIdToStartFrom)
                        {
                            //File.AppendAllText(outputFileName, line + Environment.NewLine);
                            writer.WriteLine(line);
                        }
                        n += 1;
                    }
                }
            }
            System.Windows.Forms.MessageBox.Show(String.Format("File generated"), "Done !", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        public void findAvailableZCoordInFile(float searchVal)
        {
            if (this.filename == "" || this.Zfailcoord == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format("findAvailableZCoordInFile() not all parameters set"), "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.Zfailcoord = -1;
            }
            using (var reader = new StreamReader(this.filename))
            {
                long n = 0;
                float max_f = -1;
                while (!reader.EndOfStream)
                {

                    var line = reader.ReadLine();
                    if (n > 50) // Arbitrary skip the 50th lines of the beginning in search to avoid Z moves in start gcode...
                    {
                        Regex ZregEx = new Regex(@"[Z]([0-9]*.)?[0-9]");
                        Match match = ZregEx.Match(line);
                        if (ZregEx.IsMatch(line))
                        {
                            string test = "";
                            test = match.Value;
                            test = test.TrimStart('Z');
                            float f = float.Parse(test, CultureInfo.InvariantCulture.NumberFormat);
                            if (f > max_f) max_f = f;
                            if (f >= searchVal)
                            {
                               lineIdToStartFrom = n;
                                this.Zfailcoord = f;
                                return;
                            }
                        }
                    }
                    n += 1;
                }
                    System.Windows.Forms.MessageBox.Show(String.Format("Coordinate Z{0} not found in {1}\n Zmax = {2}", searchVal, Path.GetFileName(this.filename), max_f), "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    lineIdToStartFrom = -1;

                    this.Zfailcoord = -1;
                
            }
        }
        public void setFilename(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            if (ext != ".gcode")
            {
                System.Windows.Forms.MessageBox.Show(String.Format("extension {0} not supported, only gcode accepted", ext), "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            this.filename = filePath;
        }
        public float getZfailcoord()
        {
            if(this.Zfailcoord < 0)
            {
                this.findAvailableZCoordInFile(10.0F);
            }
            return this.Zfailcoord;
        }
        public bool isFileLoaded()
        {
            if (this.filename == null)
            {
                return false;

            }else
            {
                return true;
            }
        }
        public string getFileName()
        {
            return Path.GetFileName(this.filename);
        }
    }
}
