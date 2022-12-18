using ClassTeacher_Assist.Models;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace ClassTeacher_Assist
{
    public class WordWrite
    {
        private readonly string _docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private FileInfo _fileInfo;
        public WordWrite(string fileName) 
        {
            if(File.Exists(fileName)) 
            {
                _fileInfo = new FileInfo(fileName);
            }
            else
            {
                throw new ArgumentException("Invalid file name");
            }
        }

        public void Process(Dictionary<string, string> keyValuePairs)
        {
            var app = new Word.Application();
            try
            {
                Object file = _fileInfo.FullName;
                object missing = Type.Missing;

                app.Documents.Open(file);

                foreach (var keyValue in keyValuePairs)
                {
                    Word.Find find = app.Selection.Find;
                    find.Text = keyValue.Key;
                    find.Replacement.Text = keyValue.Value;

                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replace = Word.WdReplace.wdReplaceAll;

                    find.Execute(FindText: Type.Missing, MatchCase: false, MatchAllWordForms: false, MatchWholeWord: false,
                        MatchWildcards: false, MatchSoundsLike: missing, Forward: true,
                        Wrap: wrap, Format: false, ReplaceWith: missing, Replace: replace);
                }
                Random rnd = new Random();
                string path = "enroll_" + rnd.Next(1,999999999).ToString() + ".docx";
                Object newFileName = Path.Combine(_docsPath, path);
                app.ActiveDocument.SaveAs2(newFileName);
            }
            finally
            {
                app.ActiveDocument.Close();
                app.Quit();
            }

        }
    }
}
