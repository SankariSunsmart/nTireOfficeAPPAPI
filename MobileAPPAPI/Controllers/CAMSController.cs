﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAppAPI.Models;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CAMSController : ControllerBase
    {
        public static Helper objhelper = new Helper();
        public static string strconn = objhelper.Connectionstring();


        [HttpGet]
        [Route("camsbranchcount/{strfunction}")]
        public string camsbranchcount(string strfunction)
        {
            string Logdata1 = string.Empty;
            var logdata = "";
            DataSet dsbranchcount = new DataSet();
            // SQLCONNECTION dbcon = new SQLCONNECTION();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                //string sql = "CAMS_BRANCHWISE_ASSETCOUNT";
                string sql = "MBL_CAMS_BRANCHWISE_ASSETCOUNT";
               
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.Parameters.AddWithValue("@FunctionId", strfunction);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);


                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    DataRow row = results.Rows[i];
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                }
                return Logdata1;
            }
        }

        [HttpGet]
        [Route("camscategorycount/{struserid}")]
        public string camscategorycount(string struserid)
        {
            string Logdata1 = string.Empty;
            DataSet dscategorycount = new DataSet();
            DataTable dtcategorycount = new DataTable();

            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
               // string sql = "CAMS_ASSETCATEGORY_COUNT";
                string sql = "MBL_CAMS_ASSETCATEGORY_COUNT";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USERID", struserid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    DataRow row = results.Rows[i];
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                }
                return Logdata1;
            }
        }


        [HttpGet]
        [Route("camsmaintenancecurrentmonthcount/{struserid}")]
        public string camsmaintenancecurrentmonthcount(string struserid)
        {
            string Logdata1 = string.Empty;

            DataTable dtmaintenancecurrentmonthcount = new DataTable();
            DataSet dsmaintenancecurrentmonthcount = new DataSet();
          
                DataSet DS = new DataSet();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
               // string sql = "CAMS_CURRENT_MONTH_MAINTENANCE_CHART";
                string sql = "MBL_CAMS_CURRENT_MONTH_MAINTENANCE_CHART";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USERID", struserid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dsmaintenancecurrentmonthcount);

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    DataRow row = results.Rows[i];
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                }
                return Logdata1;
            }
        }









        [HttpPost]
        [Route("assetserreqdept")]
        public async Task<ActionResult<CAMS>> assetserreqdept(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT cam.ASSET_ID,cam.ASSET_USER,bp.val as dpid,cam.ASSET_CODE,bp.Text,cam.ASSET_DESCRIPTION FROM CAMS_ASSET_MASTER as cam inner join BO_PARAMETER as bp on bp.VAL=cam.ASSET_DEPARTMENT  where cam.ASSET_CODE='" + data.assetcode + "' and cam.BRANCH_ID='" + data.branchid + "' and bp.TYPE='BO_TEAM' and bp.function_id='1' and bp.status='A'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        [HttpPost]
        [Route("assetservreqlistview")]
        public async Task<ActionResult<CAMS>> assetservreqlistview(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select distinct CAMS_ASSET_SERVICE.REPLACEMENT_ASSET_ID, CAMS_ASSET_MASTER.asset_code as ASSET_CODE1,CAMS_ASSET_MASTER.ASSET_DESCRIPTION  ,CAMS_ASSET_SERVICE.FUNCTION_ID, HISTORY_ID,BO_BRANCH_MASTER.BRANCH_DESC as branch,CAMS_ASSET_SERVICE.ASSET_CODE,CAMS_ASSET_SERVICE.ASSET_ID,CAMS_ASSET_SERVICE.VENDOR_CODE as vendor,CONVERT(VARCHAR(10),DATE_OF_SERVICE,103) as DATE_OF_SERVICE,CONVERT(VARCHAR(10),EXPECTED_DATE_OF_DELIVERY,103) as'EXPECTED_DATE_OF_DELIVERY',convert(numeric(18,2),EXPECTED_EXPENSES) as EXPECTED_EXPENSES,case CAMS_ASSET_SERVICE.STATUS when 'N' then 'New' when 'P' then 'Pending' when 'A' then 'Approved' when 'D' then 'Denied' end as status,mode,INSURANCECOMPANY,AMOUNTINSURED,CONVERT(VARCHAR(10),WARRANTYDATE,103) as WARRANTYDATE,CAMS_ASSET_SERVICE.CREATED_ON createddate from CAMS_ASSET_SERVICE inner  join BO_BRANCH_MASTER with(nolock)on BO_BRANCH_MASTER.BRANCH_ID=CAMS_ASSET_SERVICE.BRANCH_ID and BO_BRANCH_MASTER.FUNCTION_ID=CAMS_ASSET_SERVICE.FUNCTION_ID left outer join  ERP_VENDOR_MASTER with(nolock)on ERP_VENDOR_MASTER.vendor_id=CAMS_ASSET_SERVICE.VENDOR_ID and ERP_VENDOR_MASTER.function_id=CAMS_ASSET_SERVICE.FUNCTION_ID Join CAMS_ASSET_MASTER on CAMS_ASSET_MASTER.ASSET_ID = CAMS_ASSET_SERVICE.asset_id and CAMS_ASSET_MASTER.FUNCTION_ID=CAMS_ASSET_SERVICE.FUNCTION_ID and CAMS_ASSET_MASTER.BRANCH_ID=CAMS_ASSET_SERVICE.BRANCH_ID where CAMS_ASSET_SERVICE.RELEASE in ('P','A','N','D') and CAMS_ASSET_SERVICE.MODE='S' and CAMS_ASSET_SERVICE.BRANCH_ID='" + data.branchid + "' order by CAMS_ASSET_SERVICE.CREATED_ON desc";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }




        [HttpPost]
        [Route("assetservreqlistviewfilter")]
        public async Task<ActionResult<CAMS>> assetservreqlistviewfilter(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select distinct CAMS_ASSET_SERVICE.REPLACEMENT_ASSET_ID, CAMS_ASSET_MASTER.asset_code as ASSET_CODE1,CAMS_ASSET_MASTER.ASSET_DESCRIPTION  ,CAMS_ASSET_SERVICE.FUNCTION_ID, HISTORY_ID,BO_BRANCH_MASTER.BRANCH_DESC as branch,CAMS_ASSET_SERVICE.ASSET_CODE,CAMS_ASSET_SERVICE.ASSET_ID,CAMS_ASSET_SERVICE.VENDOR_CODE as vendor,CONVERT(VARCHAR(10),DATE_OF_SERVICE,103) as DATE_OF_SERVICE,CONVERT(VARCHAR(10),EXPECTED_DATE_OF_DELIVERY,103) as'EXPECTED_DATE_OF_DELIVERY',convert(numeric(18,2),EXPECTED_EXPENSES) as EXPECTED_EXPENSES,case CAMS_ASSET_SERVICE.STATUS when 'N' then 'New' when 'P' then 'Pending' when 'A' then 'Approved' when 'D' then 'Denied' end as status,mode,INSURANCECOMPANY,AMOUNTINSURED,CONVERT(VARCHAR(10),WARRANTYDATE,103) as WARRANTYDATE,CAMS_ASSET_SERVICE.CREATED_ON createddate from CAMS_ASSET_SERVICE inner  join BO_BRANCH_MASTER with(nolock)on BO_BRANCH_MASTER.BRANCH_ID=CAMS_ASSET_SERVICE.BRANCH_ID and BO_BRANCH_MASTER.FUNCTION_ID=CAMS_ASSET_SERVICE.FUNCTION_ID left outer join  ERP_VENDOR_MASTER with(nolock)on ERP_VENDOR_MASTER.vendor_id=CAMS_ASSET_SERVICE.VENDOR_ID and ERP_VENDOR_MASTER.function_id=CAMS_ASSET_SERVICE.FUNCTION_ID Join CAMS_ASSET_MASTER on CAMS_ASSET_MASTER.ASSET_ID = CAMS_ASSET_SERVICE.asset_id and CAMS_ASSET_MASTER.FUNCTION_ID=CAMS_ASSET_SERVICE.FUNCTION_ID and CAMS_ASSET_MASTER.BRANCH_ID=CAMS_ASSET_SERVICE.BRANCH_ID where CAMS_ASSET_SERVICE.RELEASE in ('P','A','N','D') and CAMS_ASSET_SERVICE.MODE='S' and CAMS_ASSET_SERVICE.BRANCH_ID='" + data.branchid + "'";

                var flag = 0;
                if (data.assetcode != null)
                {
                    if (flag == 0)
                    {
                        query = query + " AND CAMS_ASSET_MASTER.asset_code='" + data.assetcode + "'";
                        flag = 1;
                    }
                    else
                    {
                        query = query + " AND CAMS_ASSET_MASTER.asset_code='" + data.assetcode + "'";
                    }
                }
                if (data.vendorcode != null)
                {
                    if (flag == 0)
                    {
                        query = query + " AND  CAMS_ASSET_SERVICE.VENDOR_CODE='" + data.vendorcode + "'";
                        flag = 1;
                    }
                    else
                    {
                        query = query + " AND CAMS_ASSET_SERVICE.VENDOR_CODE='" + data.vendorcode + "'";
                    }
                }
                if (data.datofservice != null)
                {
                    if (flag == 0)
                    {
                        query = query + " AND  DATE_OF_SERVICE='" + data.datofservice + "'";
                         flag = 1;
                    }
                    else
                    {
                        query = query + " AND  DATE_OF_SERVICE='" + data.datofservice + "'";
                    }
                }
                query = query + "order by CAMS_ASSET_SERVICE.CREATED_ON desc";





                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


        [HttpPost]
        [Route("assetservicelistreplace")]
        public async Task<ActionResult<CAMS>> assetservicelistreplace(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            DataSet DS = new DataSet();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
               // string sql = "CAMS_ASSET_getAssetsforreplcaseasset";
                string sql = "MBL_CAMS_ASSET_getAssetsforreplcaseasset";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strstatus","A");
                cmd.Parameters.AddWithValue("@Branch",data.branchid);
                cmd.Parameters.AddWithValue("@strfunction",data.functionid);
                cmd.Parameters.AddWithValue("@assettype","0");
                cmd.Parameters.AddWithValue("@AssetReference","");
                cmd.Parameters.AddWithValue("@code",data.assetcode);
                cmd.Parameters.AddWithValue("@desc","");
                cmd.Parameters.AddWithValue("@dept","");
                cmd.Parameters.AddWithValue("@pageIndex","0");
                cmd.Parameters.AddWithValue("@pageSize","20");
                cmd.Parameters.AddWithValue("@sortExpression", "currentdate DESC");
                cmd.Parameters.AddWithValue("@alphaname","");
                cmd.Parameters.AddWithValue("@Category","0");
                cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    DataRow row = results.Rows[i];
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                }
                return Ok(Logdata1);

            }
        }




        [HttpPost]
        [Route("assetdetailslist")]
        public async Task<ActionResult<CAMS>> assetdetailslist(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT cam.ImageUrl,cam.asset_latitude,cam.asset_longitude,bumm.TUM_USER_NAME,casm.SUB_CATEGORY_DESC,cam.TYPE,cam.ASSET_ID,cam.ASSET_CODE,cam.ASSET_DESCRIPTION,cam.ASSET_VALUE,cam.ASSET_BRAND,cam.ASSET_MODE,cam.ASSET_PURCHASE_DATE,cam.ASSET_WARRANTY_TILL,cam.CAMS_ASSET_MANUFACTURER,cam.ASSET_RESIDUAL_VALUE,cam.ASSET_DEPRECIATION_TYPE,cam.ASSET_DEPRECIATION_PERCENTAGE,cam.ASSET_REMARKS,cam.ASSET_APPREQ_USERINITIATED,cam.ASSET_COUNTER_ENABLED,cam.ASSET_INSTALLATION_DATE,cam.ASSET_INSTALLED_BY,cam.ASSET_CERTIFICATE_ISSUED,cam.ASSET_WORKING_CONDITION,bum.TUM_USER_CODE,bop.TEXT as cnme,bop.VAL as cval FROM CAMS_ASSET_MASTER as cam INNER JOIN BO_PARAMETER as bop ON bop.VAL=cam.ASSET_CATEGORY LEFT OUTER JOIN BO_USER_MASTER as bum on bum.TUM_USER_ID=cam.ASSET_USER LEFT OUTER JOIN CAMS_ASSET_SUBCATEGORY_MASTER casm on casm.SUB_CATEGORY_ID=cam.ASSET_TYPE LEFT OUTER JOIN BO_USER_MASTER as bumm on bumm.TUM_USER_ID=cam.ASSET_OWNER_ID WHERE bop.FUNCTION_ID=1 and  bop.TYPE='InfCategory' and cam.ASSET_CODE='" + data.assetcode + "' and cam.BRANCH_ID='" + data.branchid + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("assetcodelist")]
        public async Task<ActionResult<CAMS>> assetcodelist(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT ASSET_CODE FROM CAMS_ASSET_MASTER where ASSET_CODE like '%"+data.assetcode+"%'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("assetreconciliationnew")]
        public async Task<ActionResult<CAMS>> assetreconciliationnew(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "INSERT INTO CAMS_PHYSICAL_INVENTORY(FUNCTION_ID,BRANCH_ID,ASSET_ID,DEP_ID,CREATED_BY,MODE,CREATED_ON,UPDATED_ON) VALUES(" + data.functionidrec + "," + data.branchidu + "," + data.assetidrec + "," + data.deprtid + "," + data.assetuser + ",'A','" + data.recrdte + "','" + data.recrdte + "')";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


        [HttpPost]
        [Route("reconassetdetail")]
        public async Task<ActionResult<CAMS>> reconassetdetail(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT cam.ImageUrl,cam.asset_latitude,cam.asset_longitude,bumm.TUM_USER_NAME,casm.SUB_CATEGORY_DESC,cam.TYPE,cam.ASSET_ID,cam.ASSET_CODE,cam.ASSET_DESCRIPTION,cam.ASSET_VALUE,cam.ASSET_BRAND,cam.ASSET_MODE,cam.ASSET_PURCHASE_DATE,cam.ASSET_WARRANTY_TILL,cam.CAMS_ASSET_MANUFACTURER,cam.ASSET_RESIDUAL_VALUE,cam.ASSET_DEPRECIATION_TYPE,cam.ASSET_DEPRECIATION_PERCENTAGE,cam.ASSET_REMARKS,cam.ASSET_APPREQ_USERINITIATED,cam.ASSET_COUNTER_ENABLED,cam.ASSET_INSTALLATION_DATE,cam.ASSET_INSTALLED_BY,cam.ASSET_CERTIFICATE_ISSUED,cam.ASSET_WORKING_CONDITION,bum.TUM_USER_CODE,bop.TEXT as cnme,bop.VAL as cval FROM CAMS_ASSET_MASTER as cam INNER JOIN BO_PARAMETER as bop ON bop.VAL=cam.ASSET_CATEGORY LEFT OUTER JOIN BO_USER_MASTER as bum on bum.TUM_USER_ID=cam.ASSET_USER LEFT OUTER JOIN CAMS_ASSET_SUBCATEGORY_MASTER casm on casm.SUB_CATEGORY_ID=cam.ASSET_TYPE LEFT OUTER JOIN BO_USER_MASTER as bumm on bumm.TUM_USER_ID=cam.ASSET_OWNER_ID WHERE bop.FUNCTION_ID=1 and  bop.TYPE='InfCategory' and cam.ASSET_CODE='" + data.assetcodeu + "' and cam.BRANCH_ID='" + data.branchidu + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        [HttpPost]
        [Route("assetreq")]
        public async Task<ActionResult<CAMS>> assetreq(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT cam.ASSET_ID,cam.ASSET_USER,bp.val as dpid,cam.ASSET_CODE,bp.Text,cam.ASSET_DESCRIPTION FROM CAMS_ASSET_MASTER as cam inner join BO_PARAMETER as bp on bp.VAL=cam.ASSET_DEPARTMENT  where cam.ASSET_CODE='" + data.assetcodeu + "' and cam.BRANCH_ID='" + data.branchidu + "' and bp.TYPE='BO_TEAM' and bp.function_id='1' and bp.status='A'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

       



        [HttpPost]
        [Route("assetservicevendorlist")]
        public async Task<ActionResult<CAMS>> assetservicevendorlist(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT * FROM ERP_VENDOR_MASTER erpm WHERE erpm.Vendor_Code='" + data.vendorcode + "' AND function_id='" + data.functionid + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


       


        [HttpPost]
        [Route("assetservicelist")]
        public async Task<ActionResult<CAMS>> assetservicelist(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
               // string Logdata1 = string.Empty;
               // string sql = "CAMS_GETCAMSServiceDetails";
                string sql = "MBL_CAMS_GETCAMSServiceDetails";//"CAMS_PENDINGDETAIL_SEARCHS1";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                SqlDataAdapter apd = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@loginUserId", loginUserId);
                cmd.Parameters.AddWithValue("@FUNCTION_ID", data.functionid);
                cmd.Parameters.AddWithValue("@BRANCH_ID", data.branchid);
                cmd.Parameters.AddWithValue("@ASSET_ID", "");
                cmd.Parameters.AddWithValue("@ASSET_CODE", data.assetcode);

                cmd.Parameters.AddWithValue("@MODE", "S");

                cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    DataRow row = results.Rows[i];
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                }
                return Ok(Logdata1);


            }
        }


        [HttpPost]
        [Route("assettransfertobranch")]
        public async Task<ActionResult<CAMS>> assettransfertobranch(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "(SELECT BRANCH_ID,BRANCH_DESC from BO_BRANCH_MASTER WHERE FUNCTION_ID=" + data.functionidrep + ") order by BRANCH_DESC asc";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("assettransferfrombranch")]
        public async Task<ActionResult<CAMS>> assettransferfrombranch(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "(SELECT cam.ASSET_ID,cam.ASSET_DEPARTMENT,cam.ASSET_OWNER_ID,cam.ASSET_CATEGORY,bom.BRANCH_ID,bom.BRANCH_DESC from BO_BRANCH_MASTER as bom INNER JOIN CAMS_ASSET_MASTER as cam on cam.BRANCH_ID=bom.BRANCH_ID WHERE cam.FUNCTION_ID=" + data.functionidrep + " AND cam.ASSET_CODE='" + data.fassetcode + "')";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        [HttpPost]
        [Route("assettransferupdatebranch")]
        public async Task<ActionResult<CAMS>> assettransferupdatebranch(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "INSERT INTO CAMS_ASSET_TRANSFER_MASTER (FUNCTION_ID,CAT_ASSET_ID,CAT_FROM_BRANCH_ID,CAT_TO_BRANCH_ID,CAT_FROM_DEPARTMENT_ID,CAT_TO_DEPARTMENT_ID,CAT_FROM_ASSET_OWNER_ID,CAT_TO_ASSET_OWNER_ID,CREATED_ON,UPDATED_ON,CREATED_BY,STATUS,CAT_CATEGORY_ID,Asset_Transfer_type,Total_Assets,transfertype)VALUES('" + data.functionidrep + "','" + data.assetid + "','" + data.oldbranchid + "','" + data.ubranchid + "','" + data.assetdepart + "','0','" + data.assetownerid + "','0','" + data.dateins + "','" + data.dateins + "','" + data.createbytf + "','P','" + data.assetcategory + "','M',1,'2');select Scope_Identity()";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("assetlocationzone")]
        public async Task<ActionResult<CAMS>> assetlocationzone(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT DISTINCT ZONE_ID,ZONE_DESC from BO_ZONE_MASTER WHERE FUNCTION_ID=" + data.functionidrep + " and ZONE_STATUS='A'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


        [HttpPost]
        [Route("assetlocationarea")]
        public async Task<ActionResult<CAMS>> assetlocationarea(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select VAL, TEXT from BO_PARAMETER where type='BO_DEPTLOCATION' and BO_PARAMETER.FUNCTION_ID=" + data.functionidrep + "";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        [HttpPost]
        [Route("assetlocationregion")]
        public async Task<ActionResult<CAMS>> assetlocationregion(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT region_id,region_desc from BO_REGION_MASTER WHERE Status='A' and FUNCTION_ID=" + data.functionidrep + " AND zone_id=" + data.zoneid + "";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


        [HttpPost]
        [Route("assetlocationdeparment")]
        public async Task<ActionResult<CAMS>> assetlocationdeparment(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select VAL, TEXT from BO_PARAMETER where type='BO_TEAM' and BO_PARAMETER.FUNCTION_ID=" + data.functionidrep + " and Status='A'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        [HttpPost]
        [Route("assetlocationbranch")]
        public async Task<ActionResult<CAMS>> assetlocationbranch(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT BRANCH_ID,BRANCH_DESC from BO_BRANCH_MASTER WHERE STATUS='A' and FUNCTION_ID=" + data.functionidrep + " AND ZONE_ID=" +data.zoneid + " AND region_id=" + data.regionid + "";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }




        [HttpPost]
        [Route("assetloctionfilterfinal")]
        public async Task<ActionResult<CAMS>> assetloctionfilterfinal(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                var pzoneid = data.fzoneid;
                var pregionid = data.fregionid;
                var pbranchid = data.fbranchid;
                var passetcatid = data.fassetcatid;
                var passetsubcatid = data.fassetsubcatid;
                var flag = 0;

                dbConn.Open();
                string query = "";
                query = "SELECT CAMS_ASSET_MASTER.FUNCTION_ID,BO_ZONE_MASTER.ZONE_ID,BO_ZONE_MASTER.ZONE_DESC AS 'Zone',BO_REGION_MASTER.region_id,BO_REGION_MASTER.region_desc as 'Region',CAMS_ASSET_MASTER.BRANCH_ID,BO_BRANCH_MASTER.BRANCH_DESC as 'Branch',CAMS_ASSET_MASTER.ASSET_CATEGORY,CAMS_ASSET_MASTER.ASSET_TYPE,BO_PARAMETER.TEXT as 'Category',CAMS_ASSET_SUBCATEGORY_MASTER.SUB_CATEGORY_DESC as 'SubCategore',CAMS_ASSET_SUBCATEGORY_MASTER.SUB_CATEGORY_ID,COUNT(CAMS_ASSET_MASTER.ASSET_ID) as Qty, CAMS_ASSET_MASTER.STATUS as 'Status' FROM CAMS_ASSET_MASTER WITH(NOLOCK) inner join BO_FUNCTION_MASTER with(nolock) on BO_FUNCTION_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID inner join BO_PARAMETER with(nolock) on BO_PARAMETER.TYPE='INFCATEGORY' and BO_PARAMETER.VAL=CAMS_ASSET_MASTER.ASSET_CATEGORY and BO_PARAMETER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID and BO_PARAMETER.STATUS='A' inner join CAMS_ASSET_SUBCATEGORY_MASTER with(nolock) on CAMS_ASSET_SUBCATEGORY_MASTER.CATEGORY_ID=CAMS_ASSET_MASTER.ASSET_CATEGORY and CAMS_ASSET_SUBCATEGORY_MASTER.SUB_CATEGORY_ID=CAMS_ASSET_MASTER.ASSET_TYPE and CAMS_ASSET_SUBCATEGORY_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID INNER JOIN   BO_BRANCH_MASTER WITH (NOLOCK) ON BO_BRANCH_MASTER.BRANCH_ID=CAMS_ASSET_MASTER.BRANCH_ID   AND BO_BRANCH_MASTER.STATUS='A'   AND   BO_BRANCH_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID       INNER JOIN BO_ZONE_MASTER WITH (NOLOCK) ON BO_ZONE_MASTER.ZONE_ID=BO_BRANCH_MASTER.ZONE_ID AND BO_ZONE_MASTER.ZONE_STATUS='A'   AND   BO_ZONE_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID INNER JOIN BO_REGION_MASTER WITH(NOLOCK) ON BO_FUNCTION_MASTER.FUNCTION_ID=BO_REGION_MASTER.function_id AND BO_ZONE_MASTER.ZONE_ID=BO_REGION_MASTER.zone_id AND BO_BRANCH_MASTER.region_id=BO_REGION_MASTER.region_id  where 1=1 AND CAMS_ASSET_MASTER.FUNCTION_ID=" + data.functionidrep + "";


                
               
                if (pzoneid != null && pzoneid!=0)
                {
                    if (flag == 0)
                    {
                         query = query + "  AND BO_ZONE_MASTER.ZONE_ID=" + data.fzoneid + " ";
                         flag = 1;
                    }
                    else
                    {
                         query = query + "  AND BO_ZONE_MASTER.ZONE_ID=" + data.fzoneid + " ";
                    }
                }
                if (pregionid != null && pregionid!=0)
                {
                    if (flag == 0)
                    {
                         query = query + "  AND BO_REGION_MASTER.region_id=" + data.fregionid + " ";
                         flag = 1;
                    }
                    else
                    {
                         query = query + " AND BO_REGION_MASTER.region_id=" + data.fregionid + " ";
                    }
                }
                if (pbranchid != null && pbranchid!=0)
                {
                    if (flag == 0)
                    {
                         query = query + "  AND CAMS_ASSET_MASTER.BRANCH_ID=" + data.fbranchid + " ";
                         flag = 1;
                    }
                    else
                    {
                         query = query + " AND CAMS_ASSET_MASTER.BRANCH_ID=" + data.fbranchid + " ";
                    }
                }
                if (passetcatid != null && passetcatid!=0)
                {
                    if (flag == 0)
                    {
                         query = query + "  AND CAMS_ASSET_MASTER.ASSET_CATEGORY=" + data.fassetcatid + " ";
                         flag = 1;
                    }
                    else
                    {
                         query = query + " AND CAMS_ASSET_MASTER.ASSET_CATEGORY=" + data.fassetcatid + " ";
                    }
                }
                if (passetsubcatid != null && passetsubcatid!=0)
                {
                    if (flag == 0)
                    {
                         query = query + "  AND CAMS_ASSET_MASTER.ASSET_TYPE=" + data.fassetsubcatid + " ";
                         flag = 1;
                    }
                    else
                    {
                         query = query + " AND CAMS_ASSET_MASTER.ASSET_TYPE=" + data.fassetsubcatid + " ";
                    }
                }
           
      
                query = query + " group by  CAMS_ASSET_MASTER.FUNCTION_ID,BO_ZONE_MASTER.ZONE_DESC ,BO_REGION_MASTER.region_desc ,BO_BRANCH_MASTER.BRANCH_DESC,BO_PARAMETER.TEXT,CAMS_ASSET_SUBCATEGORY_MASTER.SUB_CATEGORY_DESC ,CAMS_ASSET_SUBCATEGORY_MASTER.SUB_CATEGORY_ID,CAMS_ASSET_MASTER.ASSET_MODE,BO_ZONE_MASTER.ZONE_ID,BO_REGION_MASTER.region_id,CAMS_ASSET_MASTER.BRANCH_ID,CAMS_ASSET_MASTER.ASSET_CATEGORY,CAMS_ASSET_MASTER.ASSET_TYPE,CAMS_ASSET_MASTER.STATUS ";
        // console.log(fserinslistf);





                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }




        [HttpPost]
        [Route("assettransferfrombranchnew")]
        public async Task<ActionResult<CAMS>> assettransferfrombranchnew(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            System.Data.DataTable results2 = new System.Data.DataTable();
            string Logdata1 = string.Empty;
            int ASSET_ID;
            int CAT_HISTORY_ID;
            //var logdata = "";
            //var strtoken = "";
            //// var result = "";
            //int ASSET_USER;
            //int ASSET_CATEGORY;

            //int BRANCH_ID;
            //int FUNCTION_ID;
            var JSONString = new StringBuilder();
            string output = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select CAMS_ASSET_MASTER.ASSET_CATEGORY,CAMS_ASSET_MASTER.FUNCTION_ID,BRANCH_ID,ASSET_CODE,ASSET_ID,ASSET_SERIAL_NO,ASSET_OWNER_ID,isnull(BO_USER_MASTER.TUM_USER_CODE,'')  as'Assetownercode',ASSET_DEPARTMENT,'' as TO_DEPARTMENT_ID,''as Remarks,CAMS_PARAMETER_BRANCH.TEXT as departmentdescription,ASSET_DESCRIPTION from CAMS_ASSET_MASTER left outer join BO_USER_MASTER with(nolock)on BO_USER_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID and BO_USER_MASTER.TUM_BRANCH_ID=CAMS_ASSET_MASTER.BRANCH_ID and BO_USER_MASTER.TUM_USER_ID=CAMS_ASSET_MASTER.ASSET_OWNER_ID inner join CAMS_PARAMETER_BRANCH with(nolock)on CAMS_PARAMETER_BRANCH.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID AND  CAMS_PARAMETER_BRANCH.VAL=CAMS_ASSET_MASTER.ASSET_DEPARTMENT and CAMS_PARAMETER_BRANCH.TYPE='bo_team' where CAMS_ASSET_MASTER.STATUS='A' and CAMS_ASSET_MASTER.ASSET_CODE='" + data.fassetcode + "' and CAMS_ASSET_MASTER.BRANCH_ID='" + data.branch + "' ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);

                for (int i = 0; i < results.Rows.Count; i++)
                {

                    CAMS log = new CAMS();
                    DataRow row = results.Rows[i];
                    //ASSET_USER = Convert.ToInt32(row[0]);
                    // ASSET_CATEGORY = Convert.ToInt32(row[1]);
                    ASSET_ID = Convert.ToInt32(row[4]);
                    // BRANCH_ID = Convert.ToInt32(row[2]);
                    // FUNCTION_ID = Convert.ToInt32(row[3]);


                    string query1 = "";
                    query1 = "SELECT  TOP(1) CAT_HISTORY_ID FROM CAMS_ASSET_TRANSFER_Detail WHERE CAT_ASSET_ID='" + ASSET_ID + "' ORDER BY CAT_HISTORY_ID DESC";
                    SqlCommand cmd1 = new SqlCommand(query1, dbConn);
                    var reader1 = cmd1.ExecuteReader();
                    System.Data.DataTable results1 = new System.Data.DataTable();
                    results1.Load(reader1);
                    if (results1.Rows.Count > 0)
                    {


                        for (int i1 = 0; i < results1.Rows.Count; i++)
                        {
                            DataRow row1 = results1.Rows[i];
                            CAT_HISTORY_ID = Convert.ToInt32(row1[0]);
                            string query2 = "";
                            query2 = "SELECT  *  FROM CAMS_ASSET_TRANSFER_MASTER WHERE CAT_HISTORY_ID='" + CAT_HISTORY_ID + "' and STATUS IN('A','P') ";

                            SqlCommand cmd2 = new SqlCommand(query2, dbConn);
                            var reader2 = cmd2.ExecuteReader();

                            results2.Load(reader2);


                        }




                        if (results2.Rows.Count >= 1)
                        {

                            Logdata1 = DataTableToJSONWithStringBuilder(results2);
                            dbConn.Close();

                        }
                        else
                        {
                            string st = "This Asset Already Transferred";

                            //JSONString.Append("{");
                            //JSONString.Append("\"" + st + "\"");
                            //JSONString.Append("}");

                            Logdata1 = st;
                          
                            dbConn.Close();
                        }
                    }

                    else
                    {
                        Logdata1 = DataTableToJSONWithStringBuilder(results);

                    }
                }
                //Logdata1 = DataTableToJSONWithStringBuilder(results2);
                //dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);
            }


        }



        [HttpPost]
        [Route("assettransferupdatebranchnew")]
        public async Task<ActionResult<CAMS>> assettransferupdatebranchnew(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            System.Data.DataTable results2 = new System.Data.DataTable();
            System.Data.DataTable results3 = new System.Data.DataTable();
            string Logdata1 = string.Empty;
            int ASSET_ID;
            int CAT_HISTORY_ID;
            int lasttransfericeidvv;
            int WF_CONFIG_ID;
            var JSONString = new StringBuilder();


            string output = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "INSERT INTO CAMS_ASSET_TRANSFER_MASTER (FUNCTION_ID,CAT_ASSET_ID,CAT_FROM_BRANCH_ID,CAT_TO_BRANCH_ID,CAT_FROM_DEPARTMENT_ID,CAT_TO_DEPARTMENT_ID,CAT_FROM_ASSET_OWNER_ID,CAT_TO_ASSET_OWNER_ID,CREATED_ON,UPDATED_ON,CREATED_BY,STATUS,CAT_CATEGORY_ID,Asset_Transfer_type,Total_Assets,transfertype)VALUES('" + data.functionidrep + "','" + data.assetid + "','" + data.oldbranchid + "','" + data.oldbranchid + "','" + data.assetdepart + "','0','" + data.assetownerid + "','0','" + data.dateins + "','" + data.dateins + "','" + data.createbytf + "','P','" + data.assetcategory + "','M',1,'1');select Scope_Identity() ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);

                for (int i = 0; i < results.Rows.Count; i++)
                {

                    CAMS log = new CAMS();
                    DataRow row = results.Rows[i];

                    lasttransfericeidvv = Convert.ToInt32(row[0]);


                    string query1 = "";
                    query1 = "INSERT INTO CAMS_ASSET_TRANSFER_Detail (FUNCTION_ID,CAT_ASSET_ID,CAT_FROM_BRANCH_ID,CAT_TO_BRANCH_ID,CAT_FROM_DEPARTMENT_ID,CAT_TO_DEPARTMENT_ID,CAT_FROM_ASSET_OWNER_ID,CAT_TO_ASSET_OWNER_ID,CREATED_ON,UPDATED_ON,CREATED_BY,STATUS,CAT_CATEGORY_ID,CAT_HISTORY_ID,Remarks,transfertype)VALUES ('" + data.functionidrep + "','" + data.assetid + "','" + data.oldbranchid + "','" + data.oldbranchid + "','" + data.assetdepart + "','0','" + data.assetownerid + "','0','" + data.dateins + "','" + data.dateins + "','" + data.assetownerid + "','P','0','" + lasttransfericeidvv + "' ,'" + data.remarks + "','2' )";
                    SqlCommand cmd1 = new SqlCommand(query1, dbConn);
                    var reader1 = cmd1.ExecuteReader();
                    System.Data.DataTable results1 = new System.Data.DataTable();
                    results1.Load(reader1);

                    string query2 = "";
                    query2 = "select WF_CONFIG_ID from BO_WORKFLOW_CONFIGURATIONS where table_name like '%CAMS_ASSET_TRANSFER_MASTER%' and status='A' and Function_ID='1' ";
                    SqlCommand cmd2 = new SqlCommand(query2, dbConn);
                    var reader2 = cmd2.ExecuteReader();
                  
                    results2.Load(reader2);

                    for (int i2 = 0; i < results2.Rows.Count; i++)
                    {
                        DataRow row1 = results2.Rows[i];
                        WF_CONFIG_ID = Convert.ToInt32(row1[0]);

                        string query3 = "";
                        query3 = "exec usp_WF_ApprovalUsers 'CAMS_ASSET_TRANSFER_MASTER','CAT_HISTORY_ID','0','0','0','','" + lasttransfericeidvv + "','','' ,'','' ,'1' ,'" + data.createbytf + "' ,'STATUS','P' ,'" + WF_CONFIG_ID + "' ";
                        SqlCommand cmd3 = new SqlCommand(query3, dbConn);
                        var reader3 = cmd3.ExecuteReader();

                        results3.Load(reader3);
                
                    }
     

                    }


                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);
            }

        }






        [HttpPost]
        [Route("assetreconciliationrep")]
        public async Task<ActionResult<CAMS>> assetreconciliationrep(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var JSONString = new StringBuilder();

            var functionidrep = data.functionidrep;
            var rfdate = data.rfdate;
            var rtdate = data.rtdate;
            var rassetcode = data.rassetcode;
            var rstatus = data.rstatus;
            var rbranch = data.rbranch;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "set dateformat DMY;SELECT distinct cams_PI_Reconciliation_Report.Asset_Code,cams_PI_Reconciliation_Report.Asset_description,cams_PI_Reconciliation_Report.Branch,cams_PI_Reconciliation_Report.Department,cams_PI_Reconciliation_Report.Last_PI as 'ScanDate',cams_PI_Reconciliation_Report.status as 'Status'  FROM cams_PI_Reconciliation_Report inner join CAMS_ASSET_MASTER with(nolock)on CAMS_ASSET_MASTER.ASSET_ID=cams_PI_Reconciliation_Report.ASSETID";

                var flag = 0;
                if (rfdate != "" && rtdate != "")
                {
                    if (flag == 0)
                    {
                       
                      query=query+ " AND cast(cams_PI_Reconciliation_Report.Last_PI as date) BETWEEN CAST('" + data.rfdate + "' AS DATETIME2) and CAST('" + data.rtdate + "' AS DATETIME2) ";
                        
                         flag = 1;
                    }
                    else
                    {
                        //varquery=query+ " AND CAMS_PHYSICAL_INVENTORY.UPDATED_ON BETWEEN Cast('" + data.rfdate + " 00:00:00.000' as datetime) and Cast('" + data.rtdate + " 00:00:00.000' as datetime)";
                      query=query+ " AND cast(cams_PI_Reconciliation_Report.Last_PI as date) BETWEEN '" + data.rfdate + "' and '" + data.rtdate + "'";
                        // console.log(reconrep);
                    }
                }

                if (rassetcode != "")
                {
                    if (flag == 0)
                    {
                      query=query+ " AND cams_PI_Reconciliation_Report.Asset_Code='" + data.rassetcode + "'";
                         flag = 1;
                    }
                    else
                    {
                      query=query+ "AND cams_PI_Reconciliation_Report.Asset_Code='" + data.rassetcode + "'";
                    }
                }

                query = query + "order by ScanDate desc";



                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }





        [HttpPost]
        [Route("assetdepatmentwiselocation")]
        public async Task<ActionResult<CAMS>> assetdepatmentwiselocation(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var JSONString = new StringBuilder();
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var pzoneid = data.fzoneid;
            var pregionid = data.fregionid;
            var pbranchid = data.fbranchid;
           
            var depatid = data.depatmentid;
            var locatid = data.locationidd;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select DISTINCT  BO_FUNCTION_MASTER.FUNCTION_DESC as 'Function', BRANCH_DESC Branch ,BO_PARAMETER1.TEXT Department,BO_PARAMETER.TEXT Location, count (Asset_id) AssetCount   from CAMS_ASSET_MASTER  Join BO_PARAMETER on BO_PARAMETER.TYPE ='BO_DEPTLOCATION'  and CAMS_ASSET_MASTER.FUNCTION_ID =BO_PARAMETER.FUNCTION_ID and BO_PARAMETER.VAL=ASSET_LocationId Join Bo_branch_master on Bo_branch_master.FUNCTION_ID=CAMS_ASSET_MASTER.Function_id and CAMS_ASSET_MASTER.BRANCH_ID=Bo_branch_master.BRANCH_ID left Join BO_PARAMETER as BO_PARAMETER1 on BO_PARAMETER1.TYPE='BO_TEAM' and BO_PARAMETER1.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID and CAMS_ASSET_MASTER.BRANCH_ID=Bo_branch_master.BRANCH_ID and BO_PARAMETER1.val=CAMS_ASSET_MASTER.ASSET_DEPARTMENT and BO_PARAMETER1.STATUS='A' inner join BO_FUNCTION_MASTER on BO_FUNCTION_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID INNER JOIN BO_ZONE_MASTER ON BO_ZONE_MASTER.ZONE_ID=BO_BRANCH_MASTER.ZONE_ID AND ZONE_STATUS='A'   AND   BO_ZONE_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.function_id where  CAMS_ASSET_MASTER.STATUS='A'";

                var flag = 0;
                if (pzoneid != null && pzoneid !=0)
                {
                    if (flag == 0)
                    {
                        query = query + " AND BO_ZONE_MASTER.ZONE_ID=" + data.fzoneid + " ";
                         flag = 1;
                    }
                    else
                    {
                        query = query + " AND BO_ZONE_MASTER.ZONE_ID=" + data.fzoneid + " ";
                    }
                }
                if (pbranchid != null && pbranchid != 0)
                {
                    if (flag == 0)
                    {
                        query = query + " AND Bo_branch_master.BRANCH_ID=" + data.fbranchid + " ";
                         flag = 1;
                    }
                    else
                    {
                        query = query + " AND Bo_branch_master.BRANCH_ID=" + data.fbranchid + "  ";
                    }
                }
                if (depatid != null && depatid != 0  )
                {
                    if (flag == 0)
                    {
                        query = query + " AND ASSET_DEPARTMENT=" + data.depatmentid + " ";
                         flag = 1;
                    }
                    else
                    {
                        query = query + " AND ASSET_DEPARTMENT=" + data.depatmentid + "  ";
                    }
                }
                if (locatid != null && locatid != 0 )
                {
                    if (flag == 0)
                    {
                        query = query + " AND ASSET_LocationId=" + data.locationidd + " ";
                         flag = 1;
                    }
                    else
                    {
                        query = query + " AND ASSET_LocationId=" + data.locationidd + "  ";
                    }
                }
                query = query + " Group by BO_FUNCTION_MASTER.FUNCTION_DESC , BRANCH_DESC ,BO_PARAMETER1.TEXT ,BO_PARAMETER.TEXT ";
             



                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpGet]
        [Route("Pendingsearchs11")]
        public string Pendingsearchs11(string strfunction, string branch, string fdate, string tdate, string Status, string strUserId, string UserType, string drpcategory, string drptype, string TASKTYPE, string AssetCode)//, string PrDesc, string PrCode string loginUserId,, string UserType
        {
            try
            {
                using (SqlConnection dbConn = new SqlConnection(strconn))
               {
                    var JSONString = new StringBuilder();
                    dbConn.Open();

                    if (strfunction.ToString() == "0" || strfunction.ToString() == "" || strfunction.ToString() == string.Empty || strfunction.ToString() == null)
                    {
                        strfunction = "0";
                    }
                    if (strUserId.ToString() == "0" || strUserId.ToString() == "" || strUserId.ToString() == string.Empty || strUserId.ToString() == null)
                    {
                        strUserId = "0";
                    }
                    if (branch.ToString() == "0" || branch.ToString() == "" || branch.ToString() == string.Empty || branch.ToString() == null)
                    {
                        branch = "0";
                    }
                    if (fdate.ToString() == "0" || fdate.ToString() == "" || fdate.ToString() == string.Empty || fdate.ToString() == "null")
                    {
                        fdate = null;
                    }
                    if (tdate.ToString() == "0" || tdate.ToString() == "" || tdate.ToString() == string.Empty || tdate.ToString() == "null")
                    {
                        tdate = null;
                    }
                    if (Status.ToString() == "0" || Status.ToString() == "" || Status.ToString() == string.Empty || Status.ToString() == "null")
                    {
                        Status = null;
                    }
                    if (drpcategory.ToString() == "0" || drpcategory.ToString() == "" || drpcategory.ToString() == string.Empty || drpcategory.ToString() == "null")
                    {
                        drpcategory = null;
                    }
                    if (drptype.ToString() == "0" || drptype.ToString() == "" || drptype.ToString() == string.Empty || drptype.ToString() == "null")
                    {
                        drptype =null;
                    }
                    if (TASKTYPE.ToString() == "0" || TASKTYPE.ToString() == "" || TASKTYPE.ToString() == string.Empty || TASKTYPE.ToString() == "null")
                    {
                        TASKTYPE = null;
                    }
                    if (AssetCode.ToString() == "0" || AssetCode.ToString() == "" || AssetCode.ToString() == string.Empty || AssetCode.ToString() == "null")
                    {
                        AssetCode = null;
                    }

                    string Logdata1 = string.Empty;
                //string sql = "CAMS__SEARCHS_PENDINGDETAIL";
                    string sql = "MBL_CAMS__SEARCHS_PENDINGDETAIL";//"CAMS_PENDINGDETAIL_SEARCHS1";
                    SqlCommand cmd = new SqlCommand(sql, dbConn);
                SqlDataAdapter apd = new SqlDataAdapter(cmd);
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@loginUserId", loginUserId);
                cmd.Parameters.AddWithValue("@strfunction", strfunction);
                cmd.Parameters.AddWithValue("@branch", branch);
                cmd.Parameters.AddWithValue("@fdate", fdate);
                cmd.Parameters.AddWithValue("@tdate", tdate);
               
                cmd.Parameters.AddWithValue("@Status", Status);
               
                cmd.Parameters.AddWithValue("@strUserId", strUserId);
                cmd.Parameters.AddWithValue("@UserType", UserType);
                cmd.Parameters.AddWithValue("@drpcategory", drpcategory);
                cmd.Parameters.AddWithValue("@drptype", drptype);

                cmd.Parameters.AddWithValue("@TASKTYPE", TASKTYPE);
               
                cmd.Parameters.AddWithValue("@AssetCode", AssetCode);
                  //  cmd.ExecuteNonQuery();

                    var reader = cmd.ExecuteReader();
                    System.Data.DataTable results = new System.Data.DataTable();
                    results.Load(reader);
                    //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                    //if (results.Rows.Count == 0)
                    //{
                    //    string st = "No data found";
                    //    JSONString.Append("{");
                    //    JSONString.Append("\"" + st + "\"");
                    //    JSONString.Append("}");

                    //    Logdata1 = JSONString.ToString();
                    //}
                    //else
                    //{
                        for (int i = 0; i < results.Rows.Count; i++)
                        {
                            DataRow row = results.Rows[i];
                            Logdata1 = DataTableToJSONWithStringBuilder(results);
                        }
                    //}
                    return Logdata1;



                }
            }
            catch (Exception ex)
            {

                var json = new JavaScriptSerializer().Serialize(ex.Message);
                return json;
            }
        }





        [HttpGet]
        [Route("CAMSPENDING_COMPLTED_SEARCH")]
        public string CAMSPENDING_COMPLTED_SEARCH(string strfunction, string branch, string fdate, string tdate, string Status, string drpcategory, string drptype, string TASKTYPE)//, string PrDesc, string PrCode string loginUserId,, string UserType
        {
            try
            {
                using (SqlConnection dbConn = new SqlConnection(strconn))
                {
                    var JSONString = new StringBuilder();
                    dbConn.Open();

                    if (strfunction.ToString() == "0" || strfunction.ToString() == "" || strfunction.ToString() == string.Empty || strfunction.ToString() == null)
                    {
                        strfunction = "0";
                    }
                    if (branch.ToString() == "0" || branch.ToString() == "" || branch.ToString() == string.Empty || branch.ToString() == null)
                    {
                        branch = "0";
                    }
                    if (fdate.ToString() == "0" || fdate.ToString() == "" || fdate.ToString() == string.Empty || fdate.ToString() == "null")
                    {
                        fdate =null;
                    }
                    if (tdate.ToString() == "0" || tdate.ToString() == "" || tdate.ToString() == string.Empty || tdate.ToString() == "null")
                    {
                        tdate = null;
                    }
                    if (Status.ToString() == "0" || Status.ToString() == "" || Status.ToString() == string.Empty || Status.ToString() == "null")
                    {
                        Status = null;
                    }
                    if (drpcategory.ToString() == "0" || drpcategory.ToString() == "" || drpcategory.ToString() == string.Empty || drpcategory.ToString() == "null")
                    {
                        drpcategory = "0";
                    }
                    if (drptype.ToString() == "0" || drptype.ToString() == "" || drptype.ToString() == string.Empty || drptype.ToString() == "null")
                    {
                        drptype = "0";
                    }
                    if (TASKTYPE.ToString() == "0" || TASKTYPE.ToString() == "" || TASKTYPE.ToString() == string.Empty || TASKTYPE.ToString() == "null")
                    {
                        TASKTYPE = "0";
                    }

                    string Logdata1 = string.Empty;
                    //string sql = "CAMS_PENDINGDETAIL_COMPLETED_SEARCH";
                    string sql = "MBL_CAMS_PENDINGDETAIL_COMPLETED_SEARCH";//"CAMS_PENDINGDETAIL_SEARCHS1";
                    SqlCommand cmd = new SqlCommand(sql, dbConn);
                    SqlDataAdapter apd = new SqlDataAdapter(cmd);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // cmd.Parameters.AddWithValue("@loginUserId", loginUserId);
                    cmd.Parameters.AddWithValue("@strfunction", strfunction);
                    cmd.Parameters.AddWithValue("@branch", branch);
                    cmd.Parameters.AddWithValue("@fdate", fdate);
                    cmd.Parameters.AddWithValue("@tdate", tdate);

                    cmd.Parameters.AddWithValue("@Status", Status);

                  //  cmd.Parameters.AddWithValue("@strUserId", strUserId);
                  //  cmd.Parameters.AddWithValue("@UserType", UserType);
                    cmd.Parameters.AddWithValue("@drpcategory", drpcategory);
                    cmd.Parameters.AddWithValue("@drptype", drptype);

                    cmd.Parameters.AddWithValue("@TASKTYPE", TASKTYPE);

                    // cmd.Parameters.AddWithValue("@AssetCode", AssetCode);
                    cmd.ExecuteNonQuery();

                    var reader = cmd.ExecuteReader();
                    System.Data.DataTable results = new System.Data.DataTable();
                    results.Load(reader);
                    //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                    //if (results.Rows.Count == 0)
                    //{

                    //    string st = "No data found";
                    //    JSONString.Append("{");
                    //    JSONString.Append("\"" + st + "\"");
                    //    JSONString.Append("}");

                    //    Logdata1 = JSONString.ToString();
                    //}
                    //else
                    //{
                        for (int i = 0; i < results.Rows.Count; i++)
                        {
                            DataRow row = results.Rows[i];
                            Logdata1 = DataTableToJSONWithStringBuilder(results);
                        }
                    //}
                   
                    return Logdata1;


                }
            }
            catch (Exception ex)
            {

                var json = new JavaScriptSerializer().Serialize(ex.Message);
                return json;
            }
        }






        [HttpPost]
        [Route("assetlocationcategory")]
        public async Task<ActionResult<CAMS>> assetlocationcategory(CAMS data)
        {
            // string struser = data.user_lower;
            var JSONString = new StringBuilder();
            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT  FUNCTION_ID, VAL, TEXT  FROM  BO_PARAMETER with(nolock) where 1=1  and BO_PARAMETER.FUNCTION_ID=" + data.functionidrep + " and TYPE='InfCategory' and status='A'  ORDER BY TEXT asc ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{

                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("assetlocationsubcategory")]
        public async Task<ActionResult<CAMS>> assetlocationsubcategory(CAMS data)
        {
            // string struser = data.user_lower;
            var JSONString = new StringBuilder();
            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT SUB_CATEGORY_ID,SUB_CATEGORY_DESC FROM CAMS_ASSET_SUBCATEGORY_MASTER WHERE FUNCTION_ID=" + data.functionidrep + " AND CATEGORY_ID=" + data.categoryid + " order by SUB_CATEGORY_DESC ASC";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{

                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("refrencemax")]
        public async Task<ActionResult<CAMS>> refrencemax(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "select cast(isnull(max(isnull(ASSET_REF_NO,0)),0)+1 as decimal) as refnum from CAMS_LAST_MAINTENANCE with (nolock) where 1=1";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }



        [HttpPost]
        [Route("assetreqcategory")]
        public async Task<ActionResult<CAMS>> assetreqcategory(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT ASSET_TYPE,ASSET_USER,ASSET_CATEGORY,ASSET_ID,BRANCH_ID,FUNCTION_ID FROM CAMS_ASSET_MASTER  where ASSET_CODE='" + data.assetcodeu + "' and BRANCH_ID='" + data.branchidu + "' ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                System.Data.DataTable results1 = new System.Data.DataTable();
                results.Load(reader);
                for (int i = 0; i < results.Rows.Count; i++)
                {

                    CAMS log = new CAMS();
                    DataRow row = results.Rows[i];
                   int ASSET_TYPE = Convert.ToInt32(row[0]);
                   int ASSET_CATEGORY = Convert.ToInt32(row[2]);
                  
                    string query1 = "";
                    query1 = "SELECT cur.ISSUEID,cur.ISSUEDESCRIPTION FROM  CAMS_USERREQUEST_REASON as cur where CATEGORY_ID=" + ASSET_CATEGORY + " and SUB_CATEGORY_ID=" + ASSET_TYPE + " and FUNCTION_ID=1";
                    SqlCommand cmd1 = new SqlCommand(query1, dbConn);
                    var reader1 = cmd1.ExecuteReader();
                   
                    results1.Load(reader1);
                }

                //if (results1.Rows.Count==0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results1);
                //}


                   
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


        [HttpPost]
        [Route("assetreqinsert")]
        public async Task<ActionResult<CAMS>> assetreqinsert(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int ASSET_USER;
            int ASSET_CATEGORY;
            int ASSET_ID;
            int BRANCH_ID;
            int FUNCTION_ID;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT ASSET_USER,ASSET_CATEGORY,ASSET_ID,BRANCH_ID,FUNCTION_ID FROM CAMS_ASSET_MASTER  where ASSET_CODE='" + data.assetcoder + "' and BRANCH_ID='" + data.branchidr + "'  ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);

                for (int i = 0; i < results.Rows.Count; i++)
                {

                    CAMS log = new CAMS();
                    DataRow row = results.Rows[i];
                     ASSET_USER = Convert.ToInt32(row[0]);
                    ASSET_CATEGORY = Convert.ToInt32(row[1]);
                    ASSET_ID= Convert.ToInt32(row[2]);
                    BRANCH_ID = Convert.ToInt32(row[2]);
                    FUNCTION_ID= Convert.ToInt32(row[3]);

                    string query1 = "";
                    query1 = "INSERT INTO CAMS_LAST_MAINTENANCE(BRANCH_ID,FUNCTION_ID,ASSET_ASSET_ID,   ASSET_STATUS,  ASSET_FREQUENCY_MODE,  ASSET_ACTIVITY_DESC,  priority,ASSET_ACTIVITY_ID,ASSET_NEXT_MAINTENANCE,ASSET_REF_NO,CREATED_ON) VALUES (" + BRANCH_ID + "," + FUNCTION_ID + ",'" + ASSET_ID + "', 'P',  'U',  '" + data.reqdetail + "',  " + data.priority + ",0,'" + data.reqdate1 + "','" + data.refmaxno + "'," + data.reqdate + ")";
                    SqlCommand cmd1 = new SqlCommand(query1, dbConn);
                    var reader1 = cmd1.ExecuteReader();
                    System.Data.DataTable results1 = new System.Data.DataTable();
                    results1.Load(reader1);
                   
                }

                string query2 = "";
                query2= "select (prefix + '' + serial_no  + '' + isnull(suffix,'')) as wkno from BO_slno_parameter where type='URWorkOrderNumber' and slno_domain='1' ";

                SqlCommand cmd2 = new SqlCommand(query2, dbConn);
                var reader2 = cmd2.ExecuteReader();
                System.Data.DataTable results2 = new System.Data.DataTable();
                results2.Load(reader2);
                for (int i = 0; i < results2.Rows.Count; i++)
                {

                    CAMS log = new CAMS();
                    DataRow row = results2.Rows[i];
                    int wkno = Convert.ToInt32(row[0]);
                    for (int i1 = 0; i1 < results.Rows.Count; i1++)
                    {


                        DataRow row1 = results.Rows[i];
                        ASSET_USER = Convert.ToInt32(row1[0]);
                        ASSET_CATEGORY = Convert.ToInt32(row1[1]);
                        ASSET_ID = Convert.ToInt32(row1[2]);
                        BRANCH_ID = Convert.ToInt32(row1[2]);
                        FUNCTION_ID = Convert.ToInt32(row1[3]);



                        string query3 = "";
                        query3 = "INSERT INTO CAMS_ASSET_REQUEST(BRANCH_ID,FUNCTION_ID,ASSET_ID,ASSET_ACTIVITY_ID,ASSET_DETAILS,ASSET_DUE_DATE,ASSET_STATUS,ASSET_USER_ID,ASSET_PMR_REFERENCE,ASSET_CATEGORY,ASSET_PM_TYPE,ASSET_REQUESTED_BY,ASSET_DURATION,ASSET_WORKORDNO) VALUES (" + BRANCH_ID + "," + FUNCTION_ID + ",'" + ASSET_ID + "',0,'" + data.reqdetail + "','" + data.reqdate1 + "','A'," + ASSET_USER + ",'" + data.refmaxno + "'," + ASSET_CATEGORY + ",'U','" + data.assetreqid + "','00:00','" + wkno + "')";
                        SqlCommand cmd3 = new SqlCommand(query3, dbConn);
                        var reader3 = cmd3.ExecuteReader();



                        string query4 = "";
                        query4 = "Update BO_SLNO_PARAMETER set serial_no = serial_no + 1 where type = N'URWorkOrderNumber'";
                        SqlCommand cmd4 = new SqlCommand(query4, dbConn);
                        var reader4 = cmd4.ExecuteReader();


                        string query5 = "";
                        query5 = "INSERT INTO CAMS_TASKS_ASSIGNED(FUNCTION_ID, CAMS_USER_ID, created_on) VALUES(" + FUNCTION_ID + ", " + ASSET_USER + ", '" + data.reqdate + "')";
                        SqlCommand cmd5 = new SqlCommand(query5, dbConn);
                        var reader5 = cmd5.ExecuteReader();


                    }
                }


                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                    dbConn.Close();

                    var result = (new { recordsets = Logdata1 });
                    return Ok(Logdata1);
                }

            
        }


       // sep30
        [HttpPost]
        [Route("assetserviceinsert")]
        public async Task<ActionResult<CAMS>> assetserviceinsert(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int lastserviceid = 0;
            int WF_CONFIG_ID;
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SET DATEFORMAT DMY;INSERT INTO CAMS_ASSET_SERVICE(FUNCTION_ID,BRANCH_ID,ASSET_CODE,ASSET_ID,VENDOR_ID,VENDOR_CODE,DATE_OF_SERVICE,EXPECTED_DATE_OF_DELIVERY,SERVICE_DESCRIPTION,EXPECTED_EXPENSES,RELEASE,CREATED_ON,CREATED_BY,STATUS,INSURANCECOMPANY,AMOUNTINSURED,WARRANTYDATE,REPLACEMENT_TYPE,REPLACEMENT_ASSET_ID,SERVICE_CATEGORY,MODE,REPL_TILL_DATE) VALUES(" + data.serfunctionid + "," + data.serbranchid + ",'" + data.serassetcode + "'," + data.serassetid + "," + data.servendorid + ",'" + data.servendorcode + "','" + data.serdateofservice + "','" + data.serexpdateofdelivery + "','" + data.serdescription + "'," + data.serexpexpense + ",'P','" + data.sercreatedon + "'," + data.sercreatedby + ",'P','" + data.serinsucompany + "'," + data.seramountinsu + ",'" + data.serwarrantydte + "','" + data.serreplacetype + "'," + data.serreplaceassetid + "," + data.servicecategory + ",'S','" + data.srvtilldate + "');SELECT SCOPE_IDENTITY() AS id ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);

               

                    string query1 = "";
                    query1 = "update CAMS_ASSET_MASTER set status='S' where ASSET_ID=" + data.serassetid + "";
                    SqlCommand cmd1 = new SqlCommand(query1, dbConn);
                    var reader1 = cmd1.ExecuteReader();
                    System.Data.DataTable results1 = new System.Data.DataTable();
                    results1.Load(reader1);

             

                string query2 = "";
                query2 = "select WF_CONFIG_ID from BO_WORKFLOW_CONFIGURATIONS where table_name like '%CAMS_ASSET_SERVICE%' and status='A' and Function_ID='" + data.serfunctionid + "'";

                SqlCommand cmd2 = new SqlCommand(query2, dbConn);
                var reader2 = cmd2.ExecuteReader();
                System.Data.DataTable results2 = new System.Data.DataTable();
                results2.Load(reader2);
                for (int i = 0; i < results2.Rows.Count; i++)
                {

                  
                    DataRow row = results2.Rows[i];
                    int wkno = Convert.ToInt32(row[0]);
                   
                    WF_CONFIG_ID = Convert.ToInt32(row[0]);
                      



                        string query3 = "";
                        query3 = "exec usp_WF_ApprovalUsers 'CAMS_ASSET_SERVICE','HISTORY_ID','0','0','0','','" + lastserviceid + "','','' ,'','' ,'1' ,'" + data.assetreqby + "' ,'STATUS','P' ,'" + WF_CONFIG_ID + "'";
                        SqlCommand cmd3 = new SqlCommand(query3, dbConn);
                        var reader3 = cmd3.ExecuteReader();

                        System.Data.DataTable results3 = new System.Data.DataTable();
                        results3.Load(reader3);
                    //if (results.Rows.Count == 0)
                    //{
                    //    string st = "No data found";
                    //    JSONString.Append("{");
                    //    JSONString.Append("\"" + st + "\"");
                    //    JSONString.Append("}");

                    //    Logdata1 = JSONString.ToString();
                    //}
                    //else
                    //{
                        for (int i2 = 0; i2 < results3.Rows.Count; i2++)
                        {
                            Logdata1 = DataTableToJSONWithStringBuilder(results3);
                            dbConn.Close();
                        }
                    //}
                   
                }
              
            }

            var result = (new { recordsets = Logdata1 });
            return Ok(Logdata1);
        }





        [HttpPost]
        [Route("assetservicecategory")]
        public async Task<ActionResult<CAMS>> assetservicecategory(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT DISTINCT VAL AS VALUE,TEXT AS TEXT,TYPE FROM BO_PARAMETER WITH (NOLOCK) WHERE STATUS = 'A' AND TYPE like 'ServiceCategory%' AND FUNCTION_ID='" + data.functionid + "' ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        //dropdown datas POST method

        [HttpPost]
        [Route("assetcodereplace")]
        public async Task<ActionResult<CAMS>> assetcodereplace(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT ASSET_CODE FROM CAMS_ASSET_MASTER where ASSET_USER IS NULL";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

       
        [HttpPost]
        [Route("vendorcodelist")]
        public async Task<ActionResult<CAMS>> vendorcodelist(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "SELECT Vendor_Code,vendor_id,Vendor_Name FROM ERP_VENDOR_MASTER erpm WHERE  function_id='1' and Vendor_Code like '%"+ data.vendorcode +"%' ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                Logdata1 = DataTableToJSONWithStringBuilder(results);
                //if (results.Rows.Count == 0)
                //{
                //    //string st = "No data found";

                //    //Logdata1 = new JavaScriptSerializer().Serialize(st);

                //    string st = "No data found";
                //    JSONString.Append("{");
                //    JSONString.Append("\"" + st + "\"");
                //    JSONString.Append("}");

                //    Logdata1 = JSONString.ToString();
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }




        //06oct

        [HttpPost]
        [Route("assetdepatmentwiselocationmodal")]
        public async Task<ActionResult<CAMS>> assetdepatmentwiselocationmodal(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int depid = 0;
            int locid = 0;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();

                string query2 = "";
                query2 = "select VAL from  BO_PARAMETER where TEXT ='" + data.depidm + "'  and FUNCTION_ID=1 and TYPE='bo_Team'";

                SqlCommand cmd2 = new SqlCommand(query2, dbConn);
                var reader2 = cmd2.ExecuteReader();
                System.Data.DataTable results2 = new System.Data.DataTable();
                results2.Load(reader2);
                for (int i = 0; i < results2.Rows.Count; i++)
                {


                    DataRow row = results2.Rows[i];

                    depid = Convert.ToInt32(row[0]);
                }


                string query3 = "";
                query3 = "select VAL from  BO_PARAMETER where TEXT ='" + data.locidm + "'  and FUNCTION_ID=1 and TYPE='BO_DEPTLOCATION'";

                SqlCommand cmd3 = new SqlCommand(query3, dbConn);
                var reader3 = cmd3.ExecuteReader();
                System.Data.DataTable results3 = new System.Data.DataTable();
                results3.Load(reader3);
                for (int i = 0; i < results3.Rows.Count; i++)
                {


                    DataRow row = results3.Rows[i];

                    locid = Convert.ToInt32(row[0]);
                }


                
                string query = "";
                query = "Select BRANCH_DESC Branch,a.TEXT Department, BO_PARAMETER.TEXT Location,ASSET_CODE AssetCode,b.TEXT catgry,SUB_CATEGORY_DESC subcatry from CAMS_ASSET_MASTER Join Bo_branch_master on Bo_branch_master.FUNCTION_ID=CAMS_ASSET_MASTER.Function_id and CAMS_ASSET_MASTER.BRANCH_ID=Bo_branch_master.BRANCH_ID Join BO_PARAMETER on BO_PARAMETER.TYPE ='BO_DEPTLOCATION'  and CAMS_ASSET_MASTER.FUNCTION_ID =BO_PARAMETER.FUNCTION_ID and BO_PARAMETER.VAL=CAMS_ASSET_MASTER.ASSET_LocationId inner join BO_PARAMETER a with(nolock) on a.VAL=CAMS_ASSET_MASTER.ASSET_DEPARTMENT and a.TYPE='bo_Team' and a.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID inner join BO_PARAMETER b with(nolock) on b.TYPE='INFCATEGORY' and b.VAL=CAMS_ASSET_MASTER.ASSET_CATEGORY and b.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID inner join CAMS_ASSET_SUBCATEGORY_MASTER with(nolock) on CAMS_ASSET_SUBCATEGORY_MASTER.CATEGORY_ID=CAMS_ASSET_MASTER.ASSET_CATEGORY and CAMS_ASSET_SUBCATEGORY_MASTER.SUB_CATEGORY_ID=CAMS_ASSET_MASTER.ASSET_TYPE and CAMS_ASSET_SUBCATEGORY_MASTER.FUNCTION_ID=CAMS_ASSET_MASTER.FUNCTION_ID AND  CAMS_ASSET_SUBCATEGORY_MASTER.STATUS='A' where a.VAL =" + depid + " and BO_PARAMETER.VAL=" + locid + " ";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count==0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}
           
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }


        //new
        [HttpGet]
        [Route("CAMS_PENDING_REOPENED/{ASSETACTIVITYID}/{ASSET_ID}/{ASSETREFNO}/{STRFUNCTIONID}/{ASSETPMRREFERENCE}/{remarks}/{date}/{userid}/{branchid}/{workorderno}")]
        public string CAMS_PENDING_REOPENED(string ASSETACTIVITYID, string ASSET_ID, string ASSETREFNO, string STRFUNCTIONID, string ASSETPMRREFERENCE, string remarks, string date, string userid, string branchid, string workorderno)
        {
            string Logdata1 = string.Empty;
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {

                dbConn.Open();
                string sql = "MBL_CAMSMOBIAPI_CAMS_PENDING_REOPENED";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ASSETACTIVITYID", ASSETACTIVITYID);
                cmd.Parameters.AddWithValue("@ASSET_ID", ASSET_ID);
                cmd.Parameters.AddWithValue("@ASSETREFNO", ASSETREFNO);
                cmd.Parameters.AddWithValue("@STRFUNCTIONID", STRFUNCTIONID);
                cmd.Parameters.AddWithValue("@ASSETPMRREFERENCE", ASSETPMRREFERENCE);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@branchid", branchid);
                cmd.Parameters.AddWithValue("@workorderno", workorderno);
                cmd.Parameters.Add("@Result", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // string st = cmd.Parameters["@Result"].Value.ToString();
                Logdata1 = "Successfully Reopend";
                JSONString.Append("\"" + Logdata1 + "\"");
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();

                //if (results.Rows.Count==0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    for (int i = 0; i < results.Rows.Count; i++)
                    {
                        DataRow row = results.Rows[i];
                        Logdata1 = DataTableToJSONWithStringBuilder(results);
                    }
              //  }
                        
               
                return JSONString.ToString();
            }
        }



        [HttpPost]
        [Route("compltedjonclose")]
        public async Task<ActionResult<CAMS>> compltedjonclose(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int depid = 0;
            int locid = 0;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();

                string query2 = "";
                query2 = "update  CAMS_ASSET_REQUEST  set ASSET_STATUS='O',asset_start_status='O' where ASSET_ID='" + data.assetid + "' and ASSET_ACTIVITY_ID='" + data.activityid + "'";

                SqlCommand cmd2 = new SqlCommand(query2, dbConn);
                var reader2 = cmd2.ExecuteReader();
                System.Data.DataTable results2 = new System.Data.DataTable();
                results2.Load(reader2);
               
                string query3 = "";
                query3 = "update CAMS_LAST_MAINTENANCE set ASSET_STATUS='O',UPDATED_ON=" + data.todaydte + " where  ASSET_ASSET_ID='" + data.assetid + "' and ASSET_ACTIVITY_ID='" + data.activityid + "'";

                SqlCommand cmd3 = new SqlCommand(query3, dbConn);
                var reader3 = cmd3.ExecuteReader();
                System.Data.DataTable results3 = new System.Data.DataTable();
                results3.Load(reader3);


                string query4 = "";
                query4 = "SET DATEFORMAT DMY;insert into CAMS_TASKS_ASSIGNED(CAMS_REF_NO,CAMS_USER_ID,Created_by,created_on,lst_updated_by,lst_updated_on,FUNCTION_ID) values('" + data.refmaxno + "','0','" + data.createdby + "'," + data.todaydte + ",'" + data.createdby + "'," + data.todaydte + ",'" + data.functionidrep + "')";

                SqlCommand cmd4 = new SqlCommand(query4, dbConn);
                var reader4 = cmd4.ExecuteReader();
                System.Data.DataTable results4 = new System.Data.DataTable();
                results4.Load(reader4);

                string query5 = "";
                query5 = "SET DATEFORMAT DMY;insert into CAMS_LAST_MAINTENANCE (FUNCTION_ID,BRANCH_ID,ASSET_ASSET_ID,ASSET_ACTIVITY_ID,ASSET_FREQUENCY_MODE,ASSET_FREQUENCY,ASSET_STATUS,CREATED_ON,UPDATED_ON,ASSET_REF_NO,ASSET_LAST_MAINTENANCE_TYPE,ASSET_TESTED_BY,ASSET_CERTIFICATE_ISSUED,ASSET_CERTIFICATE_DETAILS,ASSET_NEXT_MAINTENANCE) values(" + data.functionidrep + "," + data.branchid + ",'" + data.assetid + "','" + data.activityid + "','T','3','A'," + data.todaydte + "," + data.todaydte + ",'" + data.createdby + "','','',' ','','')";

                SqlCommand cmd5 = new SqlCommand(query5, dbConn);
                var reader5 = cmd5.ExecuteReader();
                System.Data.DataTable results5 = new System.Data.DataTable();
                results5.Load(reader5);


                string query6 = "";
                query6 = "SET DATEFORMAT DMY;INSERT INTO CAMS_MAINTENANCE_MANPOWER_ALLOCATION(FUNCTION_ID,BRANCH_ID,ASSET_ID,ASSET_ACTIVITY_ID,ASSET_PM_REFERENCE,ASSET_USER_TYPEID,ASSET_EMP_ID,ASSET_ACTUAL_HRS,WORKING_DATE,ASSET_STATUS,ipaddress,CREATED_ON,UPDATED_ON,CREATED_BY,UPDATED_BY,Assign_Status) VALUES('" + data.functionidrep + "','" + data.branchid + "','" + data.assetid + "','" + data.activityid + "','" + data.pmrefre + "','" + data.typeid + "','3','" + data.actualhours + "'," + data.todaydte + ",'A','1'," + data.todaydte + "," + data.todaydte + ",'" + data.createdby + "','" + data.createdby + "','A')";

                SqlCommand cmd6 = new SqlCommand(query6, dbConn);
                var reader6 = cmd6.ExecuteReader();
                System.Data.DataTable results6 = new System.Data.DataTable();
                results6.Load(reader6);



                string query = "";
                query = "SELECT ASSET_CODE FROM CAMS_ASSET_MASTER";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count==0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
              //  }
               
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }




        [HttpPost]
        [Route("updatecamsstatus")]
        public async Task<ActionResult<HRMS>> updatecamsstatus(CAMS data)
        {
            // string struser = data.user_lower;

            List<HRMS> Logdata = new List<HRMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            string activityid = "";
            var JSONString = new StringBuilder();
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                string functionid = data.functionid.ToString();
                string branchid = data.branchid.ToString();
                string assetid = data.assetid.ToString();
                if (data.activityid.ToString()=="" || data.activityid.ToString() == "null")
                {
                    activityid  = "";
                }
                else
                {
                     activityid = data.activityid.ToString();
                }
                     string ref1 = data.ref1.ToString();
                string duedate = data.duedate.ToString();
                dbConn.Open();
                string sql = "MBL_CAMS_PENDING_UPDATESTARTDATE";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FUNCTION_ID", functionid);
                cmd.Parameters.AddWithValue("@BRANCH_ID", branchid);
                cmd.Parameters.AddWithValue("@ASSET_ID", assetid);
                cmd.Parameters.AddWithValue("@ASSET_ACTIVITY_ID", activityid);
                cmd.Parameters.AddWithValue("@ASSET_REFERENCE",ref1);
                cmd.Parameters.AddWithValue("@ASSET_DUE_DATE",duedate);
                                cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
              //  Logdata1 = "Updated successfully";
                string st = "Started successfully";
                string output = JsonConvert.SerializeObject(st);
                Logdata1 = output;
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                //for (int i = 0; i < results.Rows.Count; i++)
                //{
                //    DataRow row = results.Rows[i];
                //    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}

                JSONString.Append("{");
                JSONString.Append("\"" + output + "\"");
                JSONString.Append("}");




                return Ok(output);




            }
        }


        [HttpGet]
        [Route("CAMS_PENDING_COMPLETED/{ASSETCOMPLETIONDATE}/{ASSETPLANNEDHRS}/{ASSETACTUALHRS}/{ASSETTESTEDBY}/{ASSETBREAKDOWNREASON}/{ASSETBREAKDOWNREMEDY}/{ASSETBREAKDOWNCOST}/{ASSETSHUTDOWNREQUEST}/{ASSETSHUTDOWNREMEDY}/{ASSETSHUTDOWNCOST}/{ASSETSHUTDOWNSTARTDATE}/{ASSETSHUTDOWN_ENDDATE}/{ASSETTASKSCARRIEDOUT}/{ASSETDONENY}/{ASSETPERMITNO}/{ASSETCOST}/{ASSETPMRREFERENCE}/{ASSETID}/{ASSETACTIVITYID}/{ASSETCERTIFICATEISSUED}/{ASSETCERTIFICATEDETAILS}/{status}")]
        public string CAMS_PENDING_COMPLETED(string ASSETCOMPLETIONDATE, string ASSETPLANNEDHRS, string ASSETACTUALHRS, string ASSETTESTEDBY, string ASSETBREAKDOWNREASON, string ASSETBREAKDOWNREMEDY, string ASSETBREAKDOWNCOST, string ASSETSHUTDOWNREQUEST, string ASSETSHUTDOWNREMEDY, string ASSETSHUTDOWNCOST,
       string ASSETSHUTDOWNSTARTDATE, string ASSETSHUTDOWN_ENDDATE, string ASSETTASKSCARRIEDOUT, string ASSETDONENY, string ASSETPERMITNO, string ASSETCOST, string ASSETPMRREFERENCE, string ASSETID, string ASSETACTIVITYID, string ASSETCERTIFICATEISSUED, string ASSETCERTIFICATEDETAILS, string status)
        {
            string Logdata1 = string.Empty;

            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();

                string sql = "CAMSMOBIAPI_CAMS_PENDING_COMPLETED";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ASSETCOMPLETIONDATE", ASSETCOMPLETIONDATE);
                cmd.Parameters.AddWithValue("@ASSETPLANNEDHRS", ASSETPLANNEDHRS);
                cmd.Parameters.AddWithValue("@ASSETACTUALHRS", ASSETACTUALHRS);
                cmd.Parameters.AddWithValue("@ASSETTESTEDBY", ASSETTESTEDBY);
                cmd.Parameters.AddWithValue("@ASSETBREAKDOWNREASON", ASSETBREAKDOWNREASON);
                cmd.Parameters.AddWithValue("@ASSETBREAKDOWNREMEDY", ASSETBREAKDOWNREMEDY);
                cmd.Parameters.AddWithValue("@ASSETBREAKDOWNCOST", ASSETBREAKDOWNCOST);
                cmd.Parameters.AddWithValue("@ASSETSHUTDOWNREQUEST", ASSETSHUTDOWNREQUEST);
                cmd.Parameters.AddWithValue("@ASSETSHUTDOWNREMEDY", ASSETSHUTDOWNREMEDY);
                cmd.Parameters.AddWithValue("@ASSETSHUTDOWNCOST", ASSETSHUTDOWNCOST);
                cmd.Parameters.AddWithValue("@ASSETSHUTDOWNSTARTDATE", ASSETSHUTDOWNSTARTDATE);
                cmd.Parameters.AddWithValue("@ASSETSHUTDOWN_ENDDATE", ASSETSHUTDOWN_ENDDATE);
                cmd.Parameters.AddWithValue("@ASSETTASKSCARRIEDOUT", ASSETTASKSCARRIEDOUT);
                cmd.Parameters.AddWithValue("@ASSETDONENY", ASSETDONENY);
                cmd.Parameters.AddWithValue("@ASSETPERMITNO", ASSETPERMITNO);
                cmd.Parameters.AddWithValue("@ASSETCOST", ASSETCOST);
                cmd.Parameters.AddWithValue("@ASSETPMRREFERENCE", ASSETPMRREFERENCE);
                cmd.Parameters.AddWithValue("@ASSETID", ASSETID);
                cmd.Parameters.AddWithValue("@ASSETACTIVITYID", ASSETACTIVITYID);
                cmd.Parameters.AddWithValue("@ASSETCERTIFICATEISSUED", ASSETCERTIFICATEISSUED);
                cmd.Parameters.AddWithValue("@ASSETCERTIFICATEDETAILS", ASSETCERTIFICATEDETAILS);
                cmd.Parameters.AddWithValue("@status", status);

                cmd.Parameters.Add("@Result", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //string outputval = cmd.Parameters["@outputparam"].Value.ToString();
                //if (results.Rows.Count == 0)
                //{
                //   // Logdata1 = "Successfully Completed";

                //    string st = "Successfully Completed";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    for (int i = 0; i < results.Rows.Count; i++)
                    {
                        DataRow row = results.Rows[i];
                        Logdata1 = DataTableToJSONWithStringBuilder(results);
                    }
               // }

                return Logdata1;

            }
        }



        [HttpPost]
        [Route("pendingtaskdetail")]
        public async Task<ActionResult<HRMS>> pendingtaskdetail(CAMS data)
        {
            // string struser = data.user_lower;

            List<HRMS> Logdata = new List<HRMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            string activityid = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                string functionid = data.functionid.ToString();
                string branchid = data.branchid.ToString();
                string assetid = data.assetid.ToString();
                if (data.activityid.ToString() == "")
                {
                    activityid = null;
                }
                else
                {
                    activityid = data.activityid.ToString();
                }

                dbConn.Open();
                string sql = "MBL_CAMS_JC_GETTASK";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ASSET_ACTIVITY_ID", activityid);
                cmd.Parameters.AddWithValue("@FUNCTION", functionid);
                cmd.Parameters.AddWithValue("@BRANCH", branchid);
                cmd.Parameters.AddWithValue("@ASSET_ID", assetid);
                cmd.Parameters.AddWithValue("@ALPHANAME", "");
                cmd.Parameters.AddWithValue("@PAGEINDEX", "0");
                cmd.Parameters.AddWithValue("@PAGESIZE", "10");
                cmd.Parameters.AddWithValue("@SORTEXPRESSION", "points_to_be_checked");
                 cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //   // Logdata1 = "No Records Found";
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    for (int i = 0; i < results.Rows.Count; i++)
                    {
                        DataRow row = results.Rows[i];
                        Logdata1 = DataTableToJSONWithStringBuilder(results);
                    }
               // }
                return Ok(Logdata1);

            }
        }




        [HttpPost]
        [Route("reopencomments")]
        public async Task<ActionResult<CAMS>> reopencomments(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int depid = 0;
            int locid = 0;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "select * from cams_asset_reopen where FUNCTION_ID='" + data.functionid + "' AND BRANCH_ID='" + data.branchid + "' and ASSET_ID=" + data.assetid + " and ASSET_WORKORDNO='" + data.wkno + "' ORDER BY CREATED_ON DESC";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
               
            }
            return Ok(Logdata1);
        }


        [HttpPost]
        [Route("manpoweskilldtl")]
        public async Task<ActionResult<CAMS>> manpoweskilldtl(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int depid = 0;
            int locid = 0;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "SELECT distinct TYPE_ID AS VAL,DESCRIPTION AS TEXT FROM BO_USER_TYPE_MASTER WITH(NOLOCK) WHERE STATUS='A'  AND FUNCTION_ID = '" + data.functionid + "' order by text asc";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }



        [HttpPost]
        [Route("manpowerrefdtl")]
        public async Task<ActionResult<CAMS>> manpowerrefdtl(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int depid = 0;
            int locid = 0;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "SELECT  * FROM (select  TOP 20 ROW_NUMBER() OVER (ORDER BY tum_user_code) as ROW_NUM,* from (select *, ROW_NUMBER() over (order by tum_user_code desc) as gridviewcount from( select distinct  tum_user_id,tum_user_code,tum_user_name,tum_user_type,BO_USER_TYPE_MASTER.description,isnull(tum_user_emailid,'') as Email FROM BO_USER_MASTER with(nolock) inner join BO_USER_TYPE_MASTER on cast(BO_USER_TYPE_MASTER.TYPE_ID as nvarchar)=BO_USER_MASTER.TUM_USER_TYPE and BO_USER_MASTER.function_id=BO_USER_TYPE_MASTER.function_id WHERE 1=1 and   BO_USER_MASTER.FUNCTION_ID='" + data.functionid + "' and BO_USER_MASTER.TUM_BRANCH_ID='" + data.branchid + "' and BO_USER_MASTER.tum_user_type='" + data.usertype + "')gridTempTable) tblname  ORDER BY ROW_NUM) innerSelect WHERE ROW_NUM > 0";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }



        [HttpPost]
        [Route("manpoweralldatadetail")]
        public async Task<ActionResult<CAMS>> manpoweralldatadetail(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            int depid = 0;
            int locid = 0;
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "SELECT distinct TUM_USER_ID AS VAL,TUM_USER_CODE +'-' +TUM_USER_NAME AS TEXT ,TYPE_ID,BO_USER_TYPE_MASTER.DESCRIPTION AS SKILL_TEXT,CAMS_MPP_USED.ASSET_EMP_ID AS EMP_ID,convert(varchar(10),ASSET_ACTUAL_HRS,108) as actual_hrs,tum_user_name as emp_name, FTP_UPLOAD_FLAG as flag,rowuniqueid as rowuniqueid,ASSET_COST FROM CAMS_MPP_USED WITH(NOLOCK) INNER JOIN BO_USER_TYPE_MASTER WITH(NOLOCK) ON  BO_USER_TYPE_MASTER.FUNCTION_ID = CAMS_MPP_USED.FUNCTION_ID AND BO_USER_TYPE_MASTER.TYPE_ID =CAMS_MPP_USED.ASSET_SKILL_SET INNER JOIN bo_user_master WITH(NOLOCK) ON  bo_user_master.FUNCTION_ID = CAMS_MPP_USED.FUNCTION_ID AND bo_user_master.TUM_BRANCH_ID = CAMS_MPP_USED.BRANCH_ID   AND bo_user_master.TUM_USER_TYPE=convert(nvarchar,CAMS_MPP_USED.ASSET_SKILL_SET)  and bo_user_master.TUM_USER_ID=CAMS_MPP_USED.ASSET_EMP_ID  where 1=1 AND CAMS_MPP_USED.FUNCTION_ID = '" + data.functionid + "'  AND CAMS_MPP_USED.BRANCH_ID = '" + data.branchid + "'  AND CAMS_MPP_USED.asset_id = '" + data.assetid + "' and ASSET_ACTIVITY_ID='" + data.assetactivityid + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }



        [HttpPost]
        [Route("manpowerinsertapp")]
        public async Task<ActionResult<HRMS>> manpowerinsertapp(CAMS data)
        {
            // string struser = data.user_lower;
            var JSONString = new StringBuilder();

            string Logdata1 = string.Empty;
           // var lasttransfericeidvv = "";
            string strcost = string.Empty;
            System.Data.DataTable results = new System.Data.DataTable();

            string assetactivityid = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                string functionid = data.functionid.ToString();
                string branchid = data.branchid.ToString();
                string userskill = data.userskill.ToString();
                if (data.assetactivityid.ToString() == "")
                {
                    assetactivityid = null;
                }
                else
                {
                    assetactivityid = data.assetactivityid.ToString();
                }
                string assetid = data.assetid.ToString();
                string assetpmref = data.assetpmref.ToString();
                string assetempid = data.assetempid.ToString();
                string assethrs = data.assethrs.ToString();

                dbConn.Open();
                string sql = "MBL_CAMS_JC_GETUSERCOST";
                SqlCommand cmd = new SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USERTYPEID", userskill);
                cmd.Parameters.AddWithValue("@FUNCTION", functionid);
                cmd.Parameters.AddWithValue("@BRANCH", branchid);
                cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
               
                results.Load(reader);
                if (results.Rows.Count>0)
                {
                    strcost = results.Rows[0]["cost"].ToString();
                }
                else
                {
                    strcost = "";
                }
              
                if (strcost==""|| strcost==null || strcost.Length==0)
                {
                    // Logdata1 = "nocost";
                    string st = "nocost";
                    JSONString.Append("{");
                    JSONString.Append("\"" + st + "\"");
                    JSONString.Append("}");
                   

                    Logdata1 = JSONString.ToString();
                }
                else
                {
                   


                    string sql1 = "MBL_CAMS_INSERT_MPP_USED";
                    SqlCommand cmd1 = new SqlCommand(sql1, dbConn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@BRANCH_ID", branchid);
                    cmd1.Parameters.AddWithValue("@FUNCTION_ID", functionid);
                    cmd1.Parameters.AddWithValue("@ASSET_ID", assetid);
                    cmd1.Parameters.AddWithValue("@ASSET_ACTIVITY_ID", assetactivityid);
                    cmd1.Parameters.AddWithValue("@ASSET_PM_REFERENCE", assetpmref);
                    cmd1.Parameters.AddWithValue("@ASSET_SKILL_SET", userskill);
                    cmd1.Parameters.AddWithValue("@ASSET_EMP_ID", assetempid);
                    cmd1.Parameters.AddWithValue("@ASSET_ACTUAL_HRS", assethrs);
                    cmd1.Parameters.AddWithValue("@ASSET_STATUS", "A");
                    cmd1.Parameters.AddWithValue("@ASSET_COST", strcost);
                    cmd1.ExecuteNonQuery();

                    var reader1 = cmd1.ExecuteReader();
                    System.Data.DataTable results1 = new System.Data.DataTable();
                    results1.Load(reader1);


                    string query = "";
                    query = "SELECT distinct TUM_USER_ID AS VAL,TUM_USER_CODE +'-' +TUM_USER_NAME AS TEXT ,TYPE_ID,BO_USER_TYPE_MASTER.DESCRIPTION AS SKILL_TEXT,CAMS_MPP_USED.ASSET_EMP_ID AS EMP_ID,convert(varchar(10),ASSET_ACTUAL_HRS,108) as actual_hrs,tum_user_name as emp_name, FTP_UPLOAD_FLAG as flag,rowuniqueid as rowuniqueid,ASSET_COST FROM CAMS_MPP_USED WITH(NOLOCK) INNER JOIN BO_USER_TYPE_MASTER WITH(NOLOCK) ON  BO_USER_TYPE_MASTER.FUNCTION_ID = CAMS_MPP_USED.FUNCTION_ID AND BO_USER_TYPE_MASTER.TYPE_ID =CAMS_MPP_USED.ASSET_SKILL_SET INNER JOIN bo_user_master WITH(NOLOCK) ON  bo_user_master.FUNCTION_ID = CAMS_MPP_USED.FUNCTION_ID AND bo_user_master.TUM_BRANCH_ID = CAMS_MPP_USED.BRANCH_ID   AND bo_user_master.TUM_USER_TYPE=convert(nvarchar,CAMS_MPP_USED.ASSET_SKILL_SET)  and bo_user_master.TUM_USER_ID=CAMS_MPP_USED.ASSET_EMP_ID  where 1=1 AND CAMS_MPP_USED.FUNCTION_ID = '" + data.functionid + "'  AND CAMS_MPP_USED.BRANCH_ID = '" + data.branchid + "'  AND CAMS_MPP_USED.asset_id = '" + data.assetid + "' and ASSET_ACTIVITY_ID='" + data.assetactivityid + "'";

                    SqlCommand cmd2 = new SqlCommand(query, dbConn);
                    var reader2 = cmd2.ExecuteReader();
                    System.Data.DataTable results2 = new System.Data.DataTable();
                    results2.Load(reader2);
                    //if (results2.Rows.Count == 0)
                    //{
                    //    string st = "No data found";

                    //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                    //}
                    //else
                    //{
                        Logdata1 = DataTableToJSONWithStringBuilder(results);
                    //}

                    dbConn.Close();
                }
                return Ok(Logdata1);

            }
        }


        [HttpPost]
        [Route("sparealldatadetail")]
        public async Task<ActionResult<CAMS>> sparealldatadetail(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
          
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "SELECT distinct CAMS_SPARE_USED.rowuniqueid as rowuniqueid,ASSET_SPARE_SLNO as serialno,ASSET_SPARE_CODE as MaterialCode,CAMS_ITEM_MASTER.ITEM_DESCRIPTION as MaterialDescription,CAMS_ITEM_MASTER.ITEM_ID as MaterialID,  CAMS_SPARE_USED.ASSET_SPARE_QUANTITY as sparequantity,convert(numeric(18,2),ASSET_SPARECOST) as SPARECOST,convert(varchar(10),CAMS_SPARE_USED.CAMS_ASSET_INSTALLATION_DATE,103) as CAMS_ASSET_INSTALLATION_DATE  FROM CAMS_SPARE_USED WITH(NOLOCK)  Inner join CAMS_ITEM_MASTER WITH(NOLOCK)  on  CAMS_SPARE_USED.ASSET_SPARE_CODE=CAMS_ITEM_MASTER.ITEM_CODE and  CAMS_ITEM_MASTER.function_id=CAMS_SPARE_USED.function_id   where CAMS_SPARE_USED.ASSET_SPARE_FLAG = 'S' AND (CAMS_SPARE_USED.HISTORY_REF <> 'HR' OR CAMS_SPARE_USED.HISTORY_REF IS  NULL)  AND CAMS_SPARE_USED.FUNCTION_ID = '" + data.functionid + "' AND CAMS_SPARE_USED.BRANCH_ID = '" + data.branchid + "' and CAMS_SPARE_USED.ASSET_ACTIVITY_ID = '" + data.assetactivityid + "' and CAMS_SPARE_USED.ASSET_PMR_REFERENCE = '" + data.assetpmref + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }



        [HttpPost]
        [Route("spareitemdtl")]
        public async Task<ActionResult<CAMS>> spareitemdtl(CAMS data)
        {
           

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;

            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "select * from CAMS_ITEM_MASTER where function_id='" + data.functionid + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }

        [HttpPost]
        [Route("spareitemdtlval")]
        public async Task<ActionResult<CAMS>> spareitemdtlval(CAMS data)
        {


            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;

            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "select * from CAMS_ITEM_MASTER where function_id='" + data.functionid + "' and ITEM_CODE='" + data.itemid + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
                //}

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }



        [HttpPost]
        [Route("spareinsertapp")]
        public async Task<ActionResult<HRMS>> spareinsertapp(CAMS data)
        {
            // string struser = data.user_lower;


            string Logdata1 = string.Empty;
            // var lasttransfericeidvv = "";
            string strcost = string.Empty;
            string strITEM_COST = string.Empty;
            System.Data.DataTable results = new System.Data.DataTable();
            System.Data.DataTable results1 = new System.Data.DataTable();
            int itemcost = 0;

            string assetactivityid = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                string functionid = data.functionid.ToString();
                string branchid = data.branchid.ToString();
                string itemcode = data.itemcode.ToString();
                if (data.assetactivityid.ToString() == "")
                {
                    assetactivityid = null;
                }
                else
                {
                    assetactivityid = data.assetactivityid.ToString();
                }
                string assetid = data.assetid.ToString();
                string assetpmref = data.assetpmref.ToString();
               
                dbConn.Open();

                string query = "";
                query = "select count(*) as cosucount from CAMS_SPARE_USED with (nolock) where ASSET_ID = '" + data.assetid + "' and ASSET_SPARE_CODE='" + data.itemcode + "' and FUNCTION_ID='" + data.functionid + "' and BRANCH_ID='" + data.branchid + "'  AND ASSET_MSS_STATUS='A' and ASSET_SPARE_FLAG='F'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                results.Load(reader);
                results.Load(reader);

                strcost = results.Rows[0]["cosucount"].ToString();
                if (strcost != "0")
                {
                   // Logdata1 = "consume";


                    string st1 = "consume";

                    Logdata1 = new JavaScriptSerializer().Serialize(st1);
                }
                else
                {

                    string sql = "MBL_CAMS_JC_GETCITEMMASTER";
                    SqlCommand cmd1 = new SqlCommand(sql, dbConn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@ITEM_CODE", itemcode);
                    cmd1.Parameters.AddWithValue("@FUNCTION", functionid);
                    cmd1.Parameters.AddWithValue("@TYPE", "GETCOST");
                    cmd1.ExecuteNonQuery();
                    var reader1 = cmd1.ExecuteReader();
                    results1.Load(reader1);
                    if (results1.Rows.Count > 0)
                    {
                        strITEM_COST = results1.Rows[0]["ITEM_COST"].ToString();
                        itemcost = Convert.ToInt32(results1.Rows[0]["ITEM_COST"]);
                    }
                   
                    int spareqty = data.spareqty;
                    int hrscost = spareqty * itemcost;
                   
                    string query1 = "";
                    query1 = "insert into CAMS_SPARE_USED(FUNCTION_ID,BRANCH_ID,ASSET_ID,ASSET_SPARE_CODE,ASSET_SPARE_SLNO,ASSET_SPARE_QUANTITY,ASSET_SPARECOST,ASSET_MSS_STATUS,ASSET_ACTIVITY_ID,ASSET_PMR_REFERENCE,ASSET_SPARE_FLAG,CAMS_ASSET_INSTALLATION_DATE) values(" + data.functionid + "," + data.branchid + ",'" + data.assetid + "', '" + data.itemcode + "','" + data.slno + "','" + data.spareqty + "','" + hrscost + "','A','" + data.assetactivityid + "','" + data.assetpmref + "','S','" + data.instdte + "')";

                     SqlCommand cmd2 = new SqlCommand(query1, dbConn);
                        var reader2 = cmd2.ExecuteReader();
                        System.Data.DataTable results2 = new System.Data.DataTable();
                        results2.Load(reader2);


                    string query2 = "";
                    query2 = "SELECT distinct CAMS_SPARE_USED.rowuniqueid as rowuniqueid,ASSET_SPARE_SLNO as serialno,ASSET_SPARE_CODE as MaterialCode,CAMS_ITEM_MASTER.ITEM_DESCRIPTION as MaterialDescription,CAMS_ITEM_MASTER.ITEM_ID as MaterialID,  CAMS_SPARE_USED.ASSET_SPARE_QUANTITY as sparequantity,convert(numeric(18,2),ASSET_SPARECOST) as SPARECOST,convert(varchar(10),CAMS_SPARE_USED.CAMS_ASSET_INSTALLATION_DATE,103) as CAMS_ASSET_INSTALLATION_DATE  FROM CAMS_SPARE_USED WITH(NOLOCK)  Inner join CAMS_ITEM_MASTER WITH(NOLOCK)  on  CAMS_SPARE_USED.ASSET_SPARE_CODE=CAMS_ITEM_MASTER.ITEM_CODE and  CAMS_ITEM_MASTER.function_id=CAMS_SPARE_USED.function_id   where CAMS_SPARE_USED.ASSET_SPARE_FLAG = 'S' AND (CAMS_SPARE_USED.HISTORY_REF <> 'HR' OR CAMS_SPARE_USED.HISTORY_REF IS  NULL)  AND CAMS_SPARE_USED.FUNCTION_ID = '" + data.functionid + "' AND CAMS_SPARE_USED.BRANCH_ID = '" + data.branchid + "' and CAMS_SPARE_USED.ASSET_ACTIVITY_ID = '" + data.assetactivityid + "' and CAMS_SPARE_USED.ASSET_PMR_REFERENCE = '" + data.assetpmref + "'";

                    SqlCommand cmd3 = new SqlCommand(query2, dbConn);
                    var reader3 = cmd3.ExecuteReader();
                    System.Data.DataTable results3 = new System.Data.DataTable();
                    results3.Load(reader3);


                    //if (results3.Rows.Count == 0)
                    //    {

                    //    string st = "No data found";

                    //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                    //}
                    //    else
                    //    {
                            Logdata1 = DataTableToJSONWithStringBuilder(results3);
                      //  }

                        dbConn.Close();
                    }
                    return Ok(Logdata1);

                }
            }




        [HttpPost]
        [Route("spareupdateapp")]
        public async Task<ActionResult<HRMS>> spareupdateapp(CAMS data)
        {
            // string struser = data.user_lower;


            string Logdata1 = string.Empty;
            // var lasttransfericeidvv = "";
            string strcost = string.Empty;
            string strITEM_COST = string.Empty;
            System.Data.DataTable results = new System.Data.DataTable();
            System.Data.DataTable results1 = new System.Data.DataTable();
            int itemcost = 0;

            string assetactivityid = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                string functionid = data.functionid.ToString();
                string branchid = data.branchid.ToString();
                string itemcode = data.itemcode.ToString();
                if (data.assetactivityid.ToString() == "")
                {
                    assetactivityid = null;
                }
                else
                {
                    assetactivityid = data.assetactivityid.ToString();
                }
                string assetid = data.assetid.ToString();
                string assetpmref = data.assetpmref.ToString();

                dbConn.Open();

              

                    string sql = "MBL_CAMS_JC_GETCITEMMASTER";
                    SqlCommand cmd1 = new SqlCommand(sql, dbConn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@ITEM_CODE", itemcode);
                    cmd1.Parameters.AddWithValue("@FUNCTION", functionid);
                    cmd1.Parameters.AddWithValue("@TYPE", "GETCOST");
                    cmd1.ExecuteNonQuery();
                    var reader1 = cmd1.ExecuteReader();
                    results1.Load(reader1);
                    if (results1.Rows.Count > 0)
                    {
                        strITEM_COST = results1.Rows[0]["ITEM_COST"].ToString();
                        itemcost = Convert.ToInt32(results1.Rows[0]["ITEM_COST"]);
                    }

                    int spareqty = data.spareqty;
                    int hrscost = spareqty * itemcost;

                    string query1 = "";
                    query1 = "update CAMS_SPARE_USED set ASSET_SPARE_SLNO='" + data.slno + "', ASSET_SPARE_QUANTITY='" + data.spareqty + "',ASSET_SPARECOST='" + data.cost + "',CAMS_ASSET_INSTALLATION_DATE='" + data.instdte + "'  where ASSET_ID='" + data.assetid + "' and rowuniqueid='" + data.uniqueid + "' and function_id='" + data.functionid + "'";

                    SqlCommand cmd2 = new SqlCommand(query1, dbConn);
                    var reader2 = cmd2.ExecuteReader();
                    System.Data.DataTable results2 = new System.Data.DataTable();
                    results2.Load(reader2);


                    string query2 = "";
                    query2 = "SELECT distinct CAMS_SPARE_USED.rowuniqueid as rowuniqueid,ASSET_SPARE_SLNO as serialno,ASSET_SPARE_CODE as MaterialCode,CAMS_ITEM_MASTER.ITEM_DESCRIPTION as MaterialDescription,CAMS_ITEM_MASTER.ITEM_ID as MaterialID,  CAMS_SPARE_USED.ASSET_SPARE_QUANTITY as sparequantity,convert(numeric(18,2),ASSET_SPARECOST) as SPARECOST,convert(varchar(10),CAMS_SPARE_USED.CAMS_ASSET_INSTALLATION_DATE,103) as CAMS_ASSET_INSTALLATION_DATE  FROM CAMS_SPARE_USED WITH(NOLOCK)  Inner join CAMS_ITEM_MASTER WITH(NOLOCK)  on  CAMS_SPARE_USED.ASSET_SPARE_CODE=CAMS_ITEM_MASTER.ITEM_CODE and  CAMS_ITEM_MASTER.function_id=CAMS_SPARE_USED.function_id   where CAMS_SPARE_USED.ASSET_SPARE_FLAG = 'S' AND (CAMS_SPARE_USED.HISTORY_REF <> 'HR' OR CAMS_SPARE_USED.HISTORY_REF IS  NULL)  AND CAMS_SPARE_USED.FUNCTION_ID = '" + data.functionid + "' AND CAMS_SPARE_USED.BRANCH_ID = '" + data.branchid + "' and CAMS_SPARE_USED.ASSET_ACTIVITY_ID = '" + data.assetactivityid + "' and CAMS_SPARE_USED.ASSET_PMR_REFERENCE = '" + data.assetpmref + "'";

                    SqlCommand cmd3 = new SqlCommand(query2, dbConn);
                    var reader3 = cmd3.ExecuteReader();
                    System.Data.DataTable results3 = new System.Data.DataTable();
                    results3.Load(reader3);


                //    if (results3.Rows.Count == 0)
                //    {

                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //    else
                //    {
                        Logdata1 = DataTableToJSONWithStringBuilder(results3);
                 //   }

                    dbConn.Close();
                }
                return Ok(Logdata1);

            }



        [HttpPost]
        [Route("consumealldatadetail")]
        public async Task<ActionResult<CAMS>> consumealldatadetail(CAMS data)
        {


            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;

            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                dbConn.Open();
                string query = "";
                query = "select  distinct CAMS_SPARE_USED.ASSET_SPARE_CODE as spare_code,convert(numeric(18,2),ASSET_SPARECOST) as ASSET_SPARECOST,rowuniqueid as mmp_rowuniqueid1,CAMS_SPARE_USED.FTP_UPLOAD_FLAG as asm_flag,CAMS_ITEM_MASTER.ITEM_DESCRIPTION as itemdesc,CAMS_ITEM_MASTER.ITEM_ID as Materialid,CAMS_SPARE_USED.ASSET_SPARE_QUANTITY as asm_spare_qty,CAMS_SPARE_USED.ASSET_REPLACED_QTY as mss_replaced_qty,CAMS_SPARE_USED.ASSET_RETURNED_QTY as mss_returned_qty,convert(numeric(18,2),ASSET_SPARECOST) as SPARECOST,ASSET_SCRAP_QTY as mss_scrap_qty from CAMS_SPARE_USED WITH (NOLOCK)  inner join  CAMS_ITEM_MASTER  WITH (NOLOCK) on  CAMS_SPARE_USED.ASSET_SPARE_CODE=CAMS_ITEM_MASTER.item_code  where (CAMS_SPARE_USED.ASSET_SPARE_FLAG = 'C' or CAMS_SPARE_USED.ASSET_SPARE_FLAG='F') and CAMS_SPARE_USED.FUNCTION_ID=CAMS_ITEM_MASTER.function_id  and CAMS_SPARE_USED.FUNCTION_ID='" + data.functionid + "' and CAMS_SPARE_USED.BRANCH_ID='" + data.branchid + "' and  CAMS_SPARE_USED.ASSET_ID='" + data.assetid + "' and CAMS_SPARE_USED.ASSET_ACTIVITY_ID='" + data.assetactivityid + "'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{

                //    string st = "No data found";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }

                dbConn.Close();

                var result = (new { recordsets = Logdata1 });

            }
            return Ok(Logdata1);
        }



        [HttpPost]
        [Route("consumableinsertapp")]
        public async Task<ActionResult<HRMS>> consumableinsertapp(CAMS data)
        {
            // string struser = data.user_lower;


            string Logdata1 = string.Empty;
            // var lasttransfericeidvv = "";
            string strcost = string.Empty;
            string strITEM_COST = string.Empty;
            System.Data.DataTable results = new System.Data.DataTable();
            System.Data.DataTable results1 = new System.Data.DataTable();
            int itemcost = 0;

            string assetactivityid = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {
                string functionid = data.functionid.ToString();
                string branchid = data.branchid.ToString();
                string itemcode = data.itemcode.ToString();
                if (data.assetactivityid.ToString() == "")
                {
                    assetactivityid = null;
                }
                else
                {
                    assetactivityid = data.assetactivityid.ToString();
                }
                string assetid = data.assetid.ToString();
                string assetpmref = data.assetpmref.ToString();

                dbConn.Open();

                string query = "";
                query = "select count(*) as cosucount from CAMS_SPARE_USED with (nolock) where ASSET_ID = '" + data.assetid + "' and ASSET_SPARE_CODE='" + data.itemcode + "' and FUNCTION_ID='" + data.functionid + "' and BRANCH_ID='" + data.branchid + "'  AND ASSET_MSS_STATUS='A' and ASSET_SPARE_FLAG='S'";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                results.Load(reader);
                results.Load(reader);

                strcost = results.Rows[0]["cosucount"].ToString();
                if (strcost != "0")
                {
                    Logdata1 = "consume";
                }
                else
                {

                    string sql = "MBL_CAMS_JC_GETCITEMMASTER";
                    SqlCommand cmd1 = new SqlCommand(sql, dbConn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@ITEM_CODE", itemcode);
                    cmd1.Parameters.AddWithValue("@FUNCTION", functionid);
                    cmd1.Parameters.AddWithValue("@TYPE", "GETCOST");
                    cmd1.ExecuteNonQuery();
                    var reader1 = cmd1.ExecuteReader();
                    results1.Load(reader1);
                    if (results1.Rows.Count > 0)
                    {
                        strITEM_COST = results1.Rows[0]["ITEM_COST"].ToString();
                        itemcost = Convert.ToInt32(results1.Rows[0]["ITEM_COST"]);
                    }

                    int spareqty = data.spareqty;
                    int hrscost = spareqty * itemcost;

               

                    string sql1 = "MBL_CAMS_INSERT_SPARE_USED";
                    SqlCommand cmd2 = new SqlCommand(sql1, dbConn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@FUNCTION_ID", functionid);
                    cmd2.Parameters.AddWithValue("@BRANCH_ID", branchid);
                    cmd2.Parameters.AddWithValue("@ASSET_ID", assetid);
                    cmd2.Parameters.AddWithValue("@ASSET_ACTIVITY_ID",assetactivityid);
                    cmd2.Parameters.AddWithValue("@ASSET_PMR_REFERENCE", data.assetpmref.ToString());
                    cmd2.Parameters.AddWithValue("@ASSET_SPARE_CODE", data.itemcode.ToString());
                    cmd2.Parameters.AddWithValue("@ASSET_SPARE_SLNO","0");
                    cmd2.Parameters.AddWithValue("@ASSET_SPARE_QUANTITY", data.plannedqty.ToString());
                    cmd2.Parameters.AddWithValue("@ASSET_REPLACED_QTY", data.replaceqty.ToString());
                    cmd2.Parameters.AddWithValue("@ASSET_RETURNED_QTY", data.returnqty.ToString());
                    cmd2.Parameters.AddWithValue("@ASSET_SCRAP_QTY", data.scrapqty.ToString());
                    cmd2.Parameters.AddWithValue("@ASSET_MSS_STATUS","A");
                    cmd2.Parameters.AddWithValue("@ASSET_SPARECOST", hrscost);
                    cmd2.Parameters.AddWithValue("@ASSET_SPARE_FLAG","F");
                    cmd2.ExecuteNonQuery();
              
                    var reader2 = cmd2.ExecuteReader();
                    System.Data.DataTable results2 = new System.Data.DataTable();
                    results2.Load(reader2);


                    string query2 = "";
                    query2 = "select  distinct CAMS_SPARE_USED.ASSET_SPARE_CODE as spare_code,convert(numeric(18,2),ASSET_SPARECOST) as ASSET_SPARECOST,rowuniqueid as mmp_rowuniqueid1,CAMS_SPARE_USED.FTP_UPLOAD_FLAG as asm_flag,CAMS_ITEM_MASTER.ITEM_DESCRIPTION as itemdesc,CAMS_SPARE_USED.ASSET_SPARE_QUANTITY as asm_spare_qty,CAMS_SPARE_USED.ASSET_REPLACED_QTY as mss_replaced_qty,CAMS_SPARE_USED.ASSET_RETURNED_QTY as mss_returned_qty,convert(numeric(18,2),ASSET_SPARECOST) as SPARECOST,ASSET_SCRAP_QTY as mss_scrap_qty from CAMS_SPARE_USED WITH (NOLOCK)  inner join  CAMS_ITEM_MASTER  WITH (NOLOCK) on  CAMS_SPARE_USED.ASSET_SPARE_CODE=CAMS_ITEM_MASTER.item_code  where (CAMS_SPARE_USED.ASSET_SPARE_FLAG = 'C' or CAMS_SPARE_USED.ASSET_SPARE_FLAG='F') and CAMS_SPARE_USED.FUNCTION_ID=CAMS_ITEM_MASTER.function_id  and CAMS_SPARE_USED.FUNCTION_ID='" + data.functionid + "' and CAMS_SPARE_USED.BRANCH_ID='" + data.branchid + "' and  CAMS_SPARE_USED.ASSET_ID='" + data.assetid + "' and CAMS_SPARE_USED.ASSET_ACTIVITY_ID='"+ data.assetactivityid + "'";

                    SqlCommand cmd3 = new SqlCommand(query2, dbConn);
                    var reader3 = cmd3.ExecuteReader();
                    System.Data.DataTable results3 = new System.Data.DataTable();
                    results3.Load(reader3);


                    //if (results3.Rows.Count == 0)
                    //{
                        
                    //    string st = "No data found";

                    //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                    //}
                    //else
                   // {
                        Logdata1 = DataTableToJSONWithStringBuilder(results3);
                    //}

                    dbConn.Close();
                }
                return Ok(Logdata1);

            }
        }





        [HttpPost]
        [Route("uploadimage")]
        public async Task<ActionResult<CAMS>> uploadimage(CAMS data)
        {
            // string struser = data.user_lower;

            List<CAMS> Logdata = new List<CAMS>();
            string Logdata1 = string.Empty;
            var logdata = "";
            var strtoken = "";
            // var result = "";
            using (SqlConnection dbConn = new SqlConnection(strconn))
            {


                dbConn.Open();
                string query = "";
                query = "update CAMS_ASSET_MASTER SET ImageUrl='" + data.doc_path + "' where ASSET_CODE='" + data.assetcode + "' AND BRANCH_ID=" + data.branchid + " AND FUNCTION_ID=" + data.functionid + "";

                SqlCommand cmd = new SqlCommand(query, dbConn);
                var reader = cmd.ExecuteReader();
                System.Data.DataTable results = new System.Data.DataTable();
                results.Load(reader);
                //if (results.Rows.Count == 0)
                //{
                //    string st = "updated successfully";

                //    Logdata1 = new JavaScriptSerializer().Serialize(st);
                //}
                //else
                //{
                    Logdata1 = DataTableToJSONWithStringBuilder(results);
               // }
               // Logdata1 = DataTableToJSONWithStringBuilder(results);
                dbConn.Close();

                var result = (new { recordsets = Logdata1 });
                return Ok(Logdata1);


            }
        }

        //json convertion method
        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }



    }
}
