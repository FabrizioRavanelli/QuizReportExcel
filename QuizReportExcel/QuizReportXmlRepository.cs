using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using System.Text;
using System.Threading.Tasks;

namespace QuizReportExcel
{
    public class QuizReportXmlRepository
    {
        public void ReadFolder(string path)
        {
            //foreach file_in_path => readFile
        }

        public void ReadFile(string pathOfQuizReportXml)
        {
            var document = XDocument.Load(pathOfQuizReportXml, LoadOptions.SetBaseUri);
            //LogToAvailableLoggers("Öffne Zuordnungsdatei...", LogMessageType.Info, session, Task.PressImport);
            var mappingDocument = XDocument.Load(@"PressImportXmlObjectMappingConfig.xml", LoadOptions.SetBaseUri);
        }
    }
}
