using System;
using System.Collections.Generic;
using System.Linq;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.Cache
{
    public class CCache_Xuat_Kho
    {
        private static List<CDM_Xuat_Kho> Arr_Data = new List<CDM_Xuat_Kho>();
        private static Dictionary<long, CDM_Xuat_Kho> Dic_Data_ID = new Dictionary<long, CDM_Xuat_Kho>();
        private static Dictionary<string, CDM_Xuat_Kho> Dic_Data_So_Phieu = new Dictionary<string, CDM_Xuat_Kho>();

        public static void Load_Cache_Xuat_Kho()
        {
            Arr_Data.Clear();
            Dic_Data_ID.Clear();
            Dic_Data_So_Phieu.Clear();

            try
            {
                CDM_Xuat_Kho_Controller v_objCtrl = new CDM_Xuat_Kho_Controller();
                List<CDM_Xuat_Kho> v_arrTemp = v_objCtrl.FQ_108_XK_sp_sel_List_For_Cache();
                foreach (CDM_Xuat_Kho v_objData in v_arrTemp)
                    Add_Data(v_objData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Load_Cache_Xuat_Kho fail: " + ex.Message);
            }
        }

        public static void Add_Data(CDM_Xuat_Kho p_objData)
        {
            if (p_objData.Auto_ID == 0) return;
            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == false)
                Dic_Data_ID.Add(p_objData.Auto_ID, p_objData);
            if (Dic_Data_So_Phieu.ContainsKey(p_objData.So_Phieu_Xuat_Kho.ToLower()) == false)
                Dic_Data_So_Phieu.Add(p_objData.So_Phieu_Xuat_Kho.ToLower(), p_objData);
            Arr_Data.Add(p_objData);
        }

        public static void Delete_Data(long p_iAuto_ID)
        {
            if (Dic_Data_ID.ContainsKey(p_iAuto_ID) == false) return;
            CDM_Xuat_Kho v_objData = Dic_Data_ID[p_iAuto_ID];
            Arr_Data.Remove(v_objData);
            Dic_Data_ID.Remove(p_iAuto_ID);
            Dic_Data_So_Phieu.Remove(v_objData.So_Phieu_Xuat_Kho.ToLower());
        }

        public static CDM_Xuat_Kho Get_Data_By_So_Phieu(string p_strSo_Phieu)
        {
            if (Dic_Data_So_Phieu.ContainsKey(p_strSo_Phieu.ToLower()))
                return Dic_Data_So_Phieu[p_strSo_Phieu.ToLower()];
            return null;
        }

        public static List<CDM_Xuat_Kho> List_Data()
        {
            return Arr_Data.OrderByDescending(it => it.Auto_ID).ToList();
        }
    }
}