using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security.Enums;

public enum RefreshTokenValidType
{
    //Aktif
    Active = 0,
    //Süresi Geçmiş
    Expired = 1,
    //Siliniş
    Deleted = 2,
    //Bulunamadı
    NotFound = 3,
}
