﻿using QRESTModel.DAL;
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


        //2. [VALIDATING] (and placement to IMPORT_TEMP) 
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

                //get site
                T_QREST_SITES _site = db_Air.GetT_QREST_SITES_ByID(_import.SITE_IDX);

                //get allowed date/time formats
                string[] dtTmFormats = _pollConfig !=null ? UtilsText.GetDateTimeAllowedFormats(_pollConfig.DATE_FORMAT, _pollConfig.TIME_FORMAT) : null;

                //keep track if there are any dups or errors necessitating a stoppage
                bool AnyDupsOrErrors = false;

                //**************************************************************************************
                //    F                five-minute
                //    H                hourly
                //**************************************************************************************
                if (_import.IMPORT_TYPE == "F" || _import.IMPORT_TYPE == "H")
                {
                    AnyDupsOrErrors = db_Air.BulkInsertT_QREST_DATA_IMPORT_TEMP_H(allRows, _pollConfig, _import.IMPORT_USERIDX, _import.IMPORT_IDX, dtTmFormats, _site.LOCAL_TIMEZONE.ConvertOrDefault<int>(), _import.IMPORT_TYPE);
                }

                //**************************************************************************************
                //    H1                hourly (1 parameter with hours arranged as columns)
                //**************************************************************************************
                else if (_import.IMPORT_TYPE == "H1")
                {
                    T_QREST_MONITORS _monitor = db_Air.GetT_QREST_MONITORS_ByID_Simple(_import.MONITOR_IDX ?? Guid.Empty);
                    if (_monitor != null)
                    {
                        AnyDupsOrErrors = db_Air.BulkInsertT_QREST_DATA_IMPORT_TEMP_H1(allRows, new char[] { ',' }, _import.MONITOR_IDX.GetValueOrDefault(), _site.LOCAL_TIMEZONE.ConvertOrDefault<int>(), _pollConfig.TIME_POLL_TYPE, _monitor.COLLECT_UNIT_CODE, _import.IMPORT_USERIDX, _import.IMPORT_IDX, dtTmFormats);
                    }
                }
                //**************************************************************************************
                //    A                 AQS RD pipe delimited format
                //**************************************************************************************
                else if (_import.IMPORT_TYPE == "A")
                {
                    T_QREST_MONITORS _monitor = db_Air.GetT_QREST_MONITORS_ByID_Simple(_import.MONITOR_IDX ?? Guid.Empty);
                    if (_monitor != null)
                    {
                        AnyDupsOrErrors = db_Air.BulkInsertT_QREST_DATA_IMPORT_TEMP_AQS_RD(allRows, _import.IMPORT_USERIDX, _import.IMPORT_IDX, _import.MONITOR_IDX.GetValueOrDefault(), _site.LOCAL_TIMEZONE.ConvertOrDefault<int>());
                    }
                }

                //detect duplicates 
                db_Air.SP_IMPORT_DETECT_DUPES(_import.IMPORT_IDX);

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


            //if hourly data is being imported, and user selected to validate, then run min/max validation on data
            //or if five minute data is being imported and user selected to calculate hourly, then run min/max validation on data
            T_QREST_DATA_IMPORTS _imp = db_Air.GetT_QREST_DATA_IMPORTS_byID(iMPORT_IDX);
            if (_imp != null && (_imp.IMPORT_TYPE== "H1" || _imp.IMPORT_TYPE == "H" || _imp.IMPORT_TYPE == "F") && _imp.RECALC_IND==true)
                db_Air.SP_VALIDATE_HOURLY_IMPORT(iMPORT_IDX);
 
            return true;
        }
    }
}
