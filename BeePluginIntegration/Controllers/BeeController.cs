using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace BeePluginIntegration.Controllers
{
  [RoutePrefix("api/auth")]
  public class BeeController : ApiController
  {
    String clientId = "de459fdb-7d7b-4fcb-9d03-2131760633a5";
    String clientSecret = "5MJIZsTqT8bbydXSqDrNKzQWaWTdWnxxZwwn7z3TisxoxLxDWo4R";
    String uid = "test1-dotnet";

    // POST api/<controller>
    [HttpPost]
    [Route("")]
    public HttpResponseMessage Post()
    {
      try
      {
        var httpRequst = HttpContext.Current.Request;

        // TODO: configuration to get the Bee Authorizator Server  end-point
        String BeeEndPointAuthorizatorServer = ConfigurationManager.AppSettings["appAuth"];

        // Create request to get the Authorization from Server Bee
        HttpWebRequest request = CreateWebRequestToBeeAuthorizatorServer(BeeEndPointAuthorizatorServer);

        using (var response = (HttpWebResponse)request.GetResponse())
        {
          // If response is not 200... throw new App Exception
          if (response.StatusCode != HttpStatusCode.OK)
          {
            string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
            throw new ApplicationException(message);
          }

          // grab the response  
          using (var responseStream = response.GetResponseStream())
          {
            using (var reader = new StreamReader(responseStream))
            {
              // read the response from AuthorizatorServer 
              string respBeeAuthorizatorServer = reader.ReadToEnd();

              // Return the response
              return new HttpResponseMessage()
              {
                Content = new StringContent(JObject.Parse(respBeeAuthorizatorServer).ToString())
              };
            }
          }
        }
      }
      catch (Exception ex)
      {
        // Return error details for debugging
        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = new StringContent($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}")
        };
      }
    }



    #region Private method
    /// <summary>
    ///     Method used to prepare request to Bee Authorizator Server
    /// </summary>
    /// <param name="endPoint">The end-point to call</param>
    /// <returns>The request to send to Bee Authorizator Server</returns>
    private HttpWebRequest CreateWebRequestToBeeAuthorizatorServer(string endPoint)
    {
      // Create JSON payload
      var jsonPayload = new
      {
        grant_type = "password",
        client_id = clientId,
        client_secret = clientSecret,
        uid = uid
      };
      
      string jsonString = JObject.FromObject(jsonPayload).ToString();
      byte[] data = Encoding.UTF8.GetBytes(jsonString);

      // Create request
      var request = (HttpWebRequest)WebRequest.Create(endPoint);

      // parametrization of request
      request.Method = "POST";
      request.ContentLength = data.Length;
      request.ContentType = "application/json";
      request.Accept = "application/json";

      // put body for currrent request to POST
      using (Stream s = request.GetRequestStream())
      {
        s.Write(data, 0, data.Length);
        s.Close();
      }

      return request;
    }
    #endregion
  }
}