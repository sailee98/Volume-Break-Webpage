using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Volume_Break_Webpage.CLsFolder;
using VolumeBreakOut.Models;

namespace VolumeBreakOut.Controllers
{
    public class HomeController : Controller
    {
        DBConn dbConn = new DBConn();
        List<GroupName> GroupNameList = new List<GroupName>();
        GroupName grp = new GroupName();

       
            public ActionResult PriceVolumeGrid(GroupName Volgrid, string volgrp, string Volume3, string sort, string sortdir)
        {
           
            if (Session["UserName"] != null)
            {
                //ViewBag.Message = "Your application description page.";
            }
            else
            {
                return RedirectToAction("LoginAction", "Login");
            }

             try
            {

                if (volgrp != null)
                {
                    Session["volgrp"] = volgrp;
                }
                else
                {

                    volgrp = Session["volgrp"].ToString();
                }



            }
            catch
            {
                volgrp = "";

            }

           
             GetGroupVolume(Volgrid, volgrp, Volume3, sort, sortdir);
                   

            String[] strvolume;
            char delimiter = ',';
            strvolume = System.Configuration.ConfigurationManager.AppSettings["Vol3"].ToString().Split(delimiter);
            List<SelectListItem> str_volumelist = new List<SelectListItem>();

            if ((Volume3 == "") || (Volume3 == null))
            {
                Volume3 = "";
            }
            for (Int32 j = 0; j < strvolume.Length; j++)
            {
                str_volumelist.Add(new SelectListItem { Text = strvolume[j].ToString(), Value = strvolume[j].ToString(), Selected = (strvolume[j].ToString() == Volume3 ? true : false) });
            }
            ViewBag.Volume3 = str_volumelist;


            return View(GroupNameList);
        }

        public void GetGroupVolume(GroupName Volgrid, string volgrp, string Volume3, string sort, string sortdir)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringMySql"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "select group_id,group_name from STOCK_GROUP_MASTER where group_id in (114,145,147,181,177,187,193,146) order by group_name";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {

                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["group_name"].ToString(),

