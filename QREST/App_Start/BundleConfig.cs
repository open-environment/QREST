﻿using System.Web.Optimization;

namespace QREST
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //******************************************************************
            //************* JAVASCRIPT *****************************************
            //******************************************************************

            //DEFAULT MSFT TEMPLATE
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
            //CUSTOM ADDED
            bundles.Add(new ScriptBundle("~/bundles/app").Include("~/Scripts/app.js"));
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/dataTables/jquery.dataTables.min.js",
                "~/Scripts/dataTables/plugins/moment.js",
                "~/Scripts/dataTables/plugins/datetime.js" ));
            bundles.Add(new ScriptBundle("~/bundles/confirm-delete").Include("~/Scripts/confirm-delete.js"));
            bundles.Add(new ScriptBundle("~/bundles/toastr").Include("~/Scripts/toastr.min.js"));
            //bundles.Add(new ScriptBundle("~/bundles/datepicker").Include("~/Scripts/datepicker.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/filestyle").Include("~/Scripts/bootstrap-filestyle.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/cleave").Include("~/Scripts/cleave.min.js","~/Scripts/cleave.qrest.js"));
            bundles.Add(new ScriptBundle("~/bundles/areyousure").Include(
                "~/Scripts/jquery.dirtyforms.min.js",
                "~/Scripts/jquery.dirtyforms.dialogs.bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/canvasjs").Include("~/Scripts/canvasjs.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/select2").Include("~/Scripts/select2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/easypiechart").Include("~/Scripts/jquery.easypiechart.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/moment").Include("~/Scripts/dataTables/plugins/moment.js"));



            //******************************************************************
            //************* CSS        *****************************************
            //******************************************************************
            bundles.Add(new StyleBundle("~/Content/css/files").Include(
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/app.css",
                      "~/Content/css/site.css",
                      "~/Content/css/animate.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/Scripts/dataTables/DataTables-1.10.18/css/jquery.dataTables.min.css",
                      "~/Content/css/toastr.min.css",
                     // "~/Content/css/datepicker.min.css",
                      "~/Content/css/select2.min.css"
                      ));

        }
    }
}
