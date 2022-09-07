using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Reportes
{
    public class ReportesConexion
    {

        public static CrystalDecisions.Shared.ConnectionInfo GetConexion()
        {
            CrystalDecisions.Shared.ConnectionInfo infocon = new CrystalDecisions.Shared.ConnectionInfo();
            infocon.ServerName = @"transport.cd27pkoy3gew.us-east-1.rds.amazonaws.com,1433";
            infocon.DatabaseName = "Transport";
            infocon.UserID = "transadmin";
            infocon.Password = "admintrans";
            return infocon;
        }

    }
}