using System;
using System.Collections.Generic;
using System.Linq;
using TKS_Thuc_Tap_V11_Data_Access.Controller.DM;
using TKS_Thuc_Tap_V11_Data_Access.Entity.DM;

namespace TKS_Thuc_Tap_V11_Data_Access.Controller.Cache
{
    public class CCache_Nhap_Kho
    {
        private static List<CDM_Nhap_Kho> Arr_Data = new List<CDM_Nhap_Kho>();
        private static Dictionary<long, CDM_Nhap_Kho> Dic_Data_ID = new Dictionary<long, CDM_Nhap_Kho>();
        private static Dictionary<string, CDM_Nhap_Kho> Dic_Data_So_Phieu = new Dictionary<string, CDM_Nhap_Kho>();

        public static void Load_Cache_Nhap_Kho()
        {
            Arr_Data.Clear();
            Dic_Data_ID.Clear();
            Dic_Data_So_Phieu.Clear();

            // Chỉ dùng cache cho validation, không load full để tránh nặng RAM
        }

        public static void Add_Data(CDM_Nhap_Kho p_objData)
        {
            if (p_objData.Auto_ID == 0) return;

            if (Dic_Data_ID.ContainsKey(p_objData.Auto_ID) == false)
                Dic_Data_ID.Add(p_objData.Auto_ID, p_objData);

            if (Dic_Data_So_Phieu.ContainsKey(p_objData.So_Phieu_Nhap_Kho.ToLower()) == false)
                Dic_Data_So_Phieu.Add(p_objData.So_Phieu_Nhap_Kho.ToLower(), p_objData);

            Arr_Data.Add(p_objData);
        }

        public static void Delete_Data(long p_iAuto_ID)
        {
            if (Dic_Data_ID.ContainsKey(p_iAuto_ID) == false) return;
            CDM_Nhap_Kho v_objData = Dic_Data_ID[p_iAuto_ID];
            Arr_Data.Remove(v_objData);
            Dic_Data_ID.Remove(p_iAuto_ID);
            Dic_Data_So_Phieu.Remove(v_objData.So_Phieu_Nhap_Kho.ToLower());
        }

        public static CDM_Nhap_Kho Get_Data_By_So_Phieu(string p_strSo_Phieu)
        {
            if (Dic_Data_So_Phieu.ContainsKey(p_strSo_Phieu.ToLower()))
                return Dic_Data_So_Phieu[p_strSo_Phieu.ToLower()];
            return null;
        }

        public static List<CDM_Nhap_Kho> List_Data()
        {
            return Arr_Data.OrderByDescending(it => it.Auto_ID).ToList();
        }
    }
}