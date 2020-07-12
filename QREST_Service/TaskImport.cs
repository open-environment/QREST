using QREST_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRESTModel.DAL;
using QRESTModel.BLL;
using System.IO;

namespace QRESTServiceCatalog
{
    class Import
    {
        private static Import objInstance = null;
        private static bool bExecutingGenLedSvcStatus = false;

        public static Import Instance
        {
            get
            {
                if (objInstance == null)
                    objInstance = new Import();
                return objInstance;
            }
        }

        public void RunService()
        {
            try
            {
                if (!bExecutingGenLedSvcStatus)
                    ExecuteImport();
            }
            catch (Exception ex)
            {
                General.WriteToFile("Error in Import Task: Error Message : " + ex.ToString());
                bExecutingGenLedSvcStatus = false;
            }
        }

        private void ExecuteImport()
        {
            bExecutingGenLedSvcStatus = true;

            List<T_QREST_DATA_IMPORTS> _imports = db_Air.GetT_QREST_DATA_IMPORTS_ByStatus("STARTED");
            if (_imports != null)
            {
                foreach (T_QREST_DATA_IMPORTS _import in _imports)
                {
                    General.WriteToFile("Import started for - " + _import.IMPORT_IDX);
                    ImportHelper.ImportValidateAndSaveToTemp(_import.IMPORT_IDX);
                    General.WriteToFile("Import ended for - " + _import.IMPORT_IDX);
                }
            }

            bExecutingGenLedSvcStatus = false;
        }

    }
}
