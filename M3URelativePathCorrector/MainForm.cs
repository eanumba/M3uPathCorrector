using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M3URelativePathCorrector
{
    public partial class MainForm : Form
    {
        const string PLAYLIST_FILTER = "Playlist files(*.m3u)|*.m3u";

        string filepath = "";
        public string Filepath
        {
            get
            {
                return filepath;
            }

            set
            {
                filepath = value;
                txtFilePath.Text = Filepath;

                if (!string.IsNullOrWhiteSpace(filepath) && File.Exists(filepath))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(filepath);
                        txtSourcePath.Text = LongestCommonSubstring(lines);
                    }
                    catch { }
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.DefaultExt = "m3u";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = PLAYLIST_FILTER;
            dialog.Title = "Choose an M3U file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Filepath = dialog.FileName;
            }
        }
        private void btnFix_Click(object sender, EventArgs e)
        {
            if (!VerifyFileExists(Filepath))
                return;

            string[] lines = GetFileLines(Filepath);
            if (lines == null)
                return;

            if (!VerifySubstrings(lines, txtSourcePath.Text))
                return;

            string newFile = GetNewFilePath();
            if (newFile == null)
                return;

            var newLines = ReplacePaths(lines, txtSourcePath.Text, txtDestPath.Text);
            if (!WriteAllLines(newFile, newLines))
                return;
            
            try { Process.Start(Path.GetDirectoryName(newFile)); }
            catch { }

            MessageBox.Show(this, "Playlist saved successfully!", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private static string LongestCommonSubstring(IEnumerable<string> Strings)
        {
            if (Strings.Count() <= 0)
                return "";

            StringBuilder substring = new StringBuilder();

            bool breakOut = false;
            int index = 0;
            while (true)
            {
                char? c = null;
                foreach (var path in Strings)
                {
                    if (path.Length <= index)
                    {
                        breakOut = true;
                        break;
                    }

                    if (!c.HasValue)
                        c = path[index];

                    if (char.ToLower(path[index]) != char.ToLower(c.Value))
                    {
                        breakOut = true;
                        break;
                    }
                }

                if (breakOut)
                    break;

                if (c.HasValue)
                    substring.Append(c);

                index++;
            }

            return substring.ToString();
        }
        private static string ReplaceFirstOccurance(string String, string Old, string New)
        {
            if (string.IsNullOrEmpty(String) || string.IsNullOrEmpty(Old))
                return String;

            if (New == null)
                New = "";

            int position = String.IndexOf(Old);
            if (position < 0)
                return String;

            return String.Substring(0, position) + New + String.Substring(position + Old.Length, String.Length - (position + Old.Length));
        }
        private static IEnumerable<string> ReplacePaths(IEnumerable<string> Paths, string Old, string New)
        {
            return Paths.Select((path) => { return ReplaceFirstOccurance(path, Old, New); });
        }
        private bool VerifyFileExists(string Filepath)
        {
            if (!File.Exists(Filepath))
            {
                MessageBox.Show(this, "File \"" + Filepath + "\" not found!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private string[] GetFileLines(string Filepath)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(Filepath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error reading file: " + ex.Message, "Error Reading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return lines;
        }
        private bool VerifySubstrings(IEnumerable<string> Strings, string Substring)
        {
            int? position = null;
            foreach (var str in Strings)
            {
                var substringIndex = str.IndexOf(Substring);

                if (!position.HasValue && substringIndex >= 0)
                    position = substringIndex;

                if (position != substringIndex)
                {
                    var result = MessageBox.Show(this,"Source path \"" + Substring + "\" is not at the same location in every path in the playlist. Continue?", "Source Paths Inconsistent", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }
        private string GetNewFilePath()
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = "m3u";
            dialog.Filter = PLAYLIST_FILTER;
            dialog.Title = "Choose a location to save the file";
            dialog.OverwritePrompt = true;

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }

            return null;
        }
        private bool WriteAllLines(string Filepath, IEnumerable<string> Lines)
        {
            try
            {
                File.WriteAllLines(Filepath, Lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error saving file: " + ex.Message, "Error Saving File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }        
    }
}