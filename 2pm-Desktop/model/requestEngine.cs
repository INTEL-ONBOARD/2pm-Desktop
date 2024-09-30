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
using _2pm_Desktop.view;
using System.Net.Http.Headers;

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


        // Assuming this is a class-level variable
 // Declare a class-level token variable

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
                        // Correctly access the token from the response
                        token = responseData.data.token; // Change this line to access the token correctly
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

        public static async Task<bool> IsUserInDepartmentAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://2pm.revostack.com");
                System.Diagnostics.Debug.WriteLine("here is the token : " + token);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Set headers for the request
                client.DefaultRequestHeaders.Add("Accept", "application/json");  // Expecting a JSON response
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");

                try
                {
                    HttpResponseMessage myHttpResponse = await client.GetAsync("/api/v1/user-profile");

                    if (myHttpResponse.IsSuccessStatusCode)
                    {
                        string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        int departmentId = jsonResponse.data.user_detail.department;

                        return departmentId == 4;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Response Status Code: {myHttpResponse.StatusCode}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return false;
                }
            }
        }

        public static async Task<string> UploadReportData(MainWindow win)
        {
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://2pm.revostack.com");

                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");


                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    
                    var formData = new List<KeyValuePair<string, string>>
            {
                { new KeyValuePair<string, string>("type", "4") } 
            };

                    
                    int hourlyReportIndex = 0;
                    bool dailyReportAdded = false;

                    foreach (var child in win.reportPnaelView.Children) 
                    {
                        
                        System.Diagnostics.Debug.WriteLine($"Child Type: {child.GetType().Name}");

                        
                        if (child is report rp)
                        {
                            
                            System.Diagnostics.Debug.WriteLine($"Report ID: {rp.id}");
                            System.Diagnostics.Debug.WriteLine($"Report Input: {rp.input}");

                            
                            if (rp.id == "0")
                            {
                                formData.Add(new KeyValuePair<string, string>("daily_report", rp.input.ToString())); 
                                System.Diagnostics.Debug.WriteLine($"Daily report added: {rp.input}");
                                dailyReportAdded = true; 
                            }
                            
                            else
                            {
                                formData.Add(new KeyValuePair<string, string>($"hourly_report[{hourlyReportIndex}][hour]", hourlyReportIndex.ToString()));
                                formData.Add(new KeyValuePair<string, string>($"hourly_report[{hourlyReportIndex}][report]", rp.input.ToString()));
                                System.Diagnostics.Debug.WriteLine($"Hourly report {hourlyReportIndex} added: {rp.input}");
                                hourlyReportIndex++;
                            }
                        }
                        else
                        {
                            
                            System.Diagnostics.Debug.WriteLine("Child is not of type 'report'.");
                        }
                    }

                    
                    System.Diagnostics.Debug.WriteLine("Form Data:");
                    foreach (var kvp in formData)
                    {
                        System.Diagnostics.Debug.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }

                    
                    if (!dailyReportAdded)
                    {
                        System.Diagnostics.Debug.WriteLine("Daily report was not added.");
                    }

                    var content = new FormUrlEncodedContent(formData);

                    
                    string contentString = await content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("+++++++++++++++++++");
                    System.Diagnostics.Debug.WriteLine(contentString);

                    
                    HttpResponseMessage myHttpResponse = await client.PostAsync("/api/v1/attendance", content);

                    
                    System.Diagnostics.Debug.WriteLine("---------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine($"Status Code: {myHttpResponse.StatusCode}");
                    string responseContent = await myHttpResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                    
                    return myHttpResponse.IsSuccessStatusCode ? responseContent : "false"; 
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
