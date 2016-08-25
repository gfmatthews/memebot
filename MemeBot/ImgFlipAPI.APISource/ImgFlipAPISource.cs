using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ImgFlipAPI.APISource.Models;

namespace ImgFlipAPI.APISource
{
    public class ImgFlipAPISource
    {
        #region Singleton Setup
        private static ImgFlipAPISource _instance;
        public static ImgFlipAPISource Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ImgFlipAPISource();
                }
                return _instance;
            }
        }

        ImgFlipAPISource()
        {
            _defaultSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Used for the multipart form data for processing imgflip posts
        /// </summary>
        Guid BoundaryGuid = Guid.NewGuid();

        internal JsonSerializerSettings _defaultSerializerSettings = new JsonSerializerSettings();
        private HttpClient _client = new HttpClient();

        #endregion

        #region Internal Methods to Fetch and Add Data
        /// <summary>
        /// Fetches data asynchronously (and anonymously) for a given Imgur API endpoint.  Used by other calls in this 
        /// object.
        /// </summary>
        /// <param name="endpoint">The Imgur endpoint to run the HTTP get operation on</param>
        /// <returns>A string formatted response.  This will need to be parsed before it returns useful Imgur data.</returns>
        protected internal async Task<String> GetAnonymousImgflipDataAsync(String endpoint)
        {
            EnsureClientHasAuthorizationHeaders();
            HttpResponseMessage response = await _client.GetAsync(endpoint);
            return await ProcessResponseAsync(response);
        }

        /// <summary>
        /// Posts the content data to the endpoint specified.
        /// </summary>
        /// <param name="endpoint">The Imgur endpoint to run the HTTP post operation on</param>
        /// <param name="content">The HttpContent object that contains the data to post</param>
        /// <returns>A string formatted response.  This will need to be parsed before it returns useful Imgur data.</returns>
        protected internal async Task<String> PostAnonymousImgflipDataAsync(String endpoint, HttpContent content)
        {
            EnsureClientHasAuthorizationHeaders();
            HttpResponseMessage response = await _client.PostAsync(endpoint, content);
            return await ProcessResponseAsync(response);
        }

        /// <summary>
        /// Runs a delete operation on the endpoint specified.
        /// </summary>
        /// <param name="endpoint">The Imgur endpoint to run the delete operation on</param>
        /// <returns>The response</returns>
        protected internal async Task<String> DeleteImgflipDataAsync(String endpoint)
        {
            EnsureClientHasAuthorizationHeaders();
            HttpResponseMessage response = await _client.DeleteAsync(endpoint);
            return await ProcessResponseAsync(response);
        }

        /// <summary>
        /// Runs a put operation on the endpoint specified.
        /// </summary>
        /// <param name="endpoint">The Imgur endpoint to run the put operation on</param>
        /// <param name="content">Content to be included with the put operation</param>
        /// <returns>The response as a string</returns>
        protected internal async Task<String> PutAnonymousImgflipDataAsync(String endpoint, HttpContent content)
        {
            EnsureClientHasAuthorizationHeaders();
            HttpResponseMessage response = await _client.PutAsync(endpoint, content);
            return await ProcessResponseAsync(response);
        }

        /// <summary>
        /// Processes the response string from an HttpResponseMessage
        /// </summary>
        /// <param name="response">The HttpResponseMessage to process</param>
        /// <returns>The response message as a string, throws an exception if there's not a valid status code</returns>
        protected internal async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        /// <summary>
        /// Double checks that the client has the authorization header attached before continuing.
        /// </summary>
        protected internal void EnsureClientHasAuthorizationHeaders()
        {
        }
        #endregion

        #region [API] General Endpoint
        /// <summary>
        /// Gets an array of popular memes that may be captioned with this API. The size of this array and the order of memes may change at any time. When this description was written, it returned 100 memes ordered by how many times they were captioned in the last 30 days. 
        /// </summary>
        /// <returns>The get meme object root</returns>
        public async Task<GetMemeRoot> GetMemesAsync()
        {
            String responseString = await GetAnonymousImgflipDataAsync(ImgFlipEndpoints.GetMemes);
            return (await Task.Run(() => JsonConvert.DeserializeObject<GetMemeRoot>(responseString)));
        }

        /// <summary>
        /// Updates the title or description of an image. You can only update an image you own and is associated with your account. For an anonymous image, {id} must be the image's deletehash.
        /// </summary>
        /// <param name="deleteHashOrImageID">The deletehash or ID of an image (ID ONLY WORKS IF LOGGED IN!)</param>
        /// <param name="Title">The title of the image.</param>
        /// <param name="Description">The description of the image.</param>
        /// <returns></returns>
        public async Task<CaptionMemeRoot> CaptionMemeAsync(String TemplateID, String username, String password, String TopText, String BottomText)
        {
            MultipartFormDataContent content = new MultipartFormDataContent(BoundaryGuid.ToString());
            if (TemplateID != String.Empty)
            {
                content.Add(new StringContent(TemplateID), "template_id");
            }
            if (username != String.Empty)
            {
                content.Add(new StringContent(username), "username");
            }
            if (password != String.Empty)
            {
                // TODO: Handle password strings in a safer way
                content.Add(new StringContent(password), "password");
            }
            if (TopText != null)
            {
                content.Add(new StringContent(TopText), "text0");
            }
            if (BottomText != null)
            {
                content.Add(new StringContent(BottomText), "text1");
            }
            String responseString = await PostAnonymousImgflipDataAsync(ImgFlipEndpoints.CaptionImage, content);
            CaptionMemeRoot x = await Task.Run(() => JsonConvert.DeserializeObject<CaptionMemeRoot>(responseString, _defaultSerializerSettings));

            return x;
        }

        /// <summary>
        /// Creates a captioned image given a memetype and sets of text
        /// </summary>
        /// <param name="memeType">The popular meme type you want to caption</param>
        /// <param name="username">The username of the api account.  You can register at http://www.imgflip.com</param>
        /// <param name="password">The password for the api account</param>
        /// <param name="TopText">Text on the top line of the meme</param>
        /// <param name="BottomText">Text on the bottom line of the meme</param>
        /// <returns></returns>
        public async Task<CaptionMemeRoot> CaptionMemeAsync(PopularMemeTypes memeType, String username, String password, String TopText, String BottomText)
        {
            int tempID = (int)memeType;
            return await CaptionMemeAsync(tempID, username, password, TopText, BottomText);
        }

        /// <summary>
        /// Creates a captioned image given a memetype and sets of text
        /// </summary>
        /// <param name="memeID">The Id of the meme you want to caption</param>
        /// <param name="username">The username of the api account.  You can register at http://www.imgflip.com</param>
        /// <param name="password">The password for the api account</param>
        /// <param name="TopText">Text on the top line of the meme</param>
        /// <param name="BottomText">Text on the bottom line of the meme</param>
        /// <returns></returns>
        public async Task<CaptionMemeRoot> CaptionMemeAsync(int memeID, String username, String password, String TopText, String BottomText)
        {
            String templateIDToUse = memeID.ToString();
            CaptionMemeRoot x = await CaptionMemeAsync(templateIDToUse, username, password, TopText, BottomText);
            return x;
        }

        /// <summary>
        /// Finds the base image of the meme without any text
        /// </summary>
        /// <param name="memeType">the meme type you want to find</param>
        /// <returns></returns>
        public async Task<Uri> GetMemeBaseImage(PopularMemeTypes memeType)
        {
            int memeID = (int)memeType;
            return await GetMemeBaseImage(memeID);
        }

        /// <summary>
        /// Finds the base image of the meme without any text
        /// </summary>
        /// <param name="id">The id of the meme you want to find</param>
        /// <returns></returns>
        public async Task<Uri> GetMemeBaseImage(int id)
        {
            GetMemeRoot x = await GetMemesAsync();
            Meme matchedMeme = x.data.memes.Find(meme => meme.id.Contains(id.ToString()));
            return new Uri(matchedMeme.url);
        }

        /// <summary>
        /// Returns a meme object from a given search term.  If there are multiple matches, returns the first match.
        /// If a request is made that ends in a plurality (like s), also searches for the single instance version of that
        /// term.
        /// </summary>
        /// <param name="searchTerm">The search term you want to find.</param>
        /// <returns></returns>
        public async Task<List<Meme>> FindMeme(String searchTerm)
        {
            GetMemeRoot x = await GetMemesAsync();

            // we need to convert all the memes to lowercase so we can search
            foreach (Meme meme in x.data.memes)
            {
                meme.name = meme.name.ToLower();
            }
            
            List<Meme> matchedMeme = x.data.memes.FindAll(meme => meme.name.Contains(searchTerm.ToLower()));

            // if the user asked for a pluarality (e.g. cats or penguins or dogs)
            if (matchedMeme.Count == 0 && searchTerm.Last() == 's')
            {
                string NonPluralTerm = searchTerm.Remove(searchTerm.LastIndexOf('s'));
                List<Meme> secondTryMatchMeme = x.data.memes.FindAll(meme => meme.name.Contains(NonPluralTerm.ToLower()));
                if (secondTryMatchMeme.Count > 0)
                {
                    return secondTryMatchMeme;
                }
            }
            return matchedMeme;
        }

        #endregion

    }
}
