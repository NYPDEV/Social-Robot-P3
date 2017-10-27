using Facebook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SocialRobot.Application
{
    public class Facebook
    {
        private FacebookClient FBcl;
        private string accessToken;
        WebBrowser webBrowser;

        public Facebook(WebBrowser wb)
        {

            webBrowser = wb;
            wb.Navigate(new Uri("https://graph.facebook.com/oauth/authorize?client_id=1546542178984524&redirect_uri=http://www.facebook.com/connect/login_success.html&scope=publish_actions&type=user_agent&display=popup", UriKind.Absolute));
            wb.Navigated += ((object sender, NavigationEventArgs e) => {
                if (e.Uri.ToString().StartsWith("http://www.facebook.com/connect/login_success.html"))
                {
                    accessToken = e.Uri.Fragment.Split('&')[0].Replace("#access_token=", "");
                    FBcl = new FacebookClient(accessToken);
                }
            });
        }

        public void uploadPic(Bitmap pic, string text)
        {
            FacebookMediaObject FacebookUploader = new FacebookMediaObject
            {
                FileName = "picture",
                ContentType = "image/bmp"
            };

            ImageConverter converter = new ImageConverter();
            byte[] bytes = (byte[])converter.ConvertTo(pic, typeof(byte[]));
            FacebookUploader.SetValue(bytes);
            var postInfo = new Dictionary<string, object>();
            postInfo.Add("message", text);
            postInfo.Add("image", FacebookUploader);
            //   FBcl.Post("/" + "107728906314275" + "/photos", postInfo);
            FBcl.Post("/" + "107728906314275" + "/photos", postInfo); 
        }
        public void Logout()
        {
            if (accessToken == null) return;
            var oauth = new FacebookClient();
            var logoutParameters = new Dictionary<string, object>
                    {
                        {"access_token", accessToken},
                        { "next", "http://www.facebook.com/" }
                    };
            var logoutUrl = oauth.GetLogoutUrl(logoutParameters);
            webBrowser.Navigate(logoutUrl);
        }

    }
}
