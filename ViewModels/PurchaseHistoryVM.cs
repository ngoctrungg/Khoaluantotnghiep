﻿namespace KLTN_E.ViewModels
{
    public class PurchaseHistoryVM
    {
        public int MaHd { get; set; }
        public DateTime NgayDat { get; set; }
        public string TenTrangThai { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double? DonGia { get; set; }
        public string? Hinh {  get; set; }
    }
}
