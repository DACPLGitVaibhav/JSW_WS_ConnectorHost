
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WS_ConnectorHost
{
    public class BusinessLayer
    {
        dbLayer dbl ;
        public BusinessLayer(string conn)
        {
            dbl = new dbLayer(conn);
        }

        public int InsertProductionDetailsBulk(object Json, object FileName, object FileCode)
        {
            try
            {
                var listParas = new List<SqlParameter>()
            {
             new SqlParameter("@JsonData", Json),
             new SqlParameter("@DateIimport", DateTime.Now),
             new SqlParameter("@FileName", FileName),
             new SqlParameter("@FileCode", FileCode)


            };
                return dbl.ExecSqlNonQuery("SP_InsertPreProductionDetails_JSON", CommandType.StoredProcedure, listParas);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int InsertProductionDetails(object ItemId, object BiwNo, object Vcode, object ModelCode, object DateIimport, object FileName, object FileCode)
        {
            try
            {
                var listParas = new List<SqlParameter>()
            {
             new SqlParameter("@ItemId", ItemId),
             new SqlParameter("@BiwNo", BiwNo),
             new SqlParameter("@Vcode", Vcode),
             new SqlParameter("@ModelCode", ModelCode),
             new SqlParameter("@DateIimport", DateIimport),
             new SqlParameter("@FileName", FileName),
             new SqlParameter("@FileCode", FileCode)


            };
                return dbl.ExecSqlNonQuery("SP_InsertPreProductionDetails", CommandType.StoredProcedure, listParas);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        //public DataSet BindAllParts()
        //{

        //    return dbl.ExecSqlDataSet("SP_PartMaster", CommandType.StoredProcedure);
        //}

        //public DataSet BindRunningParts()
        //{
        //    return dbl.ExecSqlDataSet("SP_GetRunningPartID", CommandType.StoredProcedure);
        //}

        public object MaxFileCode()
        {
            try
            {
                return dbl.ExecSqlScalar("select dbo.Scalar_GetFileCode()  as FileCode", CommandType.Text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int FileDelete(object FileCode)
        {
            try
            {
                var listParas = new List<SqlParameter>()
            {
             new SqlParameter("@FileCode", FileCode)


            };
                return dbl.ExecSqlNonQuery("SP_FileDelete", CommandType.StoredProcedure, listParas);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        //// USer Station Verify
        //public DataSet GetPartConfigByID(object PartID)
        //{
        //    var listParas = new List<SqlParameter>()
        //    {

        //     new SqlParameter("@PartID", PartID)
        //    // new SqlParameter( "@LogDate", LogDate),


        //    };
        //    return dbl.ExecSqlDataSet("SP_GetPartUserByVariantCode", CommandType.StoredProcedure, listParas);
        //}
        //// Tool Verify
        //public DataSet GetToolBarcodeBYPartID(object PartID)
        //{
        //    var listParas = new List<SqlParameter>()
        //    {

        //     new SqlParameter("@PartID", PartID)
        //    // new SqlParameter( "@LogDate", LogDate),


        //    };
        //    return dbl.ExecSqlDataSet("SP_GetToolBarcodeBYPartID", CommandType.StoredProcedure, listParas);
        //}


        //public DataSet IspartRunning(object PartID)
        //{
        //    var listParas = new List<SqlParameter>()
        //    {
        //     new SqlParameter("@PartID", PartID)
        //    // new SqlParameter( "@LogDate", LogDate),

        //    };
        //    return dbl.ExecSqlDataSet("SP_IsPartRunning", CommandType.StoredProcedure, listParas);
        //}
        //public DataSet IsProcessComplete()
        //{

        //    return dbl.ExecSqlDataSet("SP_IsProcessComplete", CommandType.StoredProcedure);
        //}
        //public int CompleteProcess(object CompleteDate, object CompletedOperatorID)
        //{
        //    try
        //    {
        //        var listParas = new List<SqlParameter>()
        //    {

        //     new SqlParameter("@CompleteDate", CompleteDate),
        //     new SqlParameter("@CompleteOperatorID", CompletedOperatorID)             

        //    };
        //        return dbl.ExecSqlNonQuery("SP_CompleteProcess", CommandType.StoredProcedure, listParas);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //public int GetToolIDbyStationID(object StationID)
        //{
        //    try
        //    {
        //        var listParas = new List<SqlParameter>()
        //    {

        //     new SqlParameter("@StationID", StationID)

        //    };
        //        return Convert.ToInt32(dbl.ExecSqlScalar("Sp_GetToolIDByStationID", CommandType.StoredProcedure, listParas));
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

    }
}

