using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using QRESTModel.DAL;

namespace QREST_Service
{
    public partial class QRESTService : ServiceBase
    {
        private Timer timer = new Timer();

        public QRESTService()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Startup code - this method runs when the service starts up for the first time.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            General.WriteToFile("QREST Task Service started");

            try
            {
                //Reset the QREST service status so it will run again (in case it failed previously)
                bool SuccID = db_Ref.UpdateT_QREST_TASKS_ResetAll();
                if (SuccID)
                {
                    // Set up a timer that triggers every minute.
                    timer.Interval = 60000; // 60 seconds
                    timer.Elapsed += new ElapsedEventHandler(OnTimer);
                    timer.AutoReset = true;
                    timer.Enabled = true;
                    timer.Start();
                    General.WriteToFile("*************************************************");
                    General.WriteToFile("QREST Task Service timer successfully initialized");
                    General.WriteToFile("*************************************************");
                    General.WriteToFile("QREST Task Service timer set to run every " + timer.Interval + " ms");
                }
                else
                {
                    General.WriteToFile("ERROR reseting tasks");
                }

            }
            catch (Exception ex)
            {
                General.WriteToFile("Failed to start QREST - Unspecified error. " + ex.Message);
            }
        }


        /// <summary>
        /// Stops task
        /// </summary>
        protected override void OnStop()
        {
            General.WriteToFile("QREST Task has stopped");
        }


        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                //find all tasks that are scheduled to run (but not already running)
                List<T_QREST_APP_TASKS> _tasks = db_Ref.GetT_VCCB_TASKS_ReadyToRun();
                if (_tasks != null && _tasks.Count > 0)
                {
                    foreach (T_QREST_APP_TASKS _task in _tasks)
                    {
                        //SET TASK AS RUNNING
                        db_Ref.UpdateT_QREST_TASKS_SetRunning(_task.TASK_IDX);
                        General.WriteToFile(_task.TASK_NAME + " task started.");

                        try
                        {
                            Type yourType = Type.GetType("QRESTServiceCatalog." + _task.TASK_NAME);

                            object yourObject = Activator.CreateInstance(yourType);
                            object result = yourType.GetMethod("RunService").Invoke(yourObject, null);

                            //SET TASK AS COMPLETED AND SET NEXT RUN
                            db_Ref.UpdateT_QREST_TASKS_SetCompleted(_task.TASK_IDX);
                            General.WriteToFile(_task.TASK_NAME + " task completed.");
                        }
                        catch (Exception ex)
                        {
                            General.WriteToFile("ERROR invoking " + _task.TASK_NAME + " Task: " + ex.Message.ToString());
                        }

                    }
                }
                else
                    General.WriteToFile("---");
            }
            catch {
                General.WriteToFile("ERROR getting execution time information from database.");
            }
        }



    }
}
