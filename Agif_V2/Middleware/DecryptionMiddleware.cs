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
            // 1. Only process POST requests that contain Form Data
            if (context.Request.HasFormContentType && HttpMethods.IsPost(context.Request.Method))
            {
                var form = await context.Request.ReadFormAsync();

                if (form.TryGetValue("EncryptedData", out var encryptedValues))
                {
                    try
                    {
                        string encryptedData = encryptedValues.ToString();

                        // 3. Retrieve your AES Key using the Controller's constant
                        string? secretKey = context.Session.GetString(OnlineApplicationController.SessionKeySalt);

                        // ALWAYS check if the session expired or is missing before decrypting
                        if (string.IsNullOrEmpty(secretKey))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Security Error: Session expired or missing encryption key.");
                            return;
                        }

                        // 4. Decrypt the payload
                        string decryptedJson = AESEncrytDecry.DecryptAES(encryptedData, secretKey);

                        // 5. Deserialize into a flat dictionary
                        var decryptedFields = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedJson);

                        if (decryptedFields != null)
                        {
                            var newFormDictionary = new Dictionary<string, StringValues>();

                            // 6. Copy original unencrypted form fields (Crucial for the Anti-Forgery Token!)
                            foreach (var kvp in form)
                            {
                                if (kvp.Key != "EncryptedData")
                                {
                                    newFormDictionary[kvp.Key] = kvp.Value;
                                }
                            }

                            // 7. Inject the decrypted fields into the new form dictionary
                            foreach (var kvp in decryptedFields)
                            {
                                newFormDictionary[kvp.Key] = new StringValues(kvp.Value);
                            }

                            // 8. Overwrite the Request.Form with our newly reconstructed collection
                            context.Request.Form = new FormCollection(newFormDictionary);
                        }
                    }
                    catch
                    {
                        // Intercept and reject requests if someone tampers with the EncryptedData
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Security Error: Invalid or corrupted payload.");
                        return;
                    }
                }
            }

            // 9. Pass the request to the next middleware (and eventually the Controller)
            await _next(context);
        }
    }
}