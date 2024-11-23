using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Endpoint
{
    public class ServerEndpointCollection
    {
        public string S3Url
        {
            get;
            set;
        }

        public string BamUrl
        {
            get;
            set;
        }

        public string AccountsApiUrl
        {
            get;
            set;
        }

        public string RtwApiUrl
        {
            get;
            set;
        }

        public string RYFApiURL
        {
            get;
            set;
        }


        
        private static ServerEndpointCollection current;

        public static ServerEndpointCollection LocalEndPoint = new ServerEndpointCollection
        {
            AccountsApiUrl = "gt-api-proxy.sarand.net/api/gtrunandroid",
            BamUrl = "gt-api-proxy.sarand.net/api/userdata/user_data/",
            RtwApiUrl = "gt-api-proxy.sarand.net/api/gtrunrtwandroid",
            RYFApiURL = "gt-api-proxy.sarand.net/api/gtrunryfandroid",
            S3Url = "www.kingcodestudio.com"
        };

        public static ServerEndpointCollection WorldEndPoint = new ServerEndpointCollection
        {
            AccountsApiUrl = "eu-api-proxy.kingcodestudio.com/api/gtrunandroid",
            BamUrl = "eu-api-proxy.kingcodestudio.com/api/userdata/user_data/",
            RtwApiUrl = "eu-api-proxy.kingcodestudio.com/api/gtrunrtwandroid",
            RYFApiURL = "eu-api-proxy.kingcodestudio.com/api/gtrunryfandroid",
            S3Url = "eu.kingcodestudio.com"
        };


        public static ServerEndpointCollection TestEndPoint = new ServerEndpointCollection
        {
            AccountsApiUrl = "gt-dev.kingcodestudio.com/api/gtrunandroid",
            BamUrl = "gt-dev.kingcodestudio.com/api/userdata/user_data/",
            RtwApiUrl = "gt-dev.kingcodestudio.com/api/gtrunrtwandroid",
            RYFApiURL = "gt-dev.kingcodestudio.com/api/gtrunryfandroid",
            S3Url = "gt-dev.kingcodestudio.com"
        };
        

#if LOCAL_HOST
		public static ServerEndpointCollection Current
        {
            get
			{
				return new ServerEndpointCollection
				{
					AccountsApiUrl = "192.168.100.104:2505/api/gtrunandroid",
                    BamUrl = "192.168.100.104:2505/api/userdata/user_data/",
                    RtwApiUrl = "192.168.100.104:2505/api/gtrunrtwandroid",
                    RYFApiURL = "192.168.100.104:2505/api/gtrunryfandroid",
                    S3Url = "192.168.100.43",
				};
			}
            set => current = value;
        }
#else
        public static ServerEndpointCollection Current
        {
            get
            {
                if (current == null)
                {
                    if (BasePlatform.ActivePlatform.InsideCountry)
                    {
                        ServerEndpointCollection.current = ServerEndpointCollection.LocalEndPoint;
                    }
                    else
                    {
                        ServerEndpointCollection.current = ServerEndpointCollection.WorldEndPoint;
                    }
                }

                return current;
            }
            set => current = value;
        }
#endif
    }

    public static bool UseServerV2
    {
        get
        {
            return ServerEndpointCollection.Current != ServerEndpointCollection.LocalEndPoint;
        }
    }

    public static string GetS3URL(string zFileName)
    {
        string s3Url = ServerEndpointCollection.Current.S3Url;
        return "http://" + s3Url + "/" + zFileName;
    }

    public static string GetBamURL()
    {
        string bam = ServerEndpointCollection.Current.BamUrl;
        return "http://" + bam;
    }

    public static string GetEC2URL(string command = null)
    {
        //UnityEngine.Debug.Log("Url : " + ServerEndpointCollection.Current.AccountsApiUrl);
        return AppendCommandToEndpoint(ServerEndpointCollection.Current.AccountsApiUrl, command);
    }

    public static string GetEC2URL_RTW(string command = null)
    {
        return AppendCommandToEndpoint(ServerEndpointCollection.Current.RtwApiUrl, command);
    }

    public static string GetEC2URL_RYF(string command = null)
    {
        return AppendCommandToEndpoint(ServerEndpointCollection.Current.RYFApiURL, command);
    }

    public static bool GetEC2ServerSecure()
    {
        return false;
    }

    private static string AppendCommandToEndpoint(string endpoint, string command)
    {
        if (!string.IsNullOrEmpty(command))
        {
            return endpoint + "/" + command + "/";
        }
        return endpoint;
    }


}