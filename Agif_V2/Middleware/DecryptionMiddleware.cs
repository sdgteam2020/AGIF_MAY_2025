using Agif_V2.Controllers;
using Agif_V2.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agif_V2.Middlewares
{
    public class DecryptionMiddleware
    {
        private readonly RequestDelegate _next;

        public DecryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.HasFormContentType && HttpMethods.IsPost(context.Request.Method))
            {
                var form = await context.Request.ReadFormAsync();

                if (form.TryGetValue("EncryptedData", out var encryptedValues))
                {
                    try
                    {
                        string encryptedData = encryptedValues.ToString();

                        string? secretKey = context.Session.GetString(OnlineApplicationController.SessionKeySalt);

                        if (string.IsNullOrEmpty(secretKey))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Security Error: Session expired or missing encryption key.");
                            return;
                        }

                        string decryptedJson = AESEncrytDecry.DecryptAES(encryptedData, secretKey);

                        var decryptedFields = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedJson);

                        if (decryptedFields != null)
                        {
                            var newFormDictionary = new Dictionary<string, StringValues>();

                            foreach (var kvp in form)
                            {
                                if (kvp.Key != "EncryptedData")
                                {
                                    newFormDictionary[kvp.Key] = kvp.Value;
                                }
                            }

                            foreach (var kvp in decryptedFields)
                            {
                                newFormDictionary[kvp.Key] = new StringValues(kvp.Value);
                            }

                            context.Request.Form = new FormCollection(newFormDictionary);
                        }
                    }
                    catch
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Security Error: Invalid or corrupted payload.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}