using System;
using System.Collections.Generic;
using System.Linq;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.Cache
{
    public class CCache_Kho_User
    {
        private static List<CDM_Kho_User> Arr_Data = new List<CDM_Kho_User>();
        private static Dictionary<long, CDM_Kho_User> Dic_Data_ID = new Dictionary<long, CDM_Kho_User>();
        private static Dictionary<string, List<CDM_Kho_User>> Dic_Data_User = new Dictionary<string, List<CDM_Kho_User>>();

        public static void Load_Cache_Kho_User()
        {
            Arr_Data.Clear();
            Dic_Data_ID.Clear();
            Dic_Data_User.Clear();

            CDM_Kho_User_Controller v_objCtrl = new CDM_Kho_User_Controller();
            List<CDM_Kho_User> v_arrTemp = v_objCtrl.FQ_106_KU_sp_sel_List_For_Cache();
            foreach (CDM_Kho_User v_objData in v_arrTemp)
                Add_Data(v_objData);
        }

        public static void Add_Data(CDM_Kho_User p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) || p_objData.Auto_ID == 0) return;
            Dic_Data_ID.Add(p_objData.Auto_ID, p_objData);
            Arr_Data.Add(p_objData);

            string v_strKey = p_objData.Ma_Dang_Nhap.ToLower();
            if (Dic_Data_User.ContainsKey(v_strKey) == false)
                Dic_Data_User.Add(v_strKey, new List<CDM_Kho_User>());
            Dic_Data_User[v_strKey].Add(p_objData);
        }

        public static void Update_Data(CDM_Kho_User p_objData)
        {
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == false || p_objData.Auto_ID == 0) return;
            Delete_Data(p_objData.Auto_ID);
            Add_Data(p_objData);
        }

        public static void Delete_Data(long p_iAuto_ID)
        {
            if (Dic_Data_ID.ContainsKey(p_iAuto_ID) == false || p_iAuto_ID == 0) return;
            CDM_Kho_User v_objData = Dic_Data_ID[p_iAuto_ID];
            Arr_Data.Remove(v_objData);
            Dic_Data_ID.Remove(p_iAuto_ID);

            string v_strKey = v_objData.Ma_Dang_Nhap.ToLower();
            if (Dic_Data_User.ContainsKey(v_strKey))
            {
                Dic_Data_User[v_strKey].Remove(v_objData);
                if (Dic_Data_User[v_strKey].Count == 0)
                    Dic_Data_User.Remove(v_strKey);
            }
        }

        public static CDM_Kho_User Get_Data_By_ID(long p_iID)
        {
            if (Dic_Data_ID.ContainsKey(p_iID)) return Dic_Data_ID[p_iID];
            return null;
        }

        public static List<CDM_Kho_User> Get_List_By_Ma_Dang_Nhap(string p_strMa_Dang_Nhap)
        {
            string v_strKey = p_strMa_Dang_Nhap.ToLower();
            if (Dic_Data_User.ContainsKey(v_strKey)) return Dic_Data_User[v_strKey];
            return new List<CDM_Kho_User>();
        }

        public static List<CDM_Kho_User> List_Data()
        {
            return Arr_Data.OrderBy(it => it.Ma_Dang_Nhap).ThenBy(it => it.Ten_Kho).ToList();
        }
    }
}