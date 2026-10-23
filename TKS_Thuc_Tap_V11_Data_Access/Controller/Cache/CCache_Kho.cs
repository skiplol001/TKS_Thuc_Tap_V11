using System;
using System.Collections.Generic;
using System.Linq;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.Cache
{
    public class CCache_Kho
    {
        private static List<CDM_Kho> Arr_Data = new List<CDM_Kho>();
        private static Dictionary<string, CDM_Kho> Dic_Data_Ten = new Dictionary<string, CDM_Kho>();
        private static Dictionary<long, CDM_Kho> Dic_Data_ID = new Dictionary<long, CDM_Kho>();

        public static void Load_Cache_Kho()
        {
            Arr_Data.Clear();
            Dic_Data_ID.Clear();
            Dic_Data_Ten.Clear();

            CDM_Kho_Controller v_objCtrl = new CDM_Kho_Controller();
            List<CDM_Kho> v_arrTemp = v_objCtrl.FQ_105_KHO_sp_sel_List_For_Cache();
            foreach (CDM_Kho v_objData in v_arrTemp)
                Add_Data(v_objData);
        }

        public static void Add_Data(CDM_Kho p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) || p_objData.Auto_ID == 0) return;
            Dic_Data_ID.Add(p_objData.Auto_ID, p_objData);
            Arr_Data.Add(p_objData);

            if (Dic_Data_Ten.ContainsKey(p_objData.Ten_Kho.ToLower()) == false)
                Dic_Data_Ten.Add(p_objData.Ten_Kho.ToLower(), p_objData);
        }

        public static void Update_Data(CDM_Kho p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == false || p_objData.Auto_ID == 0) return;
            Delete_Data(p_objData.Auto_ID);
            Add_Data(p_objData);
        }

        public static void Delete_Data(long p_iAuto_ID)
        {
            if (Dic_Data_ID.ContainsKey(p_iAuto_ID) == false || p_iAuto_ID == 0) return;
            CDM_Kho v_objData = Dic_Data_ID[p_iAuto_ID];
            Arr_Data.Remove(v_objData);
            Dic_Data_ID.Remove(p_iAuto_ID);
            Dic_Data_Ten.Remove(v_objData.Ten_Kho.ToLower());
        }

        public static CDM_Kho Get_Data_By_ID(long p_iID)
        {
            if (Dic_Data_ID.ContainsKey(p_iID)) return Dic_Data_ID[p_iID];
            return null;
        }

        public static CDM_Kho Get_Data_By_Ten(string p_strTen)
        {
            if (Dic_Data_Ten.ContainsKey(p_strTen.ToLower())) return Dic_Data_Ten[p_strTen.ToLower()];
            return null;
        }

        public static List<CDM_Kho> List_Data()
        {
            return Arr_Data.OrderBy(it => it.Ten_Kho).ToList();
        }
    }
}