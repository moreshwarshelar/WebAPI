// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// The same code for the controller is used in both chapters of the tutorial. 
// In the first chapter this is just a protected API (ENABLE_OBO is not set)
// In this chapter, the Web API calls a downstream API on behalf of the user (OBO)
#define ENABLE_OBO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using GREATRoomAPI.Models;
using System.IO;

namespace GREATRoomAPI.Controllers
{
    [Authorize]
    [Route("api/deals")]
    public class DealsController : Controller
    {
        public DealsController(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        readonly ITokenAcquisition _tokenAcquisition;

        static readonly ConcurrentBag<Deals> TodoStore = new ConcurrentBag<Deals>();

        /// <summary>
        /// The Web API will only accept tokens 1) for users, and 
        /// 2) having the access_as_user scope for this API
        /// </summary>
        static readonly string[] scopeRequiredByApi = new string[] { "api-access" };

        // GET: api/values
        [HttpGet]
        public List<Deals> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return GetDealsOnBehalfOfUser().GetAwaiter().GetResult();
            //return new List<Deals>() { new Deals() { ID = "a", Address = "b", FundName = "c", PropertyType = "d", Title = "e" } };
        }

        [HttpGet("documents/{dealID}")]
        public List<DealDocument> GetDealDocuments(string dealID)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return GetDealDocumentsOnBehalfOfUser(dealID).GetAwaiter().GetResult();
            //return new List<DealDocument>() { new DealDocument() { Title = "doc1",Created="me", Modified="me" } };
        }

