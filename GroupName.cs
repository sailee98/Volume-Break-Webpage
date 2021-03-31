using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Volume_Break_Webpage.CLsFolder;

namespace VolumeBreakOut.Models
{
    public class GroupName
    {
        DBConn dbConn = new DBConn();
        public string GrpName { get; set; }
        public string VolType { get; set; }
        public int Grp_Id { get; set; }
        public int StockId { get; set; }
        public string Acc_Volume { get; set; }
        public string Stack_Name { get; set; }
        public string CODE_B { get; set; }
        public string Update_date_time { get; set; }
        public double Change1 { get; set; }
        public double Percent_Change { get; set; }
        public double Last_price { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }

        public List<GroupName> GroupNameList = new List<GroupName>();
    }
}