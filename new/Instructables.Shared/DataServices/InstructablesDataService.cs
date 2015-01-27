using Instructables.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Threading;

using Windows.Storage;
using Windows.Storage.Streams;

using Windows.Networking.BackgroundTransfer;
using Instructables.Common;
using System.Collections.ObjectModel;

namespace Instructables.DataServices
{

    public class InstructablesDataService
    {
        private bool _useTestData = false;

        private bool _addDelays = false;
        private int _instructableDelayLength = 5000;
        private int _summaryDelayLength = 1000;
        private int _searchDelayLength = 3000;
        private bool _simulateFailure = false;
        private int _maxTimeoutMilliseconds = 20000;

        //private const string BaseUri = "http://staging.instructables.com/";
        private const string BaseUri = "http://www.instructables.com/";
        private const string EncrptedUri = "https://ssl.instructables.com/";

        private const string AUTHTOKEN_COOKIE = "authy";

        private const string LoginUri = "json-api/login";
        private const string FacebookLoginUri = "json-api/login?auth=facebook";
        private const string SignUpUri = "json-api/register";
        private const string ForgetPasswordUri = "json-api/forgotPassword";
        private const string ResetPasswordUri = "json-api/resetPassword";

        private const string GetContestUri = "json-api/getContests";
        private const string GetContestInfoUri = "json-api/showContest";
        private const string GetContestEntriesUri = "json-api/getContestEntries";

        private const string GetList = "json-api/searchInstructables";
        private const string GeteBooksUri = "json-api/searchInstructables?sort=featured&type=guide&ebookFlag=true";
        private const string GetFollowedInstructableUri = "json-api/getIblesBySubscriptions";
        private const string GetInstrubtablesByAuthor = "json-api/getIblesByAuthor";
        private const string GetDraftsUri = "json-api/getDrafts";
        private const string GetFavoriteUri = "json-api/getFavorites";
        private const string GetInstructableUri = "json-api/showInstructable";

        private const string GetCategoryChannelListUri = "json-api/getCategories";
        private const string FavoriteUri = "ajax/favorite";
        private const string FlagUri = "/json-api/setFlag";

        private const string UploadUri = "json-api/upload";
        private const string GetAuthorUri = "json-api/showAuthor";
        private const string SaveProfileUri = "json-api/saveProfile";
        private const string FollowAuthorUri = "json-api/followAuthor";
        private const string UnFollowAuthorUri = "json-api/unfollowAuthor";

        private const string GetFollowersUri = "json-api/getFollowers";
        private const string GetSubscriptionsUri = "json-api/getSubscriptions";

        private const string VoteUri = "contest/vote";
        private const string GetCommentsUri = "json-api/getComments";
        private const string postCommentsUri = "json-api/addComment";

        private const string NewInstructableUri = "json-api/newInstructable";
        private const string SaveInstructableUri = "json-api/saveInstructable";
        private const string PublishInstructableUri = "json-api/publishInstructable";

        private static InstructablesDataService singleton = null;

        public delegate void LoginSucceedHandler(object sender, EventArgs e);
        public event LoginSucceedHandler LoginSucceed;

        public delegate void LogoutSucceedHandler(object sender, EventArgs e);
        public event LogoutSucceedHandler LogoutSucceed;

        //Factory Method
        public static InstructablesDataService DataServiceSingleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new InstructablesDataService();
                }

