using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace QREST_Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();


        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            GetCustomServiceName();
            base.Install(stateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            GetCustomServiceName();
            base.Uninstall(savedState);
        }


        //Retrieve custom service name from installutil command line parameters
        private void GetCustomServiceName()
        {
            string customServiceName = Context.Parameters["servicename"];
            if (!string.IsNullOrEmpty(customServiceName))
            {
                serviceInstaller1.ServiceName = customServiceName;
                serviceInstaller1.DisplayName = customServiceName;
            }
        }

    }


}
