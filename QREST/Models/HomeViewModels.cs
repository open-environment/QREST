using QREST.App_Logic.DataAccessLayer;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace QREST.Models
{
    public class vmHomeIndex
    {
    }

    public class vmHomeHelp {
        public List<T_QREST_HELP_DOCS> HelpTopics { get; set; }
    }

}