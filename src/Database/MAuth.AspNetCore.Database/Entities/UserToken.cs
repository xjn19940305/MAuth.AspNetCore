using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Database.Entities
{
    public class UserToken : IdentityUserToken<string>
    {
    }
}
