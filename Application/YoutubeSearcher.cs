using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace SocialRobot.Application
{
    class YoutubeSearcher
    {
        string API_KEY = "AIzaSyCx4XGrYDnAviFOOq5Lq8JfnFvNdNxjF5A";
        YouTubeService youtubeService = null;
        public YoutubeSearcher()
        {
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCx4XGrYDnAviFOOq5Lq8JfnFvNdNxjF5A",
                ApplicationName = this.GetType().ToString()
            });
        }
        public async Task<SearchListResponse> SearchAsync(string search, int MaxResult = 5)
        {
            
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = search;
            if (MainWindow.VidTrig == true)
            {
                searchListRequest.Type = "video";
            }
            else if (MainWindow.ChannelTrig == true)
            {
                searchListRequest.Type = "channel";
            }
            else if (MainWindow.PlaylistTrig == true)
            {
                searchListRequest.Type = "playlist";
            }

            searchListRequest.MaxResults = MaxResult;
            return await searchListRequest.ExecuteAsync();
        }
    }
}
