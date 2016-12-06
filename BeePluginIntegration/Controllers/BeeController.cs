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
    String clientId = "YOUR_CLIENT_ID";
    String clientSecret = "YOUR_CLIENT_SECRET";

    // POST api/<controller>
    [HttpPost]
    [Route("")]
    public HttpResponseMessage Post()
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



    #region Private method
    /// <summary>
    ///     Method used to prepare requesto to Bee Authorizator Server
    /// </summary>
    /// <param name="endPoint">The end-point to call</param>
    /// <returns>The request to send to Bee Authorizator Server</returns>
    private HttpWebRequest CreateWebRequestToBeeAuthorizatorServer(string endPoint)
    {
      byte[] data = Encoding.UTF8.GetBytes(String.Format("grant_type=password&client_id={0}&client_secret={1}", clientId, clientSecret));

      // Create request
      var request = (HttpWebRequest)WebRequest.Create(endPoint);

      // parametrization of request
      request.Method = "POST";
      request.ContentLength = data.Length;
      request.ContentType = "application/x-www-form-urlencoded";

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