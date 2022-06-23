using OAuth2Demo.Oauth2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuth2Demo.Oauth2.Repositories
{
    public class ClientRepository
    {
        public static List<Client> Clients = new List<Client>() {
            new Client{
                 Id = "test1",
                 RedirectUrl = "http://localhost:59273/",
                 Secret = "LHZ5bUlOR2dzW1Yzd1dkbHdFbXNQSVBHSEs9dTZQKTE="
            },
            new Client{
                 Id = "test2",
                 RedirectUrl = "http://XXX.XXX.XXX/",
                 Secret = "fnQuMmxXJX4rdXdSc0ZDQ0owOGJhQTM1TTZBN251XnQ="
            }
        };
    }
}