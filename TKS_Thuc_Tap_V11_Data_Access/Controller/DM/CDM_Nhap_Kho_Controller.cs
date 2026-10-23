using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using TKS_Thuc_Tap_V11_Data_Access.DataLayer;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;
using TKS_Thuc_Tap_V11_Data_Access.Utility;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.DM
{
    public class CDM_Nhap_Kho_Controller
    {
        public List<CDM_Nhap_Kho> FQ_107_NK_sp_sel_List_By_Created(DateTime? p_dtmFrom, DateTime? p_dtmTo)
        {
            List<CDM_Nhap_Kho> v_arrRes = new List<CDM_Nhap_Kho>();
            DataTable v_dt = new DataTable();
            try
            {
                p_dtmFrom = CUtility_Date.Convert_To_Dau_Ngay(p_dtmFrom);
                p_dtmTo = CUtility_Date.Convert_To_Cuoi_Ngay(p_dtmTo);
                CSqlHelper.FillDataTable(CConfig.TKS_Thuc_Tap_V11_Conn_String, v_dt, "FQ_107_NK_sp_sel_List_By_Created", p_dtmFrom, p_dtmTo);
                foreach (DataRow v_row in v_dt.Rows)
                    v_arrRes.Add(CUtility.Map_Row_To_Entity<CDM_Nhap_Kho>(v_row));
            }
            catch (Exception) { throw; }
            finally { v_dt.Dispose(); }
            return v_arrRes;
        }

        public CDM_Nhap_Kho FQ_107_NK_sp_sel_Get_By_ID(long p_iID)
        {
            CDM_Nhap_Kho v_objRes = null;
            DataTable v_dt = new DataTable();
            try
            {
                CSqlHelper.FillDataTable(CConfig.TKS_Thuc_Tap_V11_Conn_String, v_dt, "FQ_107_NK_sp_sel_Get_By_ID", p_iID);
                if (v_dt.Rows.Count > 0)
                    v_objRes = CUtility.Map_Row_To_Entity<CDM_Nhap_Kho>(v_dt.Rows[0]);
            }
            catch (Exception) { throw; }
            finally { v_dt.Dispose(); }
            return v_objRes;
        }

        public List<CDM_Nhap_Kho_Raw_Data> FQ_107_NK_sp_sel_List_Detail_By_Nhap_Kho_ID(long p_iNhap_Kho_ID)
        {
            List<CDM_Nhap_Kho_Raw_Data> v_arrRes = new List<CDM_Nhap_Kho_Raw_Data>();
            DataTable v_dt = new DataTable();
            try
            {
                CSqlHelper.FillDataTable(CConfig.TKS_Thuc_Tap_V11_Conn_String, v_dt,
                    "FQ_107_NK_sp_sel_List_Detail_By_Nhap_Kho_ID", p_iNhap_Kho_ID);

                foreach (DataRow v_row in v_dt.Rows)
                {
                    CDM_Nhap_Kho_Raw_Data v_objRes = new CDM_Nhap_Kho_Raw_Data();
                    v_objRes.Auto_ID = Convert.ToInt64(v_row["Auto_ID"]);
                    v_objRes.Nhap_Kho_ID = Convert.ToInt64(v_row["Nhap_Kho_ID"]);
                    v_objRes.San_Pham_ID = Convert.ToInt64(v_row["San_Pham_ID"]);
                    v_objRes.Ma_San_Pham = Convert.ToString(v_row["Ma_San_Pham"]) ?? "";
                    v_objRes.Ten_San_Pham = Convert.ToString(v_row["Ten_San_Pham"]) ?? "";
                    v_objRes.SL_Nhap = Convert.ToDecimal(v_row["SL_Nhap"]);          
                    v_objRes.Don_Gia_Nhap = Convert.ToDecimal(v_row["Don_Gia_Nhap"]); 
                    v_objRes.Tri_Gia = Convert.ToDecimal(v_row["Tri_Gia"]);           
                    v_arrRes.Add(v_objRes);
                }
            }
            catch (Exception) { throw; }
            finally { v_dt.Dispose(); }
            return v_arrRes;
        }

        /// <summary>
        /// Insert master + detail trong 1 TRANSACTION.
        /// Nếu 1 detail lỗi → rollback toàn bộ (cả master).
        /// </summary>
        public long FQ_107_NK_sp_ins_Insert_Master_Detail(
            CDM_Nhap_Kho p_objMaster,
            List<CDM_Nhap_Kho_Raw_Data> p_arrDetail)
        {
            long v_iMaster_ID = CConst.INT_VALUE_NULL;
            SqlConnection v_conn = null;
            SqlTransaction v_trans = null;

            try
            {
                // Mở kết nối + bắt đầu transaction
                v_conn = CSqlHelper.CreateConnection(CConfig.TKS_Thuc_Tap_V11_Conn_String);
                v_conn.Open();
                v_trans = v_conn.BeginTransaction();

                try
                {
                    // 1. Insert master
                    v_iMaster_ID = Convert.ToInt64(CSqlHelper.ExecuteScalar(
                        v_conn, v_trans, CConfig.TKS_Thuc_Tap_V11_Conn_String,
                        "FQ_107_NK_sp_ins_Insert",
                        p_objMaster.So_Phieu_Nhap_Kho, p_objMaster.Kho_ID,
                        p_objMaster.Ngay_Nhap_Kho, p_objMaster.Ghi_Chu,
                        p_objMaster.Last_Updated_By, p_objMaster.Last_Updated_By_Function));

                    // 2. Insert từng detail
                    foreach (CDM_Nhap_Kho_Raw_Data v_objDetail in p_arrDetail)
                    {
                        v_objDetail.Nhap_Kho_ID = v_iMaster_ID;
                        v_objDetail.Auto_ID = Convert.ToInt64(CSqlHelper.ExecuteScalar(
                            v_conn, v_trans, CConfig.TKS_Thuc_Tap_V11_Conn_String,
                            "FQ_107_NK_sp_ins_Insert_Detail",
                            v_objDetail.Nhap_Kho_ID, v_objDetail.San_Pham_ID,
                            v_objDetail.SL_Nhap, v_objDetail.Don_Gia_Nhap,
                            p_objMaster.Last_Updated_By, p_objMaster.Last_Updated_By_Function));
                    }

                    // 3. Nếu mọi thứ OK → commit
                    v_trans.Commit();
                }
                catch
                {
                    // 4. Nếu lỗi → rollback toàn bộ (xóa cả master đã insert)
                    if (v_trans != null) v_trans.Rollback();
                    throw;
                }
            }
            catch (Exception) { throw; }
            finally
            {
                if (v_trans != null) v_trans.Dispose();
                if (v_conn != null) { v_conn.Close(); v_conn.Dispose(); }
            }

            return v_iMaster_ID;
        }
        public void FQ_107_NK_sp_upd_Update(CDM_Nhap_Kho p_objData)
        {
            try
            {
                CSqlHelper.ExecuteNonquery(CConfig.TKS_Thuc_Tap_V11_Conn_String,
                    "FQ_107_NK_sp_upd_Update",
                    p_objData.Auto_ID,
                    p_objData.So_Phieu_Nhap_Kho,
                    p_objData.Kho_ID,
                    p_objData.Ngay_Nhap_Kho,
                    p_objData.Ghi_Chu,
                    p_objData.Last_Updated_By,
                    p_objData.Last_Updated_By_Function);
            }
            catch (Exception) { throw; }
        }

        public void FQ_107_NK_sp_upd_Update_Detail(CDM_Nhap_Kho_Raw_Data p_objData,
    string p_strLast_Updated_By, string p_strLast_Updated_By_Function)
        {
            try
            {
                CSqlHelper.ExecuteNonquery(CConfig.TKS_Thuc_Tap_V11_Conn_String,
                    "FQ_107_NK_sp_upd_Update_Detail",
                    p_objData.Auto_ID,
                    p_objData.SL_Nhap,
                    p_objData.Don_Gia_Nhap,
                    p_strLast_Updated_By,
                    p_strLast_Updated_By_Function);
            }
            catch (Exception) { throw; }
        }

        public void FQ_107_NK_sp_del_Delete_Detail_By_ID(long p_iAuto_ID,
            string p_strLast_Updated_By, string p_strLast_Updated_By_Function)
        {
            try
            {
                CSqlHelper.ExecuteNonquery(CConfig.TKS_Thuc_Tap_V11_Conn_String,
                    "FQ_107_NK_sp_del_Delete_Detail_By_ID",
                    p_iAuto_ID,
                    p_strLast_Updated_By,
                    p_strLast_Updated_By_Function);
            }
            catch (Exception) { throw; }
        }

        // Insert 1 dòng detail cho phiếu đã có (khác bài 7 - insert cùng master)
        public long FQ_107_NK_sp_ins_Insert_Detail_By_Master_ID(CDM_Nhap_Kho_Raw_Data p_objData)
        {
            long v_iRes = CConst.INT_VALUE_NULL;
            try
            {
                v_iRes = Convert.ToInt64(CSqlHelper.ExecuteScalar(
                    CConfig.TKS_Thuc_Tap_V11_Conn_String,
                    "FQ_107_NK_sp_ins_Insert_Detail",
                    p_objData.Nhap_Kho_ID,
                    p_objData.San_Pham_ID,
                    p_objData.SL_Nhap,
                    p_objData.Don_Gia_Nhap,
                    p_objData.Last_Updated_By,
                    p_objData.Last_Updated_By_Function));
            }
            catch (Exception) { throw; }
            return v_iRes;
        }
        public void FQ_107_NK_sp_del_Delete_By_ID(long p_iAuto_ID, string p_strLast_Updated_By, string p_strLast_Updated_By_Function)
        {
            try
            {
                CSqlHelper.ExecuteNonquery(CConfig.TKS_Thuc_Tap_V11_Conn_String,
                    "FQ_107_NK_sp_del_Delete_By_ID", p_iAuto_ID,
                    p_strLast_Updated_By, p_strLast_Updated_By_Function);
            }
            catch (Exception) { throw; }
        }
        /// <summary>
        /// Xuất phiếu nhập kho ra file Excel
        /// </summary>
        public byte[] Xuat_Excel_Phieu_Nhap(long p_iNhap_Kho_ID)
        {
            try
            {
                // 1. Lấy dữ liệu
                CDM_Nhap_Kho v_objMaster = FQ_107_NK_sp_sel_Get_By_ID(p_iNhap_Kho_ID);
                if (v_objMaster == null)
                    throw new Exception("Không tìm thấy phiếu nhập.");

                List<CDM_Nhap_Kho_Raw_Data> v_arrDetail = FQ_107_NK_sp_sel_List_Detail_By_Nhap_Kho_ID(p_iNhap_Kho_ID);

                // 2. Tạo Excel
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var v_package = new ExcelPackage())
                {
                    var v_sheet = v_package.Workbook.Worksheets.Add("Phieu_Nhap_Kho");

                    // ===== HEADER =====
                    v_sheet.Cells["A1:F1"].Merge = true;
                    v_sheet.Cells["A1"].Value = "PHIẾU NHẬP KHO";
                    v_sheet.Cells["A1"].Style.Font.Bold = true;
                    v_sheet.Cells["A1"].Style.Font.Size = 18;
                    v_sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Thông tin phiếu
                    v_sheet.Cells["A3"].Value = "Số phiếu:";
                    v_sheet.Cells["B3"].Value = v_objMaster.So_Phieu_Nhap_Kho;
                    v_sheet.Cells["A3"].Style.Font.Bold = true;

                    v_sheet.Cells["D3"].Value = "Ngày nhập:";
                    v_sheet.Cells["E3"].Value = v_objMaster.Ngay_Nhap_Kho?.ToString("dd/MM/yyyy");
                    v_sheet.Cells["D3"].Style.Font.Bold = true;

                    v_sheet.Cells["A4"].Value = "Kho:";
                    v_sheet.Cells["B4"].Value = v_objMaster.Ten_Kho;
                    v_sheet.Cells["A4"].Style.Font.Bold = true;

                    v_sheet.Cells["D4"].Value = "Ghi chú:";
                    v_sheet.Cells["E4"].Value = v_objMaster.Ghi_Chu;
                    v_sheet.Cells["D4"].Style.Font.Bold = true;

                    // ===== BẢNG CHI TIẾT =====
                    int v_iRow_Start = 6;

                    // Header của bảng
                    v_sheet.Cells[v_iRow_Start, 1].Value = "STT";
                    v_sheet.Cells[v_iRow_Start, 2].Value = "Mã SP";
                    v_sheet.Cells[v_iRow_Start, 3].Value = "Tên sản phẩm";
                    v_sheet.Cells[v_iRow_Start, 4].Value = "Số lượng";
                    v_sheet.Cells[v_iRow_Start, 5].Value = "Đơn giá";
                    v_sheet.Cells[v_iRow_Start, 6].Value = "Trị giá";

                    using (var v_range = v_sheet.Cells[v_iRow_Start, 1, v_iRow_Start, 6])
                    {
                        v_range.Style.Font.Bold = true;
                        v_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        v_range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        v_range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        v_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Data
                    int v_iRow = v_iRow_Start + 1;
                    int v_iSTT = 1;
                    decimal v_dcmTong_Tien = 0;

                    foreach (var v_objDetail in v_arrDetail)
                    {
                        v_sheet.Cells[v_iRow, 1].Value = v_iSTT;
                        v_sheet.Cells[v_iRow, 2].Value = v_objDetail.Ma_San_Pham;
                        v_sheet.Cells[v_iRow, 3].Value = v_objDetail.Ten_San_Pham;
                        v_sheet.Cells[v_iRow, 4].Value = v_objDetail.SL_Nhap;
                        v_sheet.Cells[v_iRow, 5].Value = v_objDetail.Don_Gia_Nhap;
                        v_sheet.Cells[v_iRow, 6].Value = v_objDetail.Tri_Gia;

                        v_sheet.Cells[v_iRow, 4].Style.Numberformat.Format = "#,##0.00";
                        v_sheet.Cells[v_iRow, 5].Style.Numberformat.Format = "#,##0.00";
                        v_sheet.Cells[v_iRow, 6].Style.Numberformat.Format = "#,##0.00";
                        v_sheet.Cells[v_iRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        v_dcmTong_Tien += v_objDetail.Tri_Gia;

                        v_iRow++;
                        v_iSTT++;
                    }

                    // Tổng cộng
                    v_sheet.Cells[v_iRow, 5].Value = "TỔNG CỘNG:";
                    v_sheet.Cells[v_iRow, 5].Style.Font.Bold = true;
                    v_sheet.Cells[v_iRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    v_sheet.Cells[v_iRow, 6].Value = v_dcmTong_Tien;
                    v_sheet.Cells[v_iRow, 6].Style.Font.Bold = true;
                    v_sheet.Cells[v_iRow, 6].Style.Numberformat.Format = "#,##0.00";

                    using (var v_range = v_sheet.Cells[v_iRow, 5, v_iRow, 6])
                    {
                        v_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        v_range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                        v_range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Ký tên
                    v_iRow += 3;
                    v_sheet.Cells[v_iRow, 1].Value = "Người lập phiếu";
                    v_sheet.Cells[v_iRow, 3].Value = "Người giao hàng";
                    v_sheet.Cells[v_iRow, 5].Value = "Thủ kho";
                    using (var v_range = v_sheet.Cells[v_iRow, 1, v_iRow, 6])
                    {
                        v_range.Style.Font.Bold = true;
                        v_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Format cột
                    v_sheet.Column(1).Width = 6;
                    v_sheet.Column(2).Width = 15;
                    v_sheet.Column(3).Width = 35;
                    v_sheet.Column(4).Width = 12;
                    v_sheet.Column(5).Width = 15;
                    v_sheet.Column(6).Width = 18;

                    // Border toàn bộ bảng
                    if (v_arrDetail.Count > 0)
                    {
                        using (var v_range = v_sheet.Cells[v_iRow_Start, 1, v_iRow_Start + v_arrDetail.Count, 6])
                        {
                            v_range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            v_range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            v_range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            v_range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    // 3. Trả về byte[]
                    return v_package.GetAsByteArray();
                }
            }
            catch (Exception) { throw; }
        }
    }
}