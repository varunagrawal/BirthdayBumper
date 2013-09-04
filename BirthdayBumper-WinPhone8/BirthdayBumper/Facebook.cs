using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Windows.Security.Authentication;
using Facebook;

namespace BirthdayBumper
{
    class Facebook
    {
        public const string App_Id = "587441381302332";

        string permissions = "user_about_me, read_stream, friends_birthday";

        FacebookClient _fb = new FacebookClient();

        

        /*
         To get FB Friend's birthdays, the HTTP Request is: https://graph.facebook.com/userid/friends?fields=name,birthday
         */

        private Uri GetFacebookLoginUrl(string appId, string extendedPermissions)
        { 
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = appId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";

            if (!string.IsNullOrEmpty(extendedPermissions)) 
            {
                parameters["scope"] = extendedPermissions;
            }
            
            return _fb.GetLoginUrl(parameters);
        }


        

        private String getFriendURL(String FriendId)
        { 
            FriendId = "1587053882";
            return "https://www.facebook.com/" + FriendId.ToString();
        }


        public void getFriendDetails(String FriendId)
        {
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.CreateHttp(this.getFriendURL(FriendId));
            webReq.BeginGetRequestStream(new AsyncCallback(ResponseCallback), webReq);

            return;

        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest webReq = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse webResp = (HttpWebResponse)webReq.EndGetResponse(asyncResult);

        }
    }
}
