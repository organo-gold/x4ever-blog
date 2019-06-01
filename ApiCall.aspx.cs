
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;

namespace OrganoCodeTest
{
    public partial class ApiCall : Page
    {
        const string API_KEY = "O3962162";
        const string DIST_ID = "1000101";
        const string LOGIN_API = "http://organogold-dts.myvoffice.com/organogold/index.cfm?service=Session.login&apikey={0}&DTSPASSWORD={1}&DTSUSERID={2}&format=json";
        const string USER_DETAIL_API = "http://organogold-dts.myvoffice.com/organogold/index.cfm?jsessionid={0}&service=Genealogy.distInfoBySavedQuery&apikey={1}&QRYID=DistConfData&DISTID={2}&APPNAME=Admin&GROUP=Reports&format=JSON&fwreturnlog=1";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Task.Factory.StartNew(async () =>
                {
                    await RequestApi("3", "F@st1234");
                });
            }
        }

        private async Task RequestApi(string username, string password)
        {
            var url = string.Format($"{LOGIN_API}", API_KEY, password, username);
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            var model = new ResponseData();
            var data = new UserData();
            await client.SendAsync(request).ContinueWith(async (response) =>
                         {
                             var jsonTask = response.Result.Content.ReadAsStringAsync();
                             model = JsonConvert.DeserializeObject<ResponseData>(jsonTask.Result);
                             if (model != null)
                             {
                                 url = string.Format($"{USER_DETAIL_API}", model.SESSION, API_KEY, DIST_ID);
                                 request = new HttpRequestMessage()
                                 {
                                     RequestUri = new Uri(url),
                                     Method = HttpMethod.Get,
                                 };
                                 await client.SendAsync(request).ContinueWith((result) =>
                                 {
                                     var json = result.Result.Content.ReadAsStringAsync();
                                     data = JsonConvert.DeserializeObject<UserData>(json.Result);
                                 });
                             }
                         });
        }

        internal class ResponseData
        {
            public string SESSION { get; set; }
            public int LOAD { get; set; }
            public string ROLES { get; set; }
        }

        internal class UserData
        {
            public int ROWCOUNT { get; set; }
            public string[] COLUMNS { get; set; }
            public IDictionary<string, string[]> DATA { get; set; }
        }
    }
}