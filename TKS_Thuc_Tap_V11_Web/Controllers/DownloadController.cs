using Microsoft.AspNetCore.Mvc;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;

namespace TKS_Thuc_Tap_V11_Web.Controllers
{
    [Route("api/download")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        /// <summary>
        /// Tải file Excel phiếu nhập kho
        /// GET: /api/download/phieu-nhap/{id}
        /// </summary>
        [HttpGet("phieu-nhap/{id}")]
        public IActionResult Download_Phieu_Nhap(long id)
        {
            try
            {
                CDM_Nhap_Kho_Controller v_objCtr = new CDM_Nhap_Kho_Controller();

                // Lấy thông tin phiếu để đặt tên file
                var v_objMaster = v_objCtr.FQ_107_NK_sp_sel_Get_By_ID(id);
                if (v_objMaster == null)
                    return NotFound("Không tìm thấy phiếu nhập.");

                // Tạo file Excel
                byte[] v_arrBytes = v_objCtr.Xuat_Excel_Phieu_Nhap(id);

                if (v_arrBytes == null || v_arrBytes.Length == 0)
                    return BadRequest("Không tạo được file Excel.");

                // Đặt tên file
                string v_strFileName = "Phieu_Nhap_Kho_" + v_objMaster.So_Phieu_Nhap_Kho
                                     + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                // Trả file về cho trình duyệt
                return File(
                    v_arrBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    v_strFileName);
            }
            catch (System.Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
        [HttpGet("phieu-xuat/{id}")]
        public IActionResult Download_Phieu_Xuat(long id)
        {
            try
            {
                CDM_Xuat_Kho_Controller v_objCtr = new CDM_Xuat_Kho_Controller();

                // Lấy thông tin phiếu để đặt tên file
                var v_objMaster = v_objCtr.FQ_108_XK_sp_sel_Get_By_ID(id);
                if (v_objMaster == null)
                    return NotFound("Không tìm thấy phiếu xuất.");

                // Tạo file Excel
                byte[] v_arrBytes = v_objCtr.Xuat_Excel_Phieu_Xuat(id);

                if (v_arrBytes == null || v_arrBytes.Length == 0)
                    return BadRequest("Không tạo được file Excel.");

                // Đặt tên file
                string v_strFileName = "Phieu_Xuat_Kho_" + v_objMaster.So_Phieu_Xuat_Kho
                                     + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                // Trả file về cho trình duyệt
                return File(
                    v_arrBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    v_strFileName);
            }
            catch (System.Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
    }
}