                return singleton;
            }
        }

        private InstructablesDataService()
        {
            if (Utils.DesignMode.IsInDesignMode())
            {
                _useTestData = true;
            }
        }

        internal async Task<RequestResult<RequestError>> FollowAuthor(string screenName,bool Follow)
        {
            if (isLogin() != true)
                return null;

            String _followAuthorUri = String.Empty;
            if(Follow == true)
            {
                _followAuthorUri = FollowAuthorUri;
            }
            else
            {
                _followAuthorUri = UnFollowAuthorUri;
            }
            var requestUri = String.Format("{0}{1}", BaseUri, _followAuthorUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("screenName", screenName));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> FollowAuthor(string screenName)
        {
            if (isLogin() != true)
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, FollowAuthorUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("screenName", screenName));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> UnfollowAuthor(string screenName)
        {
            if (isLogin() != true)
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, UnFollowAuthorUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("screenName", screenName));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> ToggleFavorite(string instructableId)
        {
            if (isLogin() != true)
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, FavoriteUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("entryId", instructableId));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> Vote(string instructableId, string contestId, bool vote)
        {
            if (isLogin() != true)
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, VoteUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);
                    client.DefaultRequestHeaders.ExpectContinue = false;

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("instructableId", instructableId));
                    postData.Add(new KeyValuePair<string, string>("contestId", contestId));
                    postData.Add(new KeyValuePair<string, string>("add", vote? "true" : "false"));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> Flag(string instructableId, string flagType)
        {
            if (isLogin() != true)
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, FavoriteUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("action", "SET"));
                    postData.Add(new KeyValuePair<string, string>("entryId", instructableId));
                    postData.Add(new KeyValuePair<string, string>("flag", flagType));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> PostComment(string json)
        {
            if (isLogin() != true)
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, postCommentsUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);
                    client.DefaultRequestHeaders.ExpectContinue = false;

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("json", json));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> Login(string userName, string passWord)
        {
            var requestUri = String.Format("{0}{1}", BaseUri, LoginUri);
            try
            {
                var cookieJar = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookieJar,
                    UseCookies = true,
                    UseDefaultCredentials = false
                };

                using (var client = new CompressedHttpClient(handler))
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    //client.DefaultRequestHeaders.ExpectContinue = false;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var postData = new List<KeyValuePair<string, string>>();
                    //postData.Add(new KeyValuePair<string, string>("100-continue", "false"));
                    postData.Add(new KeyValuePair<string, string>("u", userName));
                    postData.Add(new KeyValuePair<string, string>("p", passWord));
                    postData.Add(new KeyValuePair<string, string>("RememberME", "true"));
                    HttpContent content = new FormUrlEncodedContent(postData);

                    client.DefaultRequestHeaders.ExpectContinue = false;
                    
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    //client.DefaultRequestHeaders.ExpectContinue = true;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Uri uri = new Uri(requestUri);
                        var responseCookies = cookieJar.GetCookies(uri);

                        string APPSERVER = String.Empty;
                        string AUTHY = String.Empty;
                        string JSESSIONID = String.Empty;

                        foreach (Cookie cookie in responseCookies)
                        {
                            string cookieName = cookie.Name;
                            string cookieValue = cookie.Value;
                            if (cookieName == "APPSERVER")
                            {
                                APPSERVER = cookieValue;
                            }
                            else if (cookieName == "authy")
                            {
                                AUTHY = cookieValue;
                            }
                            else if (cookieName == "JSESSIONID")
                            {
                                JSESSIONID = cookieValue;
                            }
                        }
                        SessionManager.UpdateLoginInfo(userName, passWord, APPSERVER, AUTHY, JSESSIONID);
                        await SessionManager.WriteSession();
                        /*if(ensureLogin != true)
                        {
                            StorageFolder loginFolder = null;
                            try
                            {
                                loginFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("loginFolder", CreationCollisionOption.OpenIfExists);
                            }
                            catch (FileNotFoundException ex)
                            {
                                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                                return null;
                            }
                            
                            if (loginFolder != null)
                            {
                                try
                                {
                                    StorageFile file = await loginFolder.CreateFileAsync("loginInfo", CreationCollisionOption.ReplaceExisting);
                                    var loginString = SerializationHelper.Serialize<LoginInfo>(loginInfo);
                                    await FileIO.WriteTextAsync(file, loginString);
                                }
                                catch (FileNotFoundException ex)
                                {
                                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                                    return null;
                                }
                            }
                        }*/
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        public void FireLoginSucceed()
        {
            if (LoginSucceed != null)
                LoginSucceed(this, null);
        }

        public async Task<RequestResult<RequestError>> FacebookLogin(string facebookToken)
        {
            var requestUri = String.Format("{0}{1}", BaseUri, FacebookLoginUri);
            try
            {
                var cookieJar = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookieJar,
                    UseCookies = true,
                    UseDefaultCredentials = false
                };

                using (var client = new CompressedHttpClient(handler))
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    //client.DefaultRequestHeaders.ExpectContinue = false;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var postData = new List<KeyValuePair<string, string>>();
                    SetClientFacebookHead(client, facebookToken);
                    //postData.Add(new KeyValuePair<string, string>("u", userName));
                    //postData.Add(new KeyValuePair<string, string>("p", passWord));
                    //postData.Add(new KeyValuePair<string, string>("RememberME", "true"));
                    //HttpContent content = new FormUrlEncodedContent(postData);

                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Uri uri = new Uri(requestUri);
                        var responseCookies = cookieJar.GetCookies(uri);
                        string APPSERVER = String.Empty;
                        string authy = String.Empty;
                        string JSESSIONID = String.Empty;

                        foreach (Cookie cookie in responseCookies)
                        {
                            string cookieName = cookie.Name;
                            string cookieValue = cookie.Value;
                            if (cookieName == "APPSERVER")
                            {
                                APPSERVER = cookieValue;
                            }
                            else if (cookieName == "authy")
                            {
                                authy = cookieValue;
                            }
                            else if (cookieName == "JSESSIONID")
                            {
                                JSESSIONID = cookieValue;
                            }
                            //_cookies.Add(cookieName, cookieValue);
                        }
                        //SessionManager.UpdateLoginInfo(userName, passWord, APPSERVER, authy, JSESSIONID);
                        //await SessionManager.WriteSession();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<bool> EnsureLogin()
        {
            try
            {
                if (_addDelays)
                    await Task.Delay(_searchDelayLength);

                if (_simulateFailure)
                    return false;

                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    var requestUri = String.Format("{0}{1}", BaseUri, GetCategoryChannelListUri);

                    if (isLogin())
                    {
                        InitClientHead(client);
                    }
                    else
                    {
                        return false;
                    }

                    HttpResponseMessage result = await client.GetAsync(requestUri);
                    
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        var sessionInfo = SessionManager.sessionInfo;
                        var loginResult = await Login(sessionInfo.userName, sessionInfo.passWord);
                        return loginResult.isSucceeded;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return false;
            }
        }

        public async Task Logout()
        {
            SessionManager.ResetLoginInfo();
            await SessionManager.WriteSession();
            FireLogoutSucceed();
        }

        public void FireLogoutSucceed()
        {
            if (LogoutSucceed != null)
                LogoutSucceed(this, null);
        }

        public bool isLogin()
        {
            return SessionManager.IsLoginSuccess();
        }

        internal async Task<RequestResult<SignUpError>> SignUp(string userName, string passWord, string email, bool newsLetter)
        {
            var requestUri = String.Format("{0}{1}", EncrptedUri, SignUpUri);
            try
            {
                using (var client = new CompressedHttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                   
                    // don't allow a timeout longer than the _maxTimeout value;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("screenName", userName));
                    postData.Add(new KeyValuePair<string, string>("password", passWord));
                    postData.Add(new KeyValuePair<string, string>("passRT", passWord));
                    postData.Add(new KeyValuePair<string, string>("email", email));
                    postData.Add(new KeyValuePair<string, string>("sendNewsletter", newsLetter?"true":"false"));
                    postData.Add(new KeyValuePair<string, string>("source", "mobile"));
                    postData.Add(new KeyValuePair<string, string>("locale", "en_US"));
                    postData.Add(new KeyValuePair<string, string>("apiKey", App.ApiKey));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<SignUpError> result = new RequestResult<SignUpError>(response);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<SignUpError>> ForgetPassword(string email)
        {
            var requestUri = String.Format("{0}{1}", BaseUri, ForgetPasswordUri);
            try
            {
                using (var client = new CompressedHttpClient())
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    // don't allow a timeout longer than the _maxTimeout value;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("email", email));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<SignUpError> result = new RequestResult<SignUpError>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<SignUpError, RequestMessage>> ResetPassword(string email, string resetcode, string password)
        {
            var requestUri = String.Format("{0}{1}", BaseUri, ResetPasswordUri);
            try
            {

                using (var client = new CompressedHttpClient())
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("email", email));
                    postData.Add(new KeyValuePair<string, string>("resetCode", resetcode));
                    postData.Add(new KeyValuePair<string, string>("password", password));
                    postData.Add(new KeyValuePair<string, string>("passRT", password));
                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<SignUpError, RequestMessage> result = new RequestResult<SignUpError, RequestMessage>(response);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<InstructableCategoryList> GetFollowedSummaries(int offset = 0, int limit = 12)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);
            /*if (_useTestData)
            {
                return PrepareTestData(TestData.Recent, limit);
            }*/

            if (_simulateFailure)
            {
                return null;
            }

            var requestUri = String.Format("{0}{1}?offset={2}&limit={3}", BaseUri, GetFollowedInstructableUri, offset, limit);
            return await GetAuthySummaryList(requestUri);
        }

        public async Task<InstructableList> GetFollowedInstructable(int offset = 0, int limit = 12)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);
            /*if (_useTestData)
            {
                return PrepareTestData(TestData.Recent, limit);
            }*/

            if (_simulateFailure)
            {
                return null;
            }

            var requestUri = String.Format("{0}{1}?offset={2}&limit={3}&{4}", BaseUri, GetFollowedInstructableUri, offset, limit, DateTime.Now);
            return await GetAuthySummaryListOriginal(requestUri);
        }

        public async Task<InstructableContestList> GetContests(int offset = 0, int limit = 12)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);

            if (_simulateFailure)
            {
                return null;
            }

            var requestUri = String.Format("{0}{1}?offset={2}&limit={3}&{4}", BaseUri, GetContestUri, offset, limit, DateTime.Now);
            return await GetContestList(requestUri);
        }

        public async Task<ContestEntries> GetContestEntries(string consetId, int offset, int limit)
        {
            var requestUri = String.Format("{0}{1}?id={2}&offset={3}&limit={4}&{5}", BaseUri, GetContestEntriesUri, consetId, offset, limit, DateTime.Now);
            string responseString = await GetFromUrl(requestUri);
            if (responseString != null)
            {
                return SerializationHelper.Deserialize<ContestEntries>(responseString);
            }
            return null;
        }

        public async Task<Contest> GetContestInfo(string consetId)
        {
            var requestUri = String.Format("{0}{1}?id={2}&{3}", BaseUri, GetContestInfoUri, consetId, DateTime.Now);
            string responseString = await GetFromUrl(requestUri);
            if (responseString != null)
            {
                return SerializationHelper.Deserialize<Contest>(responseString);
            }
            return null;
        }

        private string makeCookie()
        {
            if(SessionManager.sessionInfo.APPSERVER == String.Empty)
            {
                return String.Empty;
            }
            Dictionary<string, string> cookies = new Dictionary<string, string>(); 
            cookies.Add("APPSERVER", SessionManager.sessionInfo.APPSERVER);
            cookies.Add("authy", SessionManager.sessionInfo.authy);
            cookies.Add("JSESSIONID", SessionManager.sessionInfo.JSESSIONID);
  
            string result = "";
            int cookieCount = 0;
            foreach (var cookie in cookies) 
            {
                result = result + cookie.Key + "=" + cookie.Value;
                cookieCount++;
                if (cookieCount < cookies.Count)
                    result += "; ";
            }
            return result;

        }

        private CookieContainer InitClientCookie()
        {
            var baseAddress = new Uri(BaseUri);
            CookieContainer cookies = new CookieContainer();

            if (SessionManager.sessionInfo.APPSERVER != String.Empty)
            {
                cookies.Add(baseAddress, new Cookie("APPSERVER", SessionManager.sessionInfo.APPSERVER));
                cookies.Add(baseAddress, new Cookie("authy", SessionManager.sessionInfo.authy));
                cookies.Add(baseAddress, new Cookie("JSESSIONID", SessionManager.sessionInfo.JSESSIONID));
            }            
            return cookies;
        }

        private void InitClientHead(CompressedHttpClient client)
        {
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");            
        }

        private void SetClientFacebookHead(CompressedHttpClient client,string facebookToken)
        {
            String tokenString = "fbat_" + Constants.FacebookAppId + "=" + facebookToken;
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
            client.DefaultRequestHeaders.Add("Cookie", tokenString);
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
        } 

        private async Task<InstructableCategoryList> GetAuthySummaryList(string requestUri)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);

            if (_simulateFailure)
                return null;

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    //client.DefaultRequestHeaders.ExpectContinue = false;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    if (isLogin())
                    {
                        InitClientHead(client);
                    }
                    HttpResponseMessage result = await client.GetAsync(requestUri);
                    //var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string resultString = null;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent resultContent = result.Content;
                        resultString = await resultContent.ReadAsStringAsync();
                        var resultList = SerializationHelper.Deserialize<InstructableCategoryList>(resultString);
                        return resultList;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
            //return null;
        }

        private async Task<InstructableList> GetAuthySummaryListOriginal(string requestUri)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);

            if (_simulateFailure)
                return null;

            try
            {                
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;

                using (var client = new CompressedHttpClient(handler))
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    if (isLogin())
                    {
                        InitClientHead(client);
                    }
                    HttpResponseMessage result = await client.GetAsync(requestUri);
                    //var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string resultString = null;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent resultContent = result.Content;
                        resultString = await resultContent.ReadAsStringAsync();
                        var resultList = SerializationHelper.Deserialize<InstructableList>(resultString);
                        return resultList;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
            //return null;
        }

        public async Task<InstructableCategoryList> Search(string searchString, int offset = 0, int limit = 12)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);
            if (_useTestData)
            {
                return PrepareTestData(TestData.Recent, limit);
            }

            if (_simulateFailure)
            {
                return null;
            }

            var requestUri = String.Format("{0}{1}?type=id&search={2}&offset={3}&limit={4}&{5}", BaseUri, GetList, searchString, offset, limit, DateTime.Now);
            return await GetSummaryList(requestUri);
        }

        public async Task<InstructableCategoryList> GeteBooks(int offset = 0, int limit = 20)
        {
            if (_addDelays)
                await Task.Delay(_summaryDelayLength);
            if (_useTestData)
            {
                return PrepareTestData(TestData.eBooks, limit);
            }

            var requestUri = String.Format("{0}{1}&offset={2}&limit={3}", BaseUri, GeteBooksUri, offset, limit);
            var eBooks = await GetSummaryList(requestUri);
            if (eBooks != null)
            {
                foreach (var item in eBooks.items)
                    item.instructableType = "E";
            }
            return eBooks;
        }

        private InstructableCategoryList PrepareTestData(string data, int count)
        {
            InstructableCategoryList cList = new InstructableCategoryList();
            var collection = SerializationHelper.Deserialize<InstructableCategoryList>(data);
            for (int i = 0; i < count; i++)
            {
                cList.items.Add(collection.items[i % collection.items.Count]);
            }
            return cList;
        }

        private async Task<InstructableCategoryList> GetSummaryList(string requestUri)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);

            if (_simulateFailure)
                return null;

            try
            {
                using (var client = new CompressedHttpClient())
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    //client.DefaultRequestHeaders.ExpectContinue = false;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    var result = SerializationHelper.Deserialize<InstructableCategoryList>(responseString);
                    if (result != null)
                    {
                        foreach (var item in result.items)
                        {
                            if (String.IsNullOrEmpty(item.instructableType))
                                Debug.WriteLine(String.Format("Type is: {0}", item.instructableType));
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
            //return null;
        }

        private async Task<string> GetFromUrl(string requestUri)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);

            if (_simulateFailure)
                return null;

            try
            {
                using (var client = new CompressedHttpClient())
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    return responseString;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        private async Task<InstructableContestList> GetContestList(string requestUri)
        {
            if (_addDelays)
                await Task.Delay(_searchDelayLength);

            if (_simulateFailure)
                return null;

            try
            {
                using (var client = new CompressedHttpClient())
                {
                    // don't allow a timeout longer than the _maxTimeout value;
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    var result = SerializationHelper.Deserialize<InstructableContestList>(responseString);
                    if (result != null)
                    {
                        foreach (var item in result.contests)
                        {
                            if (String.IsNullOrEmpty(item.state))
                                Debug.WriteLine(String.Format("Type is: {0}", item.state));
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
            //return null;
        }



        internal async Task<CommentsList> GetComments(string urlString)
        {
            var requestUri = String.Format("{0}{1}?urlString={2}&{3}", BaseUri, GetCommentsUri, urlString, DateTime.Now);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    if (isLogin())
                    {
                        InitClientHead(client);
                    }
                    var responseString = await client.GetStringAsync(requestUri);
                    var result = SerializationHelper.Deserialize<CommentsList>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
            //return null;

        }

        internal async Task<Instructable> GetInstructable(string instructableID)
        {
            // only here for testing the loading indicators on the detail pages
            if (_addDelays)
                await Task.Delay(_instructableDelayLength);

            if (_useTestData)
            {
                Instructable testResult;
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail1);
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail2);
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail3);
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail4);
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail5);
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail6);

                // this next two are guides
                testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail7);
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail8);

                // this is a video
                //testResult = SerializationHelper.Deserialize<Instructable>(TestData.InstructableDetail9);

                if (testResult.steps != null)
                {
                    //foreach (var step in testResult.steps)
                    //{
                    //    step.parent = testResult;
                    //}
                }
                return testResult;
            }


            if (_simulateFailure)
                return null;


            var requestUri = String.Format("{0}{1}?id={2}&{3}", BaseUri, GetInstructableUri, instructableID, DateTime.Now);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    if (isLogin())
                    {
                        InitClientHead(client);
                    }
                    
                    var getStringTask = await client.GetStringAsync(requestUri);
                    //var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    //if (firstTask == getStringTask)
                    responseString = getStringTask;
                    responseString = responseString.Replace("\"coverImage\": \"\"", "\"coverImage\":{}");
                    var result = SerializationHelper.Deserialize<Instructable>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
            //return null;
        }

        internal async Task<InstructableCategoryList> GetSummaries(string category=null, string channel=null, string sort=null, string type="id", int offset=0, int count=20)
        {
            // make initial delay longer by 5x
            if (_addDelays)
            {
                if (count == 1)
                    await Task.Delay(_summaryDelayLength * 5);
                else
                    await Task.Delay(_summaryDelayLength);
            }

            if (_useTestData)
            {
                return PrepareTestData(TestData.Featured, count);
            }
            
            if (_simulateFailure)
                return null;

            string channelParam = "";
            if (channel != null)
                channelParam = String.Format("&channel={0}", channel);

            string categoryParam = "";
            if (category != null)
                categoryParam = String.Format("&category={0}", category);

            string sortParam = "";
            if (sort != null)
                sortParam = String.Format("&sort={0}", sort);

            string requestUri = String.Format("{0}{1}?offset={2}&limit={3}&type={4}{5}{6}{7}&{8}", BaseUri, GetList, offset, count, type, sortParam, categoryParam, channelParam, DateTime.Now);

            InstructableCategoryList result = await GetSummaryList(requestUri);
            if (result != null && result.items != null)
            {
                if (type == "guide&ebookFlag=true")
                {
                    foreach (var item in result.items)
                        item.instructableType = "E";
                }
                if (type == "guide")
                {
                    foreach (var item in result.items)
                        item.instructableType = "G";
                }
                if (type == "video")
                {
                    foreach (var item in result.items)
                        item.instructableType = "V";
                }
            }
            return result;
        }

        internal async Task<Channels> GetChannels()
        {
            try
            {
                if (_addDelays)
                    await Task.Delay(_searchDelayLength);

                if (_useTestData)
                {
                    return SerializationHelper.Deserialize<Channels>(TestData.Categories);
                }
 
                if (_simulateFailure)
                    return null;
                
                using (var client = new CompressedHttpClient())
                {
                    //client.DefaultRequestHeaders.ExpectContinue = false;
                    var requestUri = String.Format("{0}{1}", BaseUri, GetCategoryChannelListUri);

                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    Channels result = SerializationHelper.Deserialize<Channels>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<UserProfile> GetUserProfile(string screenName, bool allSubsriptions = false)
        {
            try
            {
                if (_addDelays)
                    await Task.Delay(_searchDelayLength);

                if (_useTestData)
                {
                    return SerializationHelper.Deserialize<UserProfile>(TestData.Categories);
                }
 
                if (_simulateFailure)
                    return null;

                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    var requestUri = String.Format("{0}{1}?screenName={2}&{3}", BaseUri, GetAuthorUri, screenName, DateTime.Now);

                    if(isLogin())
                    {
                        InitClientHead(client);
                    }

                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    responseString = responseString.Replace("\"coverImage\": \"\"", "\"coverImage\":{}");
                    UserProfile result = SerializationHelper.Deserialize<UserProfile>(responseString);
                    
                    if (allSubsriptions == true && result.subscriptions != null )
                    {
                        result.subscriptionsForLoginUser = null;
                        var subscriptions = await GetFollowings(screenName, 0, result.subscriptionsCount);
                        result.subscriptionsForLoginUser = subscriptions.subscriptions;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<RequestResult<RequestError>> SaveUserProfile(UserProfile userProfile)
        {
            try
            {
                if (_addDelays)
                    await Task.Delay(_searchDelayLength);

                if (_simulateFailure)
                    return null;

                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    var requestUri = String.Format("{0}{1}", BaseUri, SaveProfileUri);

                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);

                    if (isLogin())
                    {
                        InitClientHead(client);
                    }

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("about", userProfile.about));
                    postData.Add(new KeyValuePair<string, string>("interests", userProfile.interests));
                    postData.Add(new KeyValuePair<string, string>("location", userProfile.location));
                    postData.Add(new KeyValuePair<string, string>("gender", userProfile.gender));
                    postData.Add(new KeyValuePair<string, string>("imageId", userProfile.avatarId));

                    HttpContent content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    RequestResult<RequestError> result = new RequestResult<RequestError>(response);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return null;
            }
        }

        internal async Task<UploadResult> UploadPhoto(byte[] ImageData)
        {
            if (!isLogin())
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, UploadUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);

                    var imageContent = new ByteArrayContent(ImageData);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                    var content = new MultipartFormDataContent();
                    content.Add(imageContent, "image", "image.jpg");

                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = SerializationHelper.Deserialize<UploadResult>(resultString);
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task UploadPhotos(Step step, List<StorageFile> files, bool retry = false)
        {
            if (files.Count <= 0)
                return ;

            try
            {
                var requestUri = String.Format("{0}{1}", BaseUri, UploadUri);

                BackgroundUploader uploader = new BackgroundUploader();
                uploader.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                uploader.SetRequestHeader("Accept-Encoding", "gzip");
                uploader.SetRequestHeader("Cookie", makeCookie());

                // Flush the upload every 3 images to avoid a single huge upload
                const int ONCEMAX = 3;

                List<BackgroundTransferContentPart> parts = new List<BackgroundTransferContentPart>();
                for (int i = 0; i < files.Count; i++)
                {
                    BackgroundTransferContentPart part = new BackgroundTransferContentPart("File" + i, files[i].Name);
                    part.SetFile(files[i]);
                    parts.Add(part);

                    if (parts.Count % ONCEMAX == 0 || i == files.Count - 1)
                    {
                        List<BackgroundTransferContentPart> upParts = new List<BackgroundTransferContentPart>();
                        upParts.AddRange(parts);
                        parts.Clear();

                        UploadOperation upload = await uploader.CreateUploadAsync(new Uri(requestUri), upParts);

                        // Force to update the files information at the first moment we got the responses message during the progress
                        // Ideally we should use the Complete event to update the files information while it's a little bit late to do so during the observation
                        upload.StartAsync().Progress = delegate
                        {
                            ResponseInformation responseInfo = upload.GetResponseInformation();
                            //If upload request return 401 error, it might be the session expired, retry to login and upload photos again.
                            if (responseInfo != null && responseInfo.StatusCode == (uint)HttpStatusCode.Unauthorized && retry == false)
                            {
                                var sessionInfo = SessionManager.sessionInfo;
                                var loginTask = Login(sessionInfo.userName, sessionInfo.passWord).AsAsyncOperation().AsTask();
                                loginTask.Wait();
                                UploadPhotos(step, files, true).Wait();
                            }
                            if (responseInfo != null && responseInfo.StatusCode == (uint)HttpStatusCode.Created)
                            {
                                using (var response = upload.GetResultStreamAt(0))
                                {
                                    uint size = (uint)upload.Progress.BytesReceived;
                                    IBuffer buffer = new Windows.Storage.Streams.Buffer(size);
                                    var readTask = response.ReadAsync(buffer, size, InputStreamOptions.None).AsTask();
                                    readTask.Wait();
                                    var buf = readTask.Result;

                                    using (var dr = DataReader.FromBuffer(buf))
                                    {
                                        string resultString = dr.ReadString(dr.UnconsumedBufferLength);
                                        var result = SerializationHelper.Deserialize<UploadResult>(resultString);
                                        if (result != null && result.loaded != null)
                                        {
                                            foreach (var rf in result.loaded)
                                            {
                                                foreach (var file in step.ImageNames)
                                                {
                                                    if (file.name == rf.name)
                                                    {
                                                        step.files.Add(rf);
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }
        }

        internal async Task DiscoverActiveUploadsAsync()
        {
            IReadOnlyList<UploadOperation> uploads = null;
            try
            {
                uploads = await BackgroundUploader.GetCurrentUploadsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                return;
            }

            if (uploads.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (UploadOperation upload in uploads)
                {
                    // Attach progress and completion handlers.
                    tasks.Add(HandleUploadAsync(upload, false));
                }

                await Task.WhenAll(tasks);

                bool isSucceed = true;
                foreach(UploadOperation upload in uploads)
                {
                    ResponseInformation responseInfo = upload.GetResponseInformation();
                    if (responseInfo != null && responseInfo.StatusCode == (uint)HttpStatusCode.Unauthorized)
                    {
                        isSucceed = false;
                        break;
                    }
                }

                if (!isSucceed)
                    await DiscoverActiveUploadsAsync();
            }
        }

        
        private async Task HandleUploadAsync(UploadOperation upload, bool isNewUpload)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                if (isNewUpload)
                {
                    // Start the upload and attach a progress handler.
                    await upload.StartAsync().AsTask(cts.Token);
                }
                else
                {
                    // The upload was already running when the application started, re-attach the progress handler.
                    await upload.AttachAsync().AsTask(cts.Token);
                }


                ResponseInformation responseInfo = upload.GetResponseInformation();
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }
        }



        internal async Task<InstructableList> GetDrafts(string screenName, int offset = 0, int limit = 12)
        {
            if (!isLogin())
                return null;

            var requestUri = String.Format("{0}{1}?screenName={2}&offset={3}&limit={4}", BaseUri, GetDraftsUri, screenName, offset, limit);
            try{
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);

                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    responseString = responseString.Replace("\"coverImage\": \"\"", "\"coverImage\":{}");
                    InstructableList result = SerializationHelper.Deserialize<InstructableList>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<InstructableFavoriteList> GetFavorites(string screenName, int offset = 0, int limit = 12)
        {
            var requestUri = String.Format("{0}{1}?screenName={2}&offset={3}&limit={4}&{5}", BaseUri, GetFavoriteUri, screenName, offset, limit, DateTime.Now);
            try
            {
                using (var client = new CompressedHttpClient())
                {
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    responseString = responseString.Replace("\"coverImage\": \"\"", "\"coverImage\":{}");
                    InstructableFavoriteList result = SerializationHelper.Deserialize<InstructableFavoriteList>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<InstructableList> GetInstructablesByAuthor(string screenName, int offset = 0, int limit = 12)
        {
            var requestUri = String.Format("{0}{1}?screenName={2}&offset={3}&limit={4}", BaseUri, GetInstrubtablesByAuthor, screenName, offset, limit);
            try
            {
                using (var client = new CompressedHttpClient())
                {
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    responseString = responseString.Replace("\"coverImage\": \"\"", "\"coverImage\":{}");
                    InstructableList result = SerializationHelper.Deserialize<InstructableList>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<FollowersList> GetFollowers(string screenName, int offset = 0, int limit = 12)
        {
            var requestUri = String.Format("{0}{1}?screenName={2}&offset={3}&limit={4}&{5}", BaseUri, GetFollowersUri, screenName, offset, limit, DateTime.Now);
            try
            {
                using (var client = new CompressedHttpClient())
                {
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    FollowersList result = SerializationHelper.Deserialize<FollowersList>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<SubscriptionsList> GetFollowings(string screenName, int offset = 0, int limit = 12)
        {
            var requestUri = String.Format("{0}{1}?screenName={2}&offset={3}&limit={4}&{5}", BaseUri, GetSubscriptionsUri, screenName, offset, limit, DateTime.Now);
            try
            {
                using (var client = new CompressedHttpClient())
                {
                    var timeoutTask = Task.Delay(_maxTimeoutMilliseconds);
                    var getStringTask = client.GetStringAsync(requestUri);
                    var firstTask = await Task.WhenAny(timeoutTask, getStringTask);
                    string responseString = null;
                    if (firstTask == getStringTask)
                        responseString = getStringTask.Result;
                    SubscriptionsList result = SerializationHelper.Deserialize<SubscriptionsList>(responseString);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<CreateResult> NewInstructable(Instructable instructable)
        {
            if (!isLogin())
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, NewInstructableUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);

                    string json = SerializationHelper.Serialize<Instructable>(instructable);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = SerializationHelper.Deserialize<CreateResult>(resultString);
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<CreateResult> SaveInstructable(Instructable instructable)
        {
            if (!isLogin())
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, SaveInstructableUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);

                    string json = SerializationHelper.Serialize<Instructable>(instructable);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = SerializationHelper.Deserialize<CreateResult>(resultString);
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }

        internal async Task<CreateResult> PublishInstructable(Instructable instructable)
        {
            if (!isLogin())
                return null;

            var requestUri = String.Format("{0}{1}", BaseUri, PublishInstructableUri);
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = InitClientCookie();
                handler.CookieContainer = cookies;
                using (var client = new CompressedHttpClient(handler))
                {
                    InitClientHead(client);

                    string json = SerializationHelper.Serialize<Instructable>(instructable);

                    // TODO:Using a tricky way to replace the license for the moment
                    string license = instructable.license.Abbr();
                    json = Regex.Replace(json, "{\\\"fullName.*?}", "\"" + license + "\"");

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = SerializationHelper.Deserialize<CreateResult>(resultString);
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0} , {1} ", requestUri, ex.Message));
                return null;
            }
        }
    }
}
