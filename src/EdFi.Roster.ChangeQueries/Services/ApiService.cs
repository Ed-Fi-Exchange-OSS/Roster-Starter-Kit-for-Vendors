using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.Resources;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class ApiService
    {
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public ApiService(IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task<ExtendedInfoResponse<List<DeletedResource>>> DeletedResources<T>(string methodName, long minVersion, long maxVersion)
        {
            var leaApi = await _apiFacade.GetApiClassInstance<T>();
            var limit = 100;
            var offset = 0;
            var currResponseRecordCount = 0;
            var response = new ExtendedInfoResponse<List<DeletedResource>>();
            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri($"{ApiRoutes.LocalEducationAgenciesResource}//deletes", offset, limit);
                ApiResponse<List<DeletedResource>> currentApiResponse = null;
                try
                {
                    var method = (Task<ApiResponse<List<DeletedResource>>>) typeof(T)
                        .GetMethod(methodName)
                        ?.Invoke(leaApi,
                            new object[]
                            {
                                offset, limit, (int?) minVersion, (int?) maxVersion, default(string),
                                default(System.Threading.CancellationToken)
                            });

                    if (method != null) currentApiResponse = await method;
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int) HttpStatusCode.Unauthorized))
                    {
                        leaApi = await _apiFacade.GetApiClassInstance<T>(true);
                        var method = (Task<ApiResponse<List<DeletedResource>>>) typeof(T)
                            .GetMethod(methodName)
                            ?.Invoke(leaApi,
                                new object[]
                                {
                                    offset, limit, (int?) minVersion, (int?) maxVersion, default(string),
                                    default(System.Threading.CancellationToken)
                                });
                        if (method != null) currentApiResponse = await method;
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            return response;
        }
    }
}
