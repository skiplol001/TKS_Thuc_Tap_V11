using System;
using TKS_Thuc_Tap_V11_Data_Access.Utility;

namespace TKS_Thuc_Tap_V11_Data_Access.Entity.DM
{
    public class CDM_Kho_User
    {
        private long m_lngAuto_ID;
        private string m_strMa_Dang_Nhap;
        private string m_strHo_Ten;
        private string m_strEmail;
        private string m_strDien_Thoai;
        private long m_lngKho_ID;
        private string m_strTen_Kho;
        private int m_intdeleted;
        private DateTime? m_dtmCreated;
        private string m_strCreated_By;
        private string m_strCreated_By_Function;
        private DateTime? m_dtmLast_Updated;
        private string m_strLast_Updated_By;
        private string m_strLast_Updated_By_Function;

        public CDM_Kho_User() { ResetData(); }

        public void ResetData()
        {
            m_lngAuto_ID = CConst.INT_VALUE_NULL;
            m_strMa_Dang_Nhap = CConst.STR_VALUE_NULL;
            m_strHo_Ten = CConst.STR_VALUE_NULL;
            m_strEmail = CConst.STR_VALUE_NULL;
            m_strDien_Thoai = CConst.STR_VALUE_NULL;
            m_lngKho_ID = CConst.INT_VALUE_NULL;
            m_strTen_Kho = CConst.STR_VALUE_NULL;
            m_intdeleted = CConst.INT_VALUE_NULL;
            m_dtmCreated = CConst.DTM_VALUE_NULL;
            m_strCreated_By = CConst.STR_VALUE_NULL;
            m_strCreated_By_Function = CConst.STR_VALUE_NULL;
            m_dtmLast_Updated = CConst.DTM_VALUE_NULL;
            m_strLast_Updated_By = CConst.STR_VALUE_NULL;
            m_strLast_Updated_By_Function = CConst.STR_VALUE_NULL;
        }

        public long Auto_ID { get { return m_lngAuto_ID; } set { m_lngAuto_ID = value; } }

        public string Ma_Dang_Nhap
        {
            get { return m_strMa_Dang_Nhap; }
            set { m_strMa_Dang_Nhap = value == null ? "" : value.Trim(); }
        }

        public string Ho_Ten
        {
            get { return m_strHo_Ten; }
            set { m_strHo_Ten = value == null ? "" : value.Trim(); }
        }

        public string Email
        {
            get { return m_strEmail; }
            set { m_strEmail = value == null ? "" : value.Trim(); }
        }

        public string Dien_Thoai
        {
            get { return m_strDien_Thoai; }
            set { m_strDien_Thoai = value == null ? "" : value.Trim(); }
        }

        public long Kho_ID { get { return m_lngKho_ID; } set { m_lngKho_ID = value; } }

        public string Ten_Kho
        {
            get { return m_strTen_Kho; }
            set { m_strTen_Kho = value == null ? "" : value.Trim(); }
        }

        public int deleted { get { return m_intdeleted; } set { m_intdeleted = value; } }
        public DateTime? Created { get { return m_dtmCreated; } set { m_dtmCreated = value; } }

        public string Created_By
        {
            get { return m_strCreated_By; }
            set { m_strCreated_By = value == null ? "" : value.Trim(); }
        }

        public string Created_By_Function
        {
            get { return m_strCreated_By_Function; }
            set { m_strCreated_By_Function = value == null ? "" : value.Trim(); }
        }

        public DateTime? Last_Updated { get { return m_dtmLast_Updated; } set { m_dtmLast_Updated = value; } }

        public string Last_Updated_By
        {
            get { return m_strLast_Updated_By; }
            set { m_strLast_Updated_By = value == null ? "" : value.Trim(); }
        }

        public string Last_Updated_By_Function
        {
            get { return m_strLast_Updated_By_Function; }
            set { m_strLast_Updated_By_Function = value == null ? "" : value.Trim(); }
        } public string Last_Updated_By_Function_;
    }
}