using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace _2pm_Desktop.model
{
    internal class requestEngine
    {
        private String baseUrl = "https://2pm.revostack.com/";
        private static String token = "";
        public requestEngine(string targeturl)
        {
            this.baseUrl = targeturl;
        }

        //public static bool validUser()
        //{
        //    Post();
        //    return false;
        //}


        public static async Task<string> logInUser(string une, string pwd)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");

                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");

                // Use FormUrlEncodedContent to send data in application/x-www-form-urlencoded format
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("email", une),
            new KeyValuePair<string, string>("password", pwd)
        });

                try
                {
                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/auth/login", content);

                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(myHttpResponse.StatusCode);

                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);

                    // Parse the response JSON
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Check the 'success' field in the JSON response
                    if (responseData.success == true)
                    {
                        token = responseData.token;
                        return "true";

                    }
                    else
                    {
                        return "false";
                    }
                }
                catch (Exception ex)
                {
                    return "false";
                }
            }
        }
        public static async Task<string> logOutUser()
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");

                // Add the Authorization header with the Bearer token
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/auth/logout", null);

                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(myHttpResponse.StatusCode);
                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);

                    // Check the response status code and return appropriate result
                    if (myHttpResponse.IsSuccessStatusCode)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return "false";
                }
            }
        }
        public static async Task<string> punchin()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    // Create form data
                    var formData = new Dictionary<string, string>
                    {
                        { "type", "1" },
                        //{ "daily_report", "Lorem Ipsum is simply dummy text of the printing and typesetting industry..." }
                    };

                    var content = new FormUrlEncodedContent(formData);

                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/attendance", content);

                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(myHttpResponse.StatusCode);
                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);

                    if (myHttpResponse.IsSuccessStatusCode)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return "false";
                }
            }
        }
        public static async Task<string> breakin()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    // Create form data
                    var formData = new Dictionary<string, string>
                    {
                        { "type", "2" },
                        //{ "daily_report", "Lorem Ipsum is simply dummy text of the printing and typesetting industry..." }
                    };

                    var content = new FormUrlEncodedContent(formData);

                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/attendance", content);

                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(myHttpResponse.StatusCode);
                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);

                    if (myHttpResponse.IsSuccessStatusCode)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return "false";
                }
            }
        }
        public static async Task<string> breakout()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    // Create form data
                    var formData = new Dictionary<string, string>
                    {
                        { "type", "3" },
                        //{ "daily_report", "Lorem Ipsum is simply dummy text of the printing and typesetting industry..." }
                    };

                    var content = new FormUrlEncodedContent(formData);

                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/attendance", content);

                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(myHttpResponse.StatusCode);
                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);

                    if (myHttpResponse.IsSuccessStatusCode)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return "false";
                }
            }
        }
        public static async Task<string> punchout(int type, bool isDefault, Dictionary<string, string> additionalFormData = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    // Adjust the dailyReport based on the isDefault parameter


                    // Create form data
                    var formData = new Dictionary<string, string>
                    {
                        { "type", type.ToString() },

                    };

                    // Add additional form data if isDefault is false
                    if (!isDefault && additionalFormData != null)
                    {
                        foreach (var kvp in additionalFormData)
                        {
                            formData[kvp.Key] = kvp.Value;
                        }
                    }

                    var content = new FormUrlEncodedContent(formData);
                    // Read the content as a string for debugging
                    string contentString = await content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine("+++++++++++++++++++");
                    System.Diagnostics.Debug.WriteLine(contentString);
                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/attendance", content);

                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(myHttpResponse.StatusCode);
                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseContent);

                    if (myHttpResponse.IsSuccessStatusCode)
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return "false";
                }
            }
        }



    }
}
