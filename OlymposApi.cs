using System.Threading.Tasks;
using Refit;

namespace Olymposer
{
    public interface IOlymposApi
    {
        [Headers("Content-Type: multipart/form-data")]
        [Post("/Inloggen/tabid/422/ctl/Bestaand/mid/896/language/nl-NL/Default.aspx")]
        Task<ApiResponse<string>> GetSessionCookie(
            [Query] GetSessionCookieQueryParams getSessionCookieQueryParams, 
            [Body] GetSessionCookieQueryBody getSessionCookieQueryBody
             );

        [Headers("X-OFFICIAL-REQUEST-MINE: TRUE")]
        [Get("/nl-nl/sportaanbod/groepslessen/allegroepslessen/details.aspx")]
        Task<ApiResponse<ReserveSlotResponse>> ReserveSlot(
            [Query] ReserveSlotQueryParams reserveSlotQueryParams,
            [Header("Cookie")] string cookie
        );
    }

    public class ReserveSlotHeader
    {
    }

    public class ReserveSlotQueryParams
    {
        [AliasAs("sportgroep")] public string SportGroup { get; set; } = "FITNESS C";
        [AliasAs("navisioncode")] public string NavisionCode { get; set; } = "FIT TIMESL";
        [AliasAs("navisionsubcode")] public string NavisionSubCode { get; set; } 
    }

    public class ReserveSlotResponse
    {
        [AliasAs("success")] public bool Success { get; set; }
        [AliasAs("redirect")] public string RedirectUrl { get; set; }
        [AliasAs("message")] public string Message { get; set; }
        [AliasAs("navisionMessage")] public string NavisionMessage { get; set; }
    }

    public class GetSessionCookieQueryBody
    {
        [AliasAs("dnn$ctr896$BestaandView$txtWachtwoord")]
        public string Password { get; set; }
    }
    
    public class GetSessionCookieQueryParams
    {
        [AliasAs("s")]
        public string Username { get; set; }
    }
}