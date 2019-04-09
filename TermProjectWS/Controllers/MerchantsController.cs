using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ClassLibrary;
using Utilities;

namespace TermProjectWS.Controllers
{
    [Produces("application/json")]
    [Route("api/service/Merchants")]
    public class MerchantsController : Controller
    {
        // GET: api/service/Merchants/GetDeparments/
        [HttpGet("GetDepartments/")]
        public List<Department> GetDepartments()
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCmd = new SqlCommand();
            List<Department> departmentList = new List<Department>();

            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "GetDepartments";

            DataSet ds = objDB.GetDataSetUsingCmdObj(objCmd);

            foreach (DataRow record in ds.Tables[0].Rows)
            {
                Department deparment = new Department();
                deparment.DepartmentID = record["DepartmentID"].ToString();
                deparment.DepartmentName = record["DepartmentName"].ToString();
                departmentList.Add(deparment);
            }

            return departmentList;
        }

        // GET: api/service/Merchants/5
        [HttpGet("GetProductCatalog/{DepartmentNumber}", Name = "GetProductCatalog")]
        public List<Product> GetProductCatalog(string DepartmentNumber)
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCmd = new SqlCommand();
            List<Product> productList = new List<Product>();

            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "GetProductCatalogByDepartmentID";
            objCmd.Parameters.AddWithValue("@theID", DepartmentNumber);

            DataSet ds = objDB.GetDataSetUsingCmdObj(objCmd);

            foreach (DataRow record in ds.Tables[0].Rows)
            {
                Product product = new Product();
                product.ProductID = record["ProductID"].ToString();
                product.ProductDesc = record["ProductDesc"].ToString();
                product.Price = float.Parse(record["Price"].ToString());
                product.ImgURL = record["ImgURL"].ToString();
                productList.Add(product);
            }

            return productList;
        }

        // POST: api/service/Merchants/RegisterSite
        [HttpPost()]
        [HttpPost("RegisterSite")]
        public Boolean RegisterSite([FromBody]Site site)
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCmd = new SqlCommand();

            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "AddSite";

            objCmd.Parameters.AddWithValue("@sitedescr", site.SiteDesc);
            objCmd.Parameters.AddWithValue("@apikey", site.APIKey);
            objCmd.Parameters.AddWithValue("@email", site.Email);
            objCmd.Parameters.AddWithValue("@addr", site.Address);
            objCmd.Parameters.AddWithValue("@phone", site.PhoneNo);

            int rows = objDB.DoUpdateUsingCmdObj(objCmd);

            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //POST: api/service/Merchants/RecordPurchase
        [HttpPost("RecordPurchase")]
        public bool RecordPurchase(string ProductID, int Quantity, string SellerSiteID, string APIKey, [FromBody] Customer c)
        {
            DBConnect objDB = new DBConnect();
            SqlCommand objCmd = new SqlCommand();
            objCmd.CommandType = CommandType.StoredProcedure;

            if (c != null)
            {
                objCmd.CommandText = "GetCustByID";
                objCmd.Parameters.AddWithValue("@custid", c.CustID);
                DataSet customer = objDB.GetDataSetUsingCmdObj(objCmd);

                if (customer.Tables[0].Rows.Count > 0) //customer already exists  
                {
                    //get sale total 
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "GetTotalCost";
                    objCmd.Parameters.AddWithValue("@productid", int.Parse(ProductID));
                    objCmd.Parameters.AddWithValue("@qty", Quantity);
                    DataSet ds = objDB.GetDataSetUsingCmdObj(objCmd);
                    decimal saletotal = decimal.Parse(ds.Tables[0].Rows[0][0].ToString());

                    //update total dollar sales for customer
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "UpdateCustSales";
                    objCmd.Parameters.AddWithValue("@custid", c.CustID);
                    objCmd.Parameters.AddWithValue("@saletotal", saletotal);
                    int retVal = objDB.DoUpdateUsingCmdObj(objCmd);

                    //record purchase
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "AddPurchase";
                    objCmd.Parameters.AddWithValue("@productid", int.Parse(ProductID));
                    objCmd.Parameters.AddWithValue("@qty", Quantity);
                    objCmd.Parameters.AddWithValue("@siteid", int.Parse(SellerSiteID));
                    objCmd.Parameters.AddWithValue("apikey", APIKey.ToString());
                    objCmd.Parameters.AddWithValue("@custid", c.CustID);
                    int retVal2 = objDB.DoUpdateUsingCmdObj(objCmd);

                    if (retVal > 0 && retVal2 > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //customer doesn't exist 
                {
                    //get sale total 
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "GetTotalCost";
                    objCmd.Parameters.AddWithValue("@productid", int.Parse(ProductID));
                    objCmd.Parameters.AddWithValue("@qty", Quantity);
                    DataSet ds =objDB.GetDataSetUsingCmdObj(objCmd);
                    decimal saletotal = decimal.Parse(ds.Tables[0].Rows[0][0].ToString());

                    //add customer
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "AddCust";
                    objCmd.Parameters.AddWithValue("@name", c.Name);
                    objCmd.Parameters.AddWithValue("@phone", c.Phone);
                    objCmd.Parameters.AddWithValue("@email", c.Email);
                    objCmd.Parameters.AddWithValue("@address", c.Address);
                    objCmd.Parameters.AddWithValue("@totalsales", saletotal);
                    int retVal = objDB.DoUpdateUsingCmdObj(objCmd);

                    ////get newly added customer id
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "GetLastCustID";
                    DataSet newid = objDB.GetDataSetUsingCmdObj(objCmd);
                    int custid = int.Parse(newid.Tables[0].Rows[0][0].ToString());

                    //record purchase
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "AddPurchase";
                    objCmd.Parameters.AddWithValue("@productid", int.Parse(ProductID));
                    objCmd.Parameters.AddWithValue("@qty", Quantity);
                    objCmd.Parameters.AddWithValue("@siteid", int.Parse(SellerSiteID));
                    objCmd.Parameters.AddWithValue("@apikey", APIKey);
                    objCmd.Parameters.AddWithValue("@custid", custid);
                    int retVal2 = objDB.DoUpdateUsingCmdObj(objCmd);

                    if (retVal > 0 && retVal2 > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else
            {
                return false;
            }
        }

        // PUT: api/service/Merchants/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }


        
        // DELETE: api/service/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
