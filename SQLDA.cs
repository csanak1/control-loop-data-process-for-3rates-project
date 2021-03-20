using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System;
using System.Collections.Generic;

using ControlLoopProcessFor3Rates.Models;

namespace ControlLoopProcessFor3Rates
{
    class SQLDA
    {
        private static readonly string _sqlConnection = ConfigurationManager.ConnectionStrings["myconnstring"].ConnectionString;

        public List<ModeTagsDataModel> GetModeTagsToProcess()
        {
            var modeTagData = new List<ModeTagsDataModel>();

            using (SqlConnection conn = new SqlConnection(_sqlConnection))
            {
                using (SqlCommand comm = new SqlCommand("[ThreeRates].[dbo].[SELECT_ModeTags]") { CommandType = CommandType.StoredProcedure })
                {
                    comm.Connection = conn;

                    try
                    {
                        //comm.Parameters.Add("@Plant", SqlDbType.VarChar).Value = plant; //we need tags from all the plants

                        conn.Open();

                        using (SqlDataReader dr = comm.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    var data = new ModeTagsDataModel
                                    {
                                        TagName = dr["ModeTag"].ToString(),
                                        PlantLoadFilterTagName = dr["PlantLoadFilterTag"].ToString(),
                                        ConditionFilter = Convert.ToBoolean(dr["HasConditionFilter"]),
                                        ConditionFilterTagName = dr["ConditionFilterTag"].ToString(),
                                        ConditionFilterType = Convert.ToInt32(dr["ConditionFilterTypeID"]),
                                        ConditionFilterValue = Convert.ToDouble(dr["ConditionFilterValue"])
                                    };

                                    modeTagData.Add(data);
                                }
                            }
                        }
                    }
                    catch (Exception e) { Console.WriteLine("An errror occured during executing the SQL query: " + e.Message); modeTagData = null; }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return modeTagData;
        }

        public void InsertProcessedControlLoopData(ControlLoopDataCollection processedCLDataCollection, string dateId)
        {
            using (SqlConnection conn = new SqlConnection(_sqlConnection))
            {
                using (SqlCommand comm = new SqlCommand("[ThreeRates].[dbo].[INSERT_ShiftHandover]") { CommandType = CommandType.StoredProcedure })
                {
                    comm.Connection = conn;

                    //comm.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output; //deleted from the db

                    comm.Parameters.Add("@DateID", SqlDbType.VarChar).Value = dateId;
                    comm.Parameters.Add("@CRData", SqlDbType.Structured).Value = processedCLDataCollection;

                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();

                        //result = Convert.ToInt32(comm.Parameters["@Result"].Value); //no returning value

                        conn.Close();
                    }
                    catch (SqlException e) { Console.WriteLine("An errror occured during executing the SQL query: " + e.Message); }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }
    }
}
