using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QRESTModel.DAL;

namespace QREST.Models
{
    public class vmHomeIndex
    {
    }

    public class vmHomeHelp {
        public List<T_QREST_HELP_DOCS> HelpTopics { get; set; }
    }


    public class vmHomeTerms {
        public string TermsAndConditions { get; set; }
    }
}