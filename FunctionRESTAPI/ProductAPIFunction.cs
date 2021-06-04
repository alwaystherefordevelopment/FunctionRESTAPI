using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FunctionRESTAPI
{
    public static class ProductAPIFunction
    {
        [FunctionName("GetAllProducts")]
        public static IActionResult GetAllProducts([HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = "product")]
            HttpRequest req)
        {
            List<Product> listProducts = new List<Product>();
            string strCon = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnection");
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                var text = "Select * From Product";
                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Product product = new Product();
                        product.ProductId = Convert.ToInt32(rdr["ProductId"]);
                        product.Name = rdr["Name"].ToString();
                        product.Description = rdr["Description"].ToString();
                        product.UnitPrice = Convert.ToDecimal(rdr["UnitPrice"]);

                        listProducts.Add(product);
                    }
                }
            }
            return new OkObjectResult(listProducts);
        }

        [FunctionName("AddProducts")]
        public static IActionResult AddProduct([HttpTrigger(AuthorizationLevel.Anonymous,"post",Route ="product/addproduct")]
        HttpRequest req)
        {
            string reqBody = new StreamReader(req.Body).ReadToEnd();
            var input = JsonSerializer.Deserialize<CreateProductModel>(reqBody);
            string strCon = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnection");
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {

                    var sql = "insert into Product (Name,Description, UnitPrice, CategoryId) values(@Name,@description,@unitPrice,@categoryId)";
                    var cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Name", input.Name);
                    cmd.Parameters.AddWithValue("@description", input.Description);
                    cmd.Parameters.AddWithValue("@unitPrice", input.UnitPrice);
                    cmd.Parameters.AddWithValue("@categoryId", input.CategoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(StatusCodes.Status400BadRequest);
            }
            return new OkObjectResult(StatusCodes.Status200OK);

        }

        [FunctionName("UpdateProducts")]
        public static IActionResult UpdateProduct([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "product/updateproduct/{Id}")] HttpRequest req)
        {
            string reqBody = new StreamReader(req.Body).ReadToEnd();
            var input = JsonSerializer.Deserialize<Product>(reqBody);
            string strCon = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnection");
            var sql = "Update Product set Name =@name,Description=@Description,UnitPrice=@UnitPrice,CategoryId = @categoryId where ProductId=@ProductId";
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@ProductId", input.ProductId);
                    cmd.Parameters.AddWithValue("@Name", input.Name);
                    cmd.Parameters.AddWithValue("@Description", input.Description);
                    cmd.Parameters.AddWithValue("@UnitPrice", input.UnitPrice);
                    cmd.Parameters.AddWithValue("@CategoryId", input.CategoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(StatusCodes.Status400BadRequest);
            }

            return new OkObjectResult(StatusCodes.Status200OK);
        }

        [FunctionName("DeleteProducts")]
        public static IActionResult DeleteProduct([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "product/deleteproduct/{Id}")] HttpRequest req, int id)
        {
            string delSql = "delete from Product where ProductId=@productId";
            var conStr = Environment.GetEnvironmentVariable("ConnectionStrings:DbConnection");
            using (SqlConnection con = new SqlConnection(conStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(delSql, con);
                    cmd.Parameters.AddWithValue("@productId", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                catch (Exception)
                {
                    return new BadRequestObjectResult(StatusCodes.Status400BadRequest);
                }

            }
            return new OkObjectResult(StatusCodes.Status200OK);
        }
    }
}
