using System;
using System.Collections.Generic;
using System.Linq;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.Cache
{
    public class CCache_San_Pham
    {
        private static List<CDM_San_Pham> Arr_Data = new List<CDM_San_Pham>();
        private static Dictionary<string, CDM_San_Pham> Dic_Data_Ma = new Dictionary<string, CDM_San_Pham>();
        private static Dictionary<long, CDM_San_Pham> Dic_Data_ID = new Dictionary<long, CDM_San_Pham>();

        public static void Load_Cache_San_Pham()
        {
            Arr_Data.Clear();
            Dic_Data_ID.Clear();
            Dic_Data_Ma.Clear();

            CDM_San_Pham_Controller v_objCtrl = new CDM_San_Pham_Controller();
            List<CDM_San_Pham> v_arrTemp = v_objCtrl.FQ_103_SP_sp_sel_List_For_Cache();
            foreach (CDM_San_Pham v_objData in v_arrTemp)
                Add_Data(v_objData);
        }

        public static void Add_Data(CDM_San_Pham p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) || p_objData.Auto_ID == 0) return;
            Dic_Data_ID.Add(p_objData.Auto_ID, p_objData);
            Arr_Data.Add(p_objData);
            if (Dic_Data_Ma.ContainsKey(p_objData.Ma_San_Pham.ToLower()) == false)
                Dic_Data_Ma.Add(p_objData.Ma_San_Pham.ToLower(), p_objData);
        }

        public static void Update_Data(CDM_San_Pham p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == false || p_objData.Auto_ID == 0) return;
            Delete_Data(p_objData.Auto_ID);
            Add_Data(p_objData);
        }

        public static void Delete_Data(long p_iAuto_ID)
        {
            if (Dic_Data_ID.ContainsKey(p_iAuto_ID) == false || p_iAuto_ID == 0) return;
            CDM_San_Pham v_objData = Dic_Data_ID[p_iAuto_ID];
            Arr_Data.Remove(v_objData);
            Dic_Data_ID.Remove(p_iAuto_ID);
            Dic_Data_Ma.Remove(v_objData.Ma_San_Pham.ToLower());
        }

        public static CDM_San_Pham Get_Data_By_ID(long p_iID)
        {
            if (Dic_Data_ID.ContainsKey(p_iID)) return Dic_Data_ID[p_iID];
            return null;
        }

        public static CDM_San_Pham Get_Data_By_Ma(string p_strMa)
        {
            if (Dic_Data_Ma.ContainsKey(p_strMa.ToLower())) return Dic_Data_Ma[p_strMa.ToLower()];
            return null;
        }

        public static List<CDM_San_Pham> List_Data()
        {
            return Arr_Data.OrderBy(it => it.Ma_San_Pham).ToList();
        }
    }
}