        [HttpPost("documents/{dealID}/upload")]
        public List<DealDocument> UploadDealDocuments(IFormFile fileKey, string dealID)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                fileKey.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return UploadDocumentsOnBehalfOfUser(bytes, dealID).GetAwaiter().GetResult();
            //return new List<DealDocument>() { new DealDocument() { Title = "doc1",Created="me", Modified="me" } };
        }


        [HttpGet("hardcoded")]
        public ActionResult<List<Deals>> GetSPListItems()
        {
            List<Deals> Deals = new List<Deals>();
            Deals.Add(new Deals() { Address = "7057 Tanglewood Ave. Kenosha, WI 53140", Title = "Tanglewood(USA)", FundName = "ABC", PropertyType = "Family", DealStage = "Awarded", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "Salunke, Vinayak", Market = "Austin", SubMarket = "Bakway", PropertyRegion = "East", TIAAValuation = "$760,000,000", SFUnit = "565,656", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "674 County Dr. Aberdeen, SD 57401", Title = "Aberdeen(USA)", FundName = "XYZ", PropertyType = "ORG", DealStage = "Closed", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "Bachhav, Atul", Market = "Columbus", SubMarket = "East", PropertyRegion = "West", TIAAValuation = "$760,000,000", SFUnit = "303", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "112 Grandrose Street Fairfax, VA 22030", Title = "Grandrose(USA)", FundName = "ERT", PropertyType = "Finance", DealStage = "LOI", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "Yes", PrimaryAcquisitionOfficer = "Mintz, Kyle j", Market = "Boston", SubMarket = "West Boston", PropertyRegion = "South", TIAAValuation = "$760,000,000", SFUnit = "600", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "318 Pleasant Ave. Milledgeville, GA 31061", Title = "Pleasant(USA)", FundName = "HYT", PropertyType = "Retail", DealStage = "New Deal", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "FaesenKloet, Kees V", Market = "Charlotte", SubMarket = "ABC", PropertyRegion = "North", TIAAValuation = "$760,000,000", SFUnit = "560", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "8766 Liberty Street Defiance, OH 43512", Title = "Liberty(USA)", FundName = "KYU", PropertyType = "Commercial", DealStage = "Prospect", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "Yes", PrimaryAcquisitionOfficer = "Mintz, Kyle j", Market = "Chicago", SubMarket = "Midway", PropertyRegion = "East", TIAAValuation = "$760,000,000", SFUnit = "569,87", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "44 Adams Road Grandville, MI 49418", Title = "Grandville(USA)", FundName = "LOJ", PropertyType = "Finance", DealStage = "Under Consideration", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "Salunke, Vinayak", Market = "Austin", SubMarket = "West", PropertyRegion = "West", TIAAValuation = "$760,000,000", SFUnit = "632", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "7672 Bald Hill Lane", Title = "Bald(USA)", FundName = "HGF", PropertyType = "Retail", DealStage = "Onboarded", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "Bachhav, Atul", Market = "Columbus", SubMarket = "East Col", PropertyRegion = "South", TIAAValuation = "$760,000,000", SFUnit = "785", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "Los Angeles, CA 90019", Title = "Angeles(USA)", FundName = "YUT", PropertyType = "Commercial", DealStage = "Awarded", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "Yes", PrimaryAcquisitionOfficer = "Mintz, Kyle j", Market = "Charlotte", SubMarket = "East Charlotte", PropertyRegion = "South", TIAAValuation = "$760,000,000", SFUnit = "962,987", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "Folsom, CA 95630", Title = "Folsom(USA)", FundName = "POI", PropertyType = "Commercial", DealStage = "Closed", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "Bachhav, Atul", Market = "Austin", SubMarket = "Bakway", PropertyRegion = "East", TIAAValuation = "$760,000,000", SFUnit = "236", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "Sunnyvale, CA 94087", Title = "Sunnyvale(USA)", FundName = "NBT", PropertyType = "Family", DealStage = "LOI", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "Yes", PrimaryAcquisitionOfficer = "Salunke, Vinayak", Market = "Columbus", SubMarket = "ABC", PropertyRegion = "South", TIAAValuation = "$760,000,000", SFUnit = "874", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "Lake Forest, CA 92630", Title = "Forest(USA)", FundName = "LRE", PropertyType = "Retail", DealStage = "LOI", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "No", PrimaryAcquisitionOfficer = "Mintz, Kyle j", Market = "Charlotte", SubMarket = "East Charlotte", PropertyRegion = "West", TIAAValuation = "$760,000,000", SFUnit = "652", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "30 East Vine Lane", Title = "Lane(USA)", FundName = "MBT", PropertyType = "Family", DealStage = "Awarded", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "Yes", PrimaryAcquisitionOfficer = "Bachhav, Atul", Market = "Austin", SubMarket = "Bakway", PropertyRegion = "South", TIAAValuation = "$760,000,000", SFUnit = "215", InvestmentID = "" });
            Deals.Add(new Deals() { Address = "640 N. Foster Street", Title = "Street(USA)", FundName = "VBT", PropertyType = "Commercial", DealStage = "Awarded", DealStatus = "Active", DealType = "Nuveen", MNPIStatus = "Yes", PrimaryAcquisitionOfficer = "Mintz, Kyle j", Market = "Columbus", SubMarket = "Bakway", PropertyRegion = "East", TIAAValuation = "$760,000,000", SFUnit = "45", InvestmentID = "" });

            return Deals;
        }

        public async Task<List<Deals>> GetDealsOnBehalfOfUser()
        {
            string[] scopes = { ".default" };

            // we use MSAL.NET to get a token to call the API On Behalf Of the current user
            try
            {
                string accessToken = await _tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(scopes);
                dynamic me = await GetDealsOnBehalfOfUser(accessToken);
                return me;
            }
            catch (MsalUiRequiredException ex)
            {
                _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex);
                return null;
            }
        }

        private static async Task<List<Deals>> GetDealsOnBehalfOfUser(string accessToken)
        {
            GraphServiceClient graphClient = new GraphServiceClient(
            new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    // Append the access token to the request.
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("bearer", accessToken);
                }));

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("expand", "fields(select=ID,Title,Address,FundName,PropertyType,DealStage,DealStatus,DealType,InvestmentID,Market,MNPIStatus,PrimaryAcquisitionOfficer,PropertyRegion,PropertyType,SFUnit,SubMarket,TIAAValuation)")
            };
            var items = await graphClient.Sites["root"].SiteWithPath("/sites/TestExternal").Lists["Deal List"].Items.Request(queryOptions).GetAsync();
            List<Deals> deals = new List<Deals>();
            foreach (var item in items)
            {
                deals.Add(new Deals()
                {
                    ID = item.Fields.Id.ToString(),
                    Title = item.Fields.AdditionalData["Title"].ToString(),
                    Address = item.Fields.AdditionalData["Address"].ToString(),
                    FundName = item.Fields.AdditionalData["FundName"].ToString(),
                    PropertyType = item.Fields.AdditionalData["PropertyType"].ToString(),
                    DealStage = item.Fields.AdditionalData["DealStage"].ToString(),
                    DealStatus = item.Fields.AdditionalData["DealStatus"].ToString(),
                    DealType = item.Fields.AdditionalData["DealType"].ToString(),
                    InvestmentID = item.Fields.AdditionalData["InvestmentID"].ToString(),
                    Market = item.Fields.AdditionalData["Market"].ToString(),
                    MNPIStatus = item.Fields.AdditionalData["MNPIStatus"].ToString(),
                    PrimaryAcquisitionOfficer = item.Fields.AdditionalData["PrimaryAcquisitionOfficer"].ToString(),
                    PropertyRegion = item.Fields.AdditionalData["PropertyRegion"].ToString(),
                    SFUnit = item.Fields.AdditionalData["SFUnit"].ToString(),
                    SubMarket = item.Fields.AdditionalData["SubMarket"].ToString(),
                    TIAAValuation = item.Fields.AdditionalData["TIAAValuation"].ToString()
                });
            }
            return deals;
        }

        public async Task<List<DealDocument>> GetDealDocumentsOnBehalfOfUser(string dealID)
        {
            string[] scopes = { "Sites.Read.All" };

            // we use MSAL.NET to get a token to call the API On Behalf Of the current user
            try
            {
                string accessToken = await _tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(scopes);
                dynamic me = await GetDealDocumentsOnBehalfOfUser(accessToken, dealID);
                return me;
            }
            catch (MsalUiRequiredException ex)
            {
                _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex);
                return null;
            }
        }

        private static async Task<List<DealDocument>> GetDealDocumentsOnBehalfOfUser(string accessToken, string dealID)
        {
            GraphServiceClient graphClient = new GraphServiceClient(
            new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    // Append the access token to the request.
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("bearer", accessToken);
                }));

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("expand", "fields"),
                new QueryOption("filter","Fields/DealID eq '"+dealID+"'")
            };
            var items = await graphClient.Sites["root"].SiteWithPath("/sites/TestExternal").Lists["Documents"].Items.Request(queryOptions).GetAsync();
            List<DealDocument> deals = new List<DealDocument>();
            foreach (var item in items)
            {
                deals.Add(new DealDocument()
                {
                    Title = item.Fields.AdditionalData["Title"].ToString(),
                    Created = item.CreatedDateTime.Value.ToString(),
                    Modified = item.LastModifiedDateTime.Value.ToString(),
                    Url = item.WebUrl,
                    CreatedBy = item.CreatedBy.User.DisplayName,
                    LastModifiedBy = item.LastModifiedBy.User.DisplayName
                });
            }
            return deals;
        }

        public async Task<List<DealDocument>> UploadDocumentsOnBehalfOfUser(byte[] bytes,string dealID)
        {
            string[] scopes = { "Sites.Manage.All" };

            // we use MSAL.NET to get a token to call the API On Behalf Of the current user
            try
            {
                string accessToken = await _tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(scopes);
                dynamic me = await UploadDocumentsOnBehalfOfUser(accessToken, bytes, dealID);
                return me;
            }
            catch (MsalUiRequiredException ex)
            {
                _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(scopes, ex);
                return null;
            }
        }

        private static async Task<bool> UploadDocumentsOnBehalfOfUser(string accessToken, byte[] bytes, string dealID)
        {
            GraphServiceClient graphClient = new GraphServiceClient(
            new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    // Append the access token to the request.
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("bearer", accessToken);
                }));

            Stream filecontents = new MemoryStream(bytes);//{103863a8-14f3-4bc3-8d6f-51f416c14595}
            var items = await graphClient.Sites["root"].Drive.Root.ItemWithPath("Angular commands.txt").Content.Request().PutAsync<DriveItem>(filecontents);
            //var items = await graphClient.Sites["root"].SiteWithPath("/sites/TestExternal").Drive.Root.Content.Request().PutAsync<DriveItem>(filecontents);
            return true;
        }
    }
}
