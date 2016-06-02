using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp;
using VirtoCommerce.CartModule.Client.Client;
using VirtoCommerce.CartModule.Client.Model;

namespace VirtoCommerce.CartModule.Client.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IVirtoCommerceCartApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ShoppingCart</returns>
        ShoppingCart CartModuleCreate(ShoppingCart cart);

        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        ApiResponse<ShoppingCart> CartModuleCreateWithHttpInfo(ShoppingCart cart);
        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns></returns>
        void CartModuleDeleteCarts(List<string> ids);

        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> CartModuleDeleteCartsWithHttpInfo(List<string> ids);
        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>ShoppingCart</returns>
        ShoppingCart CartModuleGetCartById(string id);

        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        ApiResponse<ShoppingCart> CartModuleGetCartByIdWithHttpInfo(string id);
        /// <summary>
        /// Get shopping cart by store id and customer id
        /// </summary>
        /// <remarks>
        /// Returns shopping cart or null if it is not found
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>ShoppingCart</returns>
        ShoppingCart CartModuleGetCurrentCart(string storeId, string customerId);

        /// <summary>
        /// Get shopping cart by store id and customer id
        /// </summary>
        /// <remarks>
        /// Returns shopping cart or null if it is not found
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        ApiResponse<ShoppingCart> CartModuleGetCurrentCartWithHttpInfo(string storeId, string customerId);
        /// <summary>
        /// Get payment methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>List&lt;PaymentMethod&gt;</returns>
        List<PaymentMethod> CartModuleGetPaymentMethods(string cartId);

        /// <summary>
        /// Get payment methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>ApiResponse of List&lt;PaymentMethod&gt;</returns>
        ApiResponse<List<PaymentMethod>> CartModuleGetPaymentMethodsWithHttpInfo(string cartId);
        /// <summary>
        /// Get payment methods for store
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>List&lt;PaymentMethod&gt;</returns>
        List<PaymentMethod> CartModuleGetPaymentMethodsForStore(string storeId);

        /// <summary>
        /// Get payment methods for store
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>ApiResponse of List&lt;PaymentMethod&gt;</returns>
        ApiResponse<List<PaymentMethod>> CartModuleGetPaymentMethodsForStoreWithHttpInfo(string storeId);
        /// <summary>
        /// Get shipping methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>List&lt;ShippingMethod&gt;</returns>
        List<ShippingMethod> CartModuleGetShipmentMethods(string cartId);

        /// <summary>
        /// Get shipping methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>ApiResponse of List&lt;ShippingMethod&gt;</returns>
        ApiResponse<List<ShippingMethod>> CartModuleGetShipmentMethodsWithHttpInfo(string cartId);
        /// <summary>
        /// Search for shopping carts by criteria
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>SearchResult</returns>
        SearchResult CartModuleSearch(SearchCriteria criteria);

        /// <summary>
        /// Search for shopping carts by criteria
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>ApiResponse of SearchResult</returns>
        ApiResponse<SearchResult> CartModuleSearchWithHttpInfo(SearchCriteria criteria);
        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ShoppingCart</returns>
        ShoppingCart CartModuleUpdate(ShoppingCart cart);

        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        ApiResponse<ShoppingCart> CartModuleUpdateWithHttpInfo(ShoppingCart cart);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ShoppingCart</returns>
        System.Threading.Tasks.Task<ShoppingCart> CartModuleCreateAsync(ShoppingCart cart);

        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleCreateAsyncWithHttpInfo(ShoppingCart cart);
        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task CartModuleDeleteCartsAsync(List<string> ids);

        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> CartModuleDeleteCartsAsyncWithHttpInfo(List<string> ids);
        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>Task of ShoppingCart</returns>
        System.Threading.Tasks.Task<ShoppingCart> CartModuleGetCartByIdAsync(string id);

        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleGetCartByIdAsyncWithHttpInfo(string id);
        /// <summary>
        /// Get shopping cart by store id and customer id
        /// </summary>
        /// <remarks>
        /// Returns shopping cart or null if it is not found
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>Task of ShoppingCart</returns>
        System.Threading.Tasks.Task<ShoppingCart> CartModuleGetCurrentCartAsync(string storeId, string customerId);

        /// <summary>
        /// Get shopping cart by store id and customer id
        /// </summary>
        /// <remarks>
        /// Returns shopping cart or null if it is not found
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleGetCurrentCartAsyncWithHttpInfo(string storeId, string customerId);
        /// <summary>
        /// Get payment methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of List&lt;PaymentMethod&gt;</returns>
        System.Threading.Tasks.Task<List<PaymentMethod>> CartModuleGetPaymentMethodsAsync(string cartId);

        /// <summary>
        /// Get payment methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of ApiResponse (List&lt;PaymentMethod&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<PaymentMethod>>> CartModuleGetPaymentMethodsAsyncWithHttpInfo(string cartId);
        /// <summary>
        /// Get payment methods for store
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of List&lt;PaymentMethod&gt;</returns>
        System.Threading.Tasks.Task<List<PaymentMethod>> CartModuleGetPaymentMethodsForStoreAsync(string storeId);

        /// <summary>
        /// Get payment methods for store
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of ApiResponse (List&lt;PaymentMethod&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<PaymentMethod>>> CartModuleGetPaymentMethodsForStoreAsyncWithHttpInfo(string storeId);
        /// <summary>
        /// Get shipping methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of List&lt;ShippingMethod&gt;</returns>
        System.Threading.Tasks.Task<List<ShippingMethod>> CartModuleGetShipmentMethodsAsync(string cartId);

        /// <summary>
        /// Get shipping methods for shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of ApiResponse (List&lt;ShippingMethod&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<ShippingMethod>>> CartModuleGetShipmentMethodsAsyncWithHttpInfo(string cartId);
        /// <summary>
        /// Search for shopping carts by criteria
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Task of SearchResult</returns>
        System.Threading.Tasks.Task<SearchResult> CartModuleSearchAsync(SearchCriteria criteria);

        /// <summary>
        /// Search for shopping carts by criteria
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Task of ApiResponse (SearchResult)</returns>
        System.Threading.Tasks.Task<ApiResponse<SearchResult>> CartModuleSearchAsyncWithHttpInfo(SearchCriteria criteria);
        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ShoppingCart</returns>
        System.Threading.Tasks.Task<ShoppingCart> CartModuleUpdateAsync(ShoppingCart cart);

        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleUpdateAsyncWithHttpInfo(ShoppingCart cart);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class VirtoCommerceCartApi : IVirtoCommerceCartApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtoCommerceCartApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="apiClient">An instance of ApiClient.</param>
        /// <returns></returns>
        public VirtoCommerceCartApi(ApiClient apiClient)
        {
            ApiClient = apiClient;
            Configuration = apiClient.Configuration;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the API client object
        /// </summary>
        /// <value>An instance of the ApiClient</value>
        public ApiClient ApiClient { get; set; }

        /// <summary>
        /// Create shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ShoppingCart</returns>
        public ShoppingCart CartModuleCreate(ShoppingCart cart)
        {
             ApiResponse<ShoppingCart> localVarResponse = CartModuleCreateWithHttpInfo(cart);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        public ApiResponse<ShoppingCart> CartModuleCreateWithHttpInfo(ShoppingCart cart)
        {
            // verify the required parameter 'cart' is set
            if (cart == null)
                throw new ApiException(400, "Missing required parameter 'cart' when calling VirtoCommerceCartApi->CartModuleCreate");

            var localVarPath = "/api/cart/carts";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cart.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(cart); // http body (model) parameter
            }
            else
            {
                localVarPostBody = cart; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleCreate: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleCreate: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }

        /// <summary>
        /// Create shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ShoppingCart</returns>
        public async System.Threading.Tasks.Task<ShoppingCart> CartModuleCreateAsync(ShoppingCart cart)
        {
             ApiResponse<ShoppingCart> localVarResponse = await CartModuleCreateAsyncWithHttpInfo(cart);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleCreateAsyncWithHttpInfo(ShoppingCart cart)
        {
            // verify the required parameter 'cart' is set
            if (cart == null)
                throw new ApiException(400, "Missing required parameter 'cart' when calling VirtoCommerceCartApi->CartModuleCreate");

            var localVarPath = "/api/cart/carts";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cart.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(cart); // http body (model) parameter
            }
            else
            {
                localVarPostBody = cart; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleCreate: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleCreate: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }
        /// <summary>
        /// Delete shopping carts by ids 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns></returns>
        public void CartModuleDeleteCarts(List<string> ids)
        {
             CartModuleDeleteCartsWithHttpInfo(ids);
        }

        /// <summary>
        /// Delete shopping carts by ids 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> CartModuleDeleteCartsWithHttpInfo(List<string> ids)
        {
            // verify the required parameter 'ids' is set
            if (ids == null)
                throw new ApiException(400, "Missing required parameter 'ids' when calling VirtoCommerceCartApi->CartModuleDeleteCarts");

            var localVarPath = "/api/cart/carts";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (ids != null) localVarQueryParams.Add("ids", ApiClient.ParameterToString(ids)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleDeleteCarts: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleDeleteCarts: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Delete shopping carts by ids 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task CartModuleDeleteCartsAsync(List<string> ids)
        {
             await CartModuleDeleteCartsAsyncWithHttpInfo(ids);

        }

        /// <summary>
        /// Delete shopping carts by ids 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids">Array of shopping cart ids</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> CartModuleDeleteCartsAsyncWithHttpInfo(List<string> ids)
        {
            // verify the required parameter 'ids' is set
            if (ids == null)
                throw new ApiException(400, "Missing required parameter 'ids' when calling VirtoCommerceCartApi->CartModuleDeleteCarts");

            var localVarPath = "/api/cart/carts";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (ids != null) localVarQueryParams.Add("ids", ApiClient.ParameterToString(ids)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleDeleteCarts: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleDeleteCarts: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Get shopping cart by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>ShoppingCart</returns>
        public ShoppingCart CartModuleGetCartById(string id)
        {
             ApiResponse<ShoppingCart> localVarResponse = CartModuleGetCartByIdWithHttpInfo(id);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get shopping cart by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        public ApiResponse<ShoppingCart> CartModuleGetCartByIdWithHttpInfo(string id)
        {
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling VirtoCommerceCartApi->CartModuleGetCartById");

            var localVarPath = "/api/cart/carts/{id}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (id != null) localVarPathParams.Add("id", ApiClient.ParameterToString(id)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCartById: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCartById: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }

        /// <summary>
        /// Get shopping cart by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>Task of ShoppingCart</returns>
        public async System.Threading.Tasks.Task<ShoppingCart> CartModuleGetCartByIdAsync(string id)
        {
             ApiResponse<ShoppingCart> localVarResponse = await CartModuleGetCartByIdAsyncWithHttpInfo(id);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get shopping cart by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Shopping cart id</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleGetCartByIdAsyncWithHttpInfo(string id)
        {
            // verify the required parameter 'id' is set
            if (id == null)
                throw new ApiException(400, "Missing required parameter 'id' when calling VirtoCommerceCartApi->CartModuleGetCartById");

            var localVarPath = "/api/cart/carts/{id}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (id != null) localVarPathParams.Add("id", ApiClient.ParameterToString(id)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCartById: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCartById: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }
        /// <summary>
        /// Get shopping cart by store id and customer id Returns shopping cart or null if it is not found
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>ShoppingCart</returns>
        public ShoppingCart CartModuleGetCurrentCart(string storeId, string customerId)
        {
             ApiResponse<ShoppingCart> localVarResponse = CartModuleGetCurrentCartWithHttpInfo(storeId, customerId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get shopping cart by store id and customer id Returns shopping cart or null if it is not found
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        public ApiResponse<ShoppingCart> CartModuleGetCurrentCartWithHttpInfo(string storeId, string customerId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceCartApi->CartModuleGetCurrentCart");
            // verify the required parameter 'customerId' is set
            if (customerId == null)
                throw new ApiException(400, "Missing required parameter 'customerId' when calling VirtoCommerceCartApi->CartModuleGetCurrentCart");

            var localVarPath = "/api/cart/{storeId}/{customerId}/carts/current";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (customerId != null) localVarPathParams.Add("customerId", ApiClient.ParameterToString(customerId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCurrentCart: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCurrentCart: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }

        /// <summary>
        /// Get shopping cart by store id and customer id Returns shopping cart or null if it is not found
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>Task of ShoppingCart</returns>
        public async System.Threading.Tasks.Task<ShoppingCart> CartModuleGetCurrentCartAsync(string storeId, string customerId)
        {
             ApiResponse<ShoppingCart> localVarResponse = await CartModuleGetCurrentCartAsyncWithHttpInfo(storeId, customerId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get shopping cart by store id and customer id Returns shopping cart or null if it is not found
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleGetCurrentCartAsyncWithHttpInfo(string storeId, string customerId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceCartApi->CartModuleGetCurrentCart");
            // verify the required parameter 'customerId' is set
            if (customerId == null)
                throw new ApiException(400, "Missing required parameter 'customerId' when calling VirtoCommerceCartApi->CartModuleGetCurrentCart");

            var localVarPath = "/api/cart/{storeId}/{customerId}/carts/current";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (customerId != null) localVarPathParams.Add("customerId", ApiClient.ParameterToString(customerId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCurrentCart: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetCurrentCart: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }
        /// <summary>
        /// Get payment methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>List&lt;PaymentMethod&gt;</returns>
        public List<PaymentMethod> CartModuleGetPaymentMethods(string cartId)
        {
             ApiResponse<List<PaymentMethod>> localVarResponse = CartModuleGetPaymentMethodsWithHttpInfo(cartId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get payment methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>ApiResponse of List&lt;PaymentMethod&gt;</returns>
        public ApiResponse<List<PaymentMethod>> CartModuleGetPaymentMethodsWithHttpInfo(string cartId)
        {
            // verify the required parameter 'cartId' is set
            if (cartId == null)
                throw new ApiException(400, "Missing required parameter 'cartId' when calling VirtoCommerceCartApi->CartModuleGetPaymentMethods");

            var localVarPath = "/api/cart/carts/{cartId}/paymentMethods";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cartId != null) localVarPathParams.Add("cartId", ApiClient.ParameterToString(cartId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethods: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethods: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<PaymentMethod>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<PaymentMethod>)ApiClient.Deserialize(localVarResponse, typeof(List<PaymentMethod>)));
            
        }

        /// <summary>
        /// Get payment methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of List&lt;PaymentMethod&gt;</returns>
        public async System.Threading.Tasks.Task<List<PaymentMethod>> CartModuleGetPaymentMethodsAsync(string cartId)
        {
             ApiResponse<List<PaymentMethod>> localVarResponse = await CartModuleGetPaymentMethodsAsyncWithHttpInfo(cartId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get payment methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of ApiResponse (List&lt;PaymentMethod&gt;)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<List<PaymentMethod>>> CartModuleGetPaymentMethodsAsyncWithHttpInfo(string cartId)
        {
            // verify the required parameter 'cartId' is set
            if (cartId == null)
                throw new ApiException(400, "Missing required parameter 'cartId' when calling VirtoCommerceCartApi->CartModuleGetPaymentMethods");

            var localVarPath = "/api/cart/carts/{cartId}/paymentMethods";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cartId != null) localVarPathParams.Add("cartId", ApiClient.ParameterToString(cartId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethods: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethods: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<PaymentMethod>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<PaymentMethod>)ApiClient.Deserialize(localVarResponse, typeof(List<PaymentMethod>)));
            
        }
        /// <summary>
        /// Get payment methods for store 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>List&lt;PaymentMethod&gt;</returns>
        public List<PaymentMethod> CartModuleGetPaymentMethodsForStore(string storeId)
        {
             ApiResponse<List<PaymentMethod>> localVarResponse = CartModuleGetPaymentMethodsForStoreWithHttpInfo(storeId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get payment methods for store 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>ApiResponse of List&lt;PaymentMethod&gt;</returns>
        public ApiResponse<List<PaymentMethod>> CartModuleGetPaymentMethodsForStoreWithHttpInfo(string storeId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceCartApi->CartModuleGetPaymentMethodsForStore");

            var localVarPath = "/api/cart/stores/{storeId}/paymentMethods";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethodsForStore: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethodsForStore: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<PaymentMethod>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<PaymentMethod>)ApiClient.Deserialize(localVarResponse, typeof(List<PaymentMethod>)));
            
        }

        /// <summary>
        /// Get payment methods for store 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of List&lt;PaymentMethod&gt;</returns>
        public async System.Threading.Tasks.Task<List<PaymentMethod>> CartModuleGetPaymentMethodsForStoreAsync(string storeId)
        {
             ApiResponse<List<PaymentMethod>> localVarResponse = await CartModuleGetPaymentMethodsForStoreAsyncWithHttpInfo(storeId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get payment methods for store 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of ApiResponse (List&lt;PaymentMethod&gt;)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<List<PaymentMethod>>> CartModuleGetPaymentMethodsForStoreAsyncWithHttpInfo(string storeId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceCartApi->CartModuleGetPaymentMethodsForStore");

            var localVarPath = "/api/cart/stores/{storeId}/paymentMethods";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethodsForStore: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetPaymentMethodsForStore: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<PaymentMethod>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<PaymentMethod>)ApiClient.Deserialize(localVarResponse, typeof(List<PaymentMethod>)));
            
        }
        /// <summary>
        /// Get shipping methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>List&lt;ShippingMethod&gt;</returns>
        public List<ShippingMethod> CartModuleGetShipmentMethods(string cartId)
        {
             ApiResponse<List<ShippingMethod>> localVarResponse = CartModuleGetShipmentMethodsWithHttpInfo(cartId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get shipping methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>ApiResponse of List&lt;ShippingMethod&gt;</returns>
        public ApiResponse<List<ShippingMethod>> CartModuleGetShipmentMethodsWithHttpInfo(string cartId)
        {
            // verify the required parameter 'cartId' is set
            if (cartId == null)
                throw new ApiException(400, "Missing required parameter 'cartId' when calling VirtoCommerceCartApi->CartModuleGetShipmentMethods");

            var localVarPath = "/api/cart/carts/{cartId}/shipmentMethods";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cartId != null) localVarPathParams.Add("cartId", ApiClient.ParameterToString(cartId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetShipmentMethods: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetShipmentMethods: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<ShippingMethod>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<ShippingMethod>)ApiClient.Deserialize(localVarResponse, typeof(List<ShippingMethod>)));
            
        }

        /// <summary>
        /// Get shipping methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of List&lt;ShippingMethod&gt;</returns>
        public async System.Threading.Tasks.Task<List<ShippingMethod>> CartModuleGetShipmentMethodsAsync(string cartId)
        {
             ApiResponse<List<ShippingMethod>> localVarResponse = await CartModuleGetShipmentMethodsAsyncWithHttpInfo(cartId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get shipping methods for shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cartId">Shopping cart id</param>
        /// <returns>Task of ApiResponse (List&lt;ShippingMethod&gt;)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<List<ShippingMethod>>> CartModuleGetShipmentMethodsAsyncWithHttpInfo(string cartId)
        {
            // verify the required parameter 'cartId' is set
            if (cartId == null)
                throw new ApiException(400, "Missing required parameter 'cartId' when calling VirtoCommerceCartApi->CartModuleGetShipmentMethods");

            var localVarPath = "/api/cart/carts/{cartId}/shipmentMethods";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cartId != null) localVarPathParams.Add("cartId", ApiClient.ParameterToString(cartId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetShipmentMethods: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleGetShipmentMethods: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<ShippingMethod>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<ShippingMethod>)ApiClient.Deserialize(localVarResponse, typeof(List<ShippingMethod>)));
            
        }
        /// <summary>
        /// Search for shopping carts by criteria 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>SearchResult</returns>
        public SearchResult CartModuleSearch(SearchCriteria criteria)
        {
             ApiResponse<SearchResult> localVarResponse = CartModuleSearchWithHttpInfo(criteria);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Search for shopping carts by criteria 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>ApiResponse of SearchResult</returns>
        public ApiResponse<SearchResult> CartModuleSearchWithHttpInfo(SearchCriteria criteria)
        {
            // verify the required parameter 'criteria' is set
            if (criteria == null)
                throw new ApiException(400, "Missing required parameter 'criteria' when calling VirtoCommerceCartApi->CartModuleSearch");

            var localVarPath = "/api/cart/search";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (criteria.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(criteria); // http body (model) parameter
            }
            else
            {
                localVarPostBody = criteria; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleSearch: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleSearch: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<SearchResult>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (SearchResult)ApiClient.Deserialize(localVarResponse, typeof(SearchResult)));
            
        }

        /// <summary>
        /// Search for shopping carts by criteria 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Task of SearchResult</returns>
        public async System.Threading.Tasks.Task<SearchResult> CartModuleSearchAsync(SearchCriteria criteria)
        {
             ApiResponse<SearchResult> localVarResponse = await CartModuleSearchAsyncWithHttpInfo(criteria);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Search for shopping carts by criteria 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Task of ApiResponse (SearchResult)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SearchResult>> CartModuleSearchAsyncWithHttpInfo(SearchCriteria criteria)
        {
            // verify the required parameter 'criteria' is set
            if (criteria == null)
                throw new ApiException(400, "Missing required parameter 'criteria' when calling VirtoCommerceCartApi->CartModuleSearch");

            var localVarPath = "/api/cart/search";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (criteria.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(criteria); // http body (model) parameter
            }
            else
            {
                localVarPostBody = criteria; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleSearch: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleSearch: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<SearchResult>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (SearchResult)ApiClient.Deserialize(localVarResponse, typeof(SearchResult)));
            
        }
        /// <summary>
        /// Update shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ShoppingCart</returns>
        public ShoppingCart CartModuleUpdate(ShoppingCart cart)
        {
             ApiResponse<ShoppingCart> localVarResponse = CartModuleUpdateWithHttpInfo(cart);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>ApiResponse of ShoppingCart</returns>
        public ApiResponse<ShoppingCart> CartModuleUpdateWithHttpInfo(ShoppingCart cart)
        {
            // verify the required parameter 'cart' is set
            if (cart == null)
                throw new ApiException(400, "Missing required parameter 'cart' when calling VirtoCommerceCartApi->CartModuleUpdate");

            var localVarPath = "/api/cart/carts";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cart.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(cart); // http body (model) parameter
            }
            else
            {
                localVarPostBody = cart; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleUpdate: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleUpdate: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }

        /// <summary>
        /// Update shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ShoppingCart</returns>
        public async System.Threading.Tasks.Task<ShoppingCart> CartModuleUpdateAsync(ShoppingCart cart)
        {
             ApiResponse<ShoppingCart> localVarResponse = await CartModuleUpdateAsyncWithHttpInfo(cart);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update shopping cart 
        /// </summary>
        /// <exception cref="VirtoCommerce.CartModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="cart">Shopping cart model</param>
        /// <returns>Task of ApiResponse (ShoppingCart)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<ShoppingCart>> CartModuleUpdateAsyncWithHttpInfo(ShoppingCart cart)
        {
            // verify the required parameter 'cart' is set
            if (cart == null)
                throw new ApiException(400, "Missing required parameter 'cart' when calling VirtoCommerceCartApi->CartModuleUpdate");

            var localVarPath = "/api/cart/carts";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (cart.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(cart); // http body (model) parameter
            }
            else
            {
                localVarPostBody = cart; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling CartModuleUpdate: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling CartModuleUpdate: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ShoppingCart>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ShoppingCart)ApiClient.Deserialize(localVarResponse, typeof(ShoppingCart)));
            
        }
    }
}
