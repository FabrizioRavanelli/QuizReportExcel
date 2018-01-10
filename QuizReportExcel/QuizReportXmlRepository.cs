using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QuizReportExcel
{
    public class QuizReportXmlRepository
    {
        public void ReadFolder(string path)
        {
            //foreach file_in_path => readFile
            string filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DirectoryInfo d = new DirectoryInfo(filepath);
            //TODO Anpassen....
            foreach (var file in d.GetFiles("*.xml"))
            {
                Directory.Move(file.FullName, filepath + "\\TextFiles\\" + file.Name);
            }
        }

        public void ReadFile(string pathOfQuizReportXml)
        {
            var document = XDocument.Load(pathOfQuizReportXml, LoadOptions.SetBaseUri);
            //LogToAvailableLoggers("Öffne Zuordnungsdatei...", LogMessageType.Info, session, Task.PressImport);
            var questions = document.Root.Element("questions").Elements("multipleChoiceQuestion");
            foreach (var question in questions)
            {
                var id = question.Attribute("id");
                var status = question.Attribute("status");
                var correctAnswerIndex = question.Element("answers").Attribute("correctAnswerIndex");
                var userAnswerIndex = question.Element("answers").Attribute("userAnswerIndex");
                
                Console.WriteLine("multipleChoiceQuestion: id={0}, status={1}, correctAnswerIndex={2}, userAnswerIndex={3}," +
                    id, status, correctAnswerIndex, userAnswerIndex);
            }
        }
    }
}
