using System;
using System.Collections.Generic;
using System.Linq;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.Cache
{
    public class CCache_Loai_San_Pham
    {
        private static List<CDM_Loai_San_Pham> Arr_Data = new List<CDM_Loai_San_Pham>();
        private static Dictionary<string, CDM_Loai_San_Pham> Dic_Data_Ma_LSP = new Dictionary<string, CDM_Loai_San_Pham>();
        private static Dictionary<string, CDM_Loai_San_Pham> Dic_Data_Ten_LSP = new Dictionary<string, CDM_Loai_San_Pham>();
        private static Dictionary<long, CDM_Loai_San_Pham> Dic_Data_ID = new Dictionary<long, CDM_Loai_San_Pham>();

        public static void Load_Cache_Loai_San_Pham()
        {
            Arr_Data.Clear();
            Dic_Data_ID.Clear();
            Dic_Data_Ma_LSP.Clear();
            Dic_Data_Ten_LSP.Clear();

            CDM_Loai_San_Pham_Controller v_objCtrl = new CDM_Loai_San_Pham_Controller();
            List<CDM_Loai_San_Pham> v_arrTemp_Data = v_objCtrl.FQ_102_LSP_sp_sel_List_For_Cache();

            foreach (CDM_Loai_San_Pham v_objData in v_arrTemp_Data)
                Add_Data(v_objData);
        }

        public static void Add_Data(CDM_Loai_San_Pham p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == true || p_objData.Auto_ID == 0)
                return;

            Dic_Data_ID.Add(p_objData.Auto_ID, p_objData);
            Arr_Data.Add(p_objData);

            if (Dic_Data_Ma_LSP.ContainsKey(p_objData.Ma_LSP.ToLower()) == false)
                Dic_Data_Ma_LSP.Add(p_objData.Ma_LSP.ToLower(), p_objData);

            if (Dic_Data_Ten_LSP.ContainsKey(p_objData.Ten_LSP.ToLower()) == false)
                Dic_Data_Ten_LSP.Add(p_objData.Ten_LSP.ToLower(), p_objData);
        }

        public static void Update_Data(CDM_Loai_San_Pham p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == false || p_objData.Auto_ID == 0)
                return;
            Delete_Data(p_objData.Auto_ID);
            Add_Data(p_objData);
        }

        public static void Delete_Data(long p_iAuto_ID)
        {
            if (Dic_Data_ID.ContainsKey(p_iAuto_ID) == false || p_iAuto_ID == 0)
                return;

            CDM_Loai_San_Pham v_objData = Dic_Data_ID[p_iAuto_ID];
            Arr_Data.Remove(v_objData);
            Dic_Data_ID.Remove(p_iAuto_ID);
            Dic_Data_Ma_LSP.Remove(v_objData.Ma_LSP.ToLower());
            Dic_Data_Ten_LSP.Remove(v_objData.Ten_LSP.ToLower());
        }

        public static CDM_Loai_San_Pham Get_Data_By_ID(long p_iID)
        {
            if (Dic_Data_ID.ContainsKey(p_iID)) return Dic_Data_ID[p_iID];
            return null;
        }

        public static CDM_Loai_San_Pham Get_Data_By_Ma_LSP(string p_strMa)
        {
            if (Dic_Data_Ma_LSP.ContainsKey(p_strMa.ToLower())) return Dic_Data_Ma_LSP[p_strMa.ToLower()];
            return null;
        }

        public static CDM_Loai_San_Pham Get_Data_By_Ten_LSP(string p_strTen)
        {
            if (Dic_Data_Ten_LSP.ContainsKey(p_strTen.ToLower())) return Dic_Data_Ten_LSP[p_strTen.ToLower()];
            return null;
        }

        public static List<CDM_Loai_San_Pham> List_Data()
        {
            return Arr_Data.OrderBy(it => it.Ma_LSP).ToList();
        }
    }
}