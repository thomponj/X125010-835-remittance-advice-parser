using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RemittanceParser
{
    public partial class Parser : Form
    {
        public Parser()
        {
            InitializeComponent();
        }
        private void btn_browse_Click(object sender, EventArgs e)
        {
            string mssg = "";

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open an 835 Remittance File...";
            dialog.InitialDirectory = @"C:\";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mssg = parse_to_billing_provider(dialog.FileName.ToString());
            }

            MessageBox.Show(mssg);

        }
        public static string parse_to_billing_provider(string filename)
        {
            string returnmessage = "";
            string isa_segment = "";
            string iea_segment = "";
            string gs_segment = "";
            string ge_segment = "";
            string se_segment = "";
            string st_segment = "";
            string transaction_set = "";
            string line = "";
            string segment_type = "";
            bool is835 = true;
            bool fatal = false;
            bool first = true;
            string[] output = new string[1000];
            string[] provider = new string[1000];
            int i = 0;

            DelimitedFileReader remittance = new DelimitedFileReader(filename, "~\n");

            try
            {
                line = remittance.Read();

                if (line[0] == 'I')
                {
                    do
                    {
                        string[] segment = line.Replace("~", "*").Split("*".ToCharArray());
                        segment_type = segment.GetValue(0).ToString();
                        switch (segment_type)
                        {
                            case "BPR":
                                if (is835 == true)
                                {
                                    if (first == false)
                                        i += 1;

                                    first = false;
                                    output[i] += line.ToString();
                                }
                                break;
                            case "GE":
                                ge_segment = line.ToString();
                                break;
                            case "GS":
                                gs_segment = line.ToString();
                                break;
                            case "IEA":
                                iea_segment = line.ToString();
                                for (int ii = 0; ii <= i; ii++)
                                {
                                    if (output.GetValue(ii).ToString().Contains("ISA*00") == false)
                                        output[ii] = isa_segment + gs_segment + output.GetValue(ii).ToString() + ge_segment + iea_segment;
                                }
                                break;
                            case "ISA":
                                isa_segment = line.ToString();
                                break;
                            case "N1":
                                if (is835 == true)
                                {
                                    switch (segment.GetValue(1).ToString())
                                    {
                                        case "PE":
                                            provider[i] = segment.GetValue(2).ToString();
                                            break;
                                        default:
                                            break;
                                    }
                                    output[i] += line.ToString();
                                }
                                break;
                            case "SE":
                                if (is835 == true)
                                {
                                    se_segment = line.ToString();
                                    for (int ii = 0; ii <= i; ii++)
                                    {
                                        if (output.GetValue(ii).ToString().Contains("ST*835") == false)
                                            output[ii] = st_segment + output.GetValue(ii).ToString() + se_segment;
                                    }
                                }
                                is835 = true;
                                break;
                            case "ST":
                                if (segment.GetValue(1).ToString() != "835")
                                {
                                    is835 = false;
                                }
                                else
                                {
                                    transaction_set = segment.GetValue(3).ToString();
                                    st_segment = line.ToString();
                                }
                                break;
                            default:
                                if (is835 == true)
                                {
                                    if (line.ToString().Length > 0)
                                        output[i] += line.ToString();
                                }
                                break;
                        }
                    }
                    while ((line = remittance.Read()) != "" && fatal == false);

                    int iii = WriteFile(output, provider, filename);

                    if (iii > 0)
                        returnmessage += "File Processed, " + iii.ToString() + " files created.";
                    else
                        returnmessage += "There has been a problem.";
                }
                else
                {
                    returnmessage += "This is not an X12 5010 file.";
                }
            }
            catch (Exception ex)
            {
                returnmessage = ex.ToString();
                remittance.Close();
            }
            finally
            {
                remittance.Close();
            }
            return returnmessage;
        }
        public static int WriteFile(string[] output, string[] provider, string original_filename)
        {
            int ii = 0;

            for (int i = 0; i < 1000; i++)
            {
                if (output.GetValue(i) != null)
                {
                    string new_filename = original_filename.Substring(0, original_filename.LastIndexOf(".")) + "_" + provider.GetValue(i).ToString() + "_" + i.ToString() + original_filename.Substring(original_filename.LastIndexOf("."), original_filename.Length - original_filename.LastIndexOf("."));
                    StreamWriter w835 = new StreamWriter(new_filename);
                    w835.WriteLine(output.GetValue(i).ToString());
                    w835.Close();
                    ii += 1;
                }
            }

            return ii;
        }
        public class DelimitedFileReader
        {
            private string filepath;
            private string delimiters;
            private StreamReader stream;
            private int size = 20;
            private int sizeModifier = 1;
            private char[] buffer;
            private int offset = 0;
            private char read = '0';
            public string Path { get { return filepath; } set { filepath = value; } }
            public string Delimiters { get { return delimiters; } set { delimiters = value; } }
            public int BufferSize { get { return size; } set { size = value; } }
            public DelimitedFileReader(string path, string delimits)
            {
                filepath = path;
                delimiters = delimits;
                buffer = new char[size];

                if (!File.Exists(path))
                {
                    stream = null;
                    return;
                }
                stream = new StreamReader(filepath);
            }
            public DelimitedFileReader(string path, string delimits, int buffersize)
            {
                filepath = path;
                delimiters = delimits;
                size = buffersize;
                buffer = new char[size];

                if (!File.Exists(path))
                {
                    stream = null;
                    return;
                }
                stream = new StreamReader(filepath);
            }
            public string Read()
            {
                if (stream == null)
                    return "";
                if (stream.Peek() == -1)
                {
                    Close();
                    return "";
                }
                while (delimiters.IndexOf((char)stream.Peek()) > -1)
                {
                    stream.Read();
                    if (stream.Peek() == -1)
                    {
                        Close();
                        return "";
                    }
                }
                read = '0';
                while (delimiters.IndexOf(read) < 0)
                {
                    read = ((char)stream.Read());
                    buffer[offset++] = read;
                    if (offset >= buffer.Length)
                        buffer = resize(buffer, size * ++sizeModifier);
                }
                if (delimiters.IndexOf(buffer[0]) == 0)
                {
                    Close();
                    return "";
                }

                string eval = new string(buffer, 0, offset);

                offset = 0;
                buffer = new char[size];
                sizeModifier = 1;

                return eval;
            }
            public char[] resize(char[] buf, int newsize)
            {
                char[] temp = new char[newsize];
                Array.Copy(buf, temp, buf.Length);
                return temp;
            }
            public void Close()
            {
                stream.Close();
            }
        }
    }
}
