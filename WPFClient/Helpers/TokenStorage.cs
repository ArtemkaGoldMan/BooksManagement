using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Helpers
{
    public static class TokenStorage
    {
        public static string Token { get; set; }

        public static bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public static void ClearToken()
        {
            Token = null;
        }
    }
}