                                Value = sdr["group_id"].ToString(),
                                Selected = Request["group_id"] == sdr["group_id"].ToString() ? true : false
                            });

                            ViewBag.volgrp = items.Select(x =>
                             new SelectListItem()
                             {
                                 Text = sdr["group_name"].ToString(),
                                 Value = sdr["group_id"].ToString()
                             });
                        }
                    }
                    con.Close();
                }
            }
            ViewData["volgrp"] = items;
            ViewBag.volgrp = items;
            BindData(Volgrid, volgrp, Volume3, sort, sortdir);


        }

        public Boolean checkHoliday(string currDate)
        {
            Boolean check = false;
            string query = "SELECT * FROM HOLIDAY_LIST where DATE_FORMAT(H_DATE,'%Y-%m-%d') = '" + currDate + "'";
            DataSet ds = dbConn.ReturnDataSet(query);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows != null)
            {
                check = true;
            }
            return check;
        }

        public DataTable BindData(GroupName Volgrid, string volgrp, string Volume3, string sort, string sortdir)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            int listIndex = -1;
            //string strDDLValue = /*form*/["volgrp"].ToString();
            if (volgrp == "")
            {
                volgrp = "177";
            }
            string queryGLHeadMaster = "select a.stock_id,a.acc_volume,a.stack_name,a.CODE_B,a.update_date_time,a.close,a.change1,a.percent_change,b.acc_volume,a.last_price,b.HIGH,b.low from ((select s.stock_id, s.stack_name, s.acc_Volume, s.last_price,s.CODE_B,s.close,s.update_date_time,s.change1,s.percent_change from STOCK_TRANSITION s inner join STOCK_GROUP_DETAILS g on s.stock_id = g.stock_id where group_id = '" + volgrp + "') a inner join  (SELECT d.stock_id, avg(acc_volume) acc_volume, max(HIGH) as HIGH, MIN(LOW) low FROM DAY_STATUS d INNER JOIN STOCK_GROUP_DETAILS g on d.stock_id = g.stock_id inner join(select date_format(stock_Date, '%d-%m-%Y') as s_date  from DAY_STATUS where stock_id = 7000  order by stock_Date desc LIMIT 5) h on date_format(d.stock_Date, '%d-%m-%Y') = h.s_date where g.group_id = '" + volgrp + "' group by d.stock_id) b on a.stock_id = b.stock_id) where a.acc_Volume > b.acc_Volume";
            if (Volume3 == "VOLUME_BREAK_LOW")
            {
                queryGLHeadMaster += " AND a.LAST_PRICE < b.LOW";
            }
            else if (Volume3 == "VOLUME_BREAK_HIGH")
            {
                queryGLHeadMaster += " AND a.LAST_PRICE > b.HIGH";
            }

            if ((sort != null) && (sortdir != null))
            {
                queryGLHeadMaster += " order by " + sort + " " + sortdir;

            }
            ds = dbConn.ReturnDataSet(queryGLHeadMaster);
            dt = ds.Tables[0];




            for (int i = 0; i < dt.Rows.Count; i++)
            {

                GroupName groupn = new GroupName();

                groupn.StockId = Convert.ToInt32(dt.Rows[i]["stock_id"]);
                groupn.Stack_Name = dt.Rows[i]["stack_name"].ToString();
                groupn.Last_price = Convert.ToDouble(dt.Rows[i]["last_price"]);
                groupn.Change1 = Convert.ToDouble(dt.Rows[i]["change1"]);
                groupn.Percent_Change = Convert.ToDouble(dt.Rows[i]["percent_change"]);
                groupn.Acc_Volume = dt.Rows[i]["acc_volume"].ToString();
                groupn.Close = Convert.ToDouble(dt.Rows[i]["close"]);
                groupn.High = Convert.ToDouble(dt.Rows[i]["high"]);
                groupn.Low = Convert.ToDouble(dt.Rows[i]["low"]);

                GroupNameList.Add(groupn);
                listIndex = listIndex + 1;
                //GetGroupVolume(Volgrid, volgrp);
                ViewData["viewGrid"] = GroupNameList;
                //lstGLHeadMaster.Add(gLHead);

            }
            return dt;
        }

       


        [HttpPost]
        public JsonResult GetJsonVolgrp(GroupName Volgrid, string volgrp, string Volume3)
        {
            if (volgrp == null)
            {
                volgrp = "0";
            }
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            int listIndex = -1;
            //string strDDLValue = /*form*/["volgrp"].ToString();
            if (volgrp == null)
            {
                volgrp = "0";
            }

            string queryGLHeadMaster = "select a.stock_id,a.acc_volume,a.stack_name,a.CODE_B,a.update_date_time,a.close,a.change1,a.percent_change,b.acc_volume,a.last_price,b.HIGH,b.low from ((select s.stock_id, s.stack_name, s.acc_Volume, s.last_price,s.CODE_B,s.close,s.update_date_time,s.change1,s.percent_change from STOCK_TRANSITION s inner join STOCK_GROUP_DETAILS g on s.stock_id = g.stock_id where group_id = '" + volgrp + "') a inner join  (SELECT d.stock_id, avg(acc_volume) acc_volume, max(HIGH) as HIGH, MIN(LOW) low FROM DAY_STATUS d INNER JOIN STOCK_GROUP_DETAILS g on d.stock_id = g.stock_id inner join(select date_format(stock_Date, '%d-%m-%Y') as s_date  from DAY_STATUS where stock_id = 7000  order by stock_Date desc LIMIT 5) h on date_format(d.stock_Date, '%d-%m-%Y') = h.s_date where g.group_id = '" + volgrp + "' group by d.stock_id) b on a.stock_id = b.stock_id) where a.acc_Volume > b.acc_Volume";
            if (Volume3 == "VOLUME_BREAK_LOW")
            {
                queryGLHeadMaster += " AND a.LAST_PRICE < b.LOW";
            }
            else if (Volume3 == "VOLUME_BREAK_HIGH")
            {
                queryGLHeadMaster += " AND a.LAST_PRICE > b.HIGH";
            }

            ds = dbConn.ReturnDataSet(queryGLHeadMaster);
            dt = ds.Tables[0];


            for (int i = 0; i < dt.Rows.Count; i++)
            {

                GroupName groupn = new GroupName();

                groupn.StockId = Convert.ToInt32(dt.Rows[i]["stock_id"]);
                groupn.Stack_Name = dt.Rows[i]["stack_name"].ToString();
                groupn.Last_price = Convert.ToDouble(dt.Rows[i]["last_price"]);
                groupn.Change1 = Convert.ToDouble(dt.Rows[i]["change1"]);
                groupn.Percent_Change = Convert.ToDouble(dt.Rows[i]["percent_change"]);
                groupn.Acc_Volume = dt.Rows[i]["acc_volume"].ToString();
                groupn.Close = Convert.ToDouble(dt.Rows[i]["close"]);
                groupn.High = Convert.ToDouble(dt.Rows[i]["high"]);
                groupn.Low = Convert.ToDouble(dt.Rows[i]["low"]);


                GroupNameList.Add(groupn);
                listIndex = listIndex + 1;
                //GetGroupVolume(Volgrid, volgrp);
                ViewData["viewGrid"] = GroupNameList;
                //lstGLHeadMaster.Add(gLHead);

            }

            return Json("GroupNameList", JsonRequestBehavior.AllowGet);

        }
    }


}