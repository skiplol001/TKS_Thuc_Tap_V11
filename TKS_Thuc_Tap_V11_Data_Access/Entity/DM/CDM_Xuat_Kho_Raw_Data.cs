using System;
using TKS_Thuc_Tap_V11_Data_Access.Utility;

namespace TKS_Thuc_Tap_V11_Data_Access.Entity.DM
{
    public class CDM_Xuat_Kho_Raw_Data
    {
        private long m_lngAuto_ID;
        private long m_lngXuat_Kho_ID;
        private long m_lngSan_Pham_ID;
        private string m_strMa_San_Pham;
        private string m_strTen_San_Pham;
        private decimal m_dcmSL_Xuat;
        private decimal m_dcmDon_Gia_Xuat;
        private decimal m_dcmTri_Gia;
        private int m_intdeleted;
        private DateTime? m_dtmCreated;
        private string m_strCreated_By;
        private string m_strCreated_By_Function;
        private DateTime? m_dtmLast_Updated;
        private string m_strLast_Updated_By;
        private string m_strLast_Updated_By_Function;

        public CDM_Xuat_Kho_Raw_Data() { ResetData(); }

        public void ResetData()
        {
            m_lngAuto_ID = CConst.INT_VALUE_NULL;
            m_lngXuat_Kho_ID = CConst.INT_VALUE_NULL;
            m_lngSan_Pham_ID = CConst.INT_VALUE_NULL;
            m_strMa_San_Pham = CConst.STR_VALUE_NULL;
            m_strTen_San_Pham = CConst.STR_VALUE_NULL;
            m_dcmSL_Xuat = 0;
            m_dcmDon_Gia_Xuat = 0;
            m_dcmTri_Gia = 0;
            m_intdeleted = CConst.INT_VALUE_NULL;
            m_dtmCreated = CConst.DTM_VALUE_NULL;
            m_strCreated_By = CConst.STR_VALUE_NULL;
            m_strCreated_By_Function = CConst.STR_VALUE_NULL;
            m_dtmLast_Updated = CConst.DTM_VALUE_NULL;
            m_strLast_Updated_By = CConst.STR_VALUE_NULL;
            m_strLast_Updated_By_Function = CConst.STR_VALUE_NULL;
        }

        public long Auto_ID { get { return m_lngAuto_ID; } set { m_lngAuto_ID = value; } }
        public long Xuat_Kho_ID { get { return m_lngXuat_Kho_ID; } set { m_lngXuat_Kho_ID = value; } }
        public long San_Pham_ID { get { return m_lngSan_Pham_ID; } set { m_lngSan_Pham_ID = value; } }
        public string Ma_San_Pham { get { return m_strMa_San_Pham; } set { m_strMa_San_Pham = value == null ? "" : value.Trim(); } }
        public string Ten_San_Pham { get { return m_strTen_San_Pham; } set { m_strTen_San_Pham = value == null ? "" : value.Trim(); } }
        public decimal SL_Xuat { get { return m_dcmSL_Xuat; } set { m_dcmSL_Xuat = value; } }
        public decimal Don_Gia_Xuat { get { return m_dcmDon_Gia_Xuat; } set { m_dcmDon_Gia_Xuat = value; } }
        public decimal Tri_Gia { get { return m_dcmTri_Gia; } set { m_dcmTri_Gia = value; } }
        public int deleted { get { return m_intdeleted; } set { m_intdeleted = value; } }
        public DateTime? Created { get { return m_dtmCreated; } set { m_dtmCreated = value; } }
        public string Created_By { get { return m_strCreated_By; } set { m_strCreated_By = value == null ? "" : value.Trim(); } }
        public string Created_By_Function { get { return m_strCreated_By_Function; } set { m_strCreated_By_Function = value == null ? "" : value.Trim(); } }
        public DateTime? Last_Updated { get { return m_dtmLast_Updated; } set { m_dtmLast_Updated = value; } }
        public string Last_Updated_By { get { return m_strLast_Updated_By; } set { m_strLast_Updated_By = value == null ? "" : value.Trim(); } }
        public string Last_Updated_By_Function { get { return m_strLast_Updated_By_Function; } set { m_strLast_Updated_By_Function = value == null ? "" : value.Trim(); } }
    }
}