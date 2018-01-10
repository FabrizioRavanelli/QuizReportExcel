using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Tools.Attribute;

namespace QuizReportExcel.Logic.Model
{
    public class QuizReportExcelModel
    {
        private bool _canLoadArtistTrack;
        [DoNotExport]
        public bool CanLoadArtistTrack { get; set; }

        private decimal _id;
        [DisplayName("Id")]
        public decimal Id { get; set; }

        private string _title;
        [DisplayName("Titel")]
        public string Title { get; set; }
    }
}
