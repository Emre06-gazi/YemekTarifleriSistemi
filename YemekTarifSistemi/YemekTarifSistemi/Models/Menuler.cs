using System;
using System.Collections.Generic;

namespace YemekTarifSistemi.Models;

public partial class Menuler
{
    public int MenuId { get; set; }

    public string? Baslik { get; set; }

    public string? Url { get; set; }

    public byte? Sira { get; set; }

    public int? UstId { get; set; }

    public bool? Aktif { get; set; }

    public bool? Silindi { get; set; }
}
