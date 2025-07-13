using EdgeDB;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reserve.Pages.Validation
{
    public class DatabaseValidationModel : PageModel
    {
        private readonly EdgeDBClient _edgeDBClient;

        public DatabaseValidationModel(EdgeDBClient client)
        {
            _edgeDBClient = client;
            ValidationMessage = string.Empty;
        }

        public string ValidationMessage { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                var result = await _edgeDBClient.QueryAsync("SELECT 1");
                ValidationMessage = "Connection successful. Database is reachable.";
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Database connection failed: {ex.Message}";
            }
        }
    }
}