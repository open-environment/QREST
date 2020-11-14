using QRESTModel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRESTModel.BLL
{
    public class ImportHelper
    {
        //psuedo


        //1. [STARTED] (handled in controller)
        // 1a. Save text block to IMPORT table with status = STARTED, also saving ImportType, pollConfigIDX, recalcInd (for n-min), monitoridx (for H1)


        //2. [VALIDATING] 
        // 2a. Iterate all import records with status of STARTED 
        // 2b. Update status to VALIDATING 
        // 2c. Insert to IMPORT_TEMP
        // 2d. Update status to VALIDATED
        // 2e. If no errors or dups, auto proceed to IMPORTING step

        //Z. if errors, display to user

        //3. [IMPORTING] 
        // 3a. Update status to IMPORTING
        // 3b. INSERT new record and UPDATE existing records
        // 3c. Delete records from IMPORT_TEMP
        // 3d. Update status to IMPORTED



        //public static bool ImportOrchestrator()
        //{
        //    //select all those that are STARTED
        //    List<T_QREST_DATA_IMPORTS> _imports = db_Air.GetT_QREST_DATA_IMPORTS_ByStatus("STARTED");
        //    if (_imports != null)
        //    {
        //        foreach (T_QREST_DATA_IMPORTS _import in _imports)
        //        {

        //            ImportValidateAndSaveToTemp(_import.IMPORT_IDX);
        //        }
        //    }

        //    return true;
        //}



        /// <summary>
        /// Reads the whole import text block, parses, validates, and writes result to IMPORT_TEMP table
        /// </summary>
        /// <param name="iMPORT_IDX"></param>
        /// <returns>True if import succeeded, false if exception encountered</returns>
        public static bool ImportValidateAndSaveToTemp(Guid iMPORT_IDX)
        {
            T_QREST_DATA_IMPORTS _import = db_Air.GetT_QREST_DATA_IMPORTS_byID(iMPORT_IDX);
            if (_import != null)
            {
                //update status to VALIDATING
                db_Air.InsertUpdateT_QREST_DATA_IMPORTS(_import.IMPORT_IDX, null, null, null, "VALIDATING", null, null, null, null, null, null, null);

                //split file into rows
                string[] allRows = _import.SUBMISSION_FILE.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                //get poll config
                T_QREST_SITE_POLL_CONFIG _pollConfig = db_Air.GetT_QREST_SITE_POLL_CONFIG_ByID(_import.POLL_CONFIG_IDX.GetValueOrDefault());

                //get allowed date/time formats
                string[] dtTmFormats = UtilsText.GetDateTimeAllowedFormats(_pollConfig.DATE_FORMAT, _pollConfig.TIME_FORMAT);

                //keep track if there are any dups or errors necessitating a stoppage
                bool AnyDupsOrErrors = false;

                //**************************************************************************************
                //    F                five-minute
                //    H                hourly
                //**************************************************************************************
                if (_import.IMPORT_TYPE == "F" || _import.IMPORT_TYPE == "H")
                {
                    AnyDupsOrErrors = db_Air.BulkInsertT_QREST_DATA_IMPORT_TEMP_H(allRows, _pollConfig, _import.IMPORT_USERIDX, _import.IMPORT_IDX, dtTmFormats);
                }

                //**************************************************************************************
                //    H1                hourly (1 parameter with hours arranged as columns)
                //**************************************************************************************
                else if (_import.IMPORT_TYPE == "H1")
                {
                    T_QREST_MONITORS _monitor = db_Air.GetT_QREST_MONITORS_ByID_Simple(_import.MONITOR_IDX ?? Guid.Empty);
                    if (_monitor != null)
                    {
                        AnyDupsOrErrors = db_Air.BulkInsertT_QREST_DATA_IMPORT_TEMP_H1(allRows, new char[] { ',' }, _import.MONITOR_IDX.GetValueOrDefault(), _pollConfig.LOCAL_TIMEZONE.ConvertOrDefault<int>(), _pollConfig.TIME_POLL_TYPE, _monitor.COLLECT_UNIT_CODE, _import.IMPORT_USERIDX, _import.IMPORT_IDX, dtTmFormats);
                    }
                }

                //update status to VALIDATED
                db_Air.InsertUpdateT_QREST_DATA_IMPORTS(_import.IMPORT_IDX, null, null, null, "VALIDATED", null, null, null, null, null, null, null);


                //if there are no errors or dups, then just continue on with import
                if (AnyDupsOrErrors == false)
                    ImportFinal(_import.IMPORT_IDX);


                return true;
            }
            else
                return false;
        }



        /// <summary>
        /// Copies data from IMPORT_TEMP table to either HOURLY or FIVE_MIN table, then deletes TEMP data
        /// </summary>
        /// <param name="iMPORT_IDX"></param>
        /// <returns></returns>
        public static bool ImportFinal(Guid iMPORT_IDX)
        {
            //copy from temp table to real table
            db_Air.SP_IMPORT_DATA_FROM_TEMP(iMPORT_IDX);


            //if hourly data is being imported, and user selected to revalidate, then revalidate
            T_QREST_DATA_IMPORTS _imp = db_Air.GetT_QREST_DATA_IMPORTS_byID(iMPORT_IDX);
            if (_imp != null && (_imp.IMPORT_TYPE== "H1" || _imp.IMPORT_TYPE == "H") && _imp.RECALC_IND==true)
            {
                //run min/max validation on data
                db_Air.SP_VALIDATE_HOURLY_IMPORT(iMPORT_IDX);
            }

            return true;
        }
    }
